namespace System.EnterpriseServices;

[Serializable]
public abstract class ServicedComponent : ContextBoundObject, IDisposable, IRemoteDispatch, IServicedComponentInfo
{
	public ServicedComponent()
	{
	}

	[System.MonoTODO]
	protected internal virtual void Activate()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected internal virtual bool CanBePooled()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected internal virtual void Construct(string s)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected internal virtual void Deactivate()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void Dispose()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected virtual void Dispose(bool disposing)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public static void DisposeObject(ServicedComponent sc)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	string IRemoteDispatch.RemoteDispatchAutoDone(string s)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	string IRemoteDispatch.RemoteDispatchNotAutoDone(string s)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	void IServicedComponentInfo.GetComponentInfo(ref int infoMask, out string[] infoArray)
	{
		throw new NotImplementedException();
	}
}
