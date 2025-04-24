using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[Serializable]
[ComVisible(false)]
public enum BindingOption
{
	NoBinding = 0,
	BindingToPoolThread = 1
}
