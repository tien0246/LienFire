using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Unity;

namespace System;

[Serializable]
[ComVisible(false)]
public sealed class ActivationContext : IDisposable, ISerializable
{
	public enum ContextForm
	{
		Loose = 0,
		StoreBounded = 1
	}

	private ApplicationIdentity _appid;

	private ContextForm _form;

	private bool _disposed;

	public ContextForm Form => _form;

	public ApplicationIdentity Identity => _appid;

	public byte[] ApplicationManifestBytes
	{
		get
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}

	public byte[] DeploymentManifestBytes
	{
		get
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}

	private ActivationContext(ApplicationIdentity identity)
	{
		_appid = identity;
	}

	~ActivationContext()
	{
		Dispose(disposing: false);
	}

	[MonoTODO("Missing validation")]
	public static ActivationContext CreatePartialActivationContext(ApplicationIdentity identity)
	{
		if (identity == null)
		{
			throw new ArgumentNullException("identity");
		}
		return new ActivationContext(identity);
	}

	[MonoTODO("Missing validation")]
	public static ActivationContext CreatePartialActivationContext(ApplicationIdentity identity, string[] manifestPaths)
	{
		if (identity == null)
		{
			throw new ArgumentNullException("identity");
		}
		if (manifestPaths == null)
		{
			throw new ArgumentNullException("manifestPaths");
		}
		return new ActivationContext(identity);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (_disposed)
		{
			_disposed = true;
		}
	}

	[MonoTODO("Missing serialization support")]
	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
	}

	internal ActivationContext()
	{
		ThrowStub.ThrowNotSupportedException();
	}
}
