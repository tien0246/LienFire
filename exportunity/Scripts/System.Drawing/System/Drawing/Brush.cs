using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Drawing;

public abstract class Brush : MarshalByRefObject, ICloneable, IDisposable
{
	private IntPtr _nativeBrush;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal IntPtr NativeBrush => _nativeBrush;

	public abstract object Clone();

	protected internal void SetNativeBrush(IntPtr brush)
	{
		SetNativeBrushInternal(brush);
	}

	internal void SetNativeBrushInternal(IntPtr brush)
	{
		_nativeBrush = brush;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!(_nativeBrush != IntPtr.Zero))
		{
			return;
		}
		try
		{
			GDIPlus.GdipDeleteBrush(new HandleRef(this, _nativeBrush));
		}
		catch (Exception ex) when (!ClientUtils.IsSecurityOrCriticalException(ex))
		{
		}
		finally
		{
			_nativeBrush = IntPtr.Zero;
		}
	}

	~Brush()
	{
		Dispose(disposing: false);
	}
}
