namespace System.Data.SqlClient.SNI;

internal enum SNIProviders
{
	HTTP_PROV = 0,
	NP_PROV = 1,
	SESSION_PROV = 2,
	SIGN_PROV = 3,
	SM_PROV = 4,
	SMUX_PROV = 5,
	SSL_PROV = 6,
	TCP_PROV = 7,
	MAX_PROVS = 8,
	INVALID_PROV = 9
}
