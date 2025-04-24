using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Class)]
[ComVisible(false)]
public sealed class LoadBalancingSupportedAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public LoadBalancingSupportedAttribute()
		: this(val: true)
	{
	}

	public LoadBalancingSupportedAttribute(bool val)
	{
		this.val = val;
	}
}
