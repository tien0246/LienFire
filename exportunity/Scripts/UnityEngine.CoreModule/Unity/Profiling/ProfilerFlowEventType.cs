namespace Unity.Profiling;

public enum ProfilerFlowEventType : byte
{
	Begin = 0,
	ParallelNext = 1,
	End = 2,
	Next = 3
}
