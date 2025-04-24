using System.Collections.Generic;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

public sealed class CmsSigner
{
	private static readonly Oid s_defaultAlgorithm = Oid.FromOidValue("1.3.14.3.2.26", OidGroup.HashAlgorithm);

	private SubjectIdentifierType _signerIdentifierType;

	public X509Certificate2 Certificate { get; set; }

	public AsymmetricAlgorithm PrivateKey { get; set; }

	public X509Certificate2Collection Certificates { get; private set; } = new X509Certificate2Collection();

	public Oid DigestAlgorithm { get; set; }

	public X509IncludeOption IncludeOption { get; set; }

	public CryptographicAttributeObjectCollection SignedAttributes { get; private set; } = new CryptographicAttributeObjectCollection();

	public CryptographicAttributeObjectCollection UnsignedAttributes { get; private set; } = new CryptographicAttributeObjectCollection();

	public SubjectIdentifierType SignerIdentifierType
	{
		get
		{
			return _signerIdentifierType;
		}
		set
		{
			if (value < SubjectIdentifierType.IssuerAndSerialNumber || value > SubjectIdentifierType.NoSignature)
			{
				throw new ArgumentException(global::SR.Format("The subject identifier type {0} is not valid.", value));
			}
			_signerIdentifierType = value;
		}
	}

	public CmsSigner()
		: this(SubjectIdentifierType.IssuerAndSerialNumber, null)
	{
	}

	public CmsSigner(SubjectIdentifierType signerIdentifierType)
		: this(signerIdentifierType, null)
	{
	}

	public CmsSigner(X509Certificate2 certificate)
		: this(SubjectIdentifierType.IssuerAndSerialNumber, certificate)
	{
	}

	public CmsSigner(CspParameters parameters)
	{
		throw new PlatformNotSupportedException();
	}

	public CmsSigner(SubjectIdentifierType signerIdentifierType, X509Certificate2 certificate)
	{
		switch (signerIdentifierType)
		{
		case SubjectIdentifierType.Unknown:
			_signerIdentifierType = SubjectIdentifierType.IssuerAndSerialNumber;
			IncludeOption = X509IncludeOption.ExcludeRoot;
			break;
		case SubjectIdentifierType.IssuerAndSerialNumber:
			_signerIdentifierType = signerIdentifierType;
			IncludeOption = X509IncludeOption.ExcludeRoot;
			break;
		case SubjectIdentifierType.SubjectKeyIdentifier:
			_signerIdentifierType = signerIdentifierType;
			IncludeOption = X509IncludeOption.ExcludeRoot;
			break;
		case SubjectIdentifierType.NoSignature:
			_signerIdentifierType = signerIdentifierType;
			IncludeOption = X509IncludeOption.None;
			break;
		default:
			_signerIdentifierType = SubjectIdentifierType.IssuerAndSerialNumber;
			IncludeOption = X509IncludeOption.ExcludeRoot;
			break;
		}
		Certificate = certificate;
		DigestAlgorithm = new Oid(s_defaultAlgorithm);
	}

	internal void CheckCertificateValue()
	{
		if (SignerIdentifierType != SubjectIdentifierType.NoSignature)
		{
			if (Certificate == null)
			{
				throw new PlatformNotSupportedException("No signer certificate was provided. This platform does not implement the certificate picker UI.");
			}
			if (!Certificate.HasPrivateKey)
			{
				throw new CryptographicException("A certificate with a private key is required.");
			}
		}
	}

