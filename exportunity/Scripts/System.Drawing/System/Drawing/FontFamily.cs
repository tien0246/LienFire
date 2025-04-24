using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace System.Drawing;

public sealed class FontFamily : MarshalByRefObject, IDisposable
{
	private string name;

	private IntPtr nativeFontFamily = IntPtr.Zero;

	internal IntPtr NativeObject => nativeFontFamily;

	internal IntPtr NativeFamily => nativeFontFamily;

	public string Name
	{
		get
		{
			if (nativeFontFamily == IntPtr.Zero)
			{
				throw new ArgumentException("Name", global::Locale.GetText("Object was disposed."));
			}
			if (name == null)
			{
				refreshName();
			}
			return name;
		}
	}

	public static FontFamily GenericMonospace => new FontFamily(GenericFontFamilies.Monospace);

	public static FontFamily GenericSansSerif => new FontFamily(GenericFontFamilies.SansSerif);

	public static FontFamily GenericSerif => new FontFamily(GenericFontFamilies.Serif);

	public static FontFamily[] Families => new InstalledFontCollection().Families;

	internal FontFamily(IntPtr fntfamily)
	{
		nativeFontFamily = fntfamily;
	}

	internal unsafe void refreshName()
	{
		if (!(nativeFontFamily == IntPtr.Zero))
		{
			char* ptr = stackalloc char[32];
			GDIPlus.CheckStatus(GDIPlus.GdipGetFamilyName(nativeFontFamily, (IntPtr)ptr, 0));
			name = Marshal.PtrToStringUni((IntPtr)ptr);
		}
	}

	~FontFamily()
	{
		Dispose();
	}

	public FontFamily(GenericFontFamilies genericFamily)
	{
		GDIPlus.CheckStatus(genericFamily switch
		{
			GenericFontFamilies.SansSerif => GDIPlus.GdipGetGenericFontFamilySansSerif(out nativeFontFamily), 
			GenericFontFamilies.Serif => GDIPlus.GdipGetGenericFontFamilySerif(out nativeFontFamily), 
			_ => GDIPlus.GdipGetGenericFontFamilyMonospace(out nativeFontFamily), 
		});
	}

	public FontFamily(string name)
		: this(name, null)
	{
	}

	public FontFamily(string name, FontCollection fontCollection)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateFontFamilyFromName(name, fontCollection?._nativeFontCollection ?? IntPtr.Zero, out nativeFontFamily));
	}

	public int GetCellAscent(FontStyle style)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetCellAscent(nativeFontFamily, (int)style, out var ascent));
		return ascent;
	}

	public int GetCellDescent(FontStyle style)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetCellDescent(nativeFontFamily, (int)style, out var descent));
		return descent;
	}

	public int GetEmHeight(FontStyle style)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetEmHeight(nativeFontFamily, (int)style, out var emHeight));
		return emHeight;
	}

	public int GetLineSpacing(FontStyle style)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetLineSpacing(nativeFontFamily, (int)style, out var spacing));
		return spacing;
	}

	[System.MonoDocumentationNote("When used with libgdiplus this method always return true (styles are created on demand).")]
	public bool IsStyleAvailable(FontStyle style)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipIsStyleAvailable(nativeFontFamily, (int)style, out var styleAvailable));
		return styleAvailable;
	}

	public void Dispose()
	{
		if (nativeFontFamily != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeleteFontFamily(nativeFontFamily);
			nativeFontFamily = IntPtr.Zero;
			GC.SuppressFinalize(this);
			GDIPlus.CheckStatus(status);
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is FontFamily fontFamily))
		{
			return false;
		}
		return Name == fontFamily.Name;
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}

	public static FontFamily[] GetFamilies(Graphics graphics)
	{
		if (graphics == null)
		{
			throw new ArgumentNullException("graphics");
		}
		return new InstalledFontCollection().Families;
	}

	[System.MonoLimitation("The language parameter is ignored. We always return the name using the default system language.")]
	public string GetName(int language)
	{
		return Name;
	}

	public override string ToString()
	{
		return "[FontFamily: Name=" + Name + "]";
	}
}
