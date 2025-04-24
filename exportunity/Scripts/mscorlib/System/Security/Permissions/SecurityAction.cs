using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
public enum SecurityAction
{
	Demand = 2,
	Assert = 3,
	[Obsolete("This requests should not be used")]
	Deny = 4,
	PermitOnly = 5,
	LinkDemand = 6,
	InheritanceDemand = 7,
	[Obsolete("This requests should not be used")]
	RequestMinimum = 8,
	[Obsolete("This requests should not be used")]
	RequestOptional = 9,
	[Obsolete("This requests should not be used")]
	RequestRefuse = 10
}
