namespace System.EnterpriseServices.CompensatingResourceManager;

public sealed class Clerk
{
	public int LogRecordCount
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public string TransactionUOW
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	[System.MonoTODO]
	public Clerk(string compensator, string description, CompensatorOptions flags)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public Clerk(Type compensator, string description, CompensatorOptions flags)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	~Clerk()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void ForceLog()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void ForceTransactionToAbort()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void ForgetLogRecord()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void WriteLogRecord(object record)
	{
		throw new NotImplementedException();
	}
}
