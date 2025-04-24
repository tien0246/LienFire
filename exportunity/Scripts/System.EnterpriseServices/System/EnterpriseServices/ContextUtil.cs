using System.Transactions;

namespace System.EnterpriseServices;

public sealed class ContextUtil
{
	private static bool deactivateOnReturn;

	private static TransactionVote myTransactionVote;

	public static Guid ActivityId
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public static Guid ApplicationId
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public static Guid ApplicationInstanceId
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public static Guid ContextId
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public static bool DeactivateOnReturn
	{
		get
		{
			return deactivateOnReturn;
		}
		set
		{
			deactivateOnReturn = value;
		}
	}

	public static bool IsInTransaction
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public static bool IsSecurityEnabled
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	[System.MonoTODO]
	public static TransactionVote MyTransactionVote
	{
		get
		{
			return myTransactionVote;
		}
		set
		{
			myTransactionVote = value;
		}
	}

	public static Guid PartitionId
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public static object Transaction
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public static Transaction SystemTransaction
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public static Guid TransactionId
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	internal ContextUtil()
	{
	}

	[System.MonoTODO]
	public static void DisableCommit()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public static void EnableCommit()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public static object GetNamedProperty(string name)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public static bool IsCallerInRole(string role)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public static bool IsDefaultContext()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public static void SetAbort()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public static void SetComplete()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public static void SetNamedProperty(string name, object value)
	{
		throw new NotImplementedException();
	}
}
