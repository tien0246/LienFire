namespace System.Data.SqlClient;

internal enum SqlConnectionTimeoutErrorPhase
{
	Undefined = 0,
	PreLoginBegin = 1,
	InitializeConnection = 2,
	SendPreLoginHandshake = 3,
	ConsumePreLoginHandshake = 4,
	LoginBegin = 5,
	ProcessConnectionAuth = 6,
	PostLogin = 7,
	Complete = 8,
	Count = 9
}
