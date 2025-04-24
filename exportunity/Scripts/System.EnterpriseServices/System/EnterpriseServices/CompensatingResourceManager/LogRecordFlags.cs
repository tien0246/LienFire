namespace System.EnterpriseServices.CompensatingResourceManager;

[Serializable]
[Flags]
public enum LogRecordFlags
{
	ForgetTarget = 1,
	WrittenDuringPrepare = 2,
	WrittenDuringCommit = 4,
	WrittenDuringAbort = 8,
	WrittenDurringRecovery = 0x10,
	WrittenDuringReplay = 0x20,
	ReplayInProgress = 0x40
}
