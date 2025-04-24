namespace System.Data.SqlClient;

internal enum SniContext
{
	Undefined = 0,
	Snix_Connect = 1,
	Snix_PreLoginBeforeSuccessfulWrite = 2,
	Snix_PreLogin = 3,
	Snix_LoginSspi = 4,
	Snix_ProcessSspi = 5,
	Snix_Login = 6,
	Snix_EnableMars = 7,
	Snix_AutoEnlist = 8,
	Snix_GetMarsSession = 9,
	Snix_Execute = 10,
	Snix_Read = 11,
	Snix_Close = 12,
	Snix_SendRows = 13
}
