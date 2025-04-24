namespace System.Xml.Xsl.IlGen;

internal enum PossibleXmlStates
{
	None = 0,
	WithinSequence = 1,
	EnumAttrs = 2,
	WithinContent = 3,
	WithinAttr = 4,
	WithinComment = 5,
	WithinPI = 6,
	Any = 7
}
