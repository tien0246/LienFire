using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

[Choice]
internal struct OriginatorIdentifierOrKeyAsn
{
	internal IssuerAndSerialNumberAsn? IssuerAndSerialNumber;

	[ExpectedTag(0)]
	[OctetString]
	internal ReadOnlyMemory<byte>? SubjectKeyIdentifier;

	[ExpectedTag(1)]
	internal OriginatorPublicKeyAsn OriginatorKey;
}
