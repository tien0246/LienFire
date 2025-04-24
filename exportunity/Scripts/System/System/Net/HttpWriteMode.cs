namespace System.Net;

internal enum HttpWriteMode
{
	Unknown = 0,
	ContentLength = 1,
	Chunked = 2,
	Buffer = 3,
	None = 4
}
