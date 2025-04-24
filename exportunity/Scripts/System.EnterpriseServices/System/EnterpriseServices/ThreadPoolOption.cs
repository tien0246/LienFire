using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[Serializable]
[ComVisible(false)]
public enum ThreadPoolOption
{
	None = 0,
	Inherit = 1,
	STA = 2,
	MTA = 3
}
