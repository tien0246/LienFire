using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Class)]
[ComVisible(false)]
public sealed class IISIntrinsicsAttribute : Attribute
{
	private bool val;

	public bool Value => val;

	public IISIntrinsicsAttribute()
	{
		val = true;
	}

	public IISIntrinsicsAttribute(bool val)
	{
		this.val = val;
	}
}
