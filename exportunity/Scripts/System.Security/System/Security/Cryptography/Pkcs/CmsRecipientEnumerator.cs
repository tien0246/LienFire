using System.Collections;
using Unity;

namespace System.Security.Cryptography.Pkcs;

public sealed class CmsRecipientEnumerator : IEnumerator
{
	private readonly CmsRecipientCollection _recipients;

	private int _current;

	public CmsRecipient Current => _recipients[_current];

	object IEnumerator.Current => _recipients[_current];

	internal CmsRecipientEnumerator(CmsRecipientCollection recipients)
	{
		_recipients = recipients;
		_current = -1;
	}

	public bool MoveNext()
	{
		if (_current >= _recipients.Count - 1)
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

	internal CmsRecipientEnumerator()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
