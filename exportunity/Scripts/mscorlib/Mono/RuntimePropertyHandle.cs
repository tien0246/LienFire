using System;

namespace Mono;

internal struct RuntimePropertyHandle
{
	private IntPtr value;

	public IntPtr Value => value;

	internal RuntimePropertyHandle(IntPtr v)
	{
		value = v;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		return value == ((RuntimePropertyHandle)obj).Value;
	}

	public bool Equals(RuntimePropertyHandle handle)
	{
		return value == handle.Value;
	}

	public override int GetHashCode()
	{
		return value.GetHashCode();
	}

	public static bool operator ==(RuntimePropertyHandle left, RuntimePropertyHandle right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RuntimePropertyHandle left, RuntimePropertyHandle right)
	{
		return !left.Equals(right);
	}
}
