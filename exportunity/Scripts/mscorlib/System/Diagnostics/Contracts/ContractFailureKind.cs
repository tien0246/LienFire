namespace System.Diagnostics.Contracts;

public enum ContractFailureKind
{
	Precondition = 0,
	Postcondition = 1,
	PostconditionOnException = 2,
	Invariant = 3,
	Assert = 4,
	Assume = 5
}
