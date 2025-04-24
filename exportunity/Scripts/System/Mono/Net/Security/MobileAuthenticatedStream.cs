using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.ExceptionServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Mono.Security.Interface;

namespace Mono.Net.Security;

internal abstract class MobileAuthenticatedStream : AuthenticatedStream, IMonoSslStream, IDisposable
{
	private enum Operation
	{
		None = 0,
		Handshake = 1,
		Authenticated = 2,
		Renegotiate = 3,
		Read = 4,
		Write = 5,
		Close = 6
	}

	private enum OperationType
	{
		Read = 0,
		Write = 1,
		Renegotiate = 2,
		Shutdown = 3
	}

	private MobileTlsContext xobileTlsContext;

	private ExceptionDispatchInfo lastException;

	private AsyncProtocolRequest asyncHandshakeRequest;

	private AsyncProtocolRequest asyncReadRequest;

	private AsyncProtocolRequest asyncWriteRequest;

	private BufferOffsetSize2 readBuffer;

	private BufferOffsetSize2 writeBuffer;

	private object ioLock = new object();

	private int closeRequested;

	private bool shutdown;

	private Operation operation;

	private static int uniqueNameInteger = 123;

	private static int nextId;

	internal readonly int ID = ++nextId;

	public SslStream SslStream { get; }

	public MonoTlsSettings Settings { get; }

	public MobileTlsProvider Provider { get; }

	MonoTlsProvider IMonoSslStream.Provider => Provider;

	internal bool HasContext => xobileTlsContext != null;

	internal string TargetHost { get; private set; }

	public AuthenticatedStream AuthenticatedStream => this;

	public bool CanRenegotiate
	{
		get
		{
			CheckThrow(authSuccessCheck: true);
			if (xobileTlsContext != null)
			{
				return xobileTlsContext.CanRenegotiate;
			}
			return false;
		}
	}

	public override bool IsServer
	{
		get
		{
			CheckThrow(authSuccessCheck: false);
			if (xobileTlsContext != null)
			{
				return xobileTlsContext.IsServer;
			}
			return false;
		}
	}

	public override bool IsAuthenticated
	{
		get
		{
			lock (ioLock)
			{
				return xobileTlsContext != null && lastException == null && xobileTlsContext.IsAuthenticated;
			}
		}
	}

	public override bool IsMutuallyAuthenticated
	{
		get
		{
			lock (ioLock)
			{
				if (!IsAuthenticated)
				{
					return false;
				}
				if ((xobileTlsContext.IsServer ? xobileTlsContext.LocalServerCertificate : xobileTlsContext.LocalClientCertificate) == null)
				{
					return false;
				}
				return xobileTlsContext.IsRemoteCertificateAvailable;
			}
		}
	}

	public SslProtocols SslProtocol
	{
		get
		{
			lock (ioLock)
			{
				CheckThrow(authSuccessCheck: true);
				return (SslProtocols)xobileTlsContext.NegotiatedProtocol;
			}
		}
	}

	public X509Certificate RemoteCertificate
	{
		get
		{
			lock (ioLock)
			{
				CheckThrow(authSuccessCheck: true);
				return xobileTlsContext.RemoteCertificate;
			}
		}
	}

	public X509Certificate LocalCertificate
	{
		get
		{
			lock (ioLock)
			{
				CheckThrow(authSuccessCheck: true);
				return InternalLocalCertificate;
			}
		}
	}

	public X509Certificate InternalLocalCertificate
	{
		get
		{
			lock (ioLock)
			{
				CheckThrow(authSuccessCheck: false);
				if (xobileTlsContext == null)
				{
					return null;
				}
				return xobileTlsContext.IsServer ? xobileTlsContext.LocalServerCertificate : xobileTlsContext.LocalClientCertificate;
			}
		}
	}

	public TransportContext TransportContext
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override bool CanRead
	{
		get
		{
			if (IsAuthenticated)
			{
				return base.InnerStream.CanRead;
			}
			return false;
		}
	}

	public override bool CanTimeout => base.InnerStream.CanTimeout;

	public override bool CanWrite
	{
		get
		{
			if (IsAuthenticated & base.InnerStream.CanWrite)
			{
				return !shutdown;
			}
			return false;
		}
	}

