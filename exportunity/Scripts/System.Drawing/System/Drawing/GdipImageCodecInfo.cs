using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace System.Drawing;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct GdipImageCodecInfo
{
	internal Guid Clsid;

	internal Guid FormatID;

	internal IntPtr CodecName;

	internal IntPtr DllName;

	internal IntPtr FormatDescription;

	internal IntPtr FilenameExtension;

	internal IntPtr MimeType;

	internal ImageCodecFlags Flags;

	internal int Version;

	internal int SigCount;

	internal int SigSize;

	private IntPtr SigPattern;

	private IntPtr SigMask;

	internal static void MarshalTo(GdipImageCodecInfo gdipcodec, ImageCodecInfo codec)
	{
		codec.CodecName = Marshal.PtrToStringUni(gdipcodec.CodecName);
		codec.DllName = Marshal.PtrToStringUni(gdipcodec.DllName);
		codec.FormatDescription = Marshal.PtrToStringUni(gdipcodec.FormatDescription);
		codec.FilenameExtension = Marshal.PtrToStringUni(gdipcodec.FilenameExtension);
		codec.MimeType = Marshal.PtrToStringUni(gdipcodec.MimeType);
		codec.Clsid = gdipcodec.Clsid;
		codec.FormatID = gdipcodec.FormatID;
		codec.Flags = gdipcodec.Flags;
		codec.Version = gdipcodec.Version;
		codec.SignatureMasks = new byte[gdipcodec.SigCount][];
		codec.SignaturePatterns = new byte[gdipcodec.SigCount][];
		IntPtr source = gdipcodec.SigPattern;
		IntPtr source2 = gdipcodec.SigMask;
		for (int i = 0; i < gdipcodec.SigCount; i++)
		{
			codec.SignatureMasks[i] = new byte[gdipcodec.SigSize];
			Marshal.Copy(source2, codec.SignatureMasks[i], 0, gdipcodec.SigSize);
			source2 = new IntPtr(source2.ToInt64() + gdipcodec.SigSize);
			codec.SignaturePatterns[i] = new byte[gdipcodec.SigSize];
			Marshal.Copy(source, codec.SignaturePatterns[i], 0, gdipcodec.SigSize);
			source = new IntPtr(source.ToInt64() + gdipcodec.SigSize);
		}
	}
}
