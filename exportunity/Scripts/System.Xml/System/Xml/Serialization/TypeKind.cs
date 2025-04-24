namespace System.Xml.Serialization;

internal enum TypeKind
{
	Root = 0,
	Primitive = 1,
	Enum = 2,
	Struct = 3,
	Class = 4,
	Array = 5,
	Collection = 6,
	Enumerable = 7,
	Void = 8,
	Node = 9,
	Attribute = 10,
	Serializable = 11
}
