namespace System.Xml.Schema;

internal enum ValidatorState
{
	None = 0,
	Start = 1,
	TopLevelAttribute = 2,
	TopLevelTextOrWS = 3,
	Element = 4,
	Attribute = 5,
	EndOfAttributes = 6,
	Text = 7,
	Whitespace = 8,
	EndElement = 9,
	SkipToEndElement = 10,
	Finish = 11
}
