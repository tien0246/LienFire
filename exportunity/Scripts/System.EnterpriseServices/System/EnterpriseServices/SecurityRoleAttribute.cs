using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
[ComVisible(false)]
public sealed class SecurityRoleAttribute : Attribute
{
	private string description;

	private bool everyone;

	private string role;

	public string Description
	{
		get
		{
			return description;
		}
		set
		{
			description = value;
		}
	}

	public string Role
	{
		get
		{
			return role;
		}
		set
		{
			role = value;
		}
	}

	public bool SetEveryoneAccess
	{
		get
		{
			return everyone;
		}
		set
		{
			everyone = value;
		}
	}

	public SecurityRoleAttribute(string role)
		: this(role, everyone: false)
	{
	}

	public SecurityRoleAttribute(string role, bool everyone)
	{
		description = string.Empty;
		this.everyone = everyone;
		this.role = role;
	}
}
