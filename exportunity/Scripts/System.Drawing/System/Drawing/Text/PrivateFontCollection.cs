using System.IO;

namespace System.Drawing.Text;

public sealed class PrivateFontCollection : FontCollection
{
	public PrivateFontCollection()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipNewPrivateFontCollection(out _nativeFontCollection));
	}

	public void AddFontFile(string filename)
	{
		if (filename == null)
		{
			throw new ArgumentNullException("filename");
		}
		string fullPath = Path.GetFullPath(filename);
		if (!File.Exists(fullPath))
		{
			throw new FileNotFoundException();
		}
		GDIPlus.CheckStatus(GDIPlus.GdipPrivateAddFontFile(_nativeFontCollection, fullPath));
	}

	public void AddMemoryFont(IntPtr memory, int length)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipPrivateAddMemoryFont(_nativeFontCollection, memory, length));
	}

	protected override void Dispose(bool disposing)
	{
		if (_nativeFontCollection != IntPtr.Zero)
		{
			GDIPlus.GdipDeletePrivateFontCollection(ref _nativeFontCollection);
			_nativeFontCollection = IntPtr.Zero;
		}
		base.Dispose(disposing);
	}
}
