using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LunarConsolePluginInternal;

public static class StringUtils
{
	private static readonly char[] kSpaceSplitChars = new char[1] { ' ' };

	private static List<string> s_tempList;

	private static readonly string Quote = "\"";

	private static readonly string SingleQuote = "'";

	private static readonly string EscapedQuote = "\\\"";

	private static readonly string EscapedSingleQuote = "\\'";

	internal static string TryFormat(string format, params object[] args)
	{
		if (format != null && args != null && args.Length != 0)
		{
			try
			{
				return string.Format(format, args);
			}
			catch (Exception ex)
			{
				Debug.LogError("Error while formatting string: " + ex.Message);
			}
		}
		return format;
	}

	public static bool StartsWithIgnoreCase(string str, string prefix)
	{
		if (str != null && prefix != null)
		{
			return str.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public static bool EqualsIgnoreCase(string a, string b)
	{
		return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
	}

	public static IList<string> Filter(IList<string> strings, string prefix)
	{
		if (string.IsNullOrEmpty(prefix))
		{
			return strings;
		}
		IList<string> list = new List<string>();
		foreach (string @string in strings)
		{
			if (StartsWithIgnoreCase(@string, prefix))
			{
				list.Add(@string);
			}
		}
		return list;
	}

	public static int ParseInt(string str)
	{
		return ParseInt(str, 0);
	}

	public static int ParseInt(string str, int defValue)
	{
		if (!string.IsNullOrEmpty(str))
		{
			if (!int.TryParse(str, out var result))
			{
				return defValue;
			}
			return result;
		}
		return defValue;
	}

	public static int ParseInt(string str, out bool succeed)
	{
		if (!string.IsNullOrEmpty(str))
		{
			succeed = int.TryParse(str, out var result);
			if (!succeed)
			{
				return 0;
			}
			return result;
		}
		succeed = false;
		return 0;
	}

	public static float ParseFloat(string str)
	{
		return ParseFloat(str, 0f);
	}

	public static float ParseFloat(string str, float defValue)
	{
		if (!string.IsNullOrEmpty(str))
		{
			if (!float.TryParse(str, out var result))
			{
				return defValue;
			}
			return result;
		}
		return defValue;
	}

	public static float ParseFloat(string str, out bool succeed)
	{
		if (!string.IsNullOrEmpty(str))
		{
			succeed = float.TryParse(str, out var result);
			if (!succeed)
			{
				return 0f;
			}
			return result;
		}
		succeed = false;
		return 0f;
	}

	public static bool ParseBool(string str)
	{
		return ParseBool(str, defValue: false);
	}

	public static bool ParseBool(string str, bool defValue)
	{
		if (!string.IsNullOrEmpty(str))
		{
			if (!bool.TryParse(str, out var result))
			{
				return defValue;
			}
			return result;
		}
		return defValue;
	}

	public static bool ParseBool(string str, out bool succeed)
	{
		if (!string.IsNullOrEmpty(str))
		{
			succeed = bool.TryParse(str, out var result);
			if (!succeed)
			{
				return false;
			}
			return result;
		}
		succeed = false;
		return false;
	}

	public static float[] ParseFloats(string str)
	{
		if (str == null)
		{
			return null;
		}
		return ParseFloats(str.Split(kSpaceSplitChars, StringSplitOptions.RemoveEmptyEntries));
	}

	public static float[] ParseFloats(string[] args)
	{
		if (args != null)
		{
			float[] array = new float[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				if (!float.TryParse(args[i], out array[i]))
				{
					return null;
				}
			}
			return array;
		}
		return null;
	}

	public static bool IsNumeric(string str)
	{
		double result;
		return double.TryParse(str, out result);
	}

	public static bool IsInteger(string str)
	{
		int result;
		return int.TryParse(str, out result);
	}

	internal static int StartOfTheWordOffset(string value, int index)
	{
		return StartOfTheWord(value, index) - index;
	}

	internal static int StartOfTheWord(string value, int index)
	{
		int num = index - 1;
		while (num >= 0 && IsSeparator(value[num]))
		{
			num--;
		}
		while (num >= 0 && !IsSeparator(value[num]))
		{
			num--;
		}
		return num + 1;
	}

	internal static int EndOfTheWordOffset(string value, int index)
	{
		return EndOfTheWord(value, index) - index;
	}

	internal static int EndOfTheWord(string value, int index)
	{
		int i;
		for (i = index; i < value.Length && IsSeparator(value[i]); i++)
		{
		}
		for (; i < value.Length && !IsSeparator(value[i]); i++)
		{
		}
		return i;
	}

	private static bool IsSeparator(char ch)
	{
		if (!char.IsLetter(ch))
		{
			return !char.IsDigit(ch);
		}
		return false;
	}

	internal static int MoveLineUp(string value, int index)
	{
		if (index > 0 && index <= value.Length)
		{
			int num = StartOfPrevLineIndex(value, index);
			if (num == -1)
			{
				return index;
			}
			int num2 = OffsetInLine(value, index);
			int b = EndOfPrevLineIndex(value, index);
			return Mathf.Min(num + num2, b);
		}
		return index;
	}

	internal static int MoveLineDown(string value, int index)
	{
		if (index >= 0 && index < value.Length)
		{
			int num = StartOfNextLineIndex(value, index);
			if (num == -1)
			{
				return index;
			}
			int num2 = OffsetInLine(value, index);
			int b = EndOfNextLineIndex(value, index);
			return Mathf.Min(num + num2, b);
		}
		return index;
	}

	internal static int StartOfLineOffset(string value, int index)
	{
		return StartOfLineIndex(value, index) - index;
	}

	internal static int StartOfLineIndex(string value, int index)
	{
		if (index <= 0)
		{
			return 0;
		}
		return value.LastIndexOf('\n', index - 1) + 1;
	}

	internal static int EndOfLineOffset(string value, int index)
	{
		return EndOfLineIndex(value, index) - index;
	}

	internal static int EndOfLineIndex(string value, int index)
	{
		if (index < value.Length)
		{
			int num = value.IndexOf('\n', index);
			if (num != -1)
			{
				return num;
			}
		}
		return value.Length;
	}

	internal static int OffsetInLine(string value, int index)
	{
		return index - StartOfLineIndex(value, index);
	}

	internal static int StartOfPrevLineIndex(string value, int index)
	{
		int num = EndOfPrevLineIndex(value, index);
		if (num == -1)
		{
			return -1;
		}
		return StartOfLineIndex(value, num);
	}

	internal static int EndOfPrevLineIndex(string value, int index)
	{
		return StartOfLineIndex(value, index) - 1;
	}

	internal static int StartOfNextLineIndex(string value, int index)
	{
		int num = EndOfLineIndex(value, index);
		if (num >= value.Length)
		{
			return -1;
		}
		return num + 1;
	}

	internal static int EndOfNextLineIndex(string value, int index)
	{
		int num = StartOfNextLineIndex(value, index);
		if (num == -1)
		{
			return -1;
		}
		return EndOfLineIndex(value, num);
	}

	internal static int LinesBreaksCount(string value)
	{
		if (value != null)
		{
			int num = 0;
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == '\n')
				{
					num++;
				}
			}
			return num;
		}
		return 0;
	}

