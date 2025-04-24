namespace System.Linq.Parallel;

internal enum OrdinalIndexState : byte
{
	Indexable = 0,
	Correct = 1,
	Increasing = 2,
	Shuffled = 3
}
