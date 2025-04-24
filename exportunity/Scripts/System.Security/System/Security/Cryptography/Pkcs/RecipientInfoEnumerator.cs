using System.Collections;
using Unity;

namespace System.Security.Cryptography.Pkcs;

public sealed class RecipientInfoEnumerator : IEnumerator
{
	private readonly RecipientInfoCollection _recipientInfos;

	private int _current;

	public RecipientInfo Current => _recipientInfos[_current];

	object IEnumerator.Current => _recipientInfos[_current];

	internal RecipientInfoEnumerator(RecipientInfoCollection RecipientInfos)
	{
		_recipientInfos = RecipientInfos;
		_current = -1;
	}

	public bool MoveNext()
	{
		if (_current >= _recipientInfos.Count - 1)
		{
			return false;
		}
		_current++;
		return true;
	}

	public void Reset()
	{
		_current = -1;
	}

	internal RecipientInfoEnumerator()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
