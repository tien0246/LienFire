using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Class)]
[ComVisible(false)]
public sealed class MustRunInClientContextAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public MustRunInClientContextAttribute()
		: this(val: true)
	{
	}

	public MustRunInClientContextAttribute(bool val)
	{
		this.val = val;
	}
}
