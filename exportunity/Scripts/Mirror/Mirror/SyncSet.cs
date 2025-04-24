using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror;

public class SyncSet<T> : SyncObject, ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	public delegate void SyncSetChanged(Operation op, T item);

	public enum Operation : byte
	{
		OP_ADD = 0,
		OP_CLEAR = 1,
		OP_REMOVE = 2
	}

	private struct Change
	{
		internal Operation operation;

		internal T item;
	}

	protected readonly ISet<T> objects;

	private readonly List<Change> changes = new List<Change>();

	private int changesAhead;

	public int Count => objects.Count;

	public bool IsReadOnly => !IsWritable();

	public event SyncSetChanged Callback;

	public SyncSet(ISet<T> objects)
	{
		this.objects = objects;
	}

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

	private void AddOperation(Operation op, T item, bool checkAccess)
	{
		if (checkAccess && IsReadOnly)
		{
			throw new InvalidOperationException("SyncSets can only be modified by the owner.");
		}
		Change item2 = new Change
		{
			operation = op,
			item = item
		};
		if (IsRecording())
		{
			changes.Add(item2);
			OnDirty?.Invoke();
		}
		this.Callback?.Invoke(op, item);
	}

	private void AddOperation(Operation op, bool checkAccess)
	{
		AddOperation(op, default(T), checkAccess);
	}

	public override void OnSerializeAll(NetworkWriter writer)
	{
		writer.WriteUInt((uint)objects.Count);
		foreach (T @object in objects)
		{
			writer.Write(@object);
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
			case Operation.OP_REMOVE:
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
			T val = default(T);
			switch (operation)
			{
			case Operation.OP_ADD:
				val = reader.Read<T>();
				if (flag)
				{
					objects.Add(val);
					AddOperation(Operation.OP_ADD, val, checkAccess: false);
				}
				break;
			case Operation.OP_CLEAR:
				if (flag)
				{
					objects.Clear();
					AddOperation(Operation.OP_CLEAR, checkAccess: false);
				}
				break;
			case Operation.OP_REMOVE:
				val = reader.Read<T>();
				if (flag)
				{
					objects.Remove(val);
					AddOperation(Operation.OP_REMOVE, val, checkAccess: false);
				}
				break;
			}
			if (!flag)
			{
				changesAhead--;
			}
		}
	}

	public bool Add(T item)
	{
		if (objects.Add(item))
		{
			AddOperation(Operation.OP_ADD, item, checkAccess: true);
			return true;
		}
		return false;
	}

	void ICollection<T>.Add(T item)
	{
		if (objects.Add(item))
		{
			AddOperation(Operation.OP_ADD, item, checkAccess: true);
		}
	}

	public void Clear()
	{
		objects.Clear();
		AddOperation(Operation.OP_CLEAR, checkAccess: true);
	}

	public bool Contains(T item)
	{
		return objects.Contains(item);
	}

	public void CopyTo(T[] array, int index)
	{
		objects.CopyTo(array, index);
	}

	public bool Remove(T item)
	{
		if (objects.Remove(item))
		{
			AddOperation(Operation.OP_REMOVE, item, checkAccess: true);
			return true;
		}
		return false;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return objects.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void ExceptWith(IEnumerable<T> other)
	{
		if (other == this)
		{
			Clear();
			return;
		}
		foreach (T item in other)
		{
			Remove(item);
		}
	}

	public void IntersectWith(IEnumerable<T> other)
	{
		if (other is ISet<T> otherSet)
		{
			IntersectWithSet(otherSet);
			return;
		}
		HashSet<T> otherSet2 = new HashSet<T>(other);
		IntersectWithSet(otherSet2);
	}

	private void IntersectWithSet(ISet<T> otherSet)
	{
		foreach (T item in new List<T>(objects))
		{
			if (!otherSet.Contains(item))
			{
				Remove(item);
			}
		}
	}

	public bool IsProperSubsetOf(IEnumerable<T> other)
	{
		return objects.IsProperSubsetOf(other);
	}

	public bool IsProperSupersetOf(IEnumerable<T> other)
	{
		return objects.IsProperSupersetOf(other);
	}

	public bool IsSubsetOf(IEnumerable<T> other)
	{
		return objects.IsSubsetOf(other);
	}

	public bool IsSupersetOf(IEnumerable<T> other)
	{
		return objects.IsSupersetOf(other);
	}

	public bool Overlaps(IEnumerable<T> other)
	{
		return objects.Overlaps(other);
	}

	public bool SetEquals(IEnumerable<T> other)
	{
		return objects.SetEquals(other);
	}

	public void SymmetricExceptWith(IEnumerable<T> other)
	{
		if (other == this)
		{
			Clear();
			return;
		}
		foreach (T item in other)
		{
			if (!Remove(item))
			{
				Add(item);
			}
		}
	}

	public void UnionWith(IEnumerable<T> other)
	{
		if (other == this)
		{
			return;
		}
		foreach (T item in other)
		{
			Add(item);
		}
	}
}
