namespace System.Net;

internal enum ChainPolicyType
{
	Base = 1,
	Authenticode = 2,
	Authenticode_TS = 3,
	SSL = 4,
	BasicConstraints = 5,
	NtAuth = 6
}
