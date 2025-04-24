using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

[Choice]
internal struct SignedAttributesSet
{
	[ExpectedTag(0)]
	[SetOf]
	public AttributeAsn[] SignedAttributes;
}
