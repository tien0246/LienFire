namespace System.Net;

internal enum HttpBehaviour : byte
{
	Unknown = 0,
	HTTP10 = 1,
	HTTP11PartiallyCompliant = 2,
	HTTP11 = 3
}
