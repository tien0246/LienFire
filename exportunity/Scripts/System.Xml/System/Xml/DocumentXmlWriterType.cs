namespace System.Xml;

internal enum DocumentXmlWriterType
{
	InsertSiblingAfter = 0,
	InsertSiblingBefore = 1,
	PrependChild = 2,
	AppendChild = 3,
	AppendAttribute = 4,
	ReplaceToFollowingSibling = 5
}
