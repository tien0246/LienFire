using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[Serializable]
[ComVisible(false)]
public enum PartitionOption
{
	Ignore = 0,
	Inherit = 1,
	New = 2
}
