using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

internal struct EncryptedContentInfoAsn
{
	[ObjectIdentifier]
	internal string ContentType;

	internal AlgorithmIdentifierAsn ContentEncryptionAlgorithm;

	[OptionalValue]
	[OctetString]
	[ExpectedTag(0)]
	internal ReadOnlyMemory<byte>? EncryptedContent;
}
