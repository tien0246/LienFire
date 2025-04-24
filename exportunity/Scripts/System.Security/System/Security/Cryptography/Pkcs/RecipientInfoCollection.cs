using System.Collections;
using System.Collections.Generic;

namespace System.Security.Cryptography.Pkcs;

public sealed class RecipientInfoCollection : ICollection, IEnumerable
{
	private readonly RecipientInfo[] _recipientInfos;

	public RecipientInfo this[int index]
	{
		get
		{
			if (index < 0 || index >= _recipientInfos.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			return _recipientInfos[index];
		}
	}

	public int Count => _recipientInfos.Length;

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	internal RecipientInfoCollection()
	{
		_recipientInfos = Array.Empty<RecipientInfo>();
	}

	internal RecipientInfoCollection(RecipientInfo recipientInfo)
	{
		_recipientInfos = new RecipientInfo[1] { recipientInfo };
	}

	internal RecipientInfoCollection(ICollection<RecipientInfo> recipientInfos)
	{
		_recipientInfos = new RecipientInfo[recipientInfos.Count];
		recipientInfos.CopyTo(_recipientInfos, 0);
	}

	public RecipientInfoEnumerator GetEnumerator()
	{
		return new RecipientInfoEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void CopyTo(Array array, int index)
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

	public void CopyTo(RecipientInfo[] array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (index < 0 || index >= array.Length)
		{
			throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
		}
		_recipientInfos.CopyTo(array, index);
	}
}
