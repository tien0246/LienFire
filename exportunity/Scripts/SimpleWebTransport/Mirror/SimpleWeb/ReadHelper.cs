using System;
using System.IO;

namespace Mirror.SimpleWeb;

public static class ReadHelper
{
	public static int Read(Stream stream, byte[] outBuffer, int outOffset, int length)
	{
		int i = 0;
		try
		{
			int num;
			for (; i < length; i += num)
			{
				num = stream.Read(outBuffer, outOffset + i, length - i);
				if (num == 0)
				{
					throw new ReadHelperException("returned 0");
				}
			}
		}
		catch (AggregateException ex)
		{
			Utils.CheckForInterupt();
			ex.Handle((Exception e) => false);
		}
		if (i != length)
		{
			throw new ReadHelperException("returned not equal to length");
		}
		return outOffset + i;
	}

	public static bool TryRead(Stream stream, byte[] outBuffer, int outOffset, int length)
	{
		try
		{
			Read(stream, outBuffer, outOffset, length);
			return true;
		}
		catch (ReadHelperException)
		{
			return false;
		}
		catch (IOException)
		{
			return false;
		}
		catch (Exception e)
		{
			Log.Exception(e);
			return false;
		}
	}

	public static int? SafeReadTillMatch(Stream stream, byte[] outBuffer, int outOffset, int maxLength, byte[] endOfHeader)
	{
		try
		{
			int num = 0;
			int num2 = 0;
			int num3 = endOfHeader.Length;
			while (true)
			{
				int num4 = stream.ReadByte();
				if (num4 == -1)
				{
					return null;
				}
				if (num >= maxLength)
				{
					return null;
				}
				outBuffer[outOffset + num] = (byte)num4;
				num++;
				if (endOfHeader[num2] == num4)
				{
					num2++;
					if (num2 >= num3)
					{
						break;
					}
				}
				else
				{
					num2 = 0;
				}
			}
			return num;
		}
		catch (IOException)
		{
			return null;
		}
		catch (Exception e)
		{
			Log.Exception(e);
			return null;
		}
	}
}
