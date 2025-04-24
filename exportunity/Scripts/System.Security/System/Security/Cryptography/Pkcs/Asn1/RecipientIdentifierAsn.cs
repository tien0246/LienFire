using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

[Choice]
internal struct RecipientIdentifierAsn
{
	internal IssuerAndSerialNumberAsn? IssuerAndSerialNumber;

	[OctetString]
	[ExpectedTag(0)]
	internal ReadOnlyMemory<byte>? SubjectKeyIdentifier;
}
