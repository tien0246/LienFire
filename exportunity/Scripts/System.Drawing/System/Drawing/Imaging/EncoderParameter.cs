using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
public sealed class EncoderParameter : IDisposable
{
	[MarshalAs(UnmanagedType.Struct)]
	private Guid _parameterGuid;

	private int _numberOfValues;

	private EncoderParameterValueType _parameterValueType;

	private IntPtr _parameterValue;

	public Encoder Encoder
	{
		get
		{
			return new Encoder(_parameterGuid);
		}
		set
		{
			_parameterGuid = value.Guid;
		}
	}

	public EncoderParameterValueType Type => _parameterValueType;

	public EncoderParameterValueType ValueType => _parameterValueType;

	public int NumberOfValues => _numberOfValues;

	~EncoderParameter()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.KeepAlive(this);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (_parameterValue != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(_parameterValue);
		}
		_parameterValue = IntPtr.Zero;
	}

	public EncoderParameter(Encoder encoder, byte value)
	{
		_parameterGuid = encoder.Guid;
		_parameterValueType = EncoderParameterValueType.ValueTypeByte;
		_numberOfValues = 1;
		_parameterValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)));
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		Marshal.WriteByte(_parameterValue, value);
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, byte value, bool undefined)
	{
		_parameterGuid = encoder.Guid;
		if (undefined)
		{
			_parameterValueType = EncoderParameterValueType.ValueTypeUndefined;
		}
		else
		{
			_parameterValueType = EncoderParameterValueType.ValueTypeByte;
		}
		_numberOfValues = 1;
		_parameterValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)));
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		Marshal.WriteByte(_parameterValue, value);
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, short value)
	{
		_parameterGuid = encoder.Guid;
		_parameterValueType = EncoderParameterValueType.ValueTypeShort;
		_numberOfValues = 1;
		_parameterValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(short)));
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		Marshal.WriteInt16(_parameterValue, value);
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, long value)
	{
		_parameterGuid = encoder.Guid;
		_parameterValueType = EncoderParameterValueType.ValueTypeLong;
		_numberOfValues = 1;
		_parameterValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		Marshal.WriteInt32(_parameterValue, (int)value);
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, int numerator, int denominator)
	{
		_parameterGuid = encoder.Guid;
		_parameterValueType = EncoderParameterValueType.ValueTypeRational;
		_numberOfValues = 1;
		int num = Marshal.SizeOf(typeof(int));
		_parameterValue = Marshal.AllocHGlobal(2 * num);
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		Marshal.WriteInt32(_parameterValue, numerator);
		Marshal.WriteInt32(Add(_parameterValue, num), denominator);
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, long rangebegin, long rangeend)
	{
		_parameterGuid = encoder.Guid;
		_parameterValueType = EncoderParameterValueType.ValueTypeLongRange;
		_numberOfValues = 1;
		int num = Marshal.SizeOf(typeof(int));
		_parameterValue = Marshal.AllocHGlobal(2 * num);
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		Marshal.WriteInt32(_parameterValue, (int)rangebegin);
		Marshal.WriteInt32(Add(_parameterValue, num), (int)rangeend);
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, int numerator1, int demoninator1, int numerator2, int demoninator2)
	{
		_parameterGuid = encoder.Guid;
		_parameterValueType = EncoderParameterValueType.ValueTypeRationalRange;
		_numberOfValues = 1;
		int num = Marshal.SizeOf(typeof(int));
		_parameterValue = Marshal.AllocHGlobal(4 * num);
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		Marshal.WriteInt32(_parameterValue, numerator1);
		Marshal.WriteInt32(Add(_parameterValue, num), demoninator1);
		Marshal.WriteInt32(Add(_parameterValue, 2 * num), numerator2);
		Marshal.WriteInt32(Add(_parameterValue, 3 * num), demoninator2);
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, string value)
	{
		_parameterGuid = encoder.Guid;
		_parameterValueType = EncoderParameterValueType.ValueTypeAscii;
		_numberOfValues = value.Length;
		_parameterValue = Marshal.StringToHGlobalAnsi(value);
		GC.KeepAlive(this);
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
	}

	public EncoderParameter(Encoder encoder, byte[] value)
	{
		_parameterGuid = encoder.Guid;
		_parameterValueType = EncoderParameterValueType.ValueTypeByte;
		_numberOfValues = value.Length;
		_parameterValue = Marshal.AllocHGlobal(_numberOfValues);
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		Marshal.Copy(value, 0, _parameterValue, _numberOfValues);
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, byte[] value, bool undefined)
	{
		_parameterGuid = encoder.Guid;
		if (undefined)
		{
			_parameterValueType = EncoderParameterValueType.ValueTypeUndefined;
		}
		else
		{
			_parameterValueType = EncoderParameterValueType.ValueTypeByte;
		}
		_numberOfValues = value.Length;
		_parameterValue = Marshal.AllocHGlobal(_numberOfValues);
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		Marshal.Copy(value, 0, _parameterValue, _numberOfValues);
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, short[] value)
	{
		_parameterGuid = encoder.Guid;
		_parameterValueType = EncoderParameterValueType.ValueTypeShort;
		_numberOfValues = value.Length;
		int num = Marshal.SizeOf(typeof(short));
		_parameterValue = Marshal.AllocHGlobal(checked(_numberOfValues * num));
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		Marshal.Copy(value, 0, _parameterValue, _numberOfValues);
		GC.KeepAlive(this);
	}

	public unsafe EncoderParameter(Encoder encoder, long[] value)
	{
		_parameterGuid = encoder.Guid;
		_parameterValueType = EncoderParameterValueType.ValueTypeLong;
		_numberOfValues = value.Length;
		int num = Marshal.SizeOf(typeof(int));
		_parameterValue = Marshal.AllocHGlobal(checked(_numberOfValues * num));
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		int* ptr = (int*)(void*)_parameterValue;
		fixed (long* ptr2 = value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				ptr[i] = (int)ptr2[i];
			}
		}
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, int[] numerator, int[] denominator)
	{
		_parameterGuid = encoder.Guid;
		if (numerator.Length != denominator.Length)
		{
			throw SafeNativeMethods.Gdip.StatusException(2);
		}
		_parameterValueType = EncoderParameterValueType.ValueTypeRational;
		_numberOfValues = numerator.Length;
		int num = Marshal.SizeOf(typeof(int));
		_parameterValue = Marshal.AllocHGlobal(checked(_numberOfValues * 2 * num));
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		for (int i = 0; i < _numberOfValues; i++)
		{
			Marshal.WriteInt32(Add(i * 2 * num, _parameterValue), numerator[i]);
			Marshal.WriteInt32(Add((i * 2 + 1) * num, _parameterValue), denominator[i]);
		}
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, long[] rangebegin, long[] rangeend)
	{
		_parameterGuid = encoder.Guid;
		if (rangebegin.Length != rangeend.Length)
		{
			throw SafeNativeMethods.Gdip.StatusException(2);
		}
		_parameterValueType = EncoderParameterValueType.ValueTypeLongRange;
		_numberOfValues = rangebegin.Length;
		int num = Marshal.SizeOf(typeof(int));
		_parameterValue = Marshal.AllocHGlobal(checked(_numberOfValues * 2 * num));
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		for (int i = 0; i < _numberOfValues; i++)
		{
			Marshal.WriteInt32(Add(i * 2 * num, _parameterValue), (int)rangebegin[i]);
			Marshal.WriteInt32(Add((i * 2 + 1) * num, _parameterValue), (int)rangeend[i]);
		}
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, int[] numerator1, int[] denominator1, int[] numerator2, int[] denominator2)
	{
		_parameterGuid = encoder.Guid;
		if (numerator1.Length != denominator1.Length || numerator1.Length != denominator2.Length || denominator1.Length != denominator2.Length)
		{
			throw SafeNativeMethods.Gdip.StatusException(2);
		}
		_parameterValueType = EncoderParameterValueType.ValueTypeRationalRange;
		_numberOfValues = numerator1.Length;
		int num = Marshal.SizeOf(typeof(int));
		_parameterValue = Marshal.AllocHGlobal(checked(_numberOfValues * 4 * num));
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		for (int i = 0; i < _numberOfValues; i++)
		{
			Marshal.WriteInt32(Add(_parameterValue, 4 * i * num), numerator1[i]);
			Marshal.WriteInt32(Add(_parameterValue, (4 * i + 1) * num), denominator1[i]);
			Marshal.WriteInt32(Add(_parameterValue, (4 * i + 2) * num), numerator2[i]);
			Marshal.WriteInt32(Add(_parameterValue, (4 * i + 3) * num), denominator2[i]);
		}
		GC.KeepAlive(this);
	}

	[Obsolete("This constructor has been deprecated. Use EncoderParameter(Encoder encoder, int numberValues, EncoderParameterValueType type, IntPtr value) instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
	public EncoderParameter(Encoder encoder, int NumberOfValues, int Type, int Value)
	{
		int num;
		switch ((EncoderParameterValueType)Type)
		{
		case EncoderParameterValueType.ValueTypeByte:
		case EncoderParameterValueType.ValueTypeAscii:
			num = 1;
			break;
		case EncoderParameterValueType.ValueTypeShort:
			num = 2;
			break;
		case EncoderParameterValueType.ValueTypeLong:
			num = 4;
			break;
		case EncoderParameterValueType.ValueTypeRational:
		case EncoderParameterValueType.ValueTypeLongRange:
			num = 8;
			break;
		case EncoderParameterValueType.ValueTypeUndefined:
			num = 1;
			break;
		case EncoderParameterValueType.ValueTypeRationalRange:
			num = 16;
			break;
		default:
			throw SafeNativeMethods.Gdip.StatusException(8);
		}
		int num2 = checked(num * NumberOfValues);
		_parameterValue = Marshal.AllocHGlobal(num2);
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		for (int i = 0; i < num2; i++)
		{
			Marshal.WriteByte(Add(_parameterValue, i), Marshal.ReadByte((IntPtr)(Value + i)));
		}
		_parameterValueType = (EncoderParameterValueType)Type;
		_numberOfValues = NumberOfValues;
		_parameterGuid = encoder.Guid;
		GC.KeepAlive(this);
	}

	public EncoderParameter(Encoder encoder, int numberValues, EncoderParameterValueType type, IntPtr value)
	{
		int num;
		switch (type)
		{
		case EncoderParameterValueType.ValueTypeByte:
		case EncoderParameterValueType.ValueTypeAscii:
			num = 1;
			break;
		case EncoderParameterValueType.ValueTypeShort:
			num = 2;
			break;
		case EncoderParameterValueType.ValueTypeLong:
			num = 4;
			break;
		case EncoderParameterValueType.ValueTypeRational:
		case EncoderParameterValueType.ValueTypeLongRange:
			num = 8;
			break;
		case EncoderParameterValueType.ValueTypeUndefined:
			num = 1;
			break;
		case EncoderParameterValueType.ValueTypeRationalRange:
			num = 16;
			break;
		default:
			throw SafeNativeMethods.Gdip.StatusException(8);
		}
		int num2 = checked(num * numberValues);
		_parameterValue = Marshal.AllocHGlobal(num2);
		if (_parameterValue == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(3);
		}
		for (int i = 0; i < num2; i++)
		{
			Marshal.WriteByte(Add(_parameterValue, i), Marshal.ReadByte(value + i));
		}
		_parameterValueType = type;
		_numberOfValues = numberValues;
		_parameterGuid = encoder.Guid;
		GC.KeepAlive(this);
	}

	private static IntPtr Add(IntPtr a, int b)
	{
		return (IntPtr)((long)a + b);
	}

	private static IntPtr Add(int a, IntPtr b)
	{
		return (IntPtr)(a + (long)b);
	}
}
