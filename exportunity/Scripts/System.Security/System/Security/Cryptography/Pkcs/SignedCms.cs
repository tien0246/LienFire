using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

public sealed class SignedCms
{
	private static readonly Oid s_cmsDataOid = Oid.FromOidValue("1.2.840.113549.1.7.1", OidGroup.ExtensionOrAttribute);

	private SignedDataAsn _signedData;

	private bool _hasData;

	private Memory<byte> _heldData;

	private ReadOnlyMemory<byte>? _heldContent;

	private bool _hasPkcs7Content;

	private string _contentType;

	public int Version { get; private set; }

	public ContentInfo ContentInfo { get; private set; }

	public bool Detached { get; private set; }

	public X509Certificate2Collection Certificates
	{
		get
		{
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			if (!_hasData)
			{
				return x509Certificate2Collection;
			}
			CertificateChoiceAsn[] certificateSet = _signedData.CertificateSet;
			if (certificateSet == null)
			{
				return x509Certificate2Collection;
			}
			CertificateChoiceAsn[] array = certificateSet;
			for (int i = 0; i < array.Length; i++)
			{
				CertificateChoiceAsn certificateChoiceAsn = array[i];
				x509Certificate2Collection.Add(new X509Certificate2(certificateChoiceAsn.Certificate.Value.ToArray()));
			}
			return x509Certificate2Collection;
		}
	}

	public SignerInfoCollection SignerInfos
	{
		get
		{
			if (!_hasData)
			{
				return new SignerInfoCollection();
			}
			return new SignerInfoCollection(_signedData.SignerInfos, this);
		}
	}

	private static ContentInfo MakeEmptyContentInfo()
	{
		return new ContentInfo(new Oid(s_cmsDataOid), Array.Empty<byte>());
	}

	public SignedCms()
		: this(SubjectIdentifierType.IssuerAndSerialNumber, MakeEmptyContentInfo(), detached: false)
	{
	}

	public SignedCms(SubjectIdentifierType signerIdentifierType)
		: this(signerIdentifierType, MakeEmptyContentInfo(), detached: false)
	{
	}

	public SignedCms(ContentInfo contentInfo)
		: this(SubjectIdentifierType.IssuerAndSerialNumber, contentInfo, detached: false)
	{
	}

	public SignedCms(SubjectIdentifierType signerIdentifierType, ContentInfo contentInfo)
		: this(signerIdentifierType, contentInfo, detached: false)
	{
	}

	public SignedCms(ContentInfo contentInfo, bool detached)
		: this(SubjectIdentifierType.IssuerAndSerialNumber, contentInfo, detached)
	{
	}

	public SignedCms(SubjectIdentifierType signerIdentifierType, ContentInfo contentInfo, bool detached)
	{
		if (contentInfo == null)
		{
			throw new ArgumentNullException("contentInfo");
		}
		if (contentInfo.Content == null)
		{
			throw new ArgumentNullException("contentInfo.Content");
		}
		ContentInfo = contentInfo;
		Detached = detached;
		Version = 0;
	}

	public byte[] Encode()
	{
		if (!_hasData)
		{
			throw new InvalidOperationException("The CMS message is not signed.");
		}
		try
		{
			return Helpers.EncodeContentInfo(_signedData, "1.2.840.113549.1.7.2");
		}
		catch (CryptographicException)
		{
			if (Detached)
			{
				throw;
			}
			SignedDataAsn value = _signedData;
			value.EncapContentInfo.Content = null;
			using (AsnWriter asnWriter = AsnSerializer.Serialize(value, AsnEncodingRules.DER))
			{
				value = AsnSerializer.Deserialize<SignedDataAsn>(asnWriter.Encode(), AsnEncodingRules.BER);
			}
			value.EncapContentInfo.Content = _signedData.EncapContentInfo.Content;
			return Helpers.EncodeContentInfo(value, "1.2.840.113549.1.7.2", AsnEncodingRules.BER);
		}
	}

	public void Decode(byte[] encodedMessage)
	{
		if (encodedMessage == null)
		{
			throw new ArgumentNullException("encodedMessage");
		}
		Decode(new ReadOnlyMemory<byte>(encodedMessage));
	}

	internal void Decode(ReadOnlyMemory<byte> encodedMessage)
	{
		int bytesRead;
		ContentInfoAsn contentInfoAsn = AsnSerializer.Deserialize<ContentInfoAsn>(encodedMessage, AsnEncodingRules.BER, out bytesRead);
		if (contentInfoAsn.ContentType != "1.2.840.113549.1.7.2")
		{
			throw new CryptographicException("Invalid cryptographic message type.");
		}
		_heldData = contentInfoAsn.Content.ToArray();
		_signedData = AsnSerializer.Deserialize<SignedDataAsn>(_heldData, AsnEncodingRules.BER);
		_contentType = _signedData.EncapContentInfo.ContentType;
		_hasPkcs7Content = false;
		if (!Detached)
		{
			ReadOnlyMemory<byte>? content = _signedData.EncapContentInfo.Content;
			ReadOnlyMemory<byte> value;
			if (content.HasValue)
			{
				value = GetContent(content.Value, _contentType);
				_hasPkcs7Content = content.Value.Length == value.Length;
			}
			else
			{
				value = ReadOnlyMemory<byte>.Empty;
			}
			_heldContent = value;
			ContentInfo = new ContentInfo(new Oid(_contentType), value.ToArray());
		}
		else
		{
			_heldContent = ContentInfo.Content.CloneByteArray();
		}
		Version = _signedData.Version;
		_hasData = true;
	}

