namespace kcp2k;

public enum ErrorCode : byte
{
	DnsResolve = 0,
	Timeout = 1,
	Congestion = 2,
	InvalidReceive = 3,
	InvalidSend = 4,
	ConnectionClosed = 5,
	Unexpected = 6
}
