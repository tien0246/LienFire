using System.Collections;
using Unity;

namespace System.Security.Cryptography;

public sealed class CryptographicAttributeObjectEnumerator : IEnumerator
{
	private readonly CryptographicAttributeObjectCollection _attributes;

	private int _current;

	public CryptographicAttributeObject Current => _attributes[_current];

	object IEnumerator.Current => _attributes[_current];

	internal CryptographicAttributeObjectEnumerator(CryptographicAttributeObjectCollection attributes)
	{
		_attributes = attributes;
		_current = -1;
	}

	public bool MoveNext()
	{
		if (_current >= _attributes.Count - 1)
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

	internal CryptographicAttributeObjectEnumerator()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
