using System.Collections;

namespace System.Data.Odbc;

[Serializable]
public sealed class OdbcErrorCollection : ICollection, IEnumerable
{
	private ArrayList _items = new ArrayList();

	object ICollection.SyncRoot => this;

	bool ICollection.IsSynchronized => false;

	public int Count => _items.Count;

	public OdbcError this[int i] => (OdbcError)_items[i];

	internal OdbcErrorCollection()
	{
	}

	internal void Add(OdbcError error)
	{
		_items.Add(error);
	}

	public void CopyTo(Array array, int i)
	{
		_items.CopyTo(array, i);
	}

	public void CopyTo(OdbcError[] array, int i)
	{
		_items.CopyTo(array, i);
	}

	public IEnumerator GetEnumerator()
	{
		return _items.GetEnumerator();
	}

	internal void SetSource(string Source)
	{
		foreach (OdbcError item in _items)
		{
			item.SetSource(Source);
		}
	}
}
