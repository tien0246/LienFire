namespace System.Net;

internal enum DataParseStatus
{
	NeedMoreData = 0,
	ContinueParsing = 1,
	Done = 2,
	Invalid = 3,
	DataTooBig = 4
}
