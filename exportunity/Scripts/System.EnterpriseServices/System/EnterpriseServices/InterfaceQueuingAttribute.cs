using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
[ComVisible(false)]
public sealed class InterfaceQueuingAttribute : Attribute
{
	private bool enabled;

	private string interfaceName;

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

	public string Interface
	{
		get
		{
			return interfaceName;
		}
		set
		{
			interfaceName = value;
		}
	}

	public InterfaceQueuingAttribute()
		: this(enabled: true)
	{
	}

	public InterfaceQueuingAttribute(bool enabled)
	{
		this.enabled = enabled;
		interfaceName = null;
	}
}
