using System.IO;
using System.Text;
using UnityEngine.Bindings;

namespace UnityEngine;

[VisibleToOtherModules(new string[] { "UnityEngine.UnityWebRequestWWWModule" })]
internal class WWWTranscoder
{
	private static byte[] ucHexChars = WWWForm.DefaultEncoding.GetBytes("0123456789ABCDEF");

	private static byte[] lcHexChars = WWWForm.DefaultEncoding.GetBytes("0123456789abcdef");

	private static byte urlEscapeChar = 37;

	private static byte[] urlSpace = new byte[1] { 43 };

	private static byte[] dataSpace = WWWForm.DefaultEncoding.GetBytes("%20");

	private static byte[] urlForbidden = WWWForm.DefaultEncoding.GetBytes("@&;:<>=?\"'/\\!#%+$,{}|^[]`");

	private static byte qpEscapeChar = 61;

	private static byte[] qpSpace = new byte[1] { 95 };

	private static byte[] qpForbidden = WWWForm.DefaultEncoding.GetBytes("&;=?\"'%+_");

	private static byte Hex2Byte(byte[] b, int offset)
	{
		byte b2 = 0;
		for (int i = offset; i < offset + 2; i++)
		{
			b2 *= 16;
			int num = b[i];
			if (num >= 48 && num <= 57)
			{
				num -= 48;
			}
			else if (num >= 65 && num <= 75)
			{
				num -= 55;
			}
			else if (num >= 97 && num <= 102)
			{
				num -= 87;
			}
			if (num > 15)
			{
				return 63;
			}
			b2 += (byte)num;
		}
		return b2;
	}

	private static void Byte2Hex(byte b, byte[] hexChars, out byte byte0, out byte byte1)
	{
		byte0 = hexChars[b >> 4];
		byte1 = hexChars[b & 0xF];
	}

	public static string URLEncode(string toEncode)
	{
		return URLEncode(toEncode, Encoding.UTF8);
	}

	public static string URLEncode(string toEncode, Encoding e)
	{
		byte[] array = Encode(e.GetBytes(toEncode), urlEscapeChar, urlSpace, urlForbidden, uppercase: false);
		return WWWForm.DefaultEncoding.GetString(array, 0, array.Length);
	}

	public static byte[] URLEncode(byte[] toEncode)
	{
		return Encode(toEncode, urlEscapeChar, urlSpace, urlForbidden, uppercase: false);
	}

	public static string DataEncode(string toEncode)
	{
		return DataEncode(toEncode, Encoding.UTF8);
	}

	public static string DataEncode(string toEncode, Encoding e)
	{
		byte[] array = Encode(e.GetBytes(toEncode), urlEscapeChar, dataSpace, urlForbidden, uppercase: false);
		return WWWForm.DefaultEncoding.GetString(array, 0, array.Length);
	}

	public static byte[] DataEncode(byte[] toEncode)
	{
		return Encode(toEncode, urlEscapeChar, dataSpace, urlForbidden, uppercase: false);
	}

	public static string QPEncode(string toEncode)
	{
		return QPEncode(toEncode, Encoding.UTF8);
	}

	public static string QPEncode(string toEncode, Encoding e)
	{
		byte[] array = Encode(e.GetBytes(toEncode), qpEscapeChar, qpSpace, qpForbidden, uppercase: true);
		return WWWForm.DefaultEncoding.GetString(array, 0, array.Length);
	}

	public static byte[] QPEncode(byte[] toEncode)
	{
		return Encode(toEncode, qpEscapeChar, qpSpace, qpForbidden, uppercase: true);
	}

