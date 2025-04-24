using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs;

public sealed class SignerInfo
{
	private readonly Oid _digestAlgorithm;

	private readonly AttributeAsn[] _signedAttributes;

	private readonly ReadOnlyMemory<byte>? _signedAttributesMemory;

	private readonly Oid _signatureAlgorithm;

	private readonly ReadOnlyMemory<byte>? _signatureAlgorithmParameters;

	private readonly ReadOnlyMemory<byte> _signature;

	private readonly AttributeAsn[] _unsignedAttributes;

	private readonly SignedCms _document;

	private X509Certificate2 _signerCertificate;

	private SignerInfo _parentSignerInfo;

	private CryptographicAttributeObjectCollection _parsedSignedAttrs;

	private CryptographicAttributeObjectCollection _parsedUnsignedAttrs;

	public int Version { get; }

	public SubjectIdentifier SignerIdentifier { get; }

	public CryptographicAttributeObjectCollection SignedAttributes
	{
		get
		{
			if (_parsedSignedAttrs == null)
			{
				_parsedSignedAttrs = MakeAttributeCollection(_signedAttributes);
			}
			return _parsedSignedAttrs;
		}
	}

	public CryptographicAttributeObjectCollection UnsignedAttributes
	{
		get
		{
			if (_parsedUnsignedAttrs == null)
			{
				_parsedUnsignedAttrs = MakeAttributeCollection(_unsignedAttributes);
			}
			return _parsedUnsignedAttrs;
		}
	}

	public X509Certificate2 Certificate
	{
		get
		{
			if (_signerCertificate == null)
			{
				_signerCertificate = FindSignerCertificate();
			}
			return _signerCertificate;
		}
	}

	public SignerInfoCollection CounterSignerInfos
	{
		get
		{
			if (_parentSignerInfo != null || _unsignedAttributes == null || _unsignedAttributes.Length == 0)
			{
				return new SignerInfoCollection();
			}
			return GetCounterSigners(_unsignedAttributes);
		}
	}

	public Oid DigestAlgorithm => new Oid(_digestAlgorithm);

	public Oid SignatureAlgorithm => new Oid(_signatureAlgorithm);

	internal SignerInfo(ref SignerInfoAsn parsedData, SignedCms ownerDocument)
	{
		Version = parsedData.Version;
		SignerIdentifier = new SubjectIdentifier(parsedData.Sid);
		_digestAlgorithm = parsedData.DigestAlgorithm.Algorithm;
		_signedAttributesMemory = parsedData.SignedAttributes;
		_signatureAlgorithm = parsedData.SignatureAlgorithm.Algorithm;
		_signatureAlgorithmParameters = parsedData.SignatureAlgorithm.Parameters;
		_signature = parsedData.SignatureValue;
		_unsignedAttributes = parsedData.UnsignedAttributes;
		if (_signedAttributesMemory.HasValue)
		{
			_signedAttributes = AsnSerializer.Deserialize<SignedAttributesSet>(_signedAttributesMemory.Value, AsnEncodingRules.BER).SignedAttributes;
		}
		_document = ownerDocument;
	}

	internal ReadOnlyMemory<byte> GetSignatureMemory()
	{
		return _signature;
	}

	public byte[] GetSignature()
	{
		return _signature.ToArray();
	}

	private SignerInfoCollection GetCounterSigners(AttributeAsn[] unsignedAttrs)
	{
		List<SignerInfo> list = new List<SignerInfo>();
		for (int i = 0; i < unsignedAttrs.Length; i++)
		{
			AttributeAsn attributeAsn = unsignedAttrs[i];
			if (attributeAsn.AttrType.Value == "1.2.840.113549.1.9.6")
			{
				AsnReader asnReader = new AsnReader(attributeAsn.AttrValues, AsnEncodingRules.BER);
				AsnReader asnReader2 = asnReader.ReadSetOf();
				if (asnReader.HasData)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				while (asnReader2.HasData)
				{
					SignerInfoAsn parsedData = AsnSerializer.Deserialize<SignerInfoAsn>(asnReader2.GetEncodedValue(), AsnEncodingRules.BER);
					SignerInfo item = new SignerInfo(ref parsedData, _document)
					{
						_parentSignerInfo = this
					};
					list.Add(item);
				}
			}
		}
		return new SignerInfoCollection(list.ToArray());
	}

	public void ComputeCounterSignature()
	{
		throw new PlatformNotSupportedException("No signer certificate was provided. This platform does not implement the certificate picker UI.");
	}

