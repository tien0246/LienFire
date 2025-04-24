namespace Mono.Btls;

internal enum MonoBtlsX509Purpose
{
	SSL_CLIENT = 1,
	SSL_SERVER = 2,
	NS_SSL_SERVER = 3,
	SMIME_SIGN = 4,
	SMIME_ENCRYPT = 5,
	CRL_SIGN = 6,
	ANY = 7,
	OCSP_HELPER = 8,
	TIMESTAMP_SIGN = 9
}
