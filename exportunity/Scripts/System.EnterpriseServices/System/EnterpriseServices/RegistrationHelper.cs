using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[Guid("89a86e7b-c229-4008-9baa-2f5c8411d7e0")]
public sealed class RegistrationHelper : MarshalByRefObject, IRegistrationHelper
{
	public void InstallAssembly(string assembly, ref string application, ref string tlb, InstallationFlags installFlags)
	{
		application = string.Empty;
		tlb = string.Empty;
		InstallAssembly(assembly, ref application, null, ref tlb, installFlags);
	}

	[System.MonoTODO]
	public void InstallAssembly(string assembly, ref string application, string partition, ref string tlb, InstallationFlags installFlags)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void InstallAssemblyFromConfig([MarshalAs(UnmanagedType.IUnknown)] ref RegistrationConfig regConfig)
	{
		throw new NotImplementedException();
	}

	public void UninstallAssembly(string assembly, string application)
	{
		UninstallAssembly(assembly, application, null);
	}

	[System.MonoTODO]
	public void UninstallAssembly(string assembly, string application, string partition)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void UninstallAssemblyFromConfig([MarshalAs(UnmanagedType.IUnknown)] ref RegistrationConfig regConfig)
	{
		throw new NotImplementedException();
	}
}
