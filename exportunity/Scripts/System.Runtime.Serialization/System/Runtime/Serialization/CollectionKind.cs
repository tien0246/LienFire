namespace System.Runtime.Serialization;

internal enum CollectionKind : byte
{
	None = 0,
	GenericDictionary = 1,
	Dictionary = 2,
	GenericList = 3,
	GenericCollection = 4,
	List = 5,
	GenericEnumerable = 6,
	Collection = 7,
	Enumerable = 8,
	Array = 9
}
