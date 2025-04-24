namespace System.Security.Cryptography.X509Certificates;

public enum TrustStatus
{
	Untrusted = 0,
	UnknownIdentity = 1,
	KnownIdentity = 2,
	Trusted = 3
}
