using System.Threading;
using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs;

public sealed class KeyAgreeRecipientInfo : RecipientInfo
{
	private volatile SubjectIdentifier _lazyRecipientIdentifier;

	private volatile AlgorithmIdentifier _lazyKeyEncryptionAlgorithm;

	private volatile byte[] _lazyEncryptedKey;

	private volatile SubjectIdentifierOrKey _lazyOriginatorIdentifierKey;

	private DateTime? _lazyDate;

	private volatile CryptographicAttributeObject _lazyOtherKeyAttribute;

	public override int Version => Pal.Version;

	public override SubjectIdentifier RecipientIdentifier => _lazyRecipientIdentifier ?? (_lazyRecipientIdentifier = Pal.RecipientIdentifier);

	public override AlgorithmIdentifier KeyEncryptionAlgorithm => _lazyKeyEncryptionAlgorithm ?? (_lazyKeyEncryptionAlgorithm = Pal.KeyEncryptionAlgorithm);

	public override byte[] EncryptedKey => _lazyEncryptedKey ?? (_lazyEncryptedKey = Pal.EncryptedKey);

	public SubjectIdentifierOrKey OriginatorIdentifierOrKey => _lazyOriginatorIdentifierKey ?? (_lazyOriginatorIdentifierKey = Pal.OriginatorIdentifierOrKey);

	public DateTime Date
	{
		get
		{
			if (!_lazyDate.HasValue)
			{
				_lazyDate = Pal.Date;
				Interlocked.MemoryBarrier();
			}
			return _lazyDate.Value;
		}
	}

	public CryptographicAttributeObject OtherKeyAttribute => _lazyOtherKeyAttribute ?? (_lazyOtherKeyAttribute = Pal.OtherKeyAttribute);

	private new KeyAgreeRecipientInfoPal Pal => (KeyAgreeRecipientInfoPal)base.Pal;

	internal KeyAgreeRecipientInfo(KeyAgreeRecipientInfoPal pal)
		: base(RecipientInfoType.KeyAgreement, pal)
	{
	}

	internal KeyAgreeRecipientInfo()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
