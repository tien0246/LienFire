using System.Runtime.InteropServices;

namespace System.Drawing.Text;

public abstract class FontCollection : IDisposable
{
	internal IntPtr _nativeFontCollection;

	public FontFamily[] Families
	{
		get
		{
			int found = 0;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetFontCollectionFamilyCount(new HandleRef(this, _nativeFontCollection), out found));
			IntPtr[] array = new IntPtr[found];
			int retCount = 0;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetFontCollectionFamilyList(new HandleRef(this, _nativeFontCollection), found, array, out retCount));
			FontFamily[] array2 = new FontFamily[retCount];
			for (int i = 0; i < retCount; i++)
			{
				GDIPlus.GdipCloneFontFamily(new HandleRef(null, array[i]), out var clone);
				array2[i] = new FontFamily(clone);
			}
			return array2;
		}
	}

	internal FontCollection()
	{
		_nativeFontCollection = IntPtr.Zero;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	~FontCollection()
	{
		Dispose(disposing: false);
	}
}
