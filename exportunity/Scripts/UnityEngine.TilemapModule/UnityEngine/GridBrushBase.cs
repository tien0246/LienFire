namespace UnityEngine;

public abstract class GridBrushBase : ScriptableObject
{
	public enum Tool
	{
		Select = 0,
		Move = 1,
		Paint = 2,
		Box = 3,
		Pick = 4,
		Erase = 5,
		FloodFill = 6
	}

	public enum RotationDirection
	{
		Clockwise = 0,
		CounterClockwise = 1
	}

	public enum FlipAxis
	{
		X = 0,
		Y = 1
	}

	public virtual void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
	{
	}

	public virtual void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
	{
	}

	public virtual void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
	{
		for (int i = position.zMin; i < position.zMax; i++)
		{
			for (int j = position.yMin; j < position.yMax; j++)
			{
				for (int k = position.xMin; k < position.xMax; k++)
				{
					Paint(gridLayout, brushTarget, new Vector3Int(k, j, i));
				}
			}
		}
	}

	public virtual void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
	{
		for (int i = position.zMin; i < position.zMax; i++)
		{
			for (int j = position.yMin; j < position.yMax; j++)
			{
				for (int k = position.xMin; k < position.xMax; k++)
				{
					Erase(gridLayout, brushTarget, new Vector3Int(k, j, i));
				}
			}
		}
	}

	public virtual void Select(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
	{
	}

	public virtual void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
	{
	}

	public virtual void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
	{
	}

	public virtual void Flip(FlipAxis flip, GridLayout.CellLayout layout)
	{
	}

	public virtual void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
	{
	}

	public virtual void Move(GridLayout gridLayout, GameObject brushTarget, BoundsInt from, BoundsInt to)
	{
	}

	public virtual void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
	{
	}

	public virtual void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
	{
	}

	public virtual void ChangeZPosition(int change)
	{
	}

	public virtual void ResetZPosition()
	{
	}
}
