using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[Serializable]
[Editor("System.Drawing.Design.MetafileEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
[System.MonoTODO("Metafiles, both WMF and EMF formats, are only partially supported.")]
public sealed class Metafile : Image
{
	internal sealed class MetafileHolder : IDisposable
	{
		private bool _disposed;

		private IntPtr _nativeImage;

		internal bool Disposed => _disposed;

		internal MetafileHolder()
		{
			_disposed = false;
			_nativeImage = IntPtr.Zero;
		}

		~MetafileHolder()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		internal void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				IntPtr nativeImage = _nativeImage;
				_nativeImage = IntPtr.Zero;
				_disposed = true;
				if (nativeImage != IntPtr.Zero)
				{
					GDIPlus.CheckStatus(GDIPlus.GdipDisposeImage(nativeImage));
				}
			}
		}

		internal void MetafileDisposed(IntPtr nativeImage)
		{
			_nativeImage = nativeImage;
		}

		internal void GraphicsDisposed()
		{
			Dispose();
		}
	}

	private MetafileHolder _metafileHolder;

	internal MetafileHolder AddMetafileHolder()
	{
		if (_metafileHolder != null && !_metafileHolder.Disposed)
		{
			return null;
		}
		_metafileHolder = new MetafileHolder();
		return _metafileHolder;
	}

	internal Metafile(IntPtr ptr)
	{
		nativeObject = ptr;
	}

	internal Metafile(IntPtr ptr, Stream stream)
	{
		if (GDIPlus.RunningOnWindows())
		{
			base.stream = stream;
		}
		nativeObject = ptr;
	}

	public Metafile(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentException("stream");
		}
		Status status;
		if (GDIPlus.RunningOnUnix())
		{
			GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, seekToOrigin: false);
			status = GDIPlus.GdipCreateMetafileFromDelegate_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, out nativeObject);
		}
		else
		{
			status = GDIPlus.GdipCreateMetafileFromStream(new ComIStreamWrapper(stream), out nativeObject);
		}
		GDIPlus.CheckStatus(status);
	}

	public Metafile(string filename)
	{
		if (filename == null)
		{
			throw new ArgumentNullException("filename");
		}
		if (filename.Length == 0)
		{
			throw new ArgumentException("filename");
		}
		Status num = GDIPlus.GdipCreateMetafileFromFile(filename, out nativeObject);
		if (num == Status.GenericError)
		{
			throw new ExternalException("Couldn't load specified file.");
		}
		GDIPlus.CheckStatus(num);
	}

	public Metafile(IntPtr henhmetafile, bool deleteEmf)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateMetafileFromEmf(henhmetafile, deleteEmf, out nativeObject));
	}

	public Metafile(IntPtr referenceHdc, EmfType emfType)
		: this(referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, emfType, null)
	{
	}

	public Metafile(IntPtr referenceHdc, Rectangle frameRect)
		: this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(IntPtr referenceHdc, RectangleF frameRect)
		: this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateMetafileFromEmf(hmetafile, deleteEmf: false, out nativeObject));
	}

	public Metafile(Stream stream, IntPtr referenceHdc)
		: this(stream, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc)
		: this(fileName, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(IntPtr referenceHdc, EmfType emfType, string description)
		: this(referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, emfType, description)
	{
	}

	public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit)
		: this(referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit)
		: this(referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader, bool deleteWmf)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateMetafileFromEmf(hmetafile, deleteWmf, out nativeObject));
	}

	public Metafile(Stream stream, IntPtr referenceHdc, EmfType type)
		: this(stream, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, type, null)
	{
	}

	public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect)
		: this(stream, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect)
		: this(stream, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc, EmfType type)
		: this(fileName, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, type, null)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect)
		: this(fileName, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect)
		: this(fileName, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type)
		: this(referenceHdc, frameRect, frameUnit, type, null)
	{
	}

	public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type)
		: this(referenceHdc, frameRect, frameUnit, type, null)
	{
	}

	public Metafile(Stream stream, IntPtr referenceHdc, EmfType type, string description)
		: this(stream, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, type, description)
	{
	}

	public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit)
		: this(stream, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit)
		: this(stream, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc, EmfType type, string description)
		: this(fileName, referenceHdc, default(RectangleF), MetafileFrameUnit.GdiCompatible, type, description)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit)
		: this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit)
		: this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
	{
	}

	public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string desc)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipRecordMetafileI(referenceHdc, type, ref frameRect, frameUnit, desc, out nativeObject));
	}

	public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipRecordMetafile(referenceHdc, type, ref frameRect, frameUnit, description, out nativeObject));
	}

	public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type)
		: this(stream, referenceHdc, frameRect, frameUnit, type, null)
	{
	}

	public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type)
		: this(stream, referenceHdc, frameRect, frameUnit, type, null)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type)
		: this(fileName, referenceHdc, frameRect, frameUnit, type, null)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, string description)
		: this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, description)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type)
		: this(fileName, referenceHdc, frameRect, frameUnit, type, null)
	{
	}

	public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, string desc)
		: this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, desc)
	{
	}

	public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
	{
		if (stream == null)
		{
			throw new NullReferenceException("stream");
		}
		Status status = Status.NotImplemented;
		if (GDIPlus.RunningOnUnix())
		{
			GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, seekToOrigin: false);
			status = GDIPlus.GdipRecordMetafileFromDelegateI_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, referenceHdc, type, ref frameRect, frameUnit, description, out nativeObject);
		}
		else
		{
			status = GDIPlus.GdipRecordMetafileStreamI(new ComIStreamWrapper(stream), referenceHdc, type, ref frameRect, frameUnit, description, out nativeObject);
		}
		GDIPlus.CheckStatus(status);
	}

	public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
	{
		if (stream == null)
		{
			throw new NullReferenceException("stream");
		}
		Status status = Status.NotImplemented;
		if (GDIPlus.RunningOnUnix())
		{
			GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, seekToOrigin: false);
			status = GDIPlus.GdipRecordMetafileFromDelegate_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, referenceHdc, type, ref frameRect, frameUnit, description, out nativeObject);
		}
		else
		{
			status = GDIPlus.GdipRecordMetafileStream(new ComIStreamWrapper(stream), referenceHdc, type, ref frameRect, frameUnit, description, out nativeObject);
		}
		GDIPlus.CheckStatus(status);
	}

	public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipRecordMetafileFileNameI(fileName, referenceHdc, type, ref frameRect, frameUnit, description, out nativeObject));
	}

	public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipRecordMetafileFileName(fileName, referenceHdc, type, ref frameRect, frameUnit, description, out nativeObject));
	}

	protected override void Dispose(bool disposing)
	{
		if (_metafileHolder != null && !_metafileHolder.Disposed)
		{
			_metafileHolder.MetafileDisposed(nativeObject);
			_metafileHolder = null;
			nativeObject = IntPtr.Zero;
		}
		base.Dispose(disposing);
	}

	public IntPtr GetHenhmetafile()
	{
		return nativeObject;
	}

	[System.MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
	public MetafileHeader GetMetafileHeader()
	{
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
		try
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetMetafileHeaderFromMetafile(nativeObject, intPtr));
			return new MetafileHeader(intPtr);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	[System.MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
	public static MetafileHeader GetMetafileHeader(IntPtr henhmetafile)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
		try
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetMetafileHeaderFromEmf(henhmetafile, intPtr));
			return new MetafileHeader(intPtr);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	[System.MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
	public static MetafileHeader GetMetafileHeader(Stream stream)
	{
		if (stream == null)
		{
			throw new NullReferenceException("stream");
		}
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
		try
		{
			Status status;
			if (GDIPlus.RunningOnUnix())
			{
				GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, seekToOrigin: false);
				status = GDIPlus.GdipGetMetafileHeaderFromDelegate_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, intPtr);
			}
			else
			{
				status = GDIPlus.GdipGetMetafileHeaderFromStream(new ComIStreamWrapper(stream), intPtr);
			}
			GDIPlus.CheckStatus(status);
			return new MetafileHeader(intPtr);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	[System.MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
	public static MetafileHeader GetMetafileHeader(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
		try
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetMetafileHeaderFromFile(fileName, intPtr));
			return new MetafileHeader(intPtr);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	[System.MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
	public static MetafileHeader GetMetafileHeader(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
		try
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetMetafileHeaderFromEmf(hmetafile, intPtr));
			return new MetafileHeader(intPtr);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	[System.MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
	public void PlayRecord(EmfPlusRecordType recordType, int flags, int dataSize, byte[] data)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipPlayMetafileRecord(nativeObject, recordType, flags, dataSize, data));
	}
}
