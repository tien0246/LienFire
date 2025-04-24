namespace System.Drawing.Text;

public sealed class InstalledFontCollection : FontCollection
{
	public InstalledFontCollection()
	{
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipNewInstalledFontCollection(out _nativeFontCollection));
	}
}
