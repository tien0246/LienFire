namespace System.Xml;

internal enum MimeWriterState
{
	Start = 0,
	StartPreface = 1,
	StartPart = 2,
	Header = 3,
	Content = 4,
	Closed = 5
}