	internal static int Strlen(string str)
	{
		return str?.Length ?? 0;
	}

	internal static string GetSuggestedText(string token, string[] strings, bool removeTags = false)
	{
		return GetSuggestedText0(token, strings, removeTags);
	}

	internal static string GetSuggestedText(string token, IList<string> strings, bool removeTags = false)
	{
		return GetSuggestedText0(token, (IList)strings, removeTags);
	}

	private static string GetSuggestedText0(string token, IList strings, bool removeTags)
	{
		if (token == null)
		{
			return null;
		}
		if (s_tempList == null)
		{
			s_tempList = new List<string>();
		}
		else
		{
			s_tempList.Clear();
		}
		foreach (string @string in strings)
		{
			if (token.Length == 0 || StartsWithIgnoreCase(@string, token))
			{
				s_tempList.Add(@string);
			}
		}
		return GetSuggestedTextFiltered0(token, s_tempList);
	}

	internal static string GetSuggestedTextFiltered(string token, IList<string> strings)
	{
		return GetSuggestedTextFiltered0(token, (IList)strings);
	}

	internal static string GetSuggestedTextFiltered(string token, string[] strings)
	{
		return GetSuggestedTextFiltered0(token, strings);
	}

	private static string GetSuggestedTextFiltered0(string token, IList strings)
	{
		if (token == null)
		{
			return null;
		}
		if (strings.Count == 0)
		{
			return null;
		}
		if (strings.Count == 1)
		{
			return (string)strings[0];
		}
		string text = (string)strings[0];
		if (token.Length == 0)
		{
			token = text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			char c2 = char.ToLower(c);
			bool flag = false;
			for (int j = 1; j < strings.Count; j++)
			{
				string text2 = (string)strings[j];
				if (i >= text2.Length || char.ToLower(text2[i]) != c2)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				if (stringBuilder.Length <= 0)
				{
					return null;
				}
				return stringBuilder.ToString();
			}
			stringBuilder.Append(c);
		}
		if (stringBuilder.Length <= 0)
		{
			return null;
		}
		return stringBuilder.ToString();
	}

