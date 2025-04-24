using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

public sealed class EnvelopedCms
{
	private enum LastCall
	{
		Ctor = 1,
		Encrypt = 2,
		Decode = 3,
		Decrypt = 4
	}

	private DecryptorPal _decryptorPal;

	private byte[] _encodedMessage;

	private LastCall _lastCall;

	public int Version { get; private set; }

	public ContentInfo ContentInfo { get; private set; }

	public AlgorithmIdentifier ContentEncryptionAlgorithm { get; private set; }

	public X509Certificate2Collection Certificates { get; private set; }

	public CryptographicAttributeObjectCollection UnprotectedAttributes { get; private set; }

	public RecipientInfoCollection RecipientInfos
	{
		get
		{
			switch (_lastCall)
			{
			case LastCall.Ctor:
				return new RecipientInfoCollection();
			case LastCall.Encrypt:
				throw PkcsPal.Instance.CreateRecipientInfosAfterEncryptException();
			case LastCall.Decode:
			case LastCall.Decrypt:
				return _decryptorPal.RecipientInfos;
			default:
				throw new InvalidOperationException();
			}
		}
	}

	public EnvelopedCms()
		: this(new ContentInfo(Array.Empty<byte>()))
	{
	}

	public EnvelopedCms(ContentInfo contentInfo)
		: this(contentInfo, new AlgorithmIdentifier(Oid.FromOidValue("1.2.840.113549.3.7", OidGroup.EncryptionAlgorithm)))
	{
	}

	public EnvelopedCms(ContentInfo contentInfo, AlgorithmIdentifier encryptionAlgorithm)
	{
		if (contentInfo == null)
		{
			throw new ArgumentNullException("contentInfo");
		}
		if (encryptionAlgorithm == null)
		{
			throw new ArgumentNullException("encryptionAlgorithm");
		}
		Version = 0;
		ContentInfo = contentInfo;
		ContentEncryptionAlgorithm = encryptionAlgorithm;
		Certificates = new X509Certificate2Collection();
		UnprotectedAttributes = new CryptographicAttributeObjectCollection();
		_decryptorPal = null;
		_lastCall = LastCall.Ctor;
	}

	public void Encrypt(CmsRecipient recipient)
	{
		if (recipient == null)
		{
			throw new ArgumentNullException("recipient");
		}
		Encrypt(new CmsRecipientCollection(recipient));
	}

	public void Encrypt(CmsRecipientCollection recipients)
	{
		if (recipients == null)
		{
			throw new ArgumentNullException("recipients");
		}
		if (recipients.Count == 0)
		{
			throw new PlatformNotSupportedException("The recipients collection is empty. You must specify at least one recipient. This platform does not implement the certificate picker UI.");
		}
		if (_decryptorPal != null)
		{
			_decryptorPal.Dispose();
			_decryptorPal = null;
		}
		_encodedMessage = PkcsPal.Instance.Encrypt(recipients, ContentInfo, ContentEncryptionAlgorithm, Certificates, UnprotectedAttributes);
		_lastCall = LastCall.Encrypt;
	}

	public byte[] Encode()
	{
		if (_encodedMessage == null)
		{
			throw new InvalidOperationException("The CMS message is not encrypted.");
		}
		return _encodedMessage.CloneByteArray();
	}

	public void Decode(byte[] encodedMessage)
	{
		if (encodedMessage == null)
		{
			throw new ArgumentNullException("encodedMessage");
		}
		if (_decryptorPal != null)
		{
			_decryptorPal.Dispose();
			_decryptorPal = null;
		}
		_decryptorPal = PkcsPal.Instance.Decode(encodedMessage, out var version, out var contentInfo, out var contentEncryptionAlgorithm, out var originatorCerts, out var unprotectedAttributes);
		Version = version;
		ContentInfo = contentInfo;
		ContentEncryptionAlgorithm = contentEncryptionAlgorithm;
		Certificates = originatorCerts;
		UnprotectedAttributes = unprotectedAttributes;
		_encodedMessage = contentInfo.Content.CloneByteArray();
		_lastCall = LastCall.Decode;
	}

	public void Decrypt()
	{
		DecryptContent(RecipientInfos, null);
	}

	public void Decrypt(RecipientInfo recipientInfo)
	{
		if (recipientInfo == null)
		{
			throw new ArgumentNullException("recipientInfo");
		}
		DecryptContent(new RecipientInfoCollection(recipientInfo), null);
	}

	public void Decrypt(RecipientInfo recipientInfo, X509Certificate2Collection extraStore)
	{
		if (recipientInfo == null)
		{
			throw new ArgumentNullException("recipientInfo");
		}
		if (extraStore == null)
		{
			throw new ArgumentNullException("extraStore");
		}
		DecryptContent(new RecipientInfoCollection(recipientInfo), extraStore);
	}

	public void Decrypt(X509Certificate2Collection extraStore)
	{
		if (extraStore == null)
		{
			throw new ArgumentNullException("extraStore");
		}
		DecryptContent(RecipientInfos, extraStore);
	}

	private void DecryptContent(RecipientInfoCollection recipientInfos, X509Certificate2Collection extraStore)
	{
		switch (_lastCall)
		{
		case LastCall.Ctor:
			throw new InvalidOperationException("The CMS message is not encrypted.");
		case LastCall.Encrypt:
			throw PkcsPal.Instance.CreateDecryptAfterEncryptException();
		case LastCall.Decrypt:
			throw PkcsPal.Instance.CreateDecryptTwiceException();
		default:
			throw new InvalidOperationException();
		case LastCall.Decode:
		{
			extraStore = extraStore ?? new X509Certificate2Collection();
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			PkcsPal.Instance.AddCertsFromStoreForDecryption(x509Certificate2Collection);
			x509Certificate2Collection.AddRange(extraStore);
			X509Certificate2Collection certificates = Certificates;
			ContentInfo contentInfo = null;
			Exception exception = PkcsPal.Instance.CreateRecipientsNotFoundException();
			RecipientInfoEnumerator enumerator = recipientInfos.GetEnumerator();
			while (enumerator.MoveNext())
			{
				RecipientInfo current = enumerator.Current;
				X509Certificate2 x509Certificate = x509Certificate2Collection.TryFindMatchingCertificate(current.RecipientIdentifier);
				if (x509Certificate == null)
				{
					exception = PkcsPal.Instance.CreateRecipientsNotFoundException();
					continue;
				}
				contentInfo = _decryptorPal.TryDecrypt(current, x509Certificate, certificates, extraStore, out exception);
				if (exception == null)
				{
					break;
				}
			}
			if (exception != null)
			{
				throw exception;
			}
			ContentInfo = contentInfo;
			_encodedMessage = contentInfo.Content.CloneByteArray();
			_lastCall = LastCall.Decrypt;
			break;
		}
		}
	}

	public EnvelopedCms(SubjectIdentifierType recipientIdentifierType, ContentInfo contentInfo)
		: this(contentInfo)
	{
		if (recipientIdentifierType == SubjectIdentifierType.SubjectKeyIdentifier)
		{
			Version = 2;
		}
	}

	public EnvelopedCms(SubjectIdentifierType recipientIdentifierType, ContentInfo contentInfo, AlgorithmIdentifier encryptionAlgorithm)
		: this(contentInfo, encryptionAlgorithm)
	{
		if (recipientIdentifierType == SubjectIdentifierType.SubjectKeyIdentifier)
		{
			Version = 2;
		}
	}

	public void Encrypt()
	{
		Encrypt(new CmsRecipientCollection());
	}
}
