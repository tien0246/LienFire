using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

public struct Grid2D<T>
{
	private readonly Dictionary<Vector2Int, HashSet<T>> grid;

	private readonly Vector2Int[] neighbourOffsets;

	public Grid2D(int initialCapacity)
	{
		grid = new Dictionary<Vector2Int, HashSet<T>>(initialCapacity);
		neighbourOffsets = new Vector2Int[9]
		{
			Vector2Int.up,
			Vector2Int.up + Vector2Int.left,
			Vector2Int.up + Vector2Int.right,
			Vector2Int.left,
			Vector2Int.zero,
			Vector2Int.right,
			Vector2Int.down,
			Vector2Int.down + Vector2Int.left,
			Vector2Int.down + Vector2Int.right
		};
	}

	public void Add(Vector2Int position, T value)
	{
		if (!grid.TryGetValue(position, out var value2))
		{
			value2 = new HashSet<T>(128);
			grid[position] = value2;
		}
		value2.Add(value);
	}

	private void GetAt(Vector2Int position, HashSet<T> result)
	{
		if (!grid.TryGetValue(position, out var value))
		{
			return;
		}
		foreach (T item in value)
		{
			result.Add(item);
		}
	}

	public void GetWithNeighbours(Vector2Int position, HashSet<T> result)
	{
		result.Clear();
		Vector2Int[] array = neighbourOffsets;
		foreach (Vector2Int vector2Int in array)
		{
			GetAt(position + vector2Int, result);
		}
	}

	public void ClearNonAlloc()
	{
		foreach (HashSet<T> value in grid.Values)
		{
			value.Clear();
		}
	}
}
