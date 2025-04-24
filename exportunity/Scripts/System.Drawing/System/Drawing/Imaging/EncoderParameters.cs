using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

public sealed class EncoderParameters : IDisposable
{
	private EncoderParameter[] _param;

	public EncoderParameter[] Param
	{
		get
		{
			return _param;
		}
		set
		{
			_param = value;
		}
	}

	public EncoderParameters(int count)
	{
		_param = new EncoderParameter[count];
	}

	public EncoderParameters()
	{
		_param = new EncoderParameter[1];
	}

	internal IntPtr ConvertToMemory()
	{
		int num = Marshal.SizeOf(typeof(EncoderParameter));
		int num2 = _param.Length;
		IntPtr intPtr;
		long num3;
		checked
		{
			intPtr = Marshal.AllocHGlobal(num2 * num + Marshal.SizeOf(typeof(IntPtr)));
			if (intPtr == IntPtr.Zero)
			{
				throw SafeNativeMethods.Gdip.StatusException(3);
			}
			Marshal.WriteIntPtr(intPtr, (IntPtr)num2);
			num3 = (long)intPtr + Marshal.SizeOf(typeof(IntPtr));
		}
		for (int i = 0; i < num2; i++)
		{
			Marshal.StructureToPtr(_param[i], (IntPtr)(num3 + i * num), fDeleteOld: false);
		}
		return intPtr;
	}

	internal static EncoderParameters ConvertFromMemory(IntPtr memory)
	{
		if (memory == IntPtr.Zero)
		{
			throw SafeNativeMethods.Gdip.StatusException(2);
		}
		int num = Marshal.ReadIntPtr(memory).ToInt32();
		EncoderParameters encoderParameters = new EncoderParameters(num);
		int num2 = Marshal.SizeOf(typeof(EncoderParameter));
		long num3 = (long)memory + Marshal.SizeOf(typeof(IntPtr));
		for (int i = 0; i < num; i++)
		{
			Guid guid = (Guid)Marshal.PtrToStructure((IntPtr)(i * num2 + num3), typeof(Guid));
			int numberValues = Marshal.ReadInt32((IntPtr)(i * num2 + num3 + 16));
			EncoderParameterValueType type = (EncoderParameterValueType)Marshal.ReadInt32((IntPtr)(i * num2 + num3 + 20));
			IntPtr value = Marshal.ReadIntPtr((IntPtr)(i * num2 + num3 + 24));
			encoderParameters._param[i] = new EncoderParameter(new Encoder(guid), numberValues, type, value);
		}
		return encoderParameters;
	}

	public void Dispose()
	{
		EncoderParameter[] param = _param;
		for (int i = 0; i < param.Length; i++)
		{
			param[i]?.Dispose();
		}
		_param = null;
	}
}
