using System.Runtime.InteropServices;

namespace System.EnterpriseServices.Internal;

[Guid("d8013ef1-730b-45e2-ba24-874b7242c425")]
public class IISVirtualRoot : IComSoapIISVRoot
{
	[System.MonoTODO]
	public IISVirtualRoot()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void Create(string RootWeb, string inPhysicalDirectory, string VirtualDirectory, out string Error)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void Delete(string RootWeb, string PhysicalDirectory, string VirtualDirectory, out string Error)
	{
		throw new NotImplementedException();
	}
}