	public void ComputeCounterSignature(CmsSigner signer)
	{
		if (_parentSignerInfo != null)
		{
			throw new CryptographicException("Only one level of counter-signatures are supported on this platform.");
		}
		if (signer == null)
		{
			throw new ArgumentNullException("signer");
		}
		signer.CheckCertificateValue();
		int num = _document.SignerInfos.FindIndexForSigner(this);
		if (num < 0)
		{
			throw new CryptographicException("Cannot find the original signer.");
		}
		SignerInfo signerInfo = _document.SignerInfos[num];
		X509Certificate2Collection chainCerts;
		SignerInfoAsn value = signer.Sign(signerInfo._signature, null, silent: false, out chainCerts);
		AttributeAsn attributeAsn;
		using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
		{
			asnWriter.PushSetOf();
			AsnSerializer.Serialize(value, asnWriter);
			asnWriter.PopSetOf();
			attributeAsn = new AttributeAsn
			{
				AttrType = new Oid("1.2.840.113549.1.9.6", "1.2.840.113549.1.9.6"),
				AttrValues = asnWriter.Encode()
			};
		}
		ref SignerInfoAsn reference = ref _document.GetRawData().SignerInfos[num];
		int num2;
		if (reference.UnsignedAttributes == null)
		{
			reference.UnsignedAttributes = new AttributeAsn[1];
			num2 = 0;
		}
		else
		{
			num2 = reference.UnsignedAttributes.Length;
			Array.Resize(ref reference.UnsignedAttributes, num2 + 1);
		}
		reference.UnsignedAttributes[num2] = attributeAsn;
		_document.UpdateCertificatesFromAddition(chainCerts);
		_document.Reencode();
	}

