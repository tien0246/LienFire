using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

internal struct OtherName
{
	internal string TypeId;

	[AnyValue]
	[ExpectedTag(0, ExplicitTag = true)]
	internal ReadOnlyMemory<byte> Value;
}
