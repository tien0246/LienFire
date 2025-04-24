using System.Collections;

namespace System.EnterpriseServices;

public sealed class SecurityCallers : IEnumerable
{
	public int Count
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	public SecurityIdentity this[int idx]
	{
		[System.MonoTODO]
		get
		{
			throw new NotImplementedException();
		}
	}

	internal SecurityCallers()
	{
	}

	internal SecurityCallers(ISecurityCallersColl collection)
	{
	}

	[System.MonoTODO]
	public IEnumerator GetEnumerator()
	{
		throw new NotImplementedException();
	}
}