	internal static ReadOnlyMemory<byte> GetContent(ReadOnlyMemory<byte> wrappedContent, string contentType)
	{
		byte[] array = null;
		int bytesWritten = 0;
		try
		{
			AsnReader asnReader = new AsnReader(wrappedContent, AsnEncodingRules.BER);
			if (asnReader.TryGetPrimitiveOctetStringBytes(out var contents))
			{
				return contents;
			}
			array = ArrayPool<byte>.Shared.Rent(wrappedContent.Length);
			if (!asnReader.TryCopyOctetStringBytes(array, out bytesWritten))
			{
				throw new CryptographicException();
			}
			return array.AsSpan(0, bytesWritten).ToArray();
		}
		catch (Exception)
		{
			if (contentType == "1.2.840.113549.1.7.1")
			{
				throw;
			}
			return wrappedContent;
		}
		finally
		{
			if (array != null)
			{
				array.AsSpan(0, bytesWritten).Clear();
				ArrayPool<byte>.Shared.Return(array);
			}
		}
	}

	public void ComputeSignature()
	{
		throw new PlatformNotSupportedException("No signer certificate was provided. This platform does not implement the certificate picker UI.");
	}

	public void ComputeSignature(CmsSigner signer)
	{
		ComputeSignature(signer, silent: true);
	}

	public void ComputeSignature(CmsSigner signer, bool silent)
	{
		if (signer == null)
		{
			throw new ArgumentNullException("signer");
		}
		if (ContentInfo.Content.Length == 0)
		{
			throw new CryptographicException("Cannot create CMS signature for empty content.");
		}
		ReadOnlyMemory<byte> data = _heldContent ?? ((ReadOnlyMemory<byte>)ContentInfo.Content);
		string text = _contentType ?? ContentInfo.ContentType.Value ?? "1.2.840.113549.1.7.1";
		X509Certificate2Collection chainCerts;
		SignerInfoAsn signerInfoAsn = signer.Sign(data, text, silent, out chainCerts);
		bool flag = false;
		if (!_hasData)
		{
			flag = true;
			_signedData = new SignedDataAsn
			{
				DigestAlgorithms = Array.Empty<AlgorithmIdentifierAsn>(),
				SignerInfos = Array.Empty<SignerInfoAsn>(),
				EncapContentInfo = new EncapsulatedContentInfoAsn
				{
					ContentType = text
				}
			};
			if (!Detached)
			{
				using AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
				asnWriter.WriteOctetString(data.Span);
				_signedData.EncapContentInfo.Content = asnWriter.Encode();
			}
			_hasData = true;
		}
		int num = _signedData.SignerInfos.Length;
		Array.Resize(ref _signedData.SignerInfos, num + 1);
		_signedData.SignerInfos[num] = signerInfoAsn;
		UpdateCertificatesFromAddition(chainCerts);
		ConsiderDigestAddition(signerInfoAsn.DigestAlgorithm);
		UpdateMetadata();
		if (flag)
		{
			Reencode();
		}
	}

	public void RemoveSignature(int index)
	{
		if (!_hasData)
		{
			throw new InvalidOperationException("The CMS message is not signed.");
		}
		if (index < 0 || index >= _signedData.SignerInfos.Length)
		{
			throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
		}
		AlgorithmIdentifierAsn digestAlgorithm = _signedData.SignerInfos[index].DigestAlgorithm;
		Helpers.RemoveAt(ref _signedData.SignerInfos, index);
		ConsiderDigestRemoval(digestAlgorithm);
		UpdateMetadata();
	}

	public void RemoveSignature(SignerInfo signerInfo)
	{
		if (signerInfo == null)
		{
			throw new ArgumentNullException("signerInfo");
		}
		int num = SignerInfos.FindIndexForSigner(signerInfo);
		if (num < 0)
		{
			throw new CryptographicException("Cannot find the original signer.");
		}
		RemoveSignature(num);
	}

	internal ReadOnlySpan<byte> GetHashableContentSpan()
	{
		ReadOnlyMemory<byte> value = _heldContent.Value;
		if (!_hasPkcs7Content)
		{
			return value.Span;
		}
		return new AsnReader(value, AsnEncodingRules.BER).PeekContentBytes().Span;
	}

