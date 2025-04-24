using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
[Flags]
public enum SecurityPermissionFlag
{
	NoFlags = 0,
	Assertion = 1,
	UnmanagedCode = 2,
	SkipVerification = 4,
	Execution = 8,
	ControlThread = 0x10,
	ControlEvidence = 0x20,
	ControlPolicy = 0x40,
	SerializationFormatter = 0x80,
	ControlDomainPolicy = 0x100,
	ControlPrincipal = 0x200,
	ControlAppDomain = 0x400,
	RemotingConfiguration = 0x800,
	Infrastructure = 0x1000,
	BindingRedirects = 0x2000,
	AllFlags = 0x3FFF
}
