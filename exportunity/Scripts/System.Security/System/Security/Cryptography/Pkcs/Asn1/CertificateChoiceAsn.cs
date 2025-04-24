using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

[Choice]
internal struct CertificateChoiceAsn
{
	[AnyValue]
	[ExpectedTag(TagClass.Universal, 16)]
	public ReadOnlyMemory<byte>? Certificate;
}
