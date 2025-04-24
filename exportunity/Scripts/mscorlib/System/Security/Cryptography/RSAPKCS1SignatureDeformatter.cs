using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography;

[ComVisible(true)]
public class RSAPKCS1SignatureDeformatter : AsymmetricSignatureDeformatter
{
	private RSA rsa;

	private string hashName;

	public RSAPKCS1SignatureDeformatter()
	{
	}

	public RSAPKCS1SignatureDeformatter(AsymmetricAlgorithm key)
	{
		SetKey(key);
	}

	public override void SetHashAlgorithm(string strName)
	{
		if (strName == null)
		{
			throw new ArgumentNullException("strName");
		}
		hashName = strName;
	}

	public override void SetKey(AsymmetricAlgorithm key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		rsa = (RSA)key;
	}

	public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
	{
		if (rsa == null)
		{
			throw new CryptographicUnexpectedOperationException(Locale.GetText("No public key available."));
		}
		if (hashName == null)
		{
			throw new CryptographicUnexpectedOperationException(Locale.GetText("Missing hash algorithm."));
		}
		if (rgbHash == null)
		{
			throw new ArgumentNullException("rgbHash");
		}
		if (rgbSignature == null)
		{
			throw new ArgumentNullException("rgbSignature");
		}
		return PKCS1.Verify_v15(rsa, hashName, rgbHash, rgbSignature);
	}
}
