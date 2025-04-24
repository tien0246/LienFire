using System.Security;
using System.Security.Permissions;

namespace System.Drawing.Printing;

[Serializable]
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class PrintingPermissionAttribute : CodeAccessSecurityAttribute
{
	public PrintingPermissionLevel Level { get; set; }

	public PrintingPermissionAttribute(SecurityAction action)
		: base(action)
	{
	}

	public override IPermission CreatePermission()
	{
		return null;
	}
}
