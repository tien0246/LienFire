namespace System.EnterpriseServices;

public sealed class ResourcePool
{
	public delegate void TransactionEndDelegate(object resource);

	[System.MonoTODO]
	public ResourcePool(TransactionEndDelegate cb)
	{
	}

	[System.MonoTODO]
	public object GetResource()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public bool PutResource(object resource)
	{
		throw new NotImplementedException();
	}
}
