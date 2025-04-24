using System;
using System.Collections.Generic;

namespace LunarConsolePluginInternal;

internal abstract class BaseList<T> where T : class
{
	protected readonly List<T> list;

	private readonly T nullElement;

	private int removedCount;

	private bool locked;

	public virtual int Count => list.Count - removedCount;

	protected BaseList(T nullElement)
		: this(nullElement, 0)
	{
	}

	protected BaseList(T nullElement, int capacity)
		: this(new List<T>(capacity), nullElement)
	{
		if (nullElement == null)
		{
			throw new ArgumentNullException("nullElement");
		}
	}

	protected BaseList(List<T> list, T nullElement)
	{
		this.list = list;
		this.nullElement = nullElement;
	}

	public virtual bool Add(T e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		list.Add(e);
		return true;
	}

	public virtual bool Remove(T e)
	{
		int num = list.IndexOf(e);
		if (num != -1)
		{
			RemoveAt(num);
			return true;
		}
		return false;
	}

	public virtual T Get(int index)
	{
		return list[index];
	}

	public virtual int IndexOf(T e)
	{
		return list.IndexOf(e);
	}

	public virtual void RemoveAt(int index)
	{
		if (locked)
		{
			removedCount++;
			list[index] = nullElement;
		}
		else
		{
			list.RemoveAt(index);
		}
	}

	public virtual void Clear()
	{
		if (locked)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = nullElement;
			}
			removedCount = list.Count;
		}
		else
		{
			list.Clear();
			removedCount = 0;
		}
	}

	public virtual bool Contains(T e)
	{
		return list.Contains(e);
	}

	private void ClearRemoved()
	{
		int num = list.Count - 1;
		while (removedCount > 0 && num >= 0)
		{
			if (list[num] == nullElement)
			{
				list.RemoveAt(num);
				removedCount--;
			}
			num--;
		}
	}

	protected void Lock()
	{
		locked = true;
	}

	protected void Unlock()
	{
		ClearRemoved();
		locked = false;
	}
}
