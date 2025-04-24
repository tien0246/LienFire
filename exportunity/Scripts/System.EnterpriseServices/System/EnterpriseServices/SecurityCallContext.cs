namespace System.EnterpriseServices;

public sealed class SecurityCallContext
{
	public SecurityCallers Callers
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public static SecurityCallContext CurrentCall
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public SecurityIdentity DirectCaller
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool IsSecurityEnabled
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public int MinAuthenticationLevel
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public int NumCallers
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public SecurityIdentity OriginalCaller
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	internal SecurityCallContext()
	{
	}

	internal SecurityCallContext(ISecurityCallContext context)
	{
	}

	[System.MonoTODO]
	public bool IsCallerInRole(string role)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public bool IsUserInRole(string user, string role)
	{
		throw new NotImplementedException();
	}
}
