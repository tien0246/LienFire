namespace Mono.Net.Dns;

internal enum ResolverError
{
	NoError = 0,
	FormatError = 1,
	ServerFailure = 2,
	NameError = 3,
	NotImplemented = 4,
	Refused = 5,
	ResponseHeaderError = 6,
	ResponseFormatError = 7,
	Timeout = 8
}
