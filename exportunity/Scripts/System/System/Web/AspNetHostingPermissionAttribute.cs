using System.Security;
using System.Security.Permissions;

namespace System.Web;

[Serializable]
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public sealed class AspNetHostingPermissionAttribute : CodeAccessSecurityAttribute
{
	private AspNetHostingPermissionLevel _level;

	public AspNetHostingPermissionLevel Level
	{
		get
		{
			return _level;
		}
		set
		{
			if (value < AspNetHostingPermissionLevel.None || value > AspNetHostingPermissionLevel.Unrestricted)
			{
				throw new ArgumentException(string.Format(global::Locale.GetText("Invalid enum {0}."), value), "Level");
			}
			_level = value;
		}
	}

	public AspNetHostingPermissionAttribute(SecurityAction action)
		: base(action)
	{
		_level = AspNetHostingPermissionLevel.None;
	}

	public override IPermission CreatePermission()
	{
		if (base.Unrestricted)
		{
			return new AspNetHostingPermission(PermissionState.Unrestricted);
		}
		return new AspNetHostingPermission(_level);
	}
}
