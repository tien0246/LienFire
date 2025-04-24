namespace System.Net;

internal enum WebParseErrorCode
{
	Generic = 0,
	InvalidHeaderName = 1,
	InvalidContentLength = 2,
	IncompleteHeaderLine = 3,
	CrLfError = 4,
	InvalidChunkFormat = 5,
	UnexpectedServerResponse = 6
}
