namespace System.Runtime.Serialization.Json;

internal enum JsonNodeType
{
	None = 0,
	Object = 1,
	Element = 2,
	EndElement = 3,
	QuotedText = 4,
	StandaloneText = 5,
	Collection = 6
}
