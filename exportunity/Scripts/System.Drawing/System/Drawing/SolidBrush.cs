using System.Runtime.InteropServices;

namespace System.Drawing;

public sealed class SolidBrush : Brush
{
	private Color _color = Color.Empty;

	private bool _immutable;

	public Color Color
	{
		get
		{
			if (_color == Color.Empty)
			{
				SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetSolidFillColor(new HandleRef(this, base.NativeBrush), out var color));
				_color = Color.FromArgb(color);
			}
			return _color;
		}
		set
		{
			if (_immutable)
			{
				throw new ArgumentException(global::SR.Format("Changes cannot be made to {0} because permissions are not valid.", "Brush"));
			}
			if (_color != value)
			{
				_ = _color;
				InternalSetColor(value);
			}
		}
	}

	public SolidBrush(Color color)
	{
		_color = color;
		IntPtr brush = IntPtr.Zero;
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateSolidFill(_color.ToArgb(), out brush));
		SetNativeBrushInternal(brush);
	}

	internal SolidBrush(Color color, bool immutable)
		: this(color)
	{
		_immutable = immutable;
	}

	internal SolidBrush(IntPtr nativeBrush)
	{
		SetNativeBrushInternal(nativeBrush);
	}

	public override object Clone()
	{
		IntPtr clonedBrush = IntPtr.Zero;
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCloneBrush(new HandleRef(this, base.NativeBrush), out clonedBrush));
		return new SolidBrush(clonedBrush);
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposing)
		{
			_immutable = false;
		}
		else if (_immutable)
		{
			throw new ArgumentException(global::SR.Format("Changes cannot be made to {0} because permissions are not valid.", "Brush"));
		}
		base.Dispose(disposing);
	}

	private void InternalSetColor(Color value)
	{
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetSolidFillColor(new HandleRef(this, base.NativeBrush), value.ToArgb()));
		_color = value;
	}
}
