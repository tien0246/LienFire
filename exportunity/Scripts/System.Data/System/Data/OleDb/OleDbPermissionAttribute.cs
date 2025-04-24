using System.ComponentModel;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.OleDb;

[Serializable]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class OleDbPermissionAttribute : DBDataPermissionAttribute
{
	private string _providers;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Provider property has been deprecated.  Use the Add method.  http://go.microsoft.com/fwlink/?linkid=14202")]
	[Browsable(false)]
	public string Provider
	{
		get
		{
			string providers = _providers;
			if (providers == null)
			{
				return ADP.StrEmpty;
			}
			return providers;
		}
		set
		{
			_providers = value;
		}
	}

	public OleDbPermissionAttribute(SecurityAction action)
		: base(action)
	{
	}

	public override IPermission CreatePermission()
	{
		return new OleDbPermission(this);
	}
}
