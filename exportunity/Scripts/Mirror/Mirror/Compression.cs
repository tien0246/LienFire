using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public static class Compression
{
	private const float QuaternionMinRange = -0.707107f;

	private const float QuaternionMaxRange = 0.707107f;

	private const ushort TenBitsMax = 1023;

	public static bool ScaleToLong(float value, float precision, out long result)
	{
		if (precision == 0f)
		{
			throw new DivideByZeroException("ScaleToLong: precision=0 would cause null division. If rounding isn't wanted, don't call this function.");
		}
		try
		{
			result = Convert.ToInt64(value / precision);
			return true;
		}
		catch (OverflowException)
		{
			result = ((value > 0f) ? long.MaxValue : long.MinValue);
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ScaleToLong(Vector3 value, float precision, out long x, out long y, out long z)
	{
		return (byte)(1u & (ScaleToLong(value.x, precision, out x) ? 1u : 0u) & (ScaleToLong(value.y, precision, out y) ? 1u : 0u) & (ScaleToLong(value.z, precision, out z) ? 1u : 0u)) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ScaleToLong(Vector3 value, float precision, out Vector3Long quantized)
	{
		quantized = Vector3Long.zero;
		return ScaleToLong(value, precision, out quantized.x, out quantized.y, out quantized.z);
	}

	public static float ScaleToFloat(long value, float precision)
	{
		if (precision == 0f)
		{
			throw new DivideByZeroException("ScaleToLong: precision=0 would cause null division. If rounding isn't wanted, don't call this function.");
		}
		return (float)value * precision;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 ScaleToFloat(long x, long y, long z, float precision)
	{
		Vector3 result = default(Vector3);
		result.x = ScaleToFloat(x, precision);
		result.y = ScaleToFloat(y, precision);
		result.z = ScaleToFloat(z, precision);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 ScaleToFloat(Vector3Long value, float precision)
	{
		return ScaleToFloat(value.x, value.y, value.z, precision);
	}

	public static ushort ScaleFloatToUShort(float value, float minValue, float maxValue, ushort minTarget, ushort maxTarget)
	{
		int num = maxTarget - minTarget;
		float num2 = maxValue - minValue;
		float num3 = value - minValue;
		return (ushort)(minTarget + (ushort)(num3 / num2 * (float)num));
	}

	public static float ScaleUShortToFloat(ushort value, ushort minValue, ushort maxValue, float minTarget, float maxTarget)
	{
		float num = maxTarget - minTarget;
		ushort num2 = (ushort)(maxValue - minValue);
		ushort num3 = (ushort)(value - minValue);
		return minTarget + (float)(int)num3 / (float)(int)num2 * num;
	}

	public static int LargestAbsoluteComponentIndex(Vector4 value, out float largestAbs, out Vector3 withoutLargest)
	{
		Vector4 vector = new Vector4(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z), Mathf.Abs(value.w));
		largestAbs = vector.x;
		withoutLargest = new Vector3(value.y, value.z, value.w);
		int result = 0;
		if (vector.y > largestAbs)
		{
			result = 1;
			largestAbs = vector.y;
			withoutLargest = new Vector3(value.x, value.z, value.w);
		}
		if (vector.z > largestAbs)
		{
			result = 2;
			largestAbs = vector.z;
			withoutLargest = new Vector3(value.x, value.y, value.w);
		}
		if (vector.w > largestAbs)
		{
			result = 3;
			largestAbs = vector.w;
			withoutLargest = new Vector3(value.x, value.y, value.z);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float QuaternionElement(Quaternion q, int element)
	{
		return element switch
		{
			0 => q.x, 
			1 => q.y, 
			2 => q.z, 
			3 => q.w, 
			_ => 0f, 
		};
	}

	public static uint CompressQuaternion(Quaternion q)
	{
		float largestAbs;
		Vector3 withoutLargest;
		int num = LargestAbsoluteComponentIndex(new Vector4(q.x, q.y, q.z, q.w), out largestAbs, out withoutLargest);
		if (QuaternionElement(q, num) < 0f)
		{
			withoutLargest = -withoutLargest;
		}
		ushort num2 = ScaleFloatToUShort(withoutLargest.x, -0.707107f, 0.707107f, 0, 1023);
		ushort num3 = ScaleFloatToUShort(withoutLargest.y, -0.707107f, 0.707107f, 0, 1023);
		ushort num4 = ScaleFloatToUShort(withoutLargest.z, -0.707107f, 0.707107f, 0, 1023);
		return (uint)((num << 30) | (num2 << 20) | (num3 << 10) | num4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Quaternion QuaternionNormalizeSafe(Quaternion value)
	{
		Vector4 vector = new Vector4(value.x, value.y, value.z, value.w);
		if (!(Vector4.Dot(vector, vector) > 1.1754944E-38f))
		{
			return Quaternion.identity;
		}
		return value.normalized;
	}

	public static Quaternion DecompressQuaternion(uint data)
	{
		ushort value = (ushort)(data & 0x3FF);
		ushort value2 = (ushort)((data >> 10) & 0x3FF);
		ushort value3 = (ushort)((data >> 20) & 0x3FF);
		int num = (int)(data >> 30);
		float num2 = ScaleUShortToFloat(value3, 0, 1023, -0.707107f, 0.707107f);
		float num3 = ScaleUShortToFloat(value2, 0, 1023, -0.707107f, 0.707107f);
		float num4 = ScaleUShortToFloat(value, 0, 1023, -0.707107f, 0.707107f);
		float num5 = Mathf.Sqrt(1f - num2 * num2 - num3 * num3 - num4 * num4);
		Vector4 vector = num switch
		{
			0 => new Vector4(num5, num2, num3, num4), 
			1 => new Vector4(num2, num5, num3, num4), 
			2 => new Vector4(num2, num3, num5, num4), 
			_ => new Vector4(num2, num3, num4, num5), 
		};
		return QuaternionNormalizeSafe(new Quaternion(vector.x, vector.y, vector.z, vector.w));
	}

	public static void CompressVarUInt(NetworkWriter writer, ulong value)
	{
		if (value <= 240)
		{
			writer.WriteByte((byte)value);
		}
		else if (value <= 2287)
		{
			writer.WriteByte((byte)((value - 240 >> 8) + 241));
			writer.WriteByte((byte)((value - 240) & 0xFF));
		}
		else if (value <= 67823)
		{
			writer.WriteByte(249);
			writer.WriteByte((byte)(value - 2288 >> 8));
			writer.WriteByte((byte)((value - 2288) & 0xFF));
		}
		else if (value <= 16777215)
		{
			writer.WriteByte(250);
			writer.WriteByte((byte)(value & 0xFF));
			writer.WriteByte((byte)((value >> 8) & 0xFF));
			writer.WriteByte((byte)((value >> 16) & 0xFF));
		}
		else if (value <= uint.MaxValue)
		{
			writer.WriteByte(251);
			writer.WriteByte((byte)(value & 0xFF));
			writer.WriteByte((byte)((value >> 8) & 0xFF));
			writer.WriteByte((byte)((value >> 16) & 0xFF));
			writer.WriteByte((byte)((value >> 24) & 0xFF));
		}
		else if (value <= 1099511627775L)
		{
			writer.WriteByte(252);
			writer.WriteByte((byte)(value & 0xFF));
			writer.WriteByte((byte)((value >> 8) & 0xFF));
			writer.WriteByte((byte)((value >> 16) & 0xFF));
			writer.WriteByte((byte)((value >> 24) & 0xFF));
			writer.WriteByte((byte)((value >> 32) & 0xFF));
		}
		else if (value <= 281474976710655L)
		{
			writer.WriteByte(253);
			writer.WriteByte((byte)(value & 0xFF));
			writer.WriteByte((byte)((value >> 8) & 0xFF));
			writer.WriteByte((byte)((value >> 16) & 0xFF));
			writer.WriteByte((byte)((value >> 24) & 0xFF));
			writer.WriteByte((byte)((value >> 32) & 0xFF));
			writer.WriteByte((byte)((value >> 40) & 0xFF));
		}
		else if (value <= 72057594037927935L)
		{
			writer.WriteByte(254);
			writer.WriteByte((byte)(value & 0xFF));
			writer.WriteByte((byte)((value >> 8) & 0xFF));
			writer.WriteByte((byte)((value >> 16) & 0xFF));
			writer.WriteByte((byte)((value >> 24) & 0xFF));
			writer.WriteByte((byte)((value >> 32) & 0xFF));
			writer.WriteByte((byte)((value >> 40) & 0xFF));
			writer.WriteByte((byte)((value >> 48) & 0xFF));
		}
		else
		{
			writer.WriteByte(byte.MaxValue);
			writer.WriteByte((byte)(value & 0xFF));
			writer.WriteByte((byte)((value >> 8) & 0xFF));
			writer.WriteByte((byte)((value >> 16) & 0xFF));
			writer.WriteByte((byte)((value >> 24) & 0xFF));
			writer.WriteByte((byte)((value >> 32) & 0xFF));
			writer.WriteByte((byte)((value >> 40) & 0xFF));
			writer.WriteByte((byte)((value >> 48) & 0xFF));
			writer.WriteByte((byte)((value >> 56) & 0xFF));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CompressVarInt(NetworkWriter writer, long i)
	{
		ulong value = (ulong)((i >> 63) ^ (i << 1));
		CompressVarUInt(writer, value);
	}

	public static ulong DecompressVarUInt(NetworkReader reader)
	{
		byte b = reader.ReadByte();
		if (b < 241)
		{
			return b;
		}
		byte b2 = reader.ReadByte();
		if (b <= 248)
		{
			return (ulong)(240 + ((long)b - 241L << 8) + b2);
		}
		byte b3 = reader.ReadByte();
		if (b == 249)
		{
			return 2288 + ((ulong)b2 << 8) + b3;
		}
		byte b4 = reader.ReadByte();
		if (b == 250)
		{
			return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16);
		}
		byte b5 = reader.ReadByte();
		if (b == 251)
		{
			return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24);
		}
		byte b6 = reader.ReadByte();
		if (b == 252)
		{
			return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32);
		}
		byte b7 = reader.ReadByte();
		if (b == 253)
		{
			return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32) + ((ulong)b7 << 40);
		}
		byte b8 = reader.ReadByte();
		if (b == 254)
		{
			return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32) + ((ulong)b7 << 40) + ((ulong)b8 << 48);
		}
		byte b9 = reader.ReadByte();
		if (b == byte.MaxValue)
		{
			return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32) + ((ulong)b7 << 40) + ((ulong)b8 << 48) + ((ulong)b9 << 56);
		}
		throw new IndexOutOfRangeException($"DecompressVarInt failure: {b}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long DecompressVarInt(NetworkReader reader)
	{
		ulong num = DecompressVarUInt(reader);
		return (long)((num >> 1) ^ (0L - (num & 1)));
	}
}
