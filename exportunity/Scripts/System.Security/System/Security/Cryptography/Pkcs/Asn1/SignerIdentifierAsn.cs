using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

[Choice]
internal struct SignerIdentifierAsn
{
	public IssuerAndSerialNumberAsn? IssuerAndSerialNumber;

	[ExpectedTag(0)]
	[OctetString]
	public ReadOnlyMemory<byte>? SubjectKeyIdentifier;
}