	internal SignerInfoAsn Sign(ReadOnlyMemory<byte> data, string contentTypeOid, bool silent, out X509Certificate2Collection chainCerts)
	{
		HashAlgorithmName digestAlgorithm = Helpers.GetDigestAlgorithm(DigestAlgorithm);
		IncrementalHash hasher = IncrementalHash.CreateHash(digestAlgorithm);
		Helpers.AppendData(hasher, data.Span);
		byte[] hashAndReset = hasher.GetHashAndReset();
		SignerInfoAsn result = new SignerInfoAsn
		{
			DigestAlgorithm = 
			{
				Algorithm = DigestAlgorithm
			}
		};
		CryptographicAttributeObjectCollection signedAttributes = SignedAttributes;
		if ((signedAttributes != null && signedAttributes.Count > 0) || contentTypeOid != "1.2.840.113549.1.7.1")
		{
			List<AttributeAsn> list = BuildAttributes(SignedAttributes);
			using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
			{
				asnWriter.PushSetOf();
				asnWriter.WriteOctetString(hashAndReset);
				asnWriter.PopSetOf();
				list.Add(new AttributeAsn
				{
					AttrType = new Oid("1.2.840.113549.1.9.4", "1.2.840.113549.1.9.4"),
					AttrValues = asnWriter.Encode()
				});
			}
			if (contentTypeOid != null)
			{
				using AsnWriter asnWriter2 = new AsnWriter(AsnEncodingRules.DER);
				asnWriter2.PushSetOf();
				asnWriter2.WriteObjectIdentifier(contentTypeOid);
				asnWriter2.PopSetOf();
				list.Add(new AttributeAsn
				{
					AttrType = new Oid("1.2.840.113549.1.9.3", "1.2.840.113549.1.9.3"),
					AttrValues = asnWriter2.Encode()
				});
			}
			using (AsnWriter asnWriter3 = AsnSerializer.Serialize(new SignedAttributesSet
			{
				SignedAttributes = Helpers.NormalizeSet(list.ToArray(), delegate(byte[] normalized)
				{
					AsnReader asnReader = new AsnReader(normalized, AsnEncodingRules.DER);
					Helpers.AppendData(hasher, asnReader.PeekContentBytes().Span);
				})
			}, AsnEncodingRules.BER))
			{
				result.SignedAttributes = asnWriter3.Encode();
			}
			hashAndReset = hasher.GetHashAndReset();
		}
		switch (SignerIdentifierType)
		{
		case SubjectIdentifierType.IssuerAndSerialNumber:
		{
			byte[] serialNumber = Certificate.GetSerialNumber();
			Array.Reverse(serialNumber);
			result.Sid.IssuerAndSerialNumber = new IssuerAndSerialNumberAsn
			{
				Issuer = Certificate.IssuerName.RawData,
				SerialNumber = serialNumber
			};
			result.Version = 1;
			break;
		}
		case SubjectIdentifierType.SubjectKeyIdentifier:
			result.Sid.SubjectKeyIdentifier = Certificate.GetSubjectKeyIdentifier();
			result.Version = 3;
			break;
		case SubjectIdentifierType.NoSignature:
			result.Sid.IssuerAndSerialNumber = new IssuerAndSerialNumberAsn
			{
				Issuer = SubjectIdentifier.DummySignerEncodedValue,
				SerialNumber = new byte[1]
			};
			result.Version = 1;
			break;
		default:
			throw new CryptographicException();
		}
		if (UnsignedAttributes != null && UnsignedAttributes.Count > 0)
		{
			List<AttributeAsn> list2 = BuildAttributes(UnsignedAttributes);
			result.UnsignedAttributes = Helpers.NormalizeSet(list2.ToArray());
		}
		if (!CmsSignature.Sign(hashAndReset, digestAlgorithm, Certificate, silent, out var oid, out var signatureValue))
		{
			throw new CryptographicException("Could not determine signature algorithm for the signer certificate.");
		}
		result.SignatureValue = signatureValue;
		result.SignatureAlgorithm.Algorithm = oid;
		X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
		x509Certificate2Collection.AddRange(Certificates);
		if (SignerIdentifierType != SubjectIdentifierType.NoSignature)
		{
			if (IncludeOption == X509IncludeOption.EndCertOnly)
			{
				x509Certificate2Collection.Add(Certificate);
			}
			else if (IncludeOption != X509IncludeOption.None)
			{
				X509Chain x509Chain = new X509Chain();
				x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
				x509Chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
				if (!x509Chain.Build(Certificate))
				{
					X509ChainStatus[] chainStatus = x509Chain.ChainStatus;
					foreach (X509ChainStatus x509ChainStatus in chainStatus)
					{
						if (x509ChainStatus.Status == X509ChainStatusFlags.PartialChain)
						{
							throw new CryptographicException("The certificate chain is incomplete, the self-signed root authority could not be determined.");
						}
					}
				}
				X509ChainElementCollection chainElements = x509Chain.ChainElements;
				int count = chainElements.Count;
				int num2 = count - 1;
				if (num2 == 0)
				{
					num2 = -1;
				}
				for (int num3 = 0; num3 < count; num3++)
				{
					X509Certificate2 certificate = chainElements[num3].Certificate;
					if (num3 == num2 && IncludeOption == X509IncludeOption.ExcludeRoot && certificate.SubjectName.RawData.AsSpan().SequenceEqual(certificate.IssuerName.RawData))
					{
						break;
					}
					x509Certificate2Collection.Add(certificate);
				}
			}
		}
		chainCerts = x509Certificate2Collection;
		return result;
	}

	internal static List<AttributeAsn> BuildAttributes(CryptographicAttributeObjectCollection attributes)
	{
		List<AttributeAsn> list = new List<AttributeAsn>();
		if (attributes == null || attributes.Count == 0)
		{
			return list;
		}
		CryptographicAttributeObjectEnumerator enumerator = attributes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CryptographicAttributeObject current = enumerator.Current;
			using AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
			asnWriter.PushSetOf();
			AsnEncodedDataEnumerator enumerator2 = current.Values.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				AsnEncodedData current2 = enumerator2.Current;
				asnWriter.WriteEncodedValue(current2.RawData);
			}
			asnWriter.PopSetOf();
			AttributeAsn item = new AttributeAsn
			{
				AttrType = current.Oid,
				AttrValues = asnWriter.Encode()
			};
			list.Add(item);
		}
		return list;
	}
}
