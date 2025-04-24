using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing;

[Serializable]
[ComVisible(true)]
[Editor("System.Drawing.Design.BitmapEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
public sealed class Bitmap : Image
{
	private Bitmap()
	{
	}

	internal Bitmap(IntPtr ptr)
	{
		nativeObject = ptr;
	}

	internal Bitmap(IntPtr ptr, Stream stream)
	{
		if (GDIPlus.RunningOnWindows())
		{
			base.stream = stream;
		}
		nativeObject = ptr;
	}

	public Bitmap(int width, int height)
		: this(width, height, PixelFormat.Format32bppArgb)
	{
	}

	public Bitmap(int width, int height, Graphics g)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromGraphics(width, height, g.nativeObject, out var bitmap));
		nativeObject = bitmap;
	}

	public Bitmap(int width, int height, PixelFormat format)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromScan0(width, height, 0, format, IntPtr.Zero, out var bmp));
		nativeObject = bmp;
	}

	public Bitmap(Image original)
		: this(original, original.Width, original.Height)
	{
	}

	public Bitmap(Stream stream)
		: this(stream, useIcm: false)
	{
	}

	public Bitmap(string filename)
		: this(filename, useIcm: false)
	{
	}

	public Bitmap(Image original, Size newSize)
		: this(original, newSize.Width, newSize.Height)
	{
	}

	public Bitmap(Stream stream, bool useIcm)
	{
		nativeObject = Image.InitFromStream(stream);
	}

	public Bitmap(string filename, bool useIcm)
	{
		if (filename == null)
		{
			throw new ArgumentNullException("filename");
		}
		GDIPlus.CheckStatus((!useIcm) ? GDIPlus.GdipCreateBitmapFromFile(filename, out var bitmap) : GDIPlus.GdipCreateBitmapFromFileICM(filename, out bitmap));
		nativeObject = bitmap;
	}

	public Bitmap(Type type, string resource)
	{
		if (resource == null)
		{
			throw new ArgumentException("resource");
		}
		if (type == null)
		{
			throw new NullReferenceException();
		}
		Stream manifestResourceStream = type.GetTypeInfo().Assembly.GetManifestResourceStream(type, resource);
		if (manifestResourceStream == null)
		{
			throw new FileNotFoundException(global::Locale.GetText("Resource '{0}' was not found.", resource));
		}
		nativeObject = Image.InitFromStream(manifestResourceStream);
		if (GDIPlus.RunningOnWindows())
		{
			stream = manifestResourceStream;
		}
	}

	public Bitmap(Image original, int width, int height)
		: this(width, height, PixelFormat.Format32bppArgb)
	{
		Graphics graphics = Graphics.FromImage(this);
		graphics.DrawImage(original, 0, 0, width, height);
		graphics.Dispose();
	}

	public Bitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromScan0(width, height, stride, format, scan0, out var bmp));
		nativeObject = bmp;
	}

	private Bitmap(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public Color GetPixel(int x, int y)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipBitmapGetPixel(nativeObject, x, y, out var argb));
		return Color.FromArgb(argb);
	}

	public void SetPixel(int x, int y, Color color)
	{
		Status num = GDIPlus.GdipBitmapSetPixel(nativeObject, x, y, color.ToArgb());
		if (num == Status.InvalidParameter && (base.PixelFormat & PixelFormat.Indexed) != PixelFormat.Undefined)
		{
			throw new InvalidOperationException(global::Locale.GetText("SetPixel cannot be called on indexed bitmaps."));
		}
		GDIPlus.CheckStatus(num);
	}

	public Bitmap Clone(Rectangle rect, PixelFormat format)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCloneBitmapAreaI(rect.X, rect.Y, rect.Width, rect.Height, format, nativeObject, out var bitmap));
		return new Bitmap(bitmap);
	}

	public Bitmap Clone(RectangleF rect, PixelFormat format)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCloneBitmapArea(rect.X, rect.Y, rect.Width, rect.Height, format, nativeObject, out var bitmap));
		return new Bitmap(bitmap);
	}

	public static Bitmap FromHicon(IntPtr hicon)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromHICON(hicon, out var bitmap));
		return new Bitmap(bitmap);
	}

	public static Bitmap FromResource(IntPtr hinstance, string bitmapName)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromResource(hinstance, bitmapName, out var bitmap));
		return new Bitmap(bitmap);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IntPtr GetHbitmap()
	{
		return GetHbitmap(Color.Gray);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IntPtr GetHbitmap(Color background)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateHBITMAPFromBitmap(nativeObject, out var HandleBmp, background.ToArgb()));
		return HandleBmp;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IntPtr GetHicon()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateHICONFromBitmap(nativeObject, out var HandleIcon));
		return HandleIcon;
	}

	public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
	{
		BitmapData bitmapData = new BitmapData();
		return LockBits(rect, flags, format, bitmapData);
	}

	public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipBitmapLockBits(nativeObject, ref rect, flags, format, bitmapData));
		return bitmapData;
	}

	public void MakeTransparent()
	{
		Color pixel = GetPixel(0, 0);
		MakeTransparent(pixel);
	}

	public void MakeTransparent(Color transparentColor)
	{
		Bitmap bitmap = new Bitmap(base.Width, base.Height, PixelFormat.Format32bppArgb);
		Graphics graphics = Graphics.FromImage(bitmap);
		Rectangle destRect = new Rectangle(0, 0, base.Width, base.Height);
		ImageAttributes imageAttributes = new ImageAttributes();
		imageAttributes.SetColorKey(transparentColor, transparentColor);
		graphics.DrawImage(this, destRect, 0, 0, base.Width, base.Height, GraphicsUnit.Pixel, imageAttributes);
		IntPtr intPtr = nativeObject;
		nativeObject = bitmap.nativeObject;
		bitmap.nativeObject = intPtr;
		graphics.Dispose();
		bitmap.Dispose();
		imageAttributes.Dispose();
	}

	public void SetResolution(float xDpi, float yDpi)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipBitmapSetResolution(nativeObject, xDpi, yDpi));
	}

	public void UnlockBits(BitmapData bitmapdata)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipBitmapUnlockBits(nativeObject, bitmapdata));
	}
}