	internal static string Arg(string value)
	{
		if (value != null && value.Length > 0)
		{
			value = value.Replace(Quote, EscapedQuote);
			value = value.Replace(SingleQuote, EscapedSingleQuote);
			if (value.IndexOf(' ') != -1)
			{
				value = TryFormat("\"{0}\"", value);
			}
			return value;
		}
		return "\"\"";
	}

	internal static string UnArg(string value)
	{
		if (value != null && value.Length > 0)
		{
			if ((value.StartsWith(Quote) && value.EndsWith(Quote)) || (value.StartsWith(SingleQuote) && value.EndsWith(SingleQuote)))
			{
				value = value.Substring(1, value.Length - 2);
			}
			value = value.Replace(EscapedQuote, Quote);
			value = value.Replace(EscapedSingleQuote, SingleQuote);
			return value;
		}
		return "";
	}

	internal static string NonNullOrEmpty(string str)
	{
		if (str == null)
		{
			return "";
		}
		return str;
	}

	internal static string ToString(object value)
	{
		return value?.ToString();
	}

	internal static string ToString(int value)
	{
		return value.ToString();
	}

	internal static string ToString(float value)
	{
		return value.ToString("G");
	}

	internal static string ToString(bool value)
	{
		return value.ToString();
	}

	internal static string ToString(ref Color value)
	{
		if (value.a > 0f)
		{
			return string.Format("{0} {1} {2} {3}", value.r.ToString("G"), value.g.ToString("G"), value.b.ToString("G"), value.a.ToString("G"));
		}
		return string.Format("{0} {1} {2}", value.r.ToString("G"), value.g.ToString("G"), value.b.ToString("G"));
	}

	internal static string ToString(ref Rect value)
	{
		return string.Format("{0} {1} {2} {3}", value.x.ToString("G"), value.y.ToString("G"), value.width.ToString("G"), value.height.ToString("G"));
	}

	internal static string ToString(ref Vector2 value)
	{
		return string.Format("{0} {1}", value.x.ToString("G"), value.y.ToString("G"));
	}

	internal static string ToString(ref Vector3 value)
	{
		return string.Format("{0} {1} {2}", value.x.ToString("G"), value.y.ToString("G"), value.z.ToString("G"));
	}

	internal static string ToString(ref Vector4 value)
	{
		return string.Format("{0} {1} {2} {3}", value.x.ToString("G"), value.y.ToString("G"), value.z.ToString("G"), value.w.ToString("G"));
	}

	public static string Join<T>(IList<T> list, string separator = ",")
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < list.Count; i++)
		{
			stringBuilder.Append(list[i]);
			if (i < list.Count - 1)
			{
				stringBuilder.Append(separator);
			}
		}
		return stringBuilder.ToString();
	}

	public static string ToDisplayName(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		StringBuilder stringBuilder = new StringBuilder();
		char c = '\0';
		for (int i = 0; i < value.Length; i++)
		{
			char c2 = value[i];
			if (i == 0)
			{
				c2 = char.ToUpper(c2);
			}
			else if ((char.IsUpper(c2) || (char.IsDigit(c2) && !char.IsDigit(c))) && stringBuilder.Length > 0)
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append(c2);
			c = c2;
		}
		return stringBuilder.ToString();
	}

	public static IDictionary<string, string> DeserializeString(string data)
	{
		string[] array = data.Split('\n');
		IDictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num = text.IndexOf(':');
			string key = text.Substring(0, num);
			string value = text.Substring(num + 1, text.Length - (num + 1)).Replace("\\n", "\n");
			dictionary[key] = value;
		}
		return dictionary;
	}
}
