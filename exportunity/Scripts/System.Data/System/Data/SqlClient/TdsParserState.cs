namespace System.Data.SqlClient;

internal enum TdsParserState
{
	Closed = 0,
	OpenNotLoggedIn = 1,
	OpenLoggedIn = 2,
	Broken = 3
}
