using System;
using System.Runtime.CompilerServices;

namespace Mirror;

public struct Vector3Long
{
	public long x;

	public long y;

	public long z;

	public static readonly Vector3Long zero = new Vector3Long(0L, 0L, 0L);

	public static readonly Vector3Long one = new Vector3Long(1L, 1L, 1L);

	public static readonly Vector3Long forward = new Vector3Long(0L, 0L, 1L);

	public static readonly Vector3Long back = new Vector3Long(0L, 0L, -1L);

	public static readonly Vector3Long left = new Vector3Long(-1L, 0L, 0L);

	public static readonly Vector3Long right = new Vector3Long(1L, 0L, 0L);

	public static readonly Vector3Long up = new Vector3Long(0L, 1L, 0L);

	public static readonly Vector3Long down = new Vector3Long(0L, -1L, 0L);

	public long this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return index switch
			{
				0 => x, 
				1 => y, 
				2 => z, 
				_ => throw new IndexOutOfRangeException($"Vector3Long[{index}] out of range."), 
			};
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (index)
			{
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
			case 2:
				z = value;
				break;
			default:
				throw new IndexOutOfRangeException($"Vector3Long[{index}] out of range.");
			}
		}
	}

	public Vector3Long(long x, long y, long z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Long operator +(Vector3Long a, Vector3Long b)
	{
		return new Vector3Long(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Long operator -(Vector3Long a, Vector3Long b)
	{
		return new Vector3Long(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Long operator -(Vector3Long v)
	{
		return new Vector3Long(-v.x, -v.y, -v.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Long operator *(Vector3Long a, long n)
	{
		return new Vector3Long(a.x * n, a.y * n, a.z * n);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Long operator *(long n, Vector3Long a)
	{
		return new Vector3Long(a.x * n, a.y * n, a.z * n);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vector3Long a, Vector3Long b)
	{
		if (a.x == b.x && a.y == b.y)
		{
			return a.z == b.z;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vector3Long a, Vector3Long b)
	{
		return !(a == b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		return $"({x} {y} {z})";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Vector3Long other)
	{
		if (x == other.x && y == other.y)
		{
			return z == other.z;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object other)
	{
		if (other is Vector3Long other2)
		{
			return Equals(other2);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return HashCode.Combine(x, y, z);
	}
}
