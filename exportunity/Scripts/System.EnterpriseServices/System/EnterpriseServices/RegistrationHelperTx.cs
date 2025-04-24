using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[Transaction(TransactionOption.RequiresNew)]
[Guid("C89AC250-E18A-4FC7-ABD5-B8897B6A78A5")]
public sealed class RegistrationHelperTx : ServicedComponent
{
	[System.MonoTODO]
	public RegistrationHelperTx()
	{
	}

	[System.MonoTODO]
	protected internal override void Activate()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected internal override void Deactivate()
	{
		throw new NotImplementedException();
	}

	public void InstallAssembly(string assembly, ref string application, ref string tlb, InstallationFlags installFlags, object sync)
	{
		InstallAssembly(assembly, ref application, null, ref tlb, installFlags, sync);
	}

	[System.MonoTODO]
	public void InstallAssembly(string assembly, ref string application, string partition, ref string tlb, InstallationFlags installFlags, object sync)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void InstallAssemblyFromConfig([MarshalAs(UnmanagedType.IUnknown)] ref RegistrationConfig regConfig, object sync)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public bool IsInTransaction()
	{
		throw new NotImplementedException();
	}

	public void UninstallAssembly(string assembly, string application, object sync)
	{
		UninstallAssembly(assembly, application, null, sync);
	}

	[System.MonoTODO]
	public void UninstallAssembly(string assembly, string application, string partition, object sync)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void UninstallAssemblyFromConfig([MarshalAs(UnmanagedType.IUnknown)] ref RegistrationConfig regConfig, object sync)
	{
		throw new NotImplementedException();
	}
}
