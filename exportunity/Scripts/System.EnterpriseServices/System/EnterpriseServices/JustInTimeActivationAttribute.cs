using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Class)]
[ComVisible(false)]
public sealed class JustInTimeActivationAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public JustInTimeActivationAttribute()
		: this(val: true)
	{
	}

	public JustInTimeActivationAttribute(bool val)
	{
		this.val = val;
	}
}
