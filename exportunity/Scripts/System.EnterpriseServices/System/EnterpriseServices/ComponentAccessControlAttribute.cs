using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class ComponentAccessControlAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public ComponentAccessControlAttribute()
	{
		val = false;
	}

	public ComponentAccessControlAttribute(bool val)
	{
		this.val = val;
	}
}
