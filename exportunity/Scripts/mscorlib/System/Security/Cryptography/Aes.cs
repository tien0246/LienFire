using System.Runtime.CompilerServices;

namespace System.Security.Cryptography;

[TypeForwardedFrom("System.Core, Version=3.5.0.0, Culture=Neutral, PublicKeyToken=b77a5c561934e089")]
public abstract class Aes : SymmetricAlgorithm
{
	private static KeySizes[] s_legalBlockSizes = new KeySizes[1]
	{
		new KeySizes(128, 128, 0)
	};

	private static KeySizes[] s_legalKeySizes = new KeySizes[1]
	{
		new KeySizes(128, 256, 64)
	};

	protected Aes()
	{
		LegalBlockSizesValue = s_legalBlockSizes;
		LegalKeySizesValue = s_legalKeySizes;
		BlockSizeValue = 128;
		FeedbackSizeValue = 8;
		KeySizeValue = 256;
		ModeValue = CipherMode.CBC;
	}

	public new static Aes Create()
	{
		return Create("AES");
	}

	public new static Aes Create(string algorithmName)
	{
		if (algorithmName == null)
		{
			throw new ArgumentNullException("algorithmName");
		}
		return CryptoConfig.CreateFromName(algorithmName) as Aes;
	}
}
