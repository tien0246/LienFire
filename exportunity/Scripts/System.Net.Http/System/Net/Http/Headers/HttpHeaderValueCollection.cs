using System.Collections;
using System.Collections.Generic;
using Unity;

namespace System.Net.Http.Headers;

public sealed class HttpHeaderValueCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : class
{
	private HeaderDescriptor _descriptor;

	private HttpHeaders _store;

	private T _specialValue;

	private Action<HttpHeaderValueCollection<T>, T> _validator;

	public int Count => GetCount();

	public bool IsReadOnly => false;

	internal bool IsSpecialValueSet
	{
		get
		{
			if (_specialValue == null)
			{
				return false;
			}
			return _store.ContainsParsedValue(_descriptor, _specialValue);
		}
	}

	internal HttpHeaderValueCollection(HeaderDescriptor descriptor, HttpHeaders store)
		: this(descriptor, store, (T)null, (Action<HttpHeaderValueCollection<T>, T>)null)
	{
	}

	internal HttpHeaderValueCollection(HeaderDescriptor descriptor, HttpHeaders store, Action<HttpHeaderValueCollection<T>, T> validator)
		: this(descriptor, store, (T)null, validator)
	{
	}

	internal HttpHeaderValueCollection(HeaderDescriptor descriptor, HttpHeaders store, T specialValue)
		: this(descriptor, store, specialValue, (Action<HttpHeaderValueCollection<T>, T>)null)
	{
	}

	internal HttpHeaderValueCollection(HeaderDescriptor descriptor, HttpHeaders store, T specialValue, Action<HttpHeaderValueCollection<T>, T> validator)
	{
		_store = store;
		_descriptor = descriptor;
		_specialValue = specialValue;
		_validator = validator;
	}

	public void Add(T item)
	{
		CheckValue(item);
		_store.AddParsedValue(_descriptor, item);
	}

	public void ParseAdd(string input)
	{
		_store.Add(_descriptor, input);
	}

	public bool TryParseAdd(string input)
	{
		return _store.TryParseAndAddValue(_descriptor, input);
	}

	public void Clear()
	{
		_store.Remove(_descriptor);
	}

	public bool Contains(T item)
	{
		CheckValue(item);
		return _store.ContainsParsedValue(_descriptor, item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0 || arrayIndex > array.Length)
		{
			throw new ArgumentOutOfRangeException("arrayIndex");
		}
		object parsedValues = _store.GetParsedValues(_descriptor);
		if (parsedValues == null)
		{
			return;
		}
		if (!(parsedValues is List<object> list))
		{
			if (arrayIndex == array.Length)
			{
				throw new ArgumentException("The number of elements is greater than the available space from arrayIndex to the end of the destination array.");
			}
			array[arrayIndex] = parsedValues as T;
		}
		else
		{
			list.CopyTo(array, arrayIndex);
		}
	}

	public bool Remove(T item)
	{
		CheckValue(item);
		return _store.RemoveParsedValue(_descriptor, item);
	}

	public IEnumerator<T> GetEnumerator()
	{
		object parsedValues = _store.GetParsedValues(_descriptor);
		if (parsedValues == null)
		{
			yield break;
		}
		if (!(parsedValues is List<object> list))
		{
			yield return parsedValues as T;
			yield break;
		}
		foreach (object item in list)
		{
			yield return item as T;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override string ToString()
	{
		return _store.GetHeaderString(_descriptor);
	}

	internal void SetSpecialValue()
	{
		if (!_store.ContainsParsedValue(_descriptor, _specialValue))
		{
			_store.AddParsedValue(_descriptor, _specialValue);
		}
	}

	internal void RemoveSpecialValue()
	{
		_store.RemoveParsedValue(_descriptor, _specialValue);
	}

	private void CheckValue(T item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (_validator != null)
		{
			_validator(this, item);
		}
	}

	private int GetCount()
	{
		object parsedValues = _store.GetParsedValues(_descriptor);
		if (parsedValues == null)
		{
			return 0;
		}
		if (!(parsedValues is List<object> list))
		{
			return 1;
		}
		return list.Count;
	}

	internal HttpHeaderValueCollection()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
