using System.Runtime.InteropServices;

namespace System.Security;

[Serializable]
[ComVisible(true)]
[Flags]
public enum HostSecurityManagerOptions
{
	None = 0,
	HostAppDomainEvidence = 1,
	HostPolicyLevel = 2,
	HostAssemblyEvidence = 4,
	HostDetermineApplicationTrust = 8,
	HostResolvePolicy = 0x10,
	AllFlags = 0x1F
}
