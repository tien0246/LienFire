using System.Collections;

namespace System.Security.Cryptography.Pkcs;

public sealed class SignerInfoEnumerator : IEnumerator
{
	private readonly SignerInfoCollection _signerInfos;

	private int _position;

	public SignerInfo Current => _signerInfos[_position];

	object IEnumerator.Current => _signerInfos[_position];

	private SignerInfoEnumerator()
	{
	}

	internal SignerInfoEnumerator(SignerInfoCollection signerInfos)
	{
		_signerInfos = signerInfos;
		_position = -1;
	}

	public bool MoveNext()
	{
		int num = _position + 1;
		if (num >= _signerInfos.Count)
		{
			return false;
		}
		_position = num;
		return true;
	}

	public void Reset()
	{
		_position = -1;
	}
}
