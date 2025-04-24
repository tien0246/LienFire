using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

internal struct EncapsulatedContentInfoAsn
{
	[ObjectIdentifier]
	public string ContentType;

	[OptionalValue]
	[ExpectedTag(0, ExplicitTag = true)]
	[AnyValue]
	public ReadOnlyMemory<byte>? Content;
}