	public override bool CanSeek => false;

	public override long Length => base.InnerStream.Length;

	public override long Position
	{
		get
		{
			return base.InnerStream.Position;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public override bool IsEncrypted => IsAuthenticated;

	public override bool IsSigned => IsAuthenticated;

	public override int ReadTimeout
	{
		get
		{
			return base.InnerStream.ReadTimeout;
		}
		set
		{
			base.InnerStream.ReadTimeout = value;
		}
	}

	public override int WriteTimeout
	{
		get
		{
			return base.InnerStream.WriteTimeout;
		}
		set
		{
			base.InnerStream.WriteTimeout = value;
		}
	}

	public System.Security.Authentication.CipherAlgorithmType CipherAlgorithm
	{
		get
		{
			CheckThrow(authSuccessCheck: true);
			MonoTlsConnectionInfo connectionInfo = GetConnectionInfo();
			if (connectionInfo == null)
			{
				return System.Security.Authentication.CipherAlgorithmType.None;
			}
			switch (connectionInfo.CipherAlgorithmType)
			{
			case Mono.Security.Interface.CipherAlgorithmType.Aes128:
			case Mono.Security.Interface.CipherAlgorithmType.AesGcm128:
				return System.Security.Authentication.CipherAlgorithmType.Aes128;
			case Mono.Security.Interface.CipherAlgorithmType.Aes256:
			case Mono.Security.Interface.CipherAlgorithmType.AesGcm256:
				return System.Security.Authentication.CipherAlgorithmType.Aes256;
			default:
				return System.Security.Authentication.CipherAlgorithmType.None;
			}
		}
	}

	public System.Security.Authentication.HashAlgorithmType HashAlgorithm
	{
		get
		{
			CheckThrow(authSuccessCheck: true);
			MonoTlsConnectionInfo connectionInfo = GetConnectionInfo();
			if (connectionInfo == null)
			{
				return System.Security.Authentication.HashAlgorithmType.None;
			}
			switch (connectionInfo.HashAlgorithmType)
			{
			case Mono.Security.Interface.HashAlgorithmType.Md5:
			case Mono.Security.Interface.HashAlgorithmType.Md5Sha1:
				return System.Security.Authentication.HashAlgorithmType.Md5;
			case Mono.Security.Interface.HashAlgorithmType.Sha1:
			case Mono.Security.Interface.HashAlgorithmType.Sha224:
			case Mono.Security.Interface.HashAlgorithmType.Sha256:
			case Mono.Security.Interface.HashAlgorithmType.Sha384:
			case Mono.Security.Interface.HashAlgorithmType.Sha512:
				return System.Security.Authentication.HashAlgorithmType.Sha1;
			default:
				return System.Security.Authentication.HashAlgorithmType.None;
			}
		}
	}

	public System.Security.Authentication.ExchangeAlgorithmType KeyExchangeAlgorithm
	{
		get
		{
			CheckThrow(authSuccessCheck: true);
			MonoTlsConnectionInfo connectionInfo = GetConnectionInfo();
			if (connectionInfo == null)
			{
				return System.Security.Authentication.ExchangeAlgorithmType.None;
			}
			switch (connectionInfo.ExchangeAlgorithmType)
			{
			case Mono.Security.Interface.ExchangeAlgorithmType.Rsa:
				return System.Security.Authentication.ExchangeAlgorithmType.RsaSign;
			case Mono.Security.Interface.ExchangeAlgorithmType.Dhe:
			case Mono.Security.Interface.ExchangeAlgorithmType.EcDhe:
				return System.Security.Authentication.ExchangeAlgorithmType.DiffieHellman;
			default:
				return System.Security.Authentication.ExchangeAlgorithmType.None;
			}
		}
	}

	public int CipherStrength
	{
		get
		{
			CheckThrow(authSuccessCheck: true);
			MonoTlsConnectionInfo connectionInfo = GetConnectionInfo();
			if (connectionInfo == null)
			{
				return 0;
			}
			switch (connectionInfo.CipherAlgorithmType)
			{
			case Mono.Security.Interface.CipherAlgorithmType.None:
			case Mono.Security.Interface.CipherAlgorithmType.Aes128:
			case Mono.Security.Interface.CipherAlgorithmType.AesGcm128:
				return 128;
			case Mono.Security.Interface.CipherAlgorithmType.Aes256:
			case Mono.Security.Interface.CipherAlgorithmType.AesGcm256:
				return 256;
			default:
				throw new ArgumentOutOfRangeException("CipherAlgorithmType");
			}
		}
	}

	public int HashStrength
	{
		get
		{
			CheckThrow(authSuccessCheck: true);
			MonoTlsConnectionInfo connectionInfo = GetConnectionInfo();
			if (connectionInfo == null)
			{
				return 0;
			}
			switch (connectionInfo.HashAlgorithmType)
			{
			case Mono.Security.Interface.HashAlgorithmType.Md5:
			case Mono.Security.Interface.HashAlgorithmType.Md5Sha1:
				return 128;
			case Mono.Security.Interface.HashAlgorithmType.Sha1:
				return 160;
			case Mono.Security.Interface.HashAlgorithmType.Sha224:
				return 224;
			case Mono.Security.Interface.HashAlgorithmType.Sha256:
				return 256;
			case Mono.Security.Interface.HashAlgorithmType.Sha384:
				return 384;
			case Mono.Security.Interface.HashAlgorithmType.Sha512:
				return 512;
			default:
				throw new ArgumentOutOfRangeException("HashAlgorithmType");
			}
		}
	}

	public int KeyExchangeStrength => 0;

	public bool CheckCertRevocationStatus
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public MobileAuthenticatedStream(Stream innerStream, bool leaveInnerStreamOpen, SslStream owner, MonoTlsSettings settings, MobileTlsProvider provider)
		: base(innerStream, leaveInnerStreamOpen)
	{
		SslStream = owner;
		Settings = settings;
		Provider = provider;
		readBuffer = new BufferOffsetSize2(16500);
		writeBuffer = new BufferOffsetSize2(16384);
		operation = Operation.None;
	}

	internal void CheckThrow(bool authSuccessCheck, bool shutdownCheck = false)
	{
		if (lastException != null)
		{
			lastException.Throw();
		}
		if (authSuccessCheck && !IsAuthenticated)
		{
			throw new InvalidOperationException("This operation is only allowed using a successfully authenticated context.");
		}
		if (shutdownCheck && shutdown)
		{
			throw new InvalidOperationException("Write operations are not allowed after the channel was shutdown.");
		}
	}

	internal static Exception GetSSPIException(Exception e)
	{
		if (e is OperationCanceledException || e is IOException || e is ObjectDisposedException || e is AuthenticationException || e is NotSupportedException)
		{
			return e;
		}
		return new AuthenticationException("Authentication failed, see inner exception.", e);
	}

	internal static Exception GetIOException(Exception e, string message)
	{
		if (e is OperationCanceledException || e is IOException || e is ObjectDisposedException || e is AuthenticationException || e is NotSupportedException)
		{
			return e;
		}
		return new IOException(message, e);
	}

	internal static Exception GetRenegotiationException(string message)
	{
		TlsException innerException = new TlsException(AlertDescription.NoRenegotiation, message);
		return new AuthenticationException("Authentication failed, see inner exception.", innerException);
	}

	internal static Exception GetInternalError()
	{
		throw new InvalidOperationException("Internal error.");
	}

	internal static Exception GetInvalidNestedCallException()
	{
		throw new InvalidOperationException("Invalid nested call.");
	}

	internal ExceptionDispatchInfo SetException(Exception e)
	{
		ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
		return Interlocked.CompareExchange(ref lastException, exceptionDispatchInfo, null) ?? exceptionDispatchInfo;
	}

	public void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
	{
		MonoSslClientAuthenticationOptions options = new MonoSslClientAuthenticationOptions
		{
			TargetHost = targetHost,
			ClientCertificates = clientCertificates,
			EnabledSslProtocols = enabledSslProtocols,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = EncryptionPolicy.RequireEncryption
		};
		Task task = ProcessAuthentication(runSynchronously: true, options, CancellationToken.None);
		try
		{
			task.Wait();
		}
		catch (Exception e)
		{
			throw HttpWebRequest.FlattenException(e);
		}
	}

	public void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
	{
		MonoSslServerAuthenticationOptions options = new MonoSslServerAuthenticationOptions
		{
			ServerCertificate = serverCertificate,
			ClientCertificateRequired = clientCertificateRequired,
			EnabledSslProtocols = enabledSslProtocols,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = EncryptionPolicy.RequireEncryption
		};
		Task task = ProcessAuthentication(runSynchronously: true, options, CancellationToken.None);
		try
		{
			task.Wait();
		}
		catch (Exception e)
		{
			throw HttpWebRequest.FlattenException(e);
		}
	}

	public Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
	{
		MonoSslClientAuthenticationOptions options = new MonoSslClientAuthenticationOptions
		{
			TargetHost = targetHost,
			ClientCertificates = clientCertificates,
			EnabledSslProtocols = enabledSslProtocols,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = EncryptionPolicy.RequireEncryption
		};
		return ProcessAuthentication(runSynchronously: false, options, CancellationToken.None);
	}

	public Task AuthenticateAsClientAsync(IMonoSslClientAuthenticationOptions sslClientAuthenticationOptions, CancellationToken cancellationToken)
	{
		return ProcessAuthentication(runSynchronously: false, (MonoSslClientAuthenticationOptions)sslClientAuthenticationOptions, cancellationToken);
	}

	public Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
	{
		MonoSslServerAuthenticationOptions options = new MonoSslServerAuthenticationOptions
		{
			ServerCertificate = serverCertificate,
			ClientCertificateRequired = clientCertificateRequired,
			EnabledSslProtocols = enabledSslProtocols,
			CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
			EncryptionPolicy = EncryptionPolicy.RequireEncryption
		};
		return ProcessAuthentication(runSynchronously: false, options, CancellationToken.None);
	}

	public Task AuthenticateAsServerAsync(IMonoSslServerAuthenticationOptions sslServerAuthenticationOptions, CancellationToken cancellationToken)
	{
		return ProcessAuthentication(runSynchronously: false, (MonoSslServerAuthenticationOptions)sslServerAuthenticationOptions, cancellationToken);
	}

	public Task ShutdownAsync()
	{
		AsyncShutdownRequest asyncRequest = new AsyncShutdownRequest(this);
		return StartOperation(OperationType.Shutdown, asyncRequest, CancellationToken.None);
	}

	private async Task ProcessAuthentication(bool runSynchronously, MonoSslAuthenticationOptions options, CancellationToken cancellationToken)
	{
		if (options.ServerMode)
		{
			if (options.ServerCertificate == null && options.ServerCertSelectionDelegate == null)
			{
				throw new ArgumentException("ServerCertificate");
			}
		}
		else
		{
			if (options.TargetHost == null)
			{
				throw new ArgumentException("TargetHost");
			}
			if (options.TargetHost.Length == 0)
			{
				options.TargetHost = "?" + Interlocked.Increment(ref uniqueNameInteger).ToString(NumberFormatInfo.InvariantInfo);
			}
			TargetHost = options.TargetHost;
		}
		if (lastException != null)
		{
			lastException.Throw();
		}
		AsyncHandshakeRequest asyncHandshakeRequest = new AsyncHandshakeRequest(this, runSynchronously);
		if (Interlocked.CompareExchange(ref this.asyncHandshakeRequest, asyncHandshakeRequest, null) != null)
		{
			throw GetInvalidNestedCallException();
		}
		if (Interlocked.CompareExchange(ref asyncReadRequest, asyncHandshakeRequest, null) != null)
		{
			throw GetInvalidNestedCallException();
		}
		if (Interlocked.CompareExchange(ref asyncWriteRequest, asyncHandshakeRequest, null) != null)
		{
			throw GetInvalidNestedCallException();
		}
		AsyncProtocolResult asyncProtocolResult;
		try
		{
			lock (ioLock)
			{
				if (xobileTlsContext != null)
				{
					throw new InvalidOperationException();
				}
				readBuffer.Reset();
				writeBuffer.Reset();
				xobileTlsContext = CreateContext(options);
			}
			try
			{
				asyncProtocolResult = await asyncHandshakeRequest.StartOperation(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception e)
			{
				asyncProtocolResult = new AsyncProtocolResult(SetException(GetSSPIException(e)));
			}
		}
		finally
		{
			lock (ioLock)
			{
				readBuffer.Reset();
				writeBuffer.Reset();
				asyncWriteRequest = null;
				asyncReadRequest = null;
				this.asyncHandshakeRequest = null;
			}
		}
		if (asyncProtocolResult.Error != null)
		{
			asyncProtocolResult.Error.Throw();
		}
	}

	protected abstract MobileTlsContext CreateContext(MonoSslAuthenticationOptions options);

	public override int Read(byte[] buffer, int offset, int count)
	{
		AsyncReadRequest asyncRequest = new AsyncReadRequest(this, sync: true, buffer, offset, count);
		return StartOperation(OperationType.Read, asyncRequest, CancellationToken.None).Result;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		AsyncWriteRequest asyncRequest = new AsyncWriteRequest(this, sync: true, buffer, offset, count);
		StartOperation(OperationType.Write, asyncRequest, CancellationToken.None).Wait();
	}

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		AsyncReadRequest asyncRequest = new AsyncReadRequest(this, sync: false, buffer, offset, count);
		return StartOperation(OperationType.Read, asyncRequest, cancellationToken);
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		AsyncWriteRequest asyncRequest = new AsyncWriteRequest(this, sync: false, buffer, offset, count);
		return StartOperation(OperationType.Write, asyncRequest, cancellationToken);
	}

	public Task RenegotiateAsync(CancellationToken cancellationToken)
	{
		AsyncRenegotiateRequest asyncRequest = new AsyncRenegotiateRequest(this);
		return StartOperation(OperationType.Renegotiate, asyncRequest, cancellationToken);
	}

	private async Task<int> StartOperation(OperationType type, AsyncProtocolRequest asyncRequest, CancellationToken cancellationToken)
	{
		CheckThrow(authSuccessCheck: true, type != OperationType.Read);
		switch (type)
		{
		case OperationType.Read:
			if (Interlocked.CompareExchange(ref asyncReadRequest, asyncRequest, null) != null)
			{
				throw GetInvalidNestedCallException();
			}
			break;
		case OperationType.Renegotiate:
			if (Interlocked.CompareExchange(ref asyncHandshakeRequest, asyncRequest, null) != null)
			{
				throw GetInvalidNestedCallException();
			}
			if (Interlocked.CompareExchange(ref asyncReadRequest, asyncRequest, null) != null)
			{
				throw GetInvalidNestedCallException();
			}
			if (Interlocked.CompareExchange(ref asyncWriteRequest, asyncRequest, null) != null)
			{
				throw GetInvalidNestedCallException();
			}
			break;
		default:
			if (Interlocked.CompareExchange(ref asyncWriteRequest, asyncRequest, null) != null)
			{
				throw GetInvalidNestedCallException();
			}
			break;
		}
		AsyncProtocolResult asyncProtocolResult;
		try
		{
			lock (ioLock)
			{
				if (type == OperationType.Read)
				{
					readBuffer.Reset();
				}
				else
				{
					writeBuffer.Reset();
				}
			}
			asyncProtocolResult = await asyncRequest.StartOperation(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception e)
		{
			asyncProtocolResult = new AsyncProtocolResult(SetException(GetIOException(e, asyncRequest.Name + " failed")));
		}
		finally
		{
			lock (ioLock)
			{
				switch (type)
				{
				case OperationType.Read:
					readBuffer.Reset();
					asyncReadRequest = null;
					break;
				case OperationType.Renegotiate:
					readBuffer.Reset();
					writeBuffer.Reset();
					asyncHandshakeRequest = null;
					asyncReadRequest = null;
					asyncWriteRequest = null;
					break;
				default:
					writeBuffer.Reset();
					asyncWriteRequest = null;
					break;
				}
			}
		}
		if (asyncProtocolResult.Error != null)
		{
			asyncProtocolResult.Error.Throw();
		}
		return asyncProtocolResult.UserResult;
	}

	[Conditional("MONO_TLS_DEBUG")]
	protected internal void Debug(string format, params object[] args)
	{
	}

	[Conditional("MONO_TLS_DEBUG")]
	protected internal void Debug(string message)
	{
	}

	internal int InternalRead(byte[] buffer, int offset, int size, out bool outWantMore)
	{
		try
		{
			AsyncProtocolRequest asyncRequest = asyncHandshakeRequest ?? asyncReadRequest;
			(int, bool) tuple = InternalRead(asyncRequest, readBuffer, buffer, offset, size);
			int item = tuple.Item1;
			bool item2 = tuple.Item2;
			outWantMore = item2;
			return item;
		}
		catch (Exception e)
		{
			SetException(GetIOException(e, "InternalRead() failed"));
			outWantMore = false;
			return -1;
		}
	}

	private (int, bool) InternalRead(AsyncProtocolRequest asyncRequest, BufferOffsetSize internalBuffer, byte[] buffer, int offset, int size)
	{
		if (asyncRequest == null)
		{
			throw new InvalidOperationException();
		}
		if (internalBuffer.Size == 0 && !internalBuffer.Complete)
		{
			internalBuffer.Offset = (internalBuffer.Size = 0);
			asyncRequest.RequestRead(size);
			return (0, true);
		}
		int num = System.Math.Min(internalBuffer.Size, size);
		Buffer.BlockCopy(internalBuffer.Buffer, internalBuffer.Offset, buffer, offset, num);
		internalBuffer.Offset += num;
		internalBuffer.Size -= num;
		return (num, !internalBuffer.Complete && num < size);
	}

	internal bool InternalWrite(byte[] buffer, int offset, int size)
	{
		try
		{
			AsyncProtocolRequest asyncProtocolRequest;
			switch (operation)
			{
			case Operation.Handshake:
			case Operation.Renegotiate:
				asyncProtocolRequest = asyncHandshakeRequest;
				break;
			case Operation.Write:
			case Operation.Close:
				asyncProtocolRequest = asyncWriteRequest;
				break;
			case Operation.Read:
				asyncProtocolRequest = asyncReadRequest;
				if (!xobileTlsContext.PendingRenegotiation())
				{
				}
				break;
			default:
				throw GetInternalError();
			}
			if (asyncProtocolRequest == null && operation != Operation.Close)
			{
				throw GetInternalError();
			}
			return InternalWrite(asyncProtocolRequest, writeBuffer, buffer, offset, size);
		}
		catch (Exception e)
		{
			SetException(GetIOException(e, "InternalWrite() failed"));
			return false;
		}
	}

	private bool InternalWrite(AsyncProtocolRequest asyncRequest, BufferOffsetSize2 internalBuffer, byte[] buffer, int offset, int size)
	{
		if (asyncRequest == null)
		{
			if (lastException != null)
			{
				return false;
			}
			if (Interlocked.Exchange(ref closeRequested, 1) == 0)
			{
				internalBuffer.Reset();
			}
			else if (internalBuffer.Remaining == 0)
			{
				throw new InvalidOperationException();
			}
		}
		internalBuffer.AppendData(buffer, offset, size);
		asyncRequest?.RequestWrite();
		return true;
	}

	internal async Task<int> InnerRead(bool sync, int requestedSize, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		int len = System.Math.Min(readBuffer.Remaining, requestedSize);
		if (len == 0)
		{
			throw new InvalidOperationException();
		}
		Task<int> task = ((!sync) ? base.InnerStream.ReadAsync(readBuffer.Buffer, readBuffer.EndOffset, len, cancellationToken) : Task.Run(() => base.InnerStream.Read(readBuffer.Buffer, readBuffer.EndOffset, len)));
		int num = await task.ConfigureAwait(continueOnCapturedContext: false);
		if (num >= 0)
		{
			readBuffer.Size += num;
			readBuffer.TotalBytes += num;
		}
		if (num == 0)
		{
			readBuffer.Complete = true;
			if (readBuffer.TotalBytes > 0)
			{
				num = -1;
			}
		}
		return num;
	}

	internal async Task InnerWrite(bool sync, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (writeBuffer.Size != 0)
		{
			Task task = ((!sync) ? base.InnerStream.WriteAsync(writeBuffer.Buffer, writeBuffer.Offset, writeBuffer.Size) : Task.Run(delegate
			{
				base.InnerStream.Write(writeBuffer.Buffer, writeBuffer.Offset, writeBuffer.Size);
			}));
			await task.ConfigureAwait(continueOnCapturedContext: false);
			writeBuffer.TotalBytes += writeBuffer.Size;
			writeBuffer.Offset = (writeBuffer.Size = 0);
		}
	}

	internal AsyncOperationStatus ProcessHandshake(AsyncOperationStatus status, bool renegotiate)
	{
		lock (ioLock)
		{
			switch (operation)
			{
			case Operation.None:
				if (renegotiate)
				{
					throw GetInternalError();
				}
				operation = Operation.Handshake;
				break;
			case Operation.Authenticated:
				if (!renegotiate)
				{
					throw GetInternalError();
				}
				operation = Operation.Renegotiate;
				break;
			default:
				throw GetInternalError();
			case Operation.Handshake:
			case Operation.Renegotiate:
				break;
			}
			switch (status)
			{
			case AsyncOperationStatus.Initialize:
				if (renegotiate)
				{
					xobileTlsContext.Renegotiate();
				}
				else
				{
					xobileTlsContext.StartHandshake();
				}
				return AsyncOperationStatus.Continue;
			case AsyncOperationStatus.ReadDone:
				throw new IOException("Authentication failed because the remote party has closed the transport stream.");
			default:
				throw new InvalidOperationException();
			case AsyncOperationStatus.Continue:
			{
				AsyncOperationStatus result = AsyncOperationStatus.Continue;
				try
				{
					if (xobileTlsContext.ProcessHandshake())
					{
						xobileTlsContext.FinishHandshake();
						operation = Operation.Authenticated;
						result = AsyncOperationStatus.Complete;
					}
				}
				catch (Exception e)
				{
					SetException(GetSSPIException(e));
					Dispose();
					throw;
				}
				if (lastException != null)
				{
					lastException.Throw();
				}
				return result;
			}
			}
		}
	}

	internal (int ret, bool wantMore) ProcessRead(BufferOffsetSize userBuffer)
	{
		lock (ioLock)
		{
			if (operation != Operation.Authenticated)
			{
				throw GetInternalError();
			}
			operation = Operation.Read;
			(int ret, bool wantMore) result = xobileTlsContext.Read(userBuffer.Buffer, userBuffer.Offset, userBuffer.Size);
			if (lastException != null)
			{
				lastException.Throw();
			}
			operation = Operation.Authenticated;
			return result;
		}
	}

	internal (int ret, bool wantMore) ProcessWrite(BufferOffsetSize userBuffer)
	{
		lock (ioLock)
		{
			if (operation != Operation.Authenticated)
			{
				throw GetInternalError();
			}
			operation = Operation.Write;
			(int ret, bool wantMore) result = xobileTlsContext.Write(userBuffer.Buffer, userBuffer.Offset, userBuffer.Size);
			if (lastException != null)
			{
				lastException.Throw();
			}
			operation = Operation.Authenticated;
			return result;
		}
	}

	internal AsyncOperationStatus ProcessShutdown(AsyncOperationStatus status)
	{
		lock (ioLock)
		{
			if (operation != Operation.Authenticated)
			{
				throw GetInternalError();
			}
			operation = Operation.Close;
			xobileTlsContext.Shutdown();
			shutdown = true;
			operation = Operation.Authenticated;
			return AsyncOperationStatus.Complete;
		}
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			lock (ioLock)
			{
				SetException(new ObjectDisposedException("MobileAuthenticatedStream"));
				if (xobileTlsContext != null)
				{
					xobileTlsContext.Dispose();
					xobileTlsContext = null;
				}
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	public override void Flush()
	{
		base.InnerStream.Flush();
	}

	public MonoTlsConnectionInfo GetConnectionInfo()
	{
		lock (ioLock)
		{
			CheckThrow(authSuccessCheck: true);
			return xobileTlsContext.ConnectionInfo;
		}
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		base.InnerStream.SetLength(value);
	}
}
