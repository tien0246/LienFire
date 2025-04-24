namespace System.Net.Http;

internal enum HttpConnectionKind : byte
{
	Http = 0,
	Https = 1,
	Proxy = 2,
	ProxyTunnel = 3,
	SslProxyTunnel = 4,
	ProxyConnect = 5
}
