using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

internal struct ContentInfoAsn
{
	[ObjectIdentifier]
	public string ContentType;

	[ExpectedTag(0, ExplicitTag = true)]
	[AnyValue]
	public ReadOnlyMemory<byte> Content;
}
