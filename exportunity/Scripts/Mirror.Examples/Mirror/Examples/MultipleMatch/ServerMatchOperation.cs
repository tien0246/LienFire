namespace Mirror.Examples.MultipleMatch;

public enum ServerMatchOperation : byte
{
	None = 0,
	Create = 1,
	Cancel = 2,
	Start = 3,
	Join = 4,
	Leave = 5,
	Ready = 6
}
