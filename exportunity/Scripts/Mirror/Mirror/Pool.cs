using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mirror;

public class Pool<T>
{
	private readonly Stack<T> objects = new Stack<T>();

	private readonly Func<T> objectGenerator;

	public int Count => objects.Count;

	public Pool(Func<T> objectGenerator, int initialCapacity)
	{
		this.objectGenerator = objectGenerator;
		for (int i = 0; i < initialCapacity; i++)
		{
			objects.Push(objectGenerator());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T Get()
	{
		if (objects.Count <= 0)
		{
			return objectGenerator();
		}
		return objects.Pop();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Return(T item)
	{
		objects.Push(item);
	}
}
