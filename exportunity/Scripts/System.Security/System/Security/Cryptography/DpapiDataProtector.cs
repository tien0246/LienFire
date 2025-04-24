using System.Runtime.CompilerServices;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography;

public sealed class DpapiDataProtector : DataProtector
{
	public DataProtectionScope Scope
	{
		[CompilerGenerated]
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default(DataProtectionScope);
		}
		[CompilerGenerated]
		set
		{
			Unity.ThrowStub.ThrowNotSupportedException();
		}
	}

	[SecuritySafeCritical]
	[DataProtectionPermission(SecurityAction.Demand, Unrestricted = true)]
	public DpapiDataProtector(string appName, string primaryPurpose, string[] specificPurpose)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}

	public override bool IsReprotectRequired(byte[] encryptedData)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
		return default(bool);
	}

	[SecuritySafeCritical]
	[DataProtectionPermission(SecurityAction.Assert, ProtectData = true)]
	protected override byte[] ProviderProtect(byte[] userData)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
		return null;
	}

	[SecuritySafeCritical]
	[DataProtectionPermission(SecurityAction.Assert, UnprotectData = true)]
	protected override byte[] ProviderUnprotect(byte[] encryptedData)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
		return null;
	}
}
