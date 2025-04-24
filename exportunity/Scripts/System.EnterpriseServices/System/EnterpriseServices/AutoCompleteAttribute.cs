using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Method)]
[ComVisible(false)]
public sealed class AutoCompleteAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public AutoCompleteAttribute()
	{
		val = true;
	}

	public AutoCompleteAttribute(bool val)
	{
		this.val = val;
	}
}
