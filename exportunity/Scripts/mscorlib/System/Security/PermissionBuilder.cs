using System.Security.Permissions;

namespace System.Security;

internal static class PermissionBuilder
{
	private static object[] psNone = new object[1] { PermissionState.None };

	public static IPermission Create(string fullname, PermissionState state)
	{
		if (fullname == null)
		{
			throw new ArgumentNullException("fullname");
		}
		SecurityElement securityElement = new SecurityElement("IPermission");
		securityElement.AddAttribute("class", fullname);
		securityElement.AddAttribute("version", "1");
		if (state == PermissionState.Unrestricted)
		{
			securityElement.AddAttribute("Unrestricted", "true");
		}
		return CreatePermission(fullname, securityElement);
	}

	public static IPermission Create(SecurityElement se)
	{
		if (se == null)
		{
			throw new ArgumentNullException("se");
		}
		string text = se.Attribute("class");
		if (text == null || text.Length == 0)
		{
			throw new ArgumentException("class");
		}
		return CreatePermission(text, se);
	}

	public static IPermission Create(string fullname, SecurityElement se)
	{
		if (fullname == null)
		{
			throw new ArgumentNullException("fullname");
		}
		if (se == null)
		{
			throw new ArgumentNullException("se");
		}
		return CreatePermission(fullname, se);
	}

	public static IPermission Create(Type type)
	{
		return (IPermission)Activator.CreateInstance(type, psNone);
	}

	internal static IPermission CreatePermission(string fullname, SecurityElement se)
	{
		Type type = Type.GetType(fullname);
		if (type == null)
		{
			throw new TypeLoadException(string.Format(Locale.GetText("Can't create an instance of permission class {0}."), fullname));
		}
		IPermission permission = Create(type);
		permission.FromXml(se);
		return permission;
	}
}