	internal void Reencode()
	{
		ContentInfo contentInfo = ContentInfo;
		try
		{
			byte[] encodedMessage = Encode();
			if (Detached)
			{
				_heldContent = null;
			}
			Decode(encodedMessage);
		}
		finally
		{
			ContentInfo = contentInfo;
		}
	}

	private void UpdateMetadata()
	{
		int version = 1;
		if ((_contentType ?? ContentInfo.ContentType.Value) != "1.2.840.113549.1.7.1")
		{
			version = 3;
		}
		else if (_signedData.SignerInfos.Any((SignerInfoAsn si) => si.Version == 3))
		{
			version = 3;
		}
		Version = version;
		_signedData.Version = version;
	}

	private void ConsiderDigestAddition(AlgorithmIdentifierAsn candidate)
	{
		int num = _signedData.DigestAlgorithms.Length;
		for (int i = 0; i < num; i++)
		{
			if (candidate.Equals(ref _signedData.DigestAlgorithms[i]))
			{
				return;
			}
		}
		Array.Resize(ref _signedData.DigestAlgorithms, num + 1);
		_signedData.DigestAlgorithms[num] = candidate;
	}

	private void ConsiderDigestRemoval(AlgorithmIdentifierAsn candidate)
	{
		bool flag = true;
		for (int i = 0; i < _signedData.SignerInfos.Length; i++)
		{
			if (candidate.Equals(ref _signedData.SignerInfos[i].DigestAlgorithm))
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		for (int j = 0; j < _signedData.DigestAlgorithms.Length; j++)
		{
			if (candidate.Equals(ref _signedData.DigestAlgorithms[j]))
			{
				Helpers.RemoveAt(ref _signedData.DigestAlgorithms, j);
				break;
			}
		}
	}

	internal void UpdateCertificatesFromAddition(X509Certificate2Collection newCerts)
	{
		if (newCerts.Count == 0)
		{
			return;
		}
		CertificateChoiceAsn[] certificateSet = _signedData.CertificateSet;
		int num = ((certificateSet != null) ? certificateSet.Length : 0);
		if (num > 0 || newCerts.Count > 1)
		{
			HashSet<X509Certificate2> hashSet = new HashSet<X509Certificate2>(Certificates.OfType<X509Certificate2>());
			for (int i = 0; i < newCerts.Count; i++)
			{
				X509Certificate2 item = newCerts[i];
				if (!hashSet.Add(item))
				{
					newCerts.RemoveAt(i);
					i--;
				}
			}
		}
		if (newCerts.Count != 0)
		{
			if (_signedData.CertificateSet == null)
			{
				_signedData.CertificateSet = new CertificateChoiceAsn[newCerts.Count];
			}
			else
			{
				Array.Resize(ref _signedData.CertificateSet, num + newCerts.Count);
			}
			for (int j = num; j < _signedData.CertificateSet.Length; j++)
			{
				_signedData.CertificateSet[j] = new CertificateChoiceAsn
				{
					Certificate = newCerts[j - num].RawData
				};
			}
		}
	}

	public void CheckSignature(bool verifySignatureOnly)
	{
		CheckSignature(new X509Certificate2Collection(), verifySignatureOnly);
	}

	public void CheckSignature(X509Certificate2Collection extraStore, bool verifySignatureOnly)
	{
		if (!_hasData)
		{
			throw new InvalidOperationException("The CMS message is not signed.");
		}
		if (extraStore == null)
		{
			throw new ArgumentNullException("extraStore");
		}
		CheckSignatures(SignerInfos, extraStore, verifySignatureOnly);
	}

	private static void CheckSignatures(SignerInfoCollection signers, X509Certificate2Collection extraStore, bool verifySignatureOnly)
	{
		if (signers.Count < 1)
		{
			throw new CryptographicException("The signed cryptographic message does not have a signer for the specified signer index.");
		}
		SignerInfoEnumerator enumerator = signers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SignerInfo current = enumerator.Current;
			current.CheckSignature(extraStore, verifySignatureOnly);
			SignerInfoCollection counterSignerInfos = current.CounterSignerInfos;
			if (counterSignerInfos.Count > 0)
			{
				CheckSignatures(counterSignerInfos, extraStore, verifySignatureOnly);
			}
		}
	}

	public void CheckHash()
	{
		if (!_hasData)
		{
			throw new InvalidOperationException("The CMS message is not signed.");
		}
		SignerInfoCollection signerInfos = SignerInfos;
		if (signerInfos.Count < 1)
		{
			throw new CryptographicException("The signed cryptographic message does not have a signer for the specified signer index.");
		}
		SignerInfoEnumerator enumerator = signerInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SignerInfo current = enumerator.Current;
			if (current.SignerIdentifier.Type == SubjectIdentifierType.NoSignature)
			{
				current.CheckHash();
			}
		}
	}

	internal ref SignedDataAsn GetRawData()
	{
		return ref _signedData;
	}
}
