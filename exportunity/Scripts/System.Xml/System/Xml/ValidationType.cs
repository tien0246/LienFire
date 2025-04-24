namespace System.Xml;

public enum ValidationType
{
	None = 0,
	[Obsolete("Validation type should be specified as DTD or Schema.")]
	Auto = 1,
	DTD = 2,
	[Obsolete("XDR Validation through XmlValidatingReader is obsoleted")]
	XDR = 3,
	Schema = 4
}
