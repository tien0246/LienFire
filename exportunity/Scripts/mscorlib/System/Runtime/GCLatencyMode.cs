namespace System.Runtime;

[Serializable]
public enum GCLatencyMode
{
	Batch = 0,
	Interactive = 1,
	LowLatency = 2,
	SustainedLowLatency = 3,
	NoGCRegion = 4
}
