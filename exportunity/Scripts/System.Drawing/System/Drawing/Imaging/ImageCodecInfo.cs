using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

public sealed class ImageCodecInfo
{
	private Guid _clsid;

	private Guid _formatID;

	private string _codecName;

	private string _dllName;

	private string _formatDescription;

	private string _filenameExtension;

	private string _mimeType;

	private ImageCodecFlags _flags;

	private int _version;

	private byte[][] _signaturePatterns;

	private byte[][] _signatureMasks;

	public Guid Clsid
	{
		get
		{
			return _clsid;
		}
		set
		{
			_clsid = value;
		}
	}

	public Guid FormatID
	{
		get
		{
			return _formatID;
		}
		set
		{
			_formatID = value;
		}
	}

	public string CodecName
	{
		get
		{
			return _codecName;
		}
		set
		{
			_codecName = value;
		}
	}

	public string DllName
	{
		get
		{
			return _dllName;
		}
		set
		{
			_dllName = value;
		}
	}

	public string FormatDescription
	{
		get
		{
			return _formatDescription;
		}
		set
		{
			_formatDescription = value;
		}
	}

	public string FilenameExtension
	{
		get
		{
			return _filenameExtension;
		}
		set
		{
			_filenameExtension = value;
		}
	}

	public string MimeType
	{
		get
		{
			return _mimeType;
		}
		set
		{
			_mimeType = value;
		}
	}

	public ImageCodecFlags Flags
	{
		get
		{
			return _flags;
		}
		set
		{
			_flags = value;
		}
	}

	public int Version
	{
		get
		{
			return _version;
		}
		set
		{
			_version = value;
		}
	}

	[CLSCompliant(false)]
	public byte[][] SignaturePatterns
	{
		get
		{
			return _signaturePatterns;
		}
		set
		{
			_signaturePatterns = value;
		}
	}

	[CLSCompliant(false)]
	public byte[][] SignatureMasks
	{
		get
		{
			return _signatureMasks;
		}
		set
		{
			_signatureMasks = value;
		}
	}

	internal ImageCodecInfo()
	{
	}

	public static ImageCodecInfo[] GetImageDecoders()
	{
		int num = GDIPlus.GdipGetImageDecodersSize(out var decoderNums, out var arraySize);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		IntPtr intPtr = Marshal.AllocHGlobal(arraySize);
		try
		{
			num = GDIPlus.GdipGetImageDecoders(decoderNums, arraySize, intPtr);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return ConvertFromMemory(intPtr, decoderNums);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static ImageCodecInfo[] GetImageEncoders()
	{
		int num = GDIPlus.GdipGetImageEncodersSize(out var encoderNums, out var arraySize);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		IntPtr intPtr = Marshal.AllocHGlobal(arraySize);
		try
		{
			num = GDIPlus.GdipGetImageEncoders(encoderNums, arraySize, intPtr);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return ConvertFromMemory(intPtr, encoderNums);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	private static ImageCodecInfo[] ConvertFromMemory(IntPtr memoryStart, int numCodecs)
	{
		ImageCodecInfo[] array = new ImageCodecInfo[numCodecs];
		for (int i = 0; i < numCodecs; i++)
		{
			IntPtr ptr = (IntPtr)((long)memoryStart + Marshal.SizeOf(typeof(ImageCodecInfoPrivate)) * i);
			ImageCodecInfoPrivate imageCodecInfoPrivate = new ImageCodecInfoPrivate();
			Marshal.PtrToStructure(ptr, imageCodecInfoPrivate);
			array[i] = new ImageCodecInfo();
			array[i].Clsid = imageCodecInfoPrivate.Clsid;
			array[i].FormatID = imageCodecInfoPrivate.FormatID;
			array[i].CodecName = Marshal.PtrToStringUni(imageCodecInfoPrivate.CodecName);
			array[i].DllName = Marshal.PtrToStringUni(imageCodecInfoPrivate.DllName);
			array[i].FormatDescription = Marshal.PtrToStringUni(imageCodecInfoPrivate.FormatDescription);
			array[i].FilenameExtension = Marshal.PtrToStringUni(imageCodecInfoPrivate.FilenameExtension);
			array[i].MimeType = Marshal.PtrToStringUni(imageCodecInfoPrivate.MimeType);
			array[i].Flags = (ImageCodecFlags)imageCodecInfoPrivate.Flags;
			array[i].Version = imageCodecInfoPrivate.Version;
			array[i].SignaturePatterns = new byte[imageCodecInfoPrivate.SigCount][];
			array[i].SignatureMasks = new byte[imageCodecInfoPrivate.SigCount][];
			for (int j = 0; j < imageCodecInfoPrivate.SigCount; j++)
			{
				array[i].SignaturePatterns[j] = new byte[imageCodecInfoPrivate.SigSize];
				array[i].SignatureMasks[j] = new byte[imageCodecInfoPrivate.SigSize];
				Marshal.Copy((IntPtr)((long)imageCodecInfoPrivate.SigMask + j * imageCodecInfoPrivate.SigSize), array[i].SignatureMasks[j], 0, imageCodecInfoPrivate.SigSize);
				Marshal.Copy((IntPtr)((long)imageCodecInfoPrivate.SigPattern + j * imageCodecInfoPrivate.SigSize), array[i].SignaturePatterns[j], 0, imageCodecInfoPrivate.SigSize);
			}
		}
		return array;
	}
}
