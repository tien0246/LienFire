namespace System.Data.SqlClient;

internal enum TransactionType
{
	LocalFromTSQL = 1,
	LocalFromAPI = 2,
	Delegated = 3,
	Distributed = 4,
	Context = 5
}
