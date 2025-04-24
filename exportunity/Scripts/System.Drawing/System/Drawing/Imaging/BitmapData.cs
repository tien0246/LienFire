using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
public sealed class BitmapData
{
	private int width;

	private int height;

	private int stride;

	private PixelFormat pixel_format;

	private IntPtr scan0;

	private int reserved;

	private IntPtr palette;

	private int property_count;

	private IntPtr property;

	private float dpi_horz;

	private float dpi_vert;

	private int image_flags;

	private int left;

	private int top;

	private int x;

	private int y;

	private int transparent;

	public int Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	public int Width
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
		}
	}

	public PixelFormat PixelFormat
	{
		get
		{
			return pixel_format;
		}
		set
		{
			pixel_format = value;
		}
	}

	public int Reserved
	{
		get
		{
			return reserved;
		}
		set
		{
			reserved = value;
		}
	}

	public IntPtr Scan0
	{
		get
		{
			return scan0;
		}
		set
		{
			scan0 = value;
		}
	}

	public int Stride
	{
		get
		{
			return stride;
		}
		set
		{
			stride = value;
		}
	}
}
