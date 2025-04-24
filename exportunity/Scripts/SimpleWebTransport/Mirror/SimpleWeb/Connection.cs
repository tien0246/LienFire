using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Mirror.SimpleWeb;

internal sealed class Connection : IDisposable
{
	public const int IdNotSet = -1;

	private readonly object disposedLock = new object();

	public TcpClient client;

	public int connId = -1;

	public Stream stream;

	public Thread receiveThread;

	public Thread sendThread;

	public ManualResetEventSlim sendPending = new ManualResetEventSlim(initialState: false);

	public ConcurrentQueue<ArrayBuffer> sendQueue = new ConcurrentQueue<ArrayBuffer>();

	public Action<Connection> onDispose;

	private volatile bool hasDisposed;

	public Connection(TcpClient client, Action<Connection> onDispose)
	{
		this.client = client ?? throw new ArgumentNullException("client");
		this.onDispose = onDispose;
	}

	public void Dispose()
	{
		if (hasDisposed)
		{
			return;
		}
		lock (disposedLock)
		{
			if (!hasDisposed)
			{
				hasDisposed = true;
				receiveThread.Interrupt();
				sendThread?.Interrupt();
				try
				{
					stream?.Dispose();
					stream = null;
					client.Dispose();
					client = null;
				}
				catch (Exception e)
				{
					Log.Exception(e);
				}
				sendPending.Dispose();
				ArrayBuffer result;
				while (sendQueue.TryDequeue(out result))
				{
					result.Release();
				}
				onDispose(this);
			}
		}
	}

	public override string ToString()
	{
		if (hasDisposed)
		{
			return $"[Conn:{connId}, Disposed]";
		}
		try
		{
			EndPoint arg = client?.Client?.RemoteEndPoint;
			return $"[Conn:{connId}, endPoint:{arg}]";
		}
		catch (SocketException)
		{
			return $"[Conn:{connId}, endPoint:n/a]";
		}
	}
}
