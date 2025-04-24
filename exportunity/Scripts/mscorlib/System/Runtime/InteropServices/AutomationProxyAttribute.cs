namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
public sealed class AutomationProxyAttribute : Attribute
{
	internal bool _val;

	public bool Value => _val;

	public AutomationProxyAttribute(bool val)
	{
		_val = val;
	}
}
