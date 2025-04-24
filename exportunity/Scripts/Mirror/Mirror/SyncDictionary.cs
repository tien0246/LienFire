using System.Collections.Generic;

namespace Mirror;

public class SyncDictionary<TKey, TValue> : SyncIDictionary<TKey, TValue>
{
	public new Dictionary<TKey, TValue>.ValueCollection Values => ((Dictionary<TKey, TValue>)objects).Values;

	public new Dictionary<TKey, TValue>.KeyCollection Keys => ((Dictionary<TKey, TValue>)objects).Keys;

	public SyncDictionary()
		: base((IDictionary<TKey, TValue>)new Dictionary<TKey, TValue>())
	{
	}

	public SyncDictionary(IEqualityComparer<TKey> eq)
		: base((IDictionary<TKey, TValue>)new Dictionary<TKey, TValue>(eq))
	{
	}

	public SyncDictionary(IDictionary<TKey, TValue> d)
		: base((IDictionary<TKey, TValue>)new Dictionary<TKey, TValue>(d))
	{
	}

	public new Dictionary<TKey, TValue>.Enumerator GetEnumerator()
	{
		return ((Dictionary<TKey, TValue>)objects).GetEnumerator();
	}
}
