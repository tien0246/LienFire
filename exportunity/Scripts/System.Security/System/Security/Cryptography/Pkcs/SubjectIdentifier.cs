using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs;

public sealed class SubjectIdentifier
{
	private const string DummySignerSubjectName = "CN=Dummy Signer";

	internal static readonly byte[] DummySignerEncodedValue = new X500DistinguishedName("CN=Dummy Signer").RawData;

	public SubjectIdentifierType Type { get; }

	public object Value { get; }

	internal SubjectIdentifier(SubjectIdentifierType type, object value)
	{
		Type = type;
		Value = value;
	}

	internal SubjectIdentifier(SignerIdentifierAsn signerIdentifierAsn)
		: this(signerIdentifierAsn.IssuerAndSerialNumber, signerIdentifierAsn.SubjectKeyIdentifier)
	{
	}

	internal SubjectIdentifier(IssuerAndSerialNumberAsn? issuerAndSerialNumber, ReadOnlyMemory<byte>? subjectKeyIdentifier)
	{
		if (issuerAndSerialNumber.HasValue)
		{
			IssuerAndSerialNumberAsn value = issuerAndSerialNumber.Value;
			ReadOnlySpan<byte> span = value.Issuer.Span;
			value = issuerAndSerialNumber.Value;
			ReadOnlySpan<byte> span2 = value.SerialNumber.Span;
			bool flag = false;
			for (int i = 0; i < span2.Length; i++)
			{
				if (span2[i] != 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag && DummySignerEncodedValue.AsSpan().SequenceEqual(span))
			{
				Type = SubjectIdentifierType.NoSignature;
				Value = null;
			}
			else
			{
				Type = SubjectIdentifierType.IssuerAndSerialNumber;
				X500DistinguishedName x500DistinguishedName = new X500DistinguishedName(span.ToArray());
				Value = new X509IssuerSerial(x500DistinguishedName.Name, span2.ToBigEndianHex());
			}
		}
		else
		{
			if (!subjectKeyIdentifier.HasValue)
			{
				throw new CryptographicException();
			}
			Type = SubjectIdentifierType.SubjectKeyIdentifier;
			Value = subjectKeyIdentifier.Value.Span.ToBigEndianHex();
		}
	}

	internal SubjectIdentifier()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
