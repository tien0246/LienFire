using System.Globalization;
using System.Runtime.Versioning;
using System.Security;

namespace System;

[Serializable]
public readonly struct Int64 : IComparable, IConvertible, IFormattable, IComparable<long>, IEquatable<long>, ISpanFormattable
{
	private readonly long m_value;

	public const long MaxValue = 9223372036854775807L;

	public const long MinValue = -9223372036854775808L;

	public int CompareTo(object value)
	{
		if (value == null)
		{
			return 1;
		}
		if (value is long num)
		{
			if (this < num)
			{
				return -1;
			}
			if (this > num)
			{
				return 1;
			}
			return 0;
		}
		throw new ArgumentException("Object must be of type Int64.");
	}

	public int CompareTo(long value)
	{
		if (this < value)
		{
			return -1;
		}
		if (this > value)
		{
			return 1;
		}
		return 0;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is long))
		{
			return false;
		}
		return this == (long)obj;
	}

	[NonVersionable]
	public bool Equals(long obj)
	{
		return this == obj;
	}

	public override int GetHashCode()
	{
		return (int)this ^ (int)(this >> 32);
	}

	public override string ToString()
	{
		return Number.FormatInt64(this, null, null);
	}

	[SecuritySafeCritical]
	public string ToString(IFormatProvider provider)
	{
		return Number.FormatInt64(this, null, provider);
	}

	public string ToString(string format)
	{
		return Number.FormatInt64(this, format, null);
	}

	[SecuritySafeCritical]
	public string ToString(string format, IFormatProvider provider)
	{
		return Number.FormatInt64(this, format, provider);
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider provider = null)
	{
		return Number.TryFormatInt64(this, format, provider, destination, out charsWritten);
	}

	public static long Parse(string s)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return Number.ParseInt64(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
	}

	public static long Parse(string s, NumberStyles style)
	{
		NumberFormatInfo.ValidateParseStyleInteger(style);
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return Number.ParseInt64(s, style, NumberFormatInfo.CurrentInfo);
	}

	public static long Parse(string s, IFormatProvider provider)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return Number.ParseInt64(s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
	}

	public static long Parse(string s, NumberStyles style, IFormatProvider provider)
	{
		NumberFormatInfo.ValidateParseStyleInteger(style);
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return Number.ParseInt64(s, style, NumberFormatInfo.GetInstance(provider));
	}

	public static long Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider provider = null)
	{
		NumberFormatInfo.ValidateParseStyleInteger(style);
		return Number.ParseInt64(s, style, NumberFormatInfo.GetInstance(provider));
	}

	public static bool TryParse(string s, out long result)
	{
		if (s == null)
		{
			result = 0L;
			return false;
		}
		return Number.TryParseInt64(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
	}

	public static bool TryParse(ReadOnlySpan<char> s, out long result)
	{
		return Number.TryParseInt64(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
	}

	public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out long result)
	{
		NumberFormatInfo.ValidateParseStyleInteger(style);
		if (s == null)
		{
			result = 0L;
			return false;
		}
		return Number.TryParseInt64(s, style, NumberFormatInfo.GetInstance(provider), out result);
	}

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider, out long result)
	{
		NumberFormatInfo.ValidateParseStyleInteger(style);
		return Number.TryParseInt64(s, style, NumberFormatInfo.GetInstance(provider), out result);
	}

	public TypeCode GetTypeCode()
	{
		return TypeCode.Int64;
	}

	bool IConvertible.ToBoolean(IFormatProvider provider)
	{
		return Convert.ToBoolean(this);
	}

	char IConvertible.ToChar(IFormatProvider provider)
	{
		return Convert.ToChar(this);
	}

	sbyte IConvertible.ToSByte(IFormatProvider provider)
	{
		return Convert.ToSByte(this);
	}

	byte IConvertible.ToByte(IFormatProvider provider)
	{
		return Convert.ToByte(this);
	}

	short IConvertible.ToInt16(IFormatProvider provider)
	{
		return Convert.ToInt16(this);
	}

	ushort IConvertible.ToUInt16(IFormatProvider provider)
	{
		return Convert.ToUInt16(this);
	}

	int IConvertible.ToInt32(IFormatProvider provider)
	{
		return Convert.ToInt32(this);
	}

	uint IConvertible.ToUInt32(IFormatProvider provider)
	{
		return Convert.ToUInt32(this);
	}

	long IConvertible.ToInt64(IFormatProvider provider)
	{
		return this;
	}

	ulong IConvertible.ToUInt64(IFormatProvider provider)
	{
		return Convert.ToUInt64(this);
	}

	float IConvertible.ToSingle(IFormatProvider provider)
	{
		return Convert.ToSingle(this);
	}

	double IConvertible.ToDouble(IFormatProvider provider)
	{
		return Convert.ToDouble(this);
	}

	decimal IConvertible.ToDecimal(IFormatProvider provider)
	{
		return Convert.ToDecimal(this);
	}

	DateTime IConvertible.ToDateTime(IFormatProvider provider)
	{
		throw new InvalidCastException(SR.Format("Invalid cast from '{0}' to '{1}'.", "Int64", "DateTime"));
	}

	object IConvertible.ToType(Type type, IFormatProvider provider)
	{
		return Convert.DefaultToType(this, type, provider);
	}
}
