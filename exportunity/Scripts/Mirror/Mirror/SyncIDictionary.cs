using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror;

public class SyncIDictionary<TKey, TValue> : SyncObject, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
{
	public delegate void SyncDictionaryChanged(Operation op, TKey key, TValue item);

	public enum Operation : byte
	{
		OP_ADD = 0,
		OP_CLEAR = 1,
		OP_REMOVE = 2,
		OP_SET = 3
	}

	private struct Change
	{
		internal Operation operation;

		internal TKey key;

		internal TValue item;
	}

	protected readonly IDictionary<TKey, TValue> objects;

	private readonly List<Change> changes = new List<Change>();

	private int changesAhead;

	public int Count => objects.Count;

	public bool IsReadOnly => !IsWritable();

	public ICollection<TKey> Keys => objects.Keys;

	public ICollection<TValue> Values => objects.Values;

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => objects.Keys;

	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => objects.Values;

	public TValue this[TKey i]
	{
		get
		{
			return objects[i];
		}
		set
		{
			if (ContainsKey(i))
			{
				objects[i] = value;
				AddOperation(Operation.OP_SET, i, value, checkAccess: true);
			}
			else
			{
				objects[i] = value;
				AddOperation(Operation.OP_ADD, i, value, checkAccess: true);
			}
		}
	}

	public event SyncDictionaryChanged Callback;

	public override void Reset()
	{
		changes.Clear();
		changesAhead = 0;
		objects.Clear();
	}

	public override void ClearChanges()
	{
		changes.Clear();
	}

	public SyncIDictionary(IDictionary<TKey, TValue> objects)
	{
		this.objects = objects;
	}

	private void AddOperation(Operation op, TKey key, TValue item, bool checkAccess)
	{
		if (checkAccess && IsReadOnly)
		{
			throw new InvalidOperationException("SyncDictionaries can only be modified by the owner.");
		}
		Change item2 = new Change
		{
			operation = op,
			key = key,
			item = item
		};
		if (IsRecording())
		{
			changes.Add(item2);
			OnDirty?.Invoke();
		}
		this.Callback?.Invoke(op, key, item);
	}

	public override void OnSerializeAll(NetworkWriter writer)
	{
		writer.WriteUInt((uint)objects.Count);
		foreach (KeyValuePair<TKey, TValue> @object in objects)
		{
			writer.Write(@object.Key);
			writer.Write(@object.Value);
		}
		writer.WriteUInt((uint)changes.Count);
	}

	public override void OnSerializeDelta(NetworkWriter writer)
	{
		writer.WriteUInt((uint)changes.Count);
		for (int i = 0; i < changes.Count; i++)
		{
			Change change = changes[i];
			writer.WriteByte((byte)change.operation);
			switch (change.operation)
			{
			case Operation.OP_ADD:
			case Operation.OP_SET:
				writer.Write(change.key);
				writer.Write(change.item);
				break;
			case Operation.OP_REMOVE:
				writer.Write(change.key);
				break;
			}
		}
	}

	public override void OnDeserializeAll(NetworkReader reader)
	{
		int num = (int)reader.ReadUInt();
		objects.Clear();
		changes.Clear();
		for (int i = 0; i < num; i++)
		{
			TKey key = reader.Read<TKey>();
			TValue value = reader.Read<TValue>();
			objects.Add(key, value);
		}
		changesAhead = (int)reader.ReadUInt();
	}

	public override void OnDeserializeDelta(NetworkReader reader)
	{
		int num = (int)reader.ReadUInt();
		for (int i = 0; i < num; i++)
		{
			Operation operation = (Operation)reader.ReadByte();
			bool flag = changesAhead == 0;
			TKey val = default(TKey);
			TValue value = default(TValue);
			switch (operation)
			{
			case Operation.OP_ADD:
			case Operation.OP_SET:
				val = reader.Read<TKey>();
				value = reader.Read<TValue>();
				if (flag)
				{
					if (ContainsKey(val))
					{
						objects[val] = value;
						AddOperation(Operation.OP_SET, val, value, checkAccess: false);
					}
					else
					{
						objects[val] = value;
						AddOperation(Operation.OP_ADD, val, value, checkAccess: false);
					}
				}
				break;
			case Operation.OP_CLEAR:
				if (flag)
				{
					objects.Clear();
					AddOperation(Operation.OP_CLEAR, default(TKey), default(TValue), checkAccess: false);
				}
				break;
			case Operation.OP_REMOVE:
				val = reader.Read<TKey>();
				if (flag && objects.TryGetValue(val, out value))
				{
					objects.Remove(val);
					AddOperation(Operation.OP_REMOVE, val, value, checkAccess: false);
				}
				break;
			}
			if (!flag)
			{
				changesAhead--;
			}
		}
	}

	public void Clear()
	{
		objects.Clear();
		AddOperation(Operation.OP_CLEAR, default(TKey), default(TValue), checkAccess: true);
	}

	public bool ContainsKey(TKey key)
	{
		return objects.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		if (objects.TryGetValue(key, out var value) && objects.Remove(key))
		{
			AddOperation(Operation.OP_REMOVE, key, value, checkAccess: true);
			return true;
		}
		return false;
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return objects.TryGetValue(key, out value);
	}

	public void Add(TKey key, TValue value)
	{
		objects.Add(key, value);
		AddOperation(Operation.OP_ADD, key, value, checkAccess: true);
	}

	public void Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		if (TryGetValue(item.Key, out var value))
		{
			return EqualityComparer<TValue>.Default.Equals(value, item.Value);
		}
		return false;
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (arrayIndex < 0 || arrayIndex > array.Length)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Array Index Out of Range");
		}
		if (array.Length - arrayIndex < Count)
		{
			throw new ArgumentException("The number of items in the SyncDictionary is greater than the available space from arrayIndex to the end of the destination array");
		}
		int num = arrayIndex;
		foreach (KeyValuePair<TKey, TValue> @object in objects)
		{
			array[num] = @object;
			num++;
		}
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		bool num = objects.Remove(item.Key);
		if (num)
		{
			AddOperation(Operation.OP_REMOVE, item.Key, item.Value, checkAccess: true);
		}
		return num;
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return objects.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return objects.GetEnumerator();
	}
}
