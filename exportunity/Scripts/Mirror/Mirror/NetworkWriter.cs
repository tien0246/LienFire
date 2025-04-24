using System;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Mirror;

public class NetworkWriter
{
	public const ushort MaxStringLength = 65534;

	public const int DefaultCapacity = 1500;

	internal byte[] buffer = new byte[1500];

	public int Position;

	internal readonly UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

	public int Capacity => buffer.Length;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Reset()
	{
		Position = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void EnsureCapacity(int value)
	{
		if (buffer.Length < value)
		{
			int newSize = Math.Max(value, buffer.Length * 2);
			Array.Resize(ref buffer, newSize);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[] ToArray()
	{
		byte[] array = new byte[Position];
		Array.ConstrainedCopy(buffer, 0, array, 0, Position);
		return array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ArraySegment<byte> ToArraySegment()
	{
		return new ArraySegment<byte>(buffer, 0, Position);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ArraySegment<byte>(NetworkWriter w)
	{
		return w.ToArraySegment();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe void WriteBlittable<T>(T value) where T : unmanaged
	{
		int num = sizeof(T);
		EnsureCapacity(Position + num);
		fixed (byte* destination = &buffer[Position])
		{
			T* source = stackalloc T[1] { value };
			UnsafeUtility.MemCpy(destination, source, num);
		}
		Position += num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void WriteBlittableNullable<T>(T? value) where T : unmanaged
	{
		WriteByte((byte)(value.HasValue ? 1u : 0u));
		if (value.HasValue)
		{
			WriteBlittable(value.Value);
		}
	}

	public void WriteByte(byte value)
	{
		WriteBlittable(value);
	}

	public void WriteBytes(byte[] array, int offset, int count)
	{
		EnsureCapacity(Position + count);
		Array.ConstrainedCopy(array, offset, buffer, Position, count);
		Position += count;
	}

	public unsafe bool WriteBytes(byte* ptr, int offset, int size)
	{
		EnsureCapacity(Position + size);
		fixed (byte* destination = &buffer[Position])
		{
			UnsafeUtility.MemCpy(destination, ptr + offset, size);
		}
		Position += size;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Write<T>(T value)
	{
		Action<NetworkWriter, T> write = Writer<T>.write;
		if (write == null)
		{
			Debug.LogError($"No writer found for {typeof(T)}. This happens either if you are missing a NetworkWriter extension for your custom type, or if weaving failed. Try to reimport a script to weave again.");
		}
		else
		{
			write(this, value);
		}
	}

	public override string ToString()
	{
		return $"[{ToArraySegment().ToHexString()} @ {Position}/{Capacity}]";
	}
}
