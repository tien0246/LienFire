using System.Collections;
using System.Collections.Generic;

namespace System.Security.Cryptography;

public sealed class CryptographicAttributeObjectCollection : ICollection, IEnumerable
{
	private readonly List<CryptographicAttributeObject> _list;

	public CryptographicAttributeObject this[int index] => _list[index];

	public int Count => _list.Count;

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	public CryptographicAttributeObjectCollection()
	{
		_list = new List<CryptographicAttributeObject>();
	}

	public CryptographicAttributeObjectCollection(CryptographicAttributeObject attribute)
	{
		_list = new List<CryptographicAttributeObject>();
		_list.Add(attribute);
	}

	public int Add(AsnEncodedData asnEncodedData)
	{
		if (asnEncodedData == null)
		{
			throw new ArgumentNullException("asnEncodedData");
		}
		return Add(new CryptographicAttributeObject(asnEncodedData.Oid, new AsnEncodedDataCollection(asnEncodedData)));
	}

	public int Add(CryptographicAttributeObject attribute)
	{
		if (attribute == null)
		{
			throw new ArgumentNullException("attribute");
		}
		string value = attribute.Oid.Value;
		for (int i = 0; i < _list.Count; i++)
		{
			CryptographicAttributeObject cryptographicAttributeObject = _list[i];
			if (cryptographicAttributeObject.Values == attribute.Values)
			{
				throw new InvalidOperationException("Duplicate items are not allowed in the collection.");
			}
			string value2 = cryptographicAttributeObject.Oid.Value;
			if (string.Equals(value, value2, StringComparison.OrdinalIgnoreCase))
			{
				if (string.Equals(value, "1.2.840.113549.1.9.5", StringComparison.OrdinalIgnoreCase))
				{
					throw new CryptographicException("Cannot add multiple PKCS 9 signing time attributes.");
				}
				AsnEncodedDataEnumerator enumerator = attribute.Values.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AsnEncodedData current = enumerator.Current;
					cryptographicAttributeObject.Values.Add(current);
				}
				return i;
			}
		}
		int count = _list.Count;
		_list.Add(attribute);
		return count;
	}

	internal void AddWithoutMerge(CryptographicAttributeObject attribute)
	{
		_list.Add(attribute);
	}

	public void Remove(CryptographicAttributeObject attribute)
	{
		if (attribute == null)
		{
			throw new ArgumentNullException("attribute");
		}
		_list.Remove(attribute);
	}

	public CryptographicAttributeObjectEnumerator GetEnumerator()
	{
		return new CryptographicAttributeObjectEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new CryptographicAttributeObjectEnumerator(this);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (array.Rank != 1)
		{
			throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");
		}
		if (index < 0 || index >= array.Length)
		{
			throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
		}
		if (index > array.Length - Count)
		{
			throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
		}
		for (int i = 0; i < Count; i++)
		{
			array.SetValue(this[i], index);
			index++;
		}
	}

	public void CopyTo(CryptographicAttributeObject[] array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (index < 0 || index >= array.Length)
		{
			throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
		}
		if (index > array.Length - Count)
		{
			throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
		}
		_list.CopyTo(array, index);
	}
}
