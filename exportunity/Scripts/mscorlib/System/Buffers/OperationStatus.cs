namespace System.Buffers;

public enum OperationStatus
{
	Done = 0,
	DestinationTooSmall = 1,
	NeedMoreData = 2,
	InvalidData = 3
}
