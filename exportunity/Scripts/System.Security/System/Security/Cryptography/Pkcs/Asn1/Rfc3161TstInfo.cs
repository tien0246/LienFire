using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1;

[StructLayout(LayoutKind.Sequential)]
internal sealed class Rfc3161TstInfo
{
	internal int Version;

	[ObjectIdentifier(PopulateFriendlyName = true)]
	internal Oid Policy;

	internal MessageImprint MessageImprint;

	[Integer]
	internal ReadOnlyMemory<byte> SerialNumber;

	[GeneralizedTime(DisallowFractions = false)]
	internal DateTimeOffset GenTime;

	[OptionalValue]
	internal Rfc3161Accuracy? Accuracy;

	[DefaultValue(new byte[] { 1, 1, 0 })]
	internal bool Ordering;

	[Integer]
	[OptionalValue]
	internal ReadOnlyMemory<byte>? Nonce;

	[OptionalValue]
	[ExpectedTag(0, ExplicitTag = true)]
	internal GeneralName? Tsa;

	[OptionalValue]
	[ExpectedTag(1)]
	internal X509ExtensionAsn[] Extensions;
}
