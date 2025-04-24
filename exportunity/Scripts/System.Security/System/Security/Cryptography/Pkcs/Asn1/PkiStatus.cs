namespace System.Security.Cryptography.Pkcs.Asn1;

internal enum PkiStatus
{
	Granted = 0,
	GrantedWithMods = 1,
	Rejection = 2,
	Waiting = 3,
	RevocationWarning = 4,
	RevocationNotification = 5,
	KeyUpdateWarning = 6
}
