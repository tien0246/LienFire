using System.Runtime.InteropServices;

namespace System.EnterpriseServices.Internal;

[Guid("CAA817CC-0C04-4d22-A05C-2B7E162F4E8F")]
public sealed class SoapServerVRoot : ISoapServerVRoot
{
	[System.MonoTODO]
	public SoapServerVRoot()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void CreateVirtualRootEx(string rootWebServer, string inBaseUrl, string inVirtualRoot, string homePage, string discoFile, string secureSockets, string authentication, string operation, out string baseUrl, out string virtualRoot, out string physicalPath)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void DeleteVirtualRootEx(string rootWebServer, string inBaseUrl, string inVirtualRoot)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void GetVirtualRootStatus(string RootWebServer, string inBaseUrl, string inVirtualRoot, out string Exists, out string SSL, out string WindowsAuth, out string Anonymous, out string HomePage, out string DiscoFile, out string PhysicalPath, out string BaseUrl, out string VirtualRoot)
	{
		throw new NotImplementedException();
	}
}
