namespace System.Xml.Xsl.Runtime;

internal enum XmlState
{
	WithinSequence = 0,
	EnumAttrs = 1,
	WithinContent = 2,
	WithinAttr = 3,
	WithinNmsp = 4,
	WithinComment = 5,
	WithinPI = 6
}
