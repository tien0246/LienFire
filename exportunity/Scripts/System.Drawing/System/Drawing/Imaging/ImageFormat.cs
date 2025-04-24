using System.ComponentModel;

namespace System.Drawing.Imaging;

[TypeConverter(typeof(ImageFormatConverter))]
public sealed class ImageFormat
{
	private Guid guid;

	private string name;

	private const string BmpGuid = "b96b3cab-0728-11d3-9d7b-0000f81ef32e";

	private const string EmfGuid = "b96b3cac-0728-11d3-9d7b-0000f81ef32e";

	private const string ExifGuid = "b96b3cb2-0728-11d3-9d7b-0000f81ef32e";

	private const string GifGuid = "b96b3cb0-0728-11d3-9d7b-0000f81ef32e";

	private const string TiffGuid = "b96b3cb1-0728-11d3-9d7b-0000f81ef32e";

	private const string PngGuid = "b96b3caf-0728-11d3-9d7b-0000f81ef32e";

	private const string MemoryBmpGuid = "b96b3caa-0728-11d3-9d7b-0000f81ef32e";

	private const string IconGuid = "b96b3cb5-0728-11d3-9d7b-0000f81ef32e";

	private const string JpegGuid = "b96b3cae-0728-11d3-9d7b-0000f81ef32e";

	private const string WmfGuid = "b96b3cad-0728-11d3-9d7b-0000f81ef32e";

	private static object locker = new object();

	private static ImageFormat BmpImageFormat;

	private static ImageFormat EmfImageFormat;

	private static ImageFormat ExifImageFormat;

	private static ImageFormat GifImageFormat;

	private static ImageFormat TiffImageFormat;

	private static ImageFormat PngImageFormat;

	private static ImageFormat MemoryBmpImageFormat;

	private static ImageFormat IconImageFormat;

	private static ImageFormat JpegImageFormat;

	private static ImageFormat WmfImageFormat;

	public Guid Guid => guid;

	public static ImageFormat Bmp
	{
		get
		{
			lock (locker)
			{
				if (BmpImageFormat == null)
				{
					BmpImageFormat = new ImageFormat("Bmp", "b96b3cab-0728-11d3-9d7b-0000f81ef32e");
				}
				return BmpImageFormat;
			}
		}
	}

	public static ImageFormat Emf
	{
		get
		{
			lock (locker)
			{
				if (EmfImageFormat == null)
				{
					EmfImageFormat = new ImageFormat("Emf", "b96b3cac-0728-11d3-9d7b-0000f81ef32e");
				}
				return EmfImageFormat;
			}
		}
	}

	public static ImageFormat Exif
	{
		get
		{
			lock (locker)
			{
				if (ExifImageFormat == null)
				{
					ExifImageFormat = new ImageFormat("Exif", "b96b3cb2-0728-11d3-9d7b-0000f81ef32e");
				}
				return ExifImageFormat;
			}
		}
	}

	public static ImageFormat Gif
	{
		get
		{
			lock (locker)
			{
				if (GifImageFormat == null)
				{
					GifImageFormat = new ImageFormat("Gif", "b96b3cb0-0728-11d3-9d7b-0000f81ef32e");
				}
				return GifImageFormat;
			}
		}
	}

	public static ImageFormat Icon
	{
		get
		{
			lock (locker)
			{
				if (IconImageFormat == null)
				{
					IconImageFormat = new ImageFormat("Icon", "b96b3cb5-0728-11d3-9d7b-0000f81ef32e");
				}
				return IconImageFormat;
			}
		}
	}

	public static ImageFormat Jpeg
	{
		get
		{
			lock (locker)
			{
				if (JpegImageFormat == null)
				{
					JpegImageFormat = new ImageFormat("Jpeg", "b96b3cae-0728-11d3-9d7b-0000f81ef32e");
				}
				return JpegImageFormat;
			}
		}
	}

	public static ImageFormat MemoryBmp
	{
		get
		{
			lock (locker)
			{
				if (MemoryBmpImageFormat == null)
				{
					MemoryBmpImageFormat = new ImageFormat("MemoryBMP", "b96b3caa-0728-11d3-9d7b-0000f81ef32e");
				}
				return MemoryBmpImageFormat;
			}
		}
	}

	public static ImageFormat Png
	{
		get
		{
			lock (locker)
			{
				if (PngImageFormat == null)
				{
					PngImageFormat = new ImageFormat("Png", "b96b3caf-0728-11d3-9d7b-0000f81ef32e");
				}
				return PngImageFormat;
			}
		}
	}

	public static ImageFormat Tiff
	{
		get
		{
			lock (locker)
			{
				if (TiffImageFormat == null)
				{
					TiffImageFormat = new ImageFormat("Tiff", "b96b3cb1-0728-11d3-9d7b-0000f81ef32e");
				}
				return TiffImageFormat;
			}
		}
	}

	public static ImageFormat Wmf
	{
		get
		{
			lock (locker)
			{
				if (WmfImageFormat == null)
				{
					WmfImageFormat = new ImageFormat("Wmf", "b96b3cad-0728-11d3-9d7b-0000f81ef32e");
				}
				return WmfImageFormat;
			}
		}
	}

	public ImageFormat(Guid guid)
	{
		this.guid = guid;
	}

	private ImageFormat(string name, string guid)
	{
		this.name = name;
		this.guid = new Guid(guid);
	}

	public override bool Equals(object o)
	{
		if (!(o is ImageFormat { Guid: var guid }))
		{
			return false;
		}
		return guid.Equals(this.guid);
	}

	public override int GetHashCode()
	{
		return guid.GetHashCode();
	}

	public override string ToString()
	{
		if (name != null)
		{
			return name;
		}
		return "[ImageFormat: " + guid.ToString() + "]";
	}
}
