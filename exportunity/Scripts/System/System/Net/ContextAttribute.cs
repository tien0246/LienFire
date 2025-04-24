namespace System.Net;

internal enum ContextAttribute
{
	Sizes = 0,
	Names = 1,
	Lifespan = 2,
	DceInfo = 3,
	StreamSizes = 4,
	Authority = 6,
	PackageInfo = 10,
	NegotiationInfo = 12,
	UniqueBindings = 25,
	EndpointBindings = 26,
	ClientSpecifiedSpn = 27,
	RemoteCertificate = 83,
	LocalCertificate = 84,
	RootStore = 85,
	IssuerListInfoEx = 89,
	ConnectionInfo = 90,
	UiInfo = 104
}
