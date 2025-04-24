using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D;

public class CustomLineCap : MarshalByRefObject, ICloneable, IDisposable
{
	internal SafeCustomLineCapHandle nativeCap;

	private bool _disposed;

	public LineJoin StrokeJoin
	{
		get
		{
			LineJoin lineJoin;
			int num = GDIPlus.GdipGetCustomLineCapStrokeJoin(new HandleRef(this, nativeCap), out lineJoin);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return lineJoin;
		}
		set
		{
			int num = GDIPlus.GdipSetCustomLineCapStrokeJoin(new HandleRef(this, nativeCap), value);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}
	}

	public LineCap BaseCap
	{
		get
		{
			LineCap baseCap;
			int num = GDIPlus.GdipGetCustomLineCapBaseCap(new HandleRef(this, nativeCap), out baseCap);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return baseCap;
		}
		set
		{
			int num = GDIPlus.GdipSetCustomLineCapBaseCap(new HandleRef(this, nativeCap), value);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}
	}

	public float BaseInset
	{
		get
		{
			float inset;
			int num = GDIPlus.GdipGetCustomLineCapBaseInset(new HandleRef(this, nativeCap), out inset);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return inset;
		}
		set
		{
			int num = GDIPlus.GdipSetCustomLineCapBaseInset(new HandleRef(this, nativeCap), value);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}
	}

	public float WidthScale
	{
		get
		{
			float widthScale;
			int num = GDIPlus.GdipGetCustomLineCapWidthScale(new HandleRef(this, nativeCap), out widthScale);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return widthScale;
		}
		set
		{
			int num = GDIPlus.GdipSetCustomLineCapWidthScale(new HandleRef(this, nativeCap), value);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}
	}

	internal static CustomLineCap CreateCustomLineCapObject(IntPtr cap)
	{
		return new CustomLineCap(cap);
	}

	internal CustomLineCap()
	{
	}

	public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath)
		: this(fillPath, strokePath, LineCap.Flat)
	{
	}

	public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap)
		: this(fillPath, strokePath, baseCap, 0f)
	{
	}

	public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap, float baseInset)
	{
		IntPtr customCap;
		int num = GDIPlus.GdipCreateCustomLineCap(new HandleRef(fillPath, fillPath?.nativePath ?? IntPtr.Zero), new HandleRef(strokePath, strokePath?.nativePath ?? IntPtr.Zero), baseCap, baseInset, out customCap);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		SetNativeLineCap(customCap);
	}

	internal CustomLineCap(IntPtr nativeLineCap)
	{
		SetNativeLineCap(nativeLineCap);
	}

	internal void SetNativeLineCap(IntPtr handle)
	{
		if (handle == IntPtr.Zero)
		{
			throw new ArgumentNullException("handle");
		}
		nativeCap = new SafeCustomLineCapHandle(handle);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing && nativeCap != null)
			{
				nativeCap.Dispose();
			}
			_disposed = true;
		}
	}

	~CustomLineCap()
	{
		Dispose(disposing: false);
	}

	public object Clone()
	{
		return CoreClone();
	}

	internal virtual object CoreClone()
	{
		IntPtr clonedCap;
		int num = GDIPlus.GdipCloneCustomLineCap(new HandleRef(this, nativeCap), out clonedCap);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		return CreateCustomLineCapObject(clonedCap);
	}

	public void SetStrokeCaps(LineCap startCap, LineCap endCap)
	{
		int num = GDIPlus.GdipSetCustomLineCapStrokeCaps(new HandleRef(this, nativeCap), startCap, endCap);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public void GetStrokeCaps(out LineCap startCap, out LineCap endCap)
	{
		int num = GDIPlus.GdipGetCustomLineCapStrokeCaps(new HandleRef(this, nativeCap), out startCap, out endCap);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}
}
