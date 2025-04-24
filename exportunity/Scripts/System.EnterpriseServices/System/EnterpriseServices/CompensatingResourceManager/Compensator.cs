namespace System.EnterpriseServices.CompensatingResourceManager;

public class Compensator : ServicedComponent
{
	public Clerk Clerk
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	[System.MonoTODO]
	public Compensator()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual bool AbortRecord(LogRecord rec)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual void BeginAbort(bool fRecovery)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual void BeginCommit(bool fRecovery)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual void BeginPrepare()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual bool CommitRecord(LogRecord rec)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual void EndAbort()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual void EndCommit()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual bool EndPrepare()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual bool PrepareRecord(LogRecord rec)
	{
		throw new NotImplementedException();
	}
}
