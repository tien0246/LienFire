using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Mirror.SimpleWeb;

public static class MessageProcessor
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte FirstLengthByte(byte[] buffer)
	{
		return (byte)(buffer[1] & 0x7F);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool NeedToReadShortLength(byte[] buffer)
	{
		return FirstLengthByte(buffer) == 126;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool NeedToReadLongLength(byte[] buffer)
	{
		return FirstLengthByte(buffer) == 127;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetOpcode(byte[] buffer)
	{
		return buffer[0] & 0xF;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetPayloadLength(byte[] buffer)
	{
		return GetMessageLength(buffer, 0, FirstLengthByte(buffer));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Finished(byte[] buffer)
	{
		return (buffer[0] & 0x80) != 0;
	}

	public static void ValidateHeader(byte[] buffer, int maxLength, bool expectMask, bool opCodeContinuation = false)
	{
		bool finished = Finished(buffer);
		bool hasMask = (buffer[1] & 0x80) != 0;
		int opcode = buffer[0] & 0xF;
		byte lenByte = FirstLengthByte(buffer);
		ThrowIfMaskNotExpected(hasMask, expectMask);
		ThrowIfBadOpCode(opcode, finished, opCodeContinuation);
		int messageLength = GetMessageLength(buffer, 0, lenByte);
		ThrowIfLengthZero(messageLength);
		ThrowIfMsgLengthTooLong(messageLength, maxLength);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToggleMask(byte[] src, int sourceOffset, int messageLength, byte[] maskBuffer, int maskOffset)
	{
		ToggleMask(src, sourceOffset, src, sourceOffset, messageLength, maskBuffer, maskOffset);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToggleMask(byte[] src, int sourceOffset, ArrayBuffer dst, int messageLength, byte[] maskBuffer, int maskOffset)
	{
		ToggleMask(src, sourceOffset, dst.array, 0, messageLength, maskBuffer, maskOffset);
		dst.count = messageLength;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToggleMask(byte[] src, int srcOffset, byte[] dst, int dstOffset, int messageLength, byte[] maskBuffer, int maskOffset)
	{
		for (int i = 0; i < messageLength; i++)
		{
			byte b = maskBuffer[maskOffset + i % 4];
			dst[dstOffset + i] = (byte)(src[srcOffset + i] ^ b);
		}
	}

	private static int GetMessageLength(byte[] buffer, int offset, byte lenByte)
	{
		switch (lenByte)
		{
		case 126:
			return (ushort)((ushort)(0 | (ushort)(buffer[offset + 2] << 8)) | buffer[offset + 3]);
		case 127:
		{
			ulong num = 0 | ((ulong)buffer[offset + 2] << 56) | ((ulong)buffer[offset + 3] << 48) | ((ulong)buffer[offset + 4] << 40) | ((ulong)buffer[offset + 5] << 32) | ((ulong)buffer[offset + 6] << 24) | ((ulong)buffer[offset + 7] << 16) | ((ulong)buffer[offset + 8] << 8) | buffer[offset + 9];
			if (num > int.MaxValue)
			{
				throw new NotSupportedException($"Can't receive payloads larger that int.max: {int.MaxValue}");
			}
			return (int)num;
		}
		default:
			return lenByte;
		}
	}

	private static void ThrowIfMaskNotExpected(bool hasMask, bool expectMask)
	{
		if (hasMask != expectMask)
		{
			throw new InvalidDataException($"Message expected mask to be {expectMask} but was {hasMask}");
		}
	}

	private static void ThrowIfBadOpCode(int opcode, bool finished, bool opCodeContinuation)
	{
		if (opCodeContinuation)
		{
			if (opcode != 0)
			{
				throw new InvalidDataException("Expected opcode to be Continuation");
			}
		}
		else if (!finished)
		{
			if (opcode != 2)
			{
				throw new InvalidDataException("Expected opcode to be binary");
			}
		}
		else if (opcode != 2 && opcode != 8)
		{
			throw new InvalidDataException("Expected opcode to be binary or close");
		}
	}

	private static void ThrowIfLengthZero(int msglen)
	{
		if (msglen == 0)
		{
			throw new InvalidDataException("Message length was zero");
		}
	}

	public static void ThrowIfMsgLengthTooLong(int msglen, int maxLength)
	{
		if (msglen > maxLength)
		{
			throw new InvalidDataException("Message length is greater than max length");
		}
	}
}
