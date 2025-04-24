using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Odbc;

[Serializable]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class OdbcPermissionAttribute : DBDataPermissionAttribute
{
	public OdbcPermissionAttribute(SecurityAction action)
		: base(action)
	{
	}

	public override IPermission CreatePermission()
	{
		return new OdbcPermission(this);
	}
}
