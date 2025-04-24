namespace System.Data.SqlClient;

public enum SqlAuthenticationMethod
{
	NotSpecified = 0,
	SqlPassword = 1,
	ActiveDirectoryPassword = 2,
	ActiveDirectoryIntegrated = 3,
	ActiveDirectoryInteractive = 4
}
