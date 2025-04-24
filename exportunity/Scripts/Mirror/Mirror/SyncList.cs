using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror;

public class SyncList<T> : SyncObject, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyList<T>, IReadOnlyCollection<T>
{
	public delegate void SyncListChanged(Operation op, int itemIndex, T oldItem, T newItem);

	public enum Operation : byte
	{
		OP_ADD = 0,
		OP_CLEAR = 1,
		OP_INSERT = 2,
		OP_REMOVEAT = 3,
		OP_SET = 4
	}

	private struct Change
	{
		internal Operation operation;

		internal int index;

		internal T item;
	}

	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private readonly SyncList<T> list;

		private int index;

		public T Current { get; private set; }

		object IEnumerator.Current => Current;

		public Enumerator(SyncList<T> list)
		{
			this.list = list;
			index = -1;
			Current = default(T);
		}

		public bool MoveNext()
		{
			if (++index >= list.Count)
			{
				return false;
			}
			Current = list[index];
			return true;
		}

		public void Reset()
		{
			index = -1;
		}

		public void Dispose()
		{
		}
	}

	private readonly IList<T> objects;

	private readonly IEqualityComparer<T> comparer;

	private readonly List<Change> changes = new List<Change>();

	private int changesAhead;

	public int Count => objects.Count;

	public bool IsReadOnly => !IsWritable();

	public T this[int i]
	{
		get
		{
			return objects[i];
		}
		set
		{
			if (!comparer.Equals(objects[i], value))
			{
				T oldItem = objects[i];
				objects[i] = value;
				AddOperation(Operation.OP_SET, i, oldItem, value, checkAccess: true);
			}
		}
	}

	public event SyncListChanged Callback;

	public SyncList()
		: this((IEqualityComparer<T>)EqualityComparer<T>.Default)
	{
	}

	public SyncList(IEqualityComparer<T> comparer)
	{
		this.comparer = comparer ?? EqualityComparer<T>.Default;
		objects = new List<T>();
	}

	public SyncList(IList<T> objects, IEqualityComparer<T> comparer = null)
	{
		this.comparer = comparer ?? EqualityComparer<T>.Default;
		this.objects = objects;
	}

	public override void ClearChanges()
	{
		changes.Clear();
	}

	public override void Reset()
	{
		changes.Clear();
		changesAhead = 0;
		objects.Clear();
	}

	private void AddOperation(Operation op, int itemIndex, T oldItem, T newItem, bool checkAccess)
	{
		if (checkAccess && IsReadOnly)
		{
			throw new InvalidOperationException("Synclists can only be modified by the owner.");
		}
		Change item = new Change
		{
			operation = op,
			index = itemIndex,
			item = newItem
		};
		if (IsRecording())
		{
			changes.Add(item);
			OnDirty?.Invoke();
		}
		this.Callback?.Invoke(op, itemIndex, oldItem, newItem);
	}

	public override void OnSerializeAll(NetworkWriter writer)
	{
		writer.WriteUInt((uint)objects.Count);
		for (int i = 0; i < objects.Count; i++)
		{
			T value = objects[i];
			writer.Write(value);
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
				writer.Write(change.item);
				break;
			case Operation.OP_REMOVEAT:
				writer.WriteUInt((uint)change.index);
				break;
			case Operation.OP_INSERT:
			case Operation.OP_SET:
				writer.WriteUInt((uint)change.index);
				writer.Write(change.item);
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
			T item = reader.Read<T>();
			objects.Add(item);
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
			int num2 = 0;
			T val = default(T);
			T val2 = default(T);
			switch (operation)
			{
			case Operation.OP_ADD:
				val2 = reader.Read<T>();
				if (flag)
				{
					num2 = objects.Count;
					objects.Add(val2);
					AddOperation(Operation.OP_ADD, objects.Count - 1, default(T), val2, checkAccess: false);
				}
				break;
			case Operation.OP_CLEAR:
				if (flag)
				{
					objects.Clear();
					AddOperation(Operation.OP_CLEAR, 0, default(T), default(T), checkAccess: false);
				}
				break;
			case Operation.OP_INSERT:
				num2 = (int)reader.ReadUInt();
				val2 = reader.Read<T>();
				if (flag)
				{
					objects.Insert(num2, val2);
					AddOperation(Operation.OP_INSERT, num2, default(T), val2, checkAccess: false);
				}
				break;
			case Operation.OP_REMOVEAT:
				num2 = (int)reader.ReadUInt();
				if (flag)
				{
					val = objects[num2];
					objects.RemoveAt(num2);
					AddOperation(Operation.OP_REMOVEAT, num2, val, default(T), checkAccess: false);
				}
				break;
			case Operation.OP_SET:
				num2 = (int)reader.ReadUInt();
				val2 = reader.Read<T>();
				if (flag)
				{
					val = objects[num2];
					objects[num2] = val2;
					AddOperation(Operation.OP_SET, num2, val, val2, checkAccess: false);
				}
				break;
			}
			if (!flag)
			{
				changesAhead--;
			}
		}
	}

	public void Add(T item)
	{
		objects.Add(item);
		AddOperation(Operation.OP_ADD, objects.Count - 1, default(T), item, checkAccess: true);
	}

	public void AddRange(IEnumerable<T> range)
	{
		foreach (T item in range)
		{
			Add(item);
		}
	}

	public void Clear()
	{
		objects.Clear();
		AddOperation(Operation.OP_CLEAR, 0, default(T), default(T), checkAccess: true);
	}

	public bool Contains(T item)
	{
		return IndexOf(item) >= 0;
	}

	public void CopyTo(T[] array, int index)
	{
		objects.CopyTo(array, index);
	}

	public int IndexOf(T item)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (comparer.Equals(item, objects[i]))
			{
				return i;
			}
		}
		return -1;
	}

	public int FindIndex(Predicate<T> match)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (match(objects[i]))
			{
				return i;
			}
		}
		return -1;
	}

	public T Find(Predicate<T> match)
	{
		int num = FindIndex(match);
		if (num == -1)
		{
			return default(T);
		}
		return objects[num];
	}

	public List<T> FindAll(Predicate<T> match)
	{
		List<T> list = new List<T>();
		for (int i = 0; i < objects.Count; i++)
		{
			if (match(objects[i]))
			{
				list.Add(objects[i]);
			}
		}
		return list;
	}

	public void Insert(int index, T item)
	{
		objects.Insert(index, item);
		AddOperation(Operation.OP_INSERT, index, default(T), item, checkAccess: true);
	}

	public void InsertRange(int index, IEnumerable<T> range)
	{
		foreach (T item in range)
		{
			Insert(index, item);
			index++;
		}
	}

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		bool num2 = num >= 0;
		if (num2)
		{
			RemoveAt(num);
		}
		return num2;
	}

	public void RemoveAt(int index)
	{
		T oldItem = objects[index];
		objects.RemoveAt(index);
		AddOperation(Operation.OP_REMOVEAT, index, oldItem, default(T), checkAccess: true);
	}

	public int RemoveAll(Predicate<T> match)
	{
		List<T> list = new List<T>();
		for (int i = 0; i < objects.Count; i++)
		{
			if (match(objects[i]))
			{
				list.Add(objects[i]);
			}
		}
		foreach (T item in list)
		{
			Remove(item);
		}
		return list.Count;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this);
	}
}
