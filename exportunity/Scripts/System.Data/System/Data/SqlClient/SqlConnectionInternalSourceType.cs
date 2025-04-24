namespace System.Data.SqlClient;

internal enum SqlConnectionInternalSourceType
{
	Principle = 0,
	Failover = 1,
	RoutingDestination = 2
}
