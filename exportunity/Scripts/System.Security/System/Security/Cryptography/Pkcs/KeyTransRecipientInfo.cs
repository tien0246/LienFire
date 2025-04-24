using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs;

public sealed class KeyTransRecipientInfo : RecipientInfo
{
	private volatile SubjectIdentifier _lazyRecipientIdentifier;

	private volatile AlgorithmIdentifier _lazyKeyEncryptionAlgorithm;

	private volatile byte[] _lazyEncryptedKey;

	public override int Version => Pal.Version;

	public override SubjectIdentifier RecipientIdentifier => _lazyRecipientIdentifier ?? (_lazyRecipientIdentifier = Pal.RecipientIdentifier);

	public override AlgorithmIdentifier KeyEncryptionAlgorithm => _lazyKeyEncryptionAlgorithm ?? (_lazyKeyEncryptionAlgorithm = Pal.KeyEncryptionAlgorithm);

	public override byte[] EncryptedKey => _lazyEncryptedKey ?? (_lazyEncryptedKey = Pal.EncryptedKey);

	private new KeyTransRecipientInfoPal Pal => (KeyTransRecipientInfoPal)base.Pal;

	internal KeyTransRecipientInfo(KeyTransRecipientInfoPal pal)
		: base(RecipientInfoType.KeyTransport, pal)
	{
	}

	internal KeyTransRecipientInfo()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
