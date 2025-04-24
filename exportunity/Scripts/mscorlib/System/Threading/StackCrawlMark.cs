namespace System.Threading;

[Serializable]
internal enum StackCrawlMark
{
	LookForMe = 0,
	LookForMyCaller = 1,
	LookForMyCallersCaller = 2,
	LookForThread = 3
}