	public void RemoveCounterSignature(int index)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("childIndex");
		}
		int num = _document.SignerInfos.FindIndexForSigner(this);
		if (num < 0)
		{
			throw new CryptographicException("Cannot find the original signer.");
		}
		ref SignerInfoAsn reference = ref _document.GetRawData().SignerInfos[num];
		if (reference.UnsignedAttributes == null)
		{
			throw new CryptographicException("The signed cryptographic message does not have a signer for the specified signer index.");
		}
		int num2 = -1;
		int num3 = -1;
		bool flag = false;
		int num4 = 0;
		AttributeAsn[] unsignedAttributes = reference.UnsignedAttributes;
		for (int i = 0; i < unsignedAttributes.Length; i++)
		{
			AttributeAsn attributeAsn = unsignedAttributes[i];
			if (!(attributeAsn.AttrType.Value == "1.2.840.113549.1.9.6"))
			{
				continue;
			}
			AsnReader asnReader = new AsnReader(attributeAsn.AttrValues, AsnEncodingRules.BER);
			AsnReader asnReader2 = asnReader.ReadSetOf();
			if (asnReader.HasData)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			int num5 = 0;
			while (asnReader2.HasData)
			{
				asnReader2.GetEncodedValue();
				if (num4 == index)
				{
					num2 = i;
					num3 = num5;
				}
				num4++;
				num5++;
			}
			if (num3 == 0 && num5 == 1)
			{
				flag = true;
			}
			if (num2 >= 0)
			{
				break;
			}
		}
		if (num2 < 0)
		{
			throw new CryptographicException("The signed cryptographic message does not have a signer for the specified signer index.");
		}
		if (flag)
		{
			if (unsignedAttributes.Length == 1)
			{
				reference.UnsignedAttributes = null;
			}
			else
			{
				Helpers.RemoveAt(ref reference.UnsignedAttributes, num2);
			}
			return;
		}
		ref AttributeAsn reference2 = ref unsignedAttributes[num2];
		using AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.BER);
		asnWriter.PushSetOf();
		AsnReader asnReader3 = new AsnReader(reference2.AttrValues, asnWriter.RuleSet);
		AsnReader asnReader4 = asnReader3.ReadSetOf();
		asnReader3.ThrowIfNotEmpty();
		int num6 = 0;
		while (asnReader4.HasData)
		{
			ReadOnlyMemory<byte> encodedValue = asnReader4.GetEncodedValue();
			if (num6 != num3)
			{
				asnWriter.WriteEncodedValue(encodedValue);
			}
			num6++;
		}
		asnWriter.PopSetOf();
		reference2.AttrValues = asnWriter.Encode();
	}

	public void RemoveCounterSignature(SignerInfo counterSignerInfo)
	{
		if (counterSignerInfo == null)
		{
			throw new ArgumentNullException("counterSignerInfo");
		}
		SignerInfoCollection signerInfos = _document.SignerInfos;
		int num = signerInfos.FindIndexForSigner(this);
		if (num < 0)
		{
			throw new CryptographicException("Cannot find the original signer.");
		}
		num = signerInfos[num].CounterSignerInfos.FindIndexForSigner(counterSignerInfo);
		if (num < 0)
		{
			throw new CryptographicException("Cannot find the original signer.");
		}
		RemoveCounterSignature(num);
	}

	public void CheckSignature(bool verifySignatureOnly)
	{
		CheckSignature(new X509Certificate2Collection(), verifySignatureOnly);
	}

	public void CheckSignature(X509Certificate2Collection extraStore, bool verifySignatureOnly)
	{
		if (extraStore == null)
		{
			throw new ArgumentNullException("extraStore");
		}
		X509Certificate2 x509Certificate = Certificate;
		if (x509Certificate == null)
		{
			x509Certificate = FindSignerCertificate(SignerIdentifier, extraStore);
			if (x509Certificate == null)
			{
				throw new CryptographicException("Cannot find the original signer.");
			}
		}
		Verify(extraStore, x509Certificate, verifySignatureOnly);
	}

	public void CheckHash()
	{
		if (!CheckHash(compatMode: false) && !CheckHash(compatMode: true))
		{
			throw new CryptographicException("Invalid signature.");
		}
	}

	private bool CheckHash(bool compatMode)
	{
		using IncrementalHash incrementalHash = PrepareDigest(compatMode);
		if (incrementalHash == null)
		{
			return false;
		}
		byte[] hashAndReset = incrementalHash.GetHashAndReset();
		return _signature.Span.SequenceEqual(hashAndReset);
	}

	private X509Certificate2 FindSignerCertificate()
	{
		return FindSignerCertificate(SignerIdentifier, _document.Certificates);
	}

	private static X509Certificate2 FindSignerCertificate(SubjectIdentifier signerIdentifier, X509Certificate2Collection extraStore)
	{
		if (extraStore == null || extraStore.Count == 0)
		{
			return null;
		}
		X509Certificate2Collection x509Certificate2Collection = null;
		X509Certificate2 x509Certificate = null;
		switch (signerIdentifier.Type)
		{
		case SubjectIdentifierType.IssuerAndSerialNumber:
		{
			X509IssuerSerial x509IssuerSerial = (X509IssuerSerial)signerIdentifier.Value;
			x509Certificate2Collection = extraStore.Find(X509FindType.FindBySerialNumber, x509IssuerSerial.SerialNumber, validOnly: false);
			X509Certificate2Enumerator enumerator = x509Certificate2Collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				X509Certificate2 current = enumerator.Current;
				if (current.IssuerName.Name == x509IssuerSerial.IssuerName)
				{
					x509Certificate = current;
					break;
				}
			}
			break;
		}
		case SubjectIdentifierType.SubjectKeyIdentifier:
			x509Certificate2Collection = extraStore.Find(X509FindType.FindBySubjectKeyIdentifier, signerIdentifier.Value, validOnly: false);
			if (x509Certificate2Collection.Count > 0)
			{
				x509Certificate = x509Certificate2Collection[0];
			}
			break;
		}
		if (x509Certificate2Collection != null)
		{
			X509Certificate2Enumerator enumerator = x509Certificate2Collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				X509Certificate2 current2 = enumerator.Current;
				if (current2 != x509Certificate)
				{
					current2.Dispose();
				}
			}
		}
		return x509Certificate;
	}

	private IncrementalHash PrepareDigest(bool compatMode)
	{
		IncrementalHash incrementalHash = IncrementalHash.CreateHash(GetDigestAlgorithm());
		if (_parentSignerInfo == null)
		{
			if (_document.Detached)
			{
				ref SignedDataAsn rawData = ref _document.GetRawData();
				ReadOnlyMemory<byte>? content = rawData.EncapContentInfo.Content;
				if (content.HasValue)
				{
					Helpers.AppendData(incrementalHash, SignedCms.GetContent(content.Value, rawData.EncapContentInfo.ContentType).Span);
				}
			}
			Helpers.AppendData(incrementalHash, _document.GetHashableContentSpan());
		}
		else
		{
			Helpers.AppendData(incrementalHash, _parentSignerInfo._signature.Span);
		}
		bool flag = _parentSignerInfo != null || _signedAttributes != null;
		if (_signedAttributes != null)
		{
			byte[] hashAndReset = incrementalHash.GetHashAndReset();
			using AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
			if (compatMode)
			{
				asnWriter.PushSequence();
			}
			else
			{
				asnWriter.PushSetOf();
			}
			AttributeAsn[] signedAttributes = _signedAttributes;
			for (int i = 0; i < signedAttributes.Length; i++)
			{
				AttributeAsn attributeAsn = signedAttributes[i];
				AsnSerializer.Serialize(attributeAsn, asnWriter);
				if (attributeAsn.AttrType.Value == "1.2.840.113549.1.9.4")
				{
					CryptographicAttributeObject cryptographicAttributeObject = MakeAttribute(attributeAsn);
					if (cryptographicAttributeObject.Values.Count != 1)
					{
						throw new CryptographicException("The hash value is not correct.");
					}
					Pkcs9MessageDigest pkcs9MessageDigest = (Pkcs9MessageDigest)cryptographicAttributeObject.Values[0];
					if (!hashAndReset.AsSpan().SequenceEqual(pkcs9MessageDigest.MessageDigest))
					{
						throw new CryptographicException("The hash value is not correct.");
					}
					flag = false;
				}
			}
			if (compatMode)
			{
				asnWriter.PopSequence();
				byte[] array = asnWriter.Encode();
				array[0] = 49;
				incrementalHash.AppendData(array);
			}
			else
			{
				asnWriter.PopSetOf();
				incrementalHash.AppendData(asnWriter.Encode());
			}
		}
		else if (compatMode)
		{
			return null;
		}
		if (flag)
		{
			throw new CryptographicException("The cryptographic message does not contain an expected authenticated attribute.");
		}
		return incrementalHash;
	}

	private void Verify(X509Certificate2Collection extraStore, X509Certificate2 certificate, bool verifySignatureOnly)
	{
		CmsSignature cmsSignature = CmsSignature.Resolve(SignatureAlgorithm.Value);
		if (cmsSignature == null)
		{
			throw new CryptographicException("Unknown algorithm '{0}'.", SignatureAlgorithm.Value);
		}
		if (!VerifySignature(cmsSignature, certificate, compatMode: false) && !VerifySignature(cmsSignature, certificate, compatMode: true))
		{
			throw new CryptographicException("Invalid signature.");
		}
		if (verifySignatureOnly)
		{
			return;
		}
		X509Chain x509Chain = new X509Chain();
		x509Chain.ChainPolicy.ExtraStore.AddRange(extraStore);
		x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
		x509Chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
		if (!x509Chain.Build(certificate))
		{
			throw new CryptographicException("Certificate trust could not be established. The first reported error is: {0}", x509Chain.ChainStatus.FirstOrDefault().StatusInformation);
		}
		X509ExtensionEnumerator enumerator = certificate.Extensions.GetEnumerator();
		while (enumerator.MoveNext())
		{
			X509Extension current = enumerator.Current;
			if (current.Oid.Value == "2.5.29.15")
			{
				X509KeyUsageExtension x509KeyUsageExtension = current as X509KeyUsageExtension;
				if (x509KeyUsageExtension == null)
				{
					x509KeyUsageExtension = new X509KeyUsageExtension();
					x509KeyUsageExtension.CopyFrom(current);
				}
				if ((x509KeyUsageExtension.KeyUsages & (X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature)) == 0)
				{
					throw new CryptographicException("The certificate is not valid for the requested usage.");
				}
			}
		}
	}

	private bool VerifySignature(CmsSignature signatureProcessor, X509Certificate2 certificate, bool compatMode)
	{
		using IncrementalHash incrementalHash = PrepareDigest(compatMode);
		if (incrementalHash == null)
		{
			return false;
		}
		byte[] hashAndReset = incrementalHash.GetHashAndReset();
		byte[] signature = _signature.ToArray();
		return signatureProcessor.VerifySignature(hashAndReset, signature, DigestAlgorithm.Value, incrementalHash.AlgorithmName, _signatureAlgorithmParameters, certificate);
	}

	private HashAlgorithmName GetDigestAlgorithm()
	{
		return Helpers.GetDigestAlgorithm(DigestAlgorithm.Value);
	}

	internal static CryptographicAttributeObjectCollection MakeAttributeCollection(AttributeAsn[] attributes)
	{
		CryptographicAttributeObjectCollection cryptographicAttributeObjectCollection = new CryptographicAttributeObjectCollection();
		if (attributes == null)
		{
			return cryptographicAttributeObjectCollection;
		}
		foreach (AttributeAsn attribute in attributes)
		{
			cryptographicAttributeObjectCollection.AddWithoutMerge(MakeAttribute(attribute));
		}
		return cryptographicAttributeObjectCollection;
	}

	private static CryptographicAttributeObject MakeAttribute(AttributeAsn attribute)
	{
		Oid oid = new Oid(attribute.AttrType);
		AsnReader asnReader = new AsnReader(attribute.AttrValues, AsnEncodingRules.BER);
		AsnReader asnReader2 = asnReader.ReadSetOf();
		if (asnReader.HasData)
		{
			throw new CryptographicException("ASN1 corrupted data.");
		}
		AsnEncodedDataCollection asnEncodedDataCollection = new AsnEncodedDataCollection();
		while (asnReader2.HasData)
		{
			byte[] encodedAttribute = asnReader2.GetEncodedValue().ToArray();
			asnEncodedDataCollection.Add(Helpers.CreateBestPkcs9AttributeObjectAvailable(oid, encodedAttribute));
		}
		return new CryptographicAttributeObject(oid, asnEncodedDataCollection);
	}

	internal SignerInfo()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
