using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Class)]
[ComVisible(false)]
public sealed class SynchronizationAttribute : Attribute
{
	private SynchronizationOption val;

	public SynchronizationOption Value => val;

	public SynchronizationAttribute()
		: this(SynchronizationOption.Required)
	{
	}

	public SynchronizationAttribute(SynchronizationOption val)
	{
		this.val = val;
	}
}
