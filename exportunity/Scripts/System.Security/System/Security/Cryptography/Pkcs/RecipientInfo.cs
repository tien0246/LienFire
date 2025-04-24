using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs;

public abstract class RecipientInfo
{
	public RecipientInfoType Type { get; }

	public abstract int Version { get; }

	public abstract SubjectIdentifier RecipientIdentifier { get; }

	public abstract AlgorithmIdentifier KeyEncryptionAlgorithm { get; }

	public abstract byte[] EncryptedKey { get; }

	internal RecipientInfoPal Pal { get; }

	internal RecipientInfo(RecipientInfoType type, RecipientInfoPal pal)
	{
		Type = type;
		Pal = pal;
	}

	internal RecipientInfo()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
