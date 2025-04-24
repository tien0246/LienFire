namespace System.Runtime;

internal enum ComputerNameFormat
{
	NetBIOS = 0,
	DnsHostName = 1,
	Dns = 2,
	DnsFullyQualified = 3,
	PhysicalNetBIOS = 4,
	PhysicalDnsHostName = 5,
	PhysicalDnsDomain = 6,
	PhysicalDnsFullyQualified = 7
}
