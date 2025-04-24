using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class ConstructionEnabledAttribute : Attribute
{
	private string def;

	private bool enabled;

	public string Default
	{
		get
		{
			return def;
		}
		set
		{
			def = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	public ConstructionEnabledAttribute()
	{
		def = string.Empty;
		enabled = true;
	}

	public ConstructionEnabledAttribute(bool val)
	{
		def = string.Empty;
		enabled = val;
	}
}
