namespace System.Data.Common;

public enum GroupByBehavior
{
	Unknown = 0,
	NotSupported = 1,
	Unrelated = 2,
	MustContainAll = 3,
	ExactMatch = 4
}
