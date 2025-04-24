namespace System.Net;

internal enum HttpProcessingResult
{
	Continue = 0,
	ReadWait = 1,
	WriteWait = 2
}
