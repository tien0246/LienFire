namespace System.Data.SqlClient;

internal enum TransactionState
{
	Pending = 0,
	Active = 1,
	Aborted = 2,
	Committed = 3,
	Unknown = 4
}
