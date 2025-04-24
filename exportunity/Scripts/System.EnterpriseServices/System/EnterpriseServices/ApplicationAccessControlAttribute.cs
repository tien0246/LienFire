using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ApplicationAccessControlAttribute : Attribute, IConfigurationAttribute
{
	private AccessChecksLevelOption accessChecksLevel;

	private AuthenticationOption authentication;

	private ImpersonationLevelOption impersonation;

	private bool val;

	public AccessChecksLevelOption AccessChecksLevel
	{
		get
		{
			return accessChecksLevel;
		}
		set
		{
			accessChecksLevel = value;
		}
	}

	public AuthenticationOption Authentication
	{
		get
		{
			return authentication;
		}
		set
		{
			authentication = value;
		}
	}

	public ImpersonationLevelOption ImpersonationLevel
	{
		get
		{
			return impersonation;
		}
		set
		{
			impersonation = value;
		}
	}

	public bool Value
	{
		get
		{
			return val;
		}
		set
		{
			val = value;
		}
	}

	public ApplicationAccessControlAttribute()
	{
		val = false;
	}

	public ApplicationAccessControlAttribute(bool val)
	{
		this.val = val;
	}

	bool IConfigurationAttribute.AfterSaveChanges(Hashtable info)
	{
		return false;
	}

	[System.MonoTODO]
	bool IConfigurationAttribute.Apply(Hashtable cache)
	{
		throw new NotImplementedException();
	}

	bool IConfigurationAttribute.IsValidTarget(string s)
	{
		return s == "Application";
	}
}
