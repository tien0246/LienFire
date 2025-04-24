using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
public sealed class ImageAttributes : ICloneable, IDisposable
{
	internal IntPtr nativeImageAttributes;

	internal void SetNativeImageAttributes(IntPtr handle)
	{
		if (handle == IntPtr.Zero)
		{
			throw new ArgumentNullException("handle");
		}
		nativeImageAttributes = handle;
	}

	public ImageAttributes()
	{
		IntPtr imageattr = IntPtr.Zero;
		int num = GDIPlus.GdipCreateImageAttributes(out imageattr);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		SetNativeImageAttributes(imageattr);
	}

	internal ImageAttributes(IntPtr newNativeImageAttributes)
	{
		SetNativeImageAttributes(newNativeImageAttributes);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!(nativeImageAttributes != IntPtr.Zero))
		{
			return;
		}
		try
		{
			GDIPlus.GdipDisposeImageAttributes(new HandleRef(this, nativeImageAttributes));
		}
		catch (Exception ex)
		{
			if (ClientUtils.IsSecurityOrCriticalException(ex))
			{
				throw;
			}
		}
		finally
		{
			nativeImageAttributes = IntPtr.Zero;
		}
	}

	~ImageAttributes()
	{
		Dispose(disposing: false);
	}

	public object Clone()
	{
		IntPtr cloneImageattr = IntPtr.Zero;
		int num = GDIPlus.GdipCloneImageAttributes(new HandleRef(this, nativeImageAttributes), out cloneImageattr);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		return new ImageAttributes(cloneImageattr);
	}

	public void SetColorMatrix(ColorMatrix newColorMatrix)
	{
		SetColorMatrix(newColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
	}

	public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag flags)
	{
		SetColorMatrix(newColorMatrix, flags, ColorAdjustType.Default);
	}

	public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag mode, ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesColorMatrix(new HandleRef(this, nativeImageAttributes), type, enableFlag: true, newColorMatrix, null, mode);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void ClearColorMatrix()
	{
		ClearColorMatrix(ColorAdjustType.Default);
	}

	public void ClearColorMatrix(ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesColorMatrix(new HandleRef(this, nativeImageAttributes), type, enableFlag: false, null, null, ColorMatrixFlag.Default);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix)
	{
		SetColorMatrices(newColorMatrix, grayMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
	}

	public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag flags)
	{
		SetColorMatrices(newColorMatrix, grayMatrix, flags, ColorAdjustType.Default);
	}

	public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag mode, ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesColorMatrix(new HandleRef(this, nativeImageAttributes), type, enableFlag: true, newColorMatrix, grayMatrix, mode);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void SetThreshold(float threshold)
	{
		SetThreshold(threshold, ColorAdjustType.Default);
	}

	public void SetThreshold(float threshold, ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesThreshold(new HandleRef(this, nativeImageAttributes), type, enableFlag: true, threshold);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void ClearThreshold()
	{
		ClearThreshold(ColorAdjustType.Default);
	}

	public void ClearThreshold(ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesThreshold(new HandleRef(this, nativeImageAttributes), type, enableFlag: false, 0f);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void SetGamma(float gamma)
	{
		SetGamma(gamma, ColorAdjustType.Default);
	}

	public void SetGamma(float gamma, ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesGamma(new HandleRef(this, nativeImageAttributes), type, enableFlag: true, gamma);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void ClearGamma()
	{
		ClearGamma(ColorAdjustType.Default);
	}

	public void ClearGamma(ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesGamma(new HandleRef(this, nativeImageAttributes), type, enableFlag: false, 0f);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void SetNoOp()
	{
		SetNoOp(ColorAdjustType.Default);
	}

	public void SetNoOp(ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesNoOp(new HandleRef(this, nativeImageAttributes), type, enableFlag: true);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void ClearNoOp()
	{
		ClearNoOp(ColorAdjustType.Default);
	}

	public void ClearNoOp(ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesNoOp(new HandleRef(this, nativeImageAttributes), type, enableFlag: false);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void SetColorKey(Color colorLow, Color colorHigh)
	{
		SetColorKey(colorLow, colorHigh, ColorAdjustType.Default);
	}

	public void SetColorKey(Color colorLow, Color colorHigh, ColorAdjustType type)
	{
		int colorLow2 = colorLow.ToArgb();
		int colorHigh2 = colorHigh.ToArgb();
		int num = GDIPlus.GdipSetImageAttributesColorKeys(new HandleRef(this, nativeImageAttributes), type, enableFlag: true, colorLow2, colorHigh2);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void ClearColorKey()
	{
		ClearColorKey(ColorAdjustType.Default);
	}

	public void ClearColorKey(ColorAdjustType type)
	{
		int num = 0;
		int num2 = GDIPlus.GdipSetImageAttributesColorKeys(new HandleRef(this, nativeImageAttributes), type, enableFlag: false, num, num);
		if (num2 != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num2);
		}
	}

	public void SetOutputChannel(ColorChannelFlag flags)
	{
		SetOutputChannel(flags, ColorAdjustType.Default);
	}

	public void SetOutputChannel(ColorChannelFlag flags, ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesOutputChannel(new HandleRef(this, nativeImageAttributes), type, enableFlag: true, flags);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void ClearOutputChannel()
	{
		ClearOutputChannel(ColorAdjustType.Default);
	}

	public void ClearOutputChannel(ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesOutputChannel(new HandleRef(this, nativeImageAttributes), type, enableFlag: false, ColorChannelFlag.ColorChannelLast);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void SetOutputChannelColorProfile(string colorProfileFilename)
	{
		SetOutputChannelColorProfile(colorProfileFilename, ColorAdjustType.Default);
	}

	public void SetOutputChannelColorProfile(string colorProfileFilename, ColorAdjustType type)
	{
		Path.GetFullPath(colorProfileFilename);
		int num = GDIPlus.GdipSetImageAttributesOutputChannelColorProfile(new HandleRef(this, nativeImageAttributes), type, enableFlag: true, colorProfileFilename);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void ClearOutputChannelColorProfile()
	{
		ClearOutputChannel(ColorAdjustType.Default);
	}

	public void ClearOutputChannelColorProfile(ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesOutputChannel(new HandleRef(this, nativeImageAttributes), type, enableFlag: false, ColorChannelFlag.ColorChannelLast);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void SetRemapTable(ColorMap[] map)
	{
		SetRemapTable(map, ColorAdjustType.Default);
	}

	public void SetRemapTable(ColorMap[] map, ColorAdjustType type)
	{
		int num = 0;
		int num2 = map.Length;
		int num3 = 4;
		IntPtr intPtr = Marshal.AllocHGlobal(checked(num2 * num3 * 2));
		try
		{
			for (num = 0; num < num2; num++)
			{
				Marshal.StructureToPtr(map[num].OldColor.ToArgb(), (IntPtr)((long)intPtr + num * num3 * 2), fDeleteOld: false);
				Marshal.StructureToPtr(map[num].NewColor.ToArgb(), (IntPtr)((long)intPtr + num * num3 * 2 + num3), fDeleteOld: false);
			}
			int num4 = GDIPlus.GdipSetImageAttributesRemapTable(new HandleRef(this, nativeImageAttributes), type, enableFlag: true, num2, new HandleRef(null, intPtr));
			if (num4 != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num4);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public void ClearRemapTable()
	{
		ClearRemapTable(ColorAdjustType.Default);
	}

	public void ClearRemapTable(ColorAdjustType type)
	{
		int num = GDIPlus.GdipSetImageAttributesRemapTable(new HandleRef(this, nativeImageAttributes), type, enableFlag: false, 0, NativeMethods.NullHandleRef);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void SetBrushRemapTable(ColorMap[] map)
	{
		SetRemapTable(map, ColorAdjustType.Brush);
	}

	public void ClearBrushRemapTable()
	{
		ClearRemapTable(ColorAdjustType.Brush);
	}

	public void SetWrapMode(WrapMode mode)
	{
		SetWrapMode(mode, default(Color), clamp: false);
	}

	public void SetWrapMode(WrapMode mode, Color color)
	{
		SetWrapMode(mode, color, clamp: false);
	}

	public void SetWrapMode(WrapMode mode, Color color, bool clamp)
	{
		int num = GDIPlus.GdipSetImageAttributesWrapMode(new HandleRef(this, nativeImageAttributes), (int)mode, color.ToArgb(), clamp);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void GetAdjustedPalette(ColorPalette palette, ColorAdjustType type)
	{
		IntPtr intPtr = palette.ConvertToMemory();
		try
		{
			int num = GDIPlus.GdipGetImageAttributesAdjustedPalette(new HandleRef(this, nativeImageAttributes), new HandleRef(null, intPtr), type);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			palette.ConvertFromMemory(intPtr);
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}
}
