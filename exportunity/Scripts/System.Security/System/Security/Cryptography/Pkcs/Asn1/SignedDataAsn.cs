using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

internal struct SignedDataAsn
{
	public int Version;

	[SetOf]
	public AlgorithmIdentifierAsn[] DigestAlgorithms;

	public EncapsulatedContentInfoAsn EncapContentInfo;

	[OptionalValue]
	[ExpectedTag(0)]
	[SetOf]
	public CertificateChoiceAsn[] CertificateSet;

	[OptionalValue]
	[ExpectedTag(1)]
	[AnyValue]
	public ReadOnlyMemory<byte>? RevocationInfoChoices;

	[SetOf]
	public SignerInfoAsn[] SignerInfos;
}
