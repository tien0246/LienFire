using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D;

public sealed class AdjustableArrowCap : CustomLineCap
{
	public float Height
	{
		get
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetAdjustableArrowCapHeight(new HandleRef(this, nativeCap), out var height));
			return height;
		}
		set
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetAdjustableArrowCapHeight(new HandleRef(this, nativeCap), value));
		}
	}

	public float Width
	{
		get
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetAdjustableArrowCapWidth(new HandleRef(this, nativeCap), out var width));
			return width;
		}
		set
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetAdjustableArrowCapWidth(new HandleRef(this, nativeCap), value));
		}
	}

	public float MiddleInset
	{
		get
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetAdjustableArrowCapMiddleInset(new HandleRef(this, nativeCap), out var middleInset));
			return middleInset;
		}
		set
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetAdjustableArrowCapMiddleInset(new HandleRef(this, nativeCap), value));
		}
	}

	public bool Filled
	{
		get
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetAdjustableArrowCapFillState(new HandleRef(this, nativeCap), out var isFilled));
			return isFilled;
		}
		set
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetAdjustableArrowCapFillState(new HandleRef(this, nativeCap), value));
		}
	}

	internal AdjustableArrowCap(IntPtr nativeCap)
		: base(nativeCap)
	{
	}

	public AdjustableArrowCap(float width, float height)
		: this(width, height, isFilled: true)
	{
	}

	public AdjustableArrowCap(float width, float height, bool isFilled)
	{
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateAdjustableArrowCap(height, width, isFilled, out var arrowCap));
		SetNativeLineCap(arrowCap);
	}
}
