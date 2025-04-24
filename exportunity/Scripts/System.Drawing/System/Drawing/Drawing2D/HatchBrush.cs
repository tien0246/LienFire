using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D;

public sealed class HatchBrush : Brush
{
	public HatchStyle HatchStyle
	{
		get
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetHatchStyle(new HandleRef(this, base.NativeBrush), out var hatchstyle));
			return (HatchStyle)hatchstyle;
		}
	}

	public Color ForegroundColor
	{
		get
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetHatchForegroundColor(new HandleRef(this, base.NativeBrush), out var foreColor));
			return Color.FromArgb(foreColor);
		}
	}

	public Color BackgroundColor
	{
		get
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetHatchBackgroundColor(new HandleRef(this, base.NativeBrush), out var backColor));
			return Color.FromArgb(backColor);
		}
	}

	public HatchBrush(HatchStyle hatchstyle, Color foreColor)
		: this(hatchstyle, foreColor, Color.FromArgb(-16777216))
	{
	}

	public HatchBrush(HatchStyle hatchstyle, Color foreColor, Color backColor)
	{
		if (hatchstyle < HatchStyle.Horizontal || hatchstyle > HatchStyle.SolidDiamond)
		{
			throw new ArgumentException(global::SR.Format("The value of argument '{0}' ({1}) is invalid for Enum type '{2}'.", "hatchstyle", hatchstyle, "HatchStyle"), "hatchstyle");
		}
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateHatchBrush((int)hatchstyle, foreColor.ToArgb(), backColor.ToArgb(), out var brush));
		SetNativeBrushInternal(brush);
	}

	internal HatchBrush(IntPtr nativeBrush)
	{
		SetNativeBrushInternal(nativeBrush);
	}

	public override object Clone()
	{
		IntPtr clonedBrush = IntPtr.Zero;
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCloneBrush(new HandleRef(this, base.NativeBrush), out clonedBrush));
		return new HatchBrush(clonedBrush);
	}
}
