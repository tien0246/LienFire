namespace System.EnterpriseServices.CompensatingResourceManager;

[Serializable]
[Flags]
public enum CompensatorOptions
{
	PreparePhase = 1,
	CommitPhase = 2,
	AbortPhase = 4,
	AllPhases = 7,
	FailIfInDoubtsRemain = 0x10
}
