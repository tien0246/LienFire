namespace System.EnterpriseServices.CompensatingResourceManager;

[Serializable]
public enum TransactionState
{
	Active = 0,
	Committed = 1,
	Aborted = 2,
	Indoubt = 3
}