	public static byte[] Encode(byte[] input, byte escapeChar, byte[] space, byte[] forbidden, bool uppercase)
	{
		using MemoryStream memoryStream = new MemoryStream(input.Length * 2);
		for (int i = 0; i < input.Length; i++)
		{
			if (input[i] == 32)
			{
				memoryStream.Write(space, 0, space.Length);
			}
			else if (input[i] < 32 || input[i] > 126 || ByteArrayContains(forbidden, input[i]))
			{
				memoryStream.WriteByte(escapeChar);
				Byte2Hex(input[i], uppercase ? ucHexChars : lcHexChars, out var @byte, out var byte2);
				memoryStream.WriteByte(@byte);
				memoryStream.WriteByte(byte2);
			}
			else
			{
				memoryStream.WriteByte(input[i]);
			}
		}
		return memoryStream.ToArray();
	}

	private static bool ByteArrayContains(byte[] array, byte b)
	{
		int num = array.Length;
		for (int i = 0; i < num; i++)
		{
			if (array[i] == b)
			{
				return true;
			}
		}
		return false;
	}

	public static string URLDecode(string toEncode)
	{
		return URLDecode(toEncode, Encoding.UTF8);
	}

	public static string URLDecode(string toEncode, Encoding e)
	{
		byte[] array = Decode(WWWForm.DefaultEncoding.GetBytes(toEncode), urlEscapeChar, urlSpace);
		return e.GetString(array, 0, array.Length);
	}

	public static byte[] URLDecode(byte[] toEncode)
	{
		return Decode(toEncode, urlEscapeChar, urlSpace);
	}

	public static string DataDecode(string toDecode)
	{
		return DataDecode(toDecode, Encoding.UTF8);
	}

	public static string DataDecode(string toDecode, Encoding e)
	{
		byte[] array = Decode(WWWForm.DefaultEncoding.GetBytes(toDecode), urlEscapeChar, dataSpace);
		return e.GetString(array, 0, array.Length);
	}

	public static byte[] DataDecode(byte[] toDecode)
	{
		return Decode(toDecode, urlEscapeChar, dataSpace);
	}

	public static string QPDecode(string toEncode)
	{
		return QPDecode(toEncode, Encoding.UTF8);
	}

	public static string QPDecode(string toEncode, Encoding e)
	{
		byte[] array = Decode(WWWForm.DefaultEncoding.GetBytes(toEncode), qpEscapeChar, qpSpace);
		return e.GetString(array, 0, array.Length);
	}

	public static byte[] QPDecode(byte[] toEncode)
	{
		return Decode(toEncode, qpEscapeChar, qpSpace);
	}

	private static bool ByteSubArrayEquals(byte[] array, int index, byte[] comperand)
	{
		if (array.Length - index < comperand.Length)
		{
			return false;
		}
		for (int i = 0; i < comperand.Length; i++)
		{
			if (array[index + i] != comperand[i])
			{
				return false;
			}
		}
		return true;
	}

	public static byte[] Decode(byte[] input, byte escapeChar, byte[] space)
	{
		using MemoryStream memoryStream = new MemoryStream(input.Length);
		for (int i = 0; i < input.Length; i++)
		{
			if (ByteSubArrayEquals(input, i, space))
			{
				i += space.Length - 1;
				memoryStream.WriteByte(32);
			}
			else if (input[i] == escapeChar && i + 2 < input.Length)
			{
				i++;
				memoryStream.WriteByte(Hex2Byte(input, i++));
			}
			else
			{
				memoryStream.WriteByte(input[i]);
			}
		}
		return memoryStream.ToArray();
	}

	public static bool SevenBitClean(string s)
	{
		return SevenBitClean(s, Encoding.UTF8);
	}

	public unsafe static bool SevenBitClean(string s, Encoding e)
	{
		int num = s.Length * 2;
		byte* ptr = stackalloc byte[(int)(uint)num];
		int bytes;
		fixed (char* chars = s)
		{
			bytes = e.GetBytes(chars, s.Length, ptr, num);
		}
		return SevenBitClean(ptr, bytes);
	}

	public unsafe static bool SevenBitClean(byte* input, int inputLength)
	{
		for (int i = 0; i < inputLength; i++)
		{
			if (input[i] < 32 || input[i] > 126)
			{
				return false;
			}
		}
		return true;
	}
}
