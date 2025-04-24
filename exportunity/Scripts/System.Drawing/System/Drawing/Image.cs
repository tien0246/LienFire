using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing;

[Serializable]
[ImmutableObject(true)]
[TypeConverter(typeof(ImageConverter))]
[Editor("System.Drawing.Design.ImageEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
[ComVisible(true)]
public abstract class Image : MarshalByRefObject, IDisposable, ICloneable, ISerializable
{
	public delegate bool GetThumbnailImageAbort();

	private object tag;

	internal IntPtr nativeObject = IntPtr.Zero;

	internal Stream stream;

	[Browsable(false)]
	public int Flags
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageFlags(nativeObject, out var flag));
			return flag;
		}
	}

	[Browsable(false)]
	public Guid[] FrameDimensionsList
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipImageGetFrameDimensionsCount(nativeObject, out var count));
			Guid[] array = new Guid[count];
			GDIPlus.CheckStatus(GDIPlus.GdipImageGetFrameDimensionsList(nativeObject, array, count));
			return array;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[DefaultValue(false)]
	public int Height
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageHeight(nativeObject, out var height));
			return (int)height;
		}
	}

	public float HorizontalResolution
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageHorizontalResolution(nativeObject, out var resolution));
			return resolution;
		}
	}

	[Browsable(false)]
	public ColorPalette Palette
	{
		get
		{
			return retrieveGDIPalette();
		}
		set
		{
			storeGDIPalette(value);
		}
	}

	public SizeF PhysicalDimension
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageDimension(nativeObject, out var width, out var height));
			return new SizeF(width, height);
		}
	}

	public PixelFormat PixelFormat
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetImagePixelFormat(nativeObject, out var format));
			return format;
		}
	}

	[Browsable(false)]
	public int[] PropertyIdList
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPropertyCount(nativeObject, out var propNumbers));
			int[] array = new int[propNumbers];
			GDIPlus.CheckStatus(GDIPlus.GdipGetPropertyIdList(nativeObject, propNumbers, array));
			return array;
		}
	}

	[Browsable(false)]
	public PropertyItem[] PropertyItems
	{
		get
		{
			GdipPropertyItem structure = default(GdipPropertyItem);
			GDIPlus.CheckStatus(GDIPlus.GdipGetPropertySize(nativeObject, out var bufferSize, out var propNumbers));
			PropertyItem[] array = new PropertyItem[propNumbers];
			if (propNumbers == 0)
			{
				return array;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(bufferSize * propNumbers);
			try
			{
				GDIPlus.CheckStatus(GDIPlus.GdipGetAllPropertyItems(nativeObject, bufferSize, propNumbers, intPtr));
				int num = Marshal.SizeOf(structure);
				IntPtr ptr = intPtr;
				int num2 = 0;
				while (num2 < propNumbers)
				{
					structure = (GdipPropertyItem)Marshal.PtrToStructure(ptr, typeof(GdipPropertyItem));
					array[num2] = new PropertyItem();
					GdipPropertyItem.MarshalTo(structure, array[num2]);
					num2++;
					ptr = new IntPtr(ptr.ToInt64() + num);
				}
				return array;
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public ImageFormat RawFormat
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageRawFormat(nativeObject, out var format));
			return new ImageFormat(format);
		}
	}

	public Size Size => new Size(Width, Height);

	[DefaultValue(null)]
	[Localizable(false)]
	[Bindable(true)]
	[TypeConverter(typeof(StringConverter))]
	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	public float VerticalResolution
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageVerticalResolution(nativeObject, out var resolution));
			return resolution;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DefaultValue(false)]
	public int Width
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetImageWidth(nativeObject, out var width));
			return (int)width;
		}
	}

	internal IntPtr NativeObject
	{
		get
		{
			return nativeObject;
		}
		set
		{
			nativeObject = value;
		}
	}

	internal IntPtr nativeImage => nativeObject;

	internal Image()
	{
	}

	internal Image(SerializationInfo info, StreamingContext context)
	{
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SerializationEntry current = enumerator.Current;
			if (string.Compare(current.Name, "Data", ignoreCase: true) != 0)
			{
				continue;
			}
			byte[] array = (byte[])current.Value;
			if (array != null)
			{
				MemoryStream memoryStream = new MemoryStream(array);
				nativeObject = InitFromStream(memoryStream);
				if (GDIPlus.RunningOnWindows())
				{
					stream = memoryStream;
				}
			}
		}
	}

	void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
	{
		using MemoryStream memoryStream = new MemoryStream();
		if (RawFormat.Equals(ImageFormat.Icon))
		{
			Save(memoryStream, ImageFormat.Png);
		}
		else
		{
			Save(memoryStream, RawFormat);
		}
		si.AddValue("Data", memoryStream.ToArray());
	}

	public static Image FromFile(string filename)
	{
		return FromFile(filename, useEmbeddedColorManagement: false);
	}

	public static Image FromFile(string filename, bool useEmbeddedColorManagement)
	{
		if (!File.Exists(filename))
		{
			throw new FileNotFoundException(filename);
		}
		IntPtr image;
		Status status = ((!useEmbeddedColorManagement) ? GDIPlus.GdipLoadImageFromFile(filename, out image) : GDIPlus.GdipLoadImageFromFileICM(filename, out image));
		GDIPlus.CheckStatus(status);
		return CreateFromHandle(image);
	}

	public static Bitmap FromHbitmap(IntPtr hbitmap)
	{
		return FromHbitmap(hbitmap, IntPtr.Zero);
	}

	public static Bitmap FromHbitmap(IntPtr hbitmap, IntPtr hpalette)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateBitmapFromHBITMAP(hbitmap, hpalette, out var image));
		return new Bitmap(image);
	}

	public static Image FromStream(Stream stream)
	{
		return LoadFromStream(stream, keepAlive: false);
	}

	[System.MonoLimitation("useEmbeddedColorManagement  isn't supported.")]
	public static Image FromStream(Stream stream, bool useEmbeddedColorManagement)
	{
		return LoadFromStream(stream, keepAlive: false);
	}

	[System.MonoLimitation("useEmbeddedColorManagement  and validateImageData aren't supported.")]
	public static Image FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
	{
		return LoadFromStream(stream, keepAlive: false);
	}

	internal static Image LoadFromStream(Stream stream, bool keepAlive)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		Image image = CreateFromHandle(InitFromStream(stream));
		if (keepAlive && GDIPlus.RunningOnWindows())
		{
			image.stream = stream;
		}
		return image;
	}

	internal static Image CreateImageObject(IntPtr nativeImage)
	{
		return CreateFromHandle(nativeImage);
	}

	internal static Image CreateFromHandle(IntPtr handle)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetImageType(handle, out var type));
		return type switch
		{
			ImageType.Bitmap => new Bitmap(handle), 
			ImageType.Metafile => new Metafile(handle), 
			_ => throw new NotSupportedException(global::Locale.GetText("Unknown image type.")), 
		};
	}

	public static int GetPixelFormatSize(PixelFormat pixfmt)
	{
		int result = 0;
		switch (pixfmt)
		{
		case PixelFormat.Format16bppRgb555:
		case PixelFormat.Format16bppRgb565:
		case PixelFormat.Format16bppArgb1555:
		case PixelFormat.Format16bppGrayScale:
			result = 16;
			break;
		case PixelFormat.Format1bppIndexed:
			result = 1;
			break;
		case PixelFormat.Format24bppRgb:
			result = 24;
			break;
		case PixelFormat.Format32bppRgb:
		case PixelFormat.Format32bppPArgb:
		case PixelFormat.Format32bppArgb:
			result = 32;
			break;
		case PixelFormat.Format48bppRgb:
			result = 48;
			break;
		case PixelFormat.Format4bppIndexed:
			result = 4;
			break;
		case PixelFormat.Format64bppPArgb:
		case PixelFormat.Format64bppArgb:
			result = 64;
			break;
		case PixelFormat.Format8bppIndexed:
			result = 8;
			break;
		}
		return result;
	}

	public static bool IsAlphaPixelFormat(PixelFormat pixfmt)
	{
		bool result = false;
		switch (pixfmt)
		{
		case PixelFormat.Format16bppArgb1555:
		case PixelFormat.Format32bppPArgb:
		case PixelFormat.Format64bppPArgb:
		case PixelFormat.Format32bppArgb:
		case PixelFormat.Format64bppArgb:
			result = true;
			break;
		case PixelFormat.Format16bppRgb555:
		case PixelFormat.Format16bppRgb565:
		case PixelFormat.Format24bppRgb:
		case PixelFormat.Format32bppRgb:
		case PixelFormat.Format1bppIndexed:
		case PixelFormat.Format4bppIndexed:
		case PixelFormat.Format8bppIndexed:
		case PixelFormat.Format16bppGrayScale:
		case PixelFormat.Format48bppRgb:
			result = false;
			break;
		}
		return result;
	}

	public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
	{
		return (pixfmt & PixelFormat.Canonical) != 0;
	}

	public static bool IsExtendedPixelFormat(PixelFormat pixfmt)
	{
		return (pixfmt & PixelFormat.Extended) != 0;
	}

	internal static IntPtr InitFromStream(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentException("stream");
		}
		if (!stream.CanSeek)
		{
			byte[] array = new byte[256];
			int num = 0;
			int num2;
			do
			{
				if (array.Length < num + 256)
				{
					byte[] array2 = new byte[array.Length * 2];
					Array.Copy(array, array2, array.Length);
					array = array2;
				}
				num2 = stream.Read(array, num, 256);
				num += num2;
			}
			while (num2 != 0);
			stream = new MemoryStream(array, 0, num);
		}
		Status status;
		IntPtr image;
		if (GDIPlus.RunningOnUnix())
		{
			GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, seekToOrigin: true);
			status = GDIPlus.GdipLoadImageFromDelegate_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, out image);
		}
		else
		{
			status = GDIPlus.GdipLoadImageFromStream(new ComIStreamWrapper(stream), out image);
		}
		if (status != Status.Ok)
		{
			return IntPtr.Zero;
		}
		return image;
	}

	public RectangleF GetBounds(ref GraphicsUnit pageUnit)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetImageBounds(nativeObject, out var source, ref pageUnit));
		return source;
	}

	public EncoderParameters GetEncoderParameterList(Guid encoder)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetEncoderParameterListSize(nativeObject, ref encoder, out var size));
		IntPtr intPtr = Marshal.AllocHGlobal((int)size);
		try
		{
			Status status = GDIPlus.GdipGetEncoderParameterList(nativeObject, ref encoder, size, intPtr);
			EncoderParameters result = EncoderParameters.ConvertFromMemory(intPtr);
			GDIPlus.CheckStatus(status);
			return result;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public int GetFrameCount(FrameDimension dimension)
	{
		Guid guidDimension = dimension.Guid;
		GDIPlus.CheckStatus(GDIPlus.GdipImageGetFrameCount(nativeObject, ref guidDimension, out var count));
		return (int)count;
	}

	public PropertyItem GetPropertyItem(int propid)
	{
		PropertyItem propertyItem = new PropertyItem();
		GDIPlus.CheckStatus(GDIPlus.GdipGetPropertyItemSize(nativeObject, propid, out var propertySize));
		IntPtr intPtr = Marshal.AllocHGlobal(propertySize);
		try
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPropertyItem(nativeObject, propid, propertySize, intPtr));
			GdipPropertyItem.MarshalTo((GdipPropertyItem)Marshal.PtrToStructure(intPtr, typeof(GdipPropertyItem)), propertyItem);
			return propertyItem;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public Image GetThumbnailImage(int thumbWidth, int thumbHeight, GetThumbnailImageAbort callback, IntPtr callbackData)
	{
		if (thumbWidth <= 0 || thumbHeight <= 0)
		{
			throw new OutOfMemoryException("Invalid thumbnail size");
		}
		Image image = new Bitmap(thumbWidth, thumbHeight);
		using Graphics graphics = Graphics.FromImage(image);
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRectI(graphics.nativeObject, nativeObject, 0, 0, thumbWidth, thumbHeight, 0, 0, Width, Height, GraphicsUnit.Pixel, IntPtr.Zero, null, IntPtr.Zero));
		return image;
	}

	public void RemovePropertyItem(int propid)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipRemovePropertyItem(nativeObject, propid));
	}

	public void RotateFlip(RotateFlipType rotateFlipType)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipImageRotateFlip(nativeObject, rotateFlipType));
	}

	internal ImageCodecInfo findEncoderForFormat(ImageFormat format)
	{
		ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
		ImageCodecInfo result = null;
		if (format.Guid.Equals(ImageFormat.MemoryBmp.Guid))
		{
			format = ImageFormat.Png;
		}
		for (int i = 0; i < imageEncoders.Length; i++)
		{
			if (imageEncoders[i].FormatID.Equals(format.Guid))
			{
				result = imageEncoders[i];
				break;
			}
		}
		return result;
	}

	public void Save(string filename)
	{
		Save(filename, RawFormat);
	}

	public void Save(string filename, ImageFormat format)
	{
		ImageCodecInfo imageCodecInfo = findEncoderForFormat(format);
		if (imageCodecInfo == null)
		{
			imageCodecInfo = findEncoderForFormat(RawFormat);
			if (imageCodecInfo == null)
			{
				throw new ArgumentException(global::Locale.GetText("No codec available for saving format '{0}'.", format.Guid), "format");
			}
		}
		Save(filename, imageCodecInfo, null);
	}

	public void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams)
	{
		Guid encoderClsID = encoder.Clsid;
		Status status;
		if (encoderParams == null)
		{
			status = GDIPlus.GdipSaveImageToFile(nativeObject, filename, ref encoderClsID, IntPtr.Zero);
		}
		else
		{
			IntPtr intPtr = encoderParams.ConvertToMemory();
			status = GDIPlus.GdipSaveImageToFile(nativeObject, filename, ref encoderClsID, intPtr);
			Marshal.FreeHGlobal(intPtr);
		}
		GDIPlus.CheckStatus(status);
	}

	public void Save(Stream stream, ImageFormat format)
	{
		ImageCodecInfo imageCodecInfo = findEncoderForFormat(format);
		if (imageCodecInfo == null)
		{
			throw new ArgumentException("No codec available for format:" + format.Guid.ToString());
		}
		Save(stream, imageCodecInfo, null);
	}

	public void Save(Stream stream, ImageCodecInfo encoder, EncoderParameters encoderParams)
	{
		Guid clsidEncoder = encoder.Clsid;
		IntPtr intPtr = encoderParams?.ConvertToMemory() ?? IntPtr.Zero;
		Status status;
		try
		{
			if (GDIPlus.RunningOnUnix())
			{
				GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, seekToOrigin: false);
				status = GDIPlus.GdipSaveImageToDelegate_linux(nativeObject, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, ref clsidEncoder, intPtr);
			}
			else
			{
				status = GDIPlus.GdipSaveImageToStream(new HandleRef(this, nativeObject), new ComIStreamWrapper(stream), ref clsidEncoder, new HandleRef(encoderParams, intPtr));
			}
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
		GDIPlus.CheckStatus(status);
	}

	public void SaveAdd(EncoderParameters encoderParams)
	{
		IntPtr intPtr = encoderParams.ConvertToMemory();
		Status status = GDIPlus.GdipSaveAdd(nativeObject, intPtr);
		Marshal.FreeHGlobal(intPtr);
		GDIPlus.CheckStatus(status);
	}

	public void SaveAdd(Image image, EncoderParameters encoderParams)
	{
		IntPtr intPtr = encoderParams.ConvertToMemory();
		Status status = GDIPlus.GdipSaveAddImage(nativeObject, image.NativeObject, intPtr);
		Marshal.FreeHGlobal(intPtr);
		GDIPlus.CheckStatus(status);
	}

	public int SelectActiveFrame(FrameDimension dimension, int frameIndex)
	{
		Guid guidDimension = dimension.Guid;
		GDIPlus.CheckStatus(GDIPlus.GdipImageSelectActiveFrame(nativeObject, ref guidDimension, frameIndex));
		return frameIndex;
	}

	public unsafe void SetPropertyItem(PropertyItem propitem)
	{
		if (propitem == null)
		{
			throw new ArgumentNullException("propitem");
		}
		int num = Marshal.SizeOf(propitem.Value[0]) * propitem.Value.Length;
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		try
		{
			GdipPropertyItem gdipPropertyItem = new GdipPropertyItem
			{
				id = propitem.Id,
				len = propitem.Len,
				type = propitem.Type
			};
			Marshal.Copy(propitem.Value, 0, intPtr, num);
			gdipPropertyItem.value = intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipSetPropertyItem(nativeObject, &gdipPropertyItem));
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	internal ColorPalette retrieveGDIPalette()
	{
		ColorPalette colorPalette = new ColorPalette();
		GDIPlus.CheckStatus(GDIPlus.GdipGetImagePaletteSize(nativeObject, out var size));
		IntPtr intPtr = Marshal.AllocHGlobal(size);
		try
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetImagePalette(nativeObject, intPtr, size));
			colorPalette.ConvertFromMemory(intPtr);
			return colorPalette;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	internal void storeGDIPalette(ColorPalette palette)
	{
		if (palette == null)
		{
			throw new ArgumentNullException("palette");
		}
		IntPtr intPtr = palette.ConvertToMemory();
		if (intPtr == IntPtr.Zero)
		{
			return;
		}
		try
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetImagePalette(nativeObject, intPtr));
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~Image()
	{
		Dispose(disposing: false);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (GDIPlus.GdiPlusToken != 0L && nativeObject != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDisposeImage(nativeObject);
			if (stream != null)
			{
				stream.Dispose();
				stream = null;
			}
			nativeObject = IntPtr.Zero;
			GDIPlus.CheckStatus(status);
		}
	}

	public object Clone()
	{
		if (GDIPlus.RunningOnWindows() && stream != null)
		{
			return CloneFromStream();
		}
		IntPtr imageclone = IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipCloneImage(NativeObject, out imageclone));
		if (this is Bitmap)
		{
			return new Bitmap(imageclone);
		}
		return new Metafile(imageclone);
	}

	private object CloneFromStream()
	{
		MemoryStream memoryStream = new MemoryStream(new byte[stream.Length]);
		int num = (int)((stream.Length < 4096) ? stream.Length : 4096);
		byte[] buffer = new byte[num];
		stream.Position = 0L;
		do
		{
			num = stream.Read(buffer, 0, num);
			memoryStream.Write(buffer, 0, num);
		}
		while (num == 4096);
		IntPtr zero = IntPtr.Zero;
		zero = InitFromStream(memoryStream);
		if (this is Bitmap)
		{
			return new Bitmap(zero, memoryStream);
		}
		return new Metafile(zero, memoryStream);
	}
}
