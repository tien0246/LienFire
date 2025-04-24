using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Mirror;

public class NetworkReader
{
	internal ArraySegment<byte> buffer;

	public int Position;

	internal readonly UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

	public int Remaining => buffer.Count - Position;

	public int Capacity => buffer.Count;

	public NetworkReader(ArraySegment<byte> segment)
	{
		buffer = segment;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetBuffer(ArraySegment<byte> segment)
	{
		buffer = segment;
		Position = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe T ReadBlittable<T>() where T : unmanaged
	{
		int num = sizeof(T);
		if (Remaining < num)
		{
			throw new EndOfStreamException($"ReadBlittable<{typeof(T)}> not enough data in buffer to read {num} bytes: {ToString()}");
		}
		T result;
		fixed (byte* source = &buffer.Array[buffer.Offset + Position])
		{
			T* intPtr = stackalloc T[1];
			UnsafeUtility.MemCpy(intPtr, source, num);
			result = *intPtr;
		}
		Position += num;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal T? ReadBlittableNullable<T>() where T : unmanaged
	{
		if (ReadByte() == 0)
		{
			return null;
		}
		return ReadBlittable<T>();
	}

	public byte ReadByte()
	{
		return ReadBlittable<byte>();
	}

	public byte[] ReadBytes(byte[] bytes, int count)
	{
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("ReadBytes requires count >= 0");
		}
		if (count > bytes.Length)
		{
			throw new EndOfStreamException($"ReadBytes can't read {count} + bytes because the passed byte[] only has length {bytes.Length}");
		}
		if (Remaining < count)
		{
			throw new EndOfStreamException($"ReadBytesSegment can't read {count} bytes because it would read past the end of the stream. {ToString()}");
		}
		Array.Copy(buffer.Array, buffer.Offset + Position, bytes, 0, count);
		Position += count;
		return bytes;
	}

	public ArraySegment<byte> ReadBytesSegment(int count)
	{
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("ReadBytesSegment requires count >= 0");
		}
		if (Remaining < count)
		{
			throw new EndOfStreamException($"ReadBytesSegment can't read {count} bytes because it would read past the end of the stream. {ToString()}");
		}
		ArraySegment<byte> result = new ArraySegment<byte>(buffer.Array, buffer.Offset + Position, count);
		Position += count;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T Read<T>()
	{
		Func<NetworkReader, T> read = Reader<T>.read;
		if (read == null)
		{
			Debug.LogError($"No reader found for {typeof(T)}. Use a type supported by Mirror or define a custom reader extension for {typeof(T)}.");
			return default(T);
		}
		return read(this);
	}

	public override string ToString()
	{
		return $"[{buffer.ToHexString()} @ {Position}/{Capacity}]";
	}
}
