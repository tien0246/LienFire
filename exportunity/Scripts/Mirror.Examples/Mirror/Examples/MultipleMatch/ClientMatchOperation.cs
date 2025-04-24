namespace Mirror.Examples.MultipleMatch;

public enum ClientMatchOperation : byte
{
	None = 0,
	List = 1,
	Created = 2,
	Cancelled = 3,
	Joined = 4,
	Departed = 5,
	UpdateRoom = 6,
	Started = 7
}
