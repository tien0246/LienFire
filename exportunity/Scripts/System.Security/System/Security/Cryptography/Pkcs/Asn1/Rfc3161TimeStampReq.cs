using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

internal struct Rfc3161TimeStampReq
{
	public int Version;

	public MessageImprint MessageImprint;

	[OptionalValue]
	public Oid ReqPolicy;

	[OptionalValue]
	[Integer]
	public ReadOnlyMemory<byte>? Nonce;

	[DefaultValue(new byte[] { 1, 1, 0 })]
	public bool CertReq;

	[OptionalValue]
	[ExpectedTag(0)]
	internal X509ExtensionAsn[] Extensions;
}
