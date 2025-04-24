using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Unity;

namespace System.Drawing;

public sealed class Graphics : MarshalByRefObject, IDisposable, IDeviceContext
{
	public delegate bool EnumerateMetafileProc(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData);

	public delegate bool DrawImageAbort(IntPtr callbackdata);

	internal IntPtr nativeObject;

	internal IMacContext maccontext;

	private bool disposed;

	private static float defDpiX;

	private static float defDpiY;

	private IntPtr deviceContextHdc;

	private Metafile.MetafileHolder _metafileHolder;

	private const string MetafileEnumeration = "Metafiles enumeration, for both WMF and EMF formats, isn't supported.";

	internal static float systemDpiX
	{
		get
		{
			if (defDpiX == 0f)
			{
				Graphics graphics = FromImage(new Bitmap(1, 1));
				defDpiX = graphics.DpiX;
				defDpiY = graphics.DpiY;
			}
			return defDpiX;
		}
	}

	internal static float systemDpiY
	{
		get
		{
			if (defDpiY == 0f)
			{
				Graphics graphics = FromImage(new Bitmap(1, 1));
				defDpiX = graphics.DpiX;
				defDpiY = graphics.DpiY;
			}
			return defDpiY;
		}
	}

	internal IntPtr NativeGraphics => nativeObject;

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

	public Region Clip
	{
		get
		{
			Region region = new Region();
			GDIPlus.CheckStatus(GDIPlus.GdipGetClip(nativeObject, region.NativeObject));
			return region;
		}
		set
		{
			SetClip(value, CombineMode.Replace);
		}
	}

	public RectangleF ClipBounds
	{
		get
		{
			RectangleF rect = default(RectangleF);
			GDIPlus.CheckStatus(GDIPlus.GdipGetClipBounds(nativeObject, out rect));
			return rect;
		}
	}

	public CompositingMode CompositingMode
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetCompositingMode(nativeObject, out var compositingMode));
			return compositingMode;
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetCompositingMode(nativeObject, value));
		}
	}

	public CompositingQuality CompositingQuality
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetCompositingQuality(nativeObject, out var compositingQuality));
			return compositingQuality;
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetCompositingQuality(nativeObject, value));
		}
	}

	public float DpiX
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetDpiX(nativeObject, out var dpi));
			return dpi;
		}
	}

	public float DpiY
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetDpiY(nativeObject, out var dpi));
			return dpi;
		}
	}

	public InterpolationMode InterpolationMode
	{
		get
		{
			InterpolationMode interpolationMode = InterpolationMode.Invalid;
			GDIPlus.CheckStatus(GDIPlus.GdipGetInterpolationMode(nativeObject, out interpolationMode));
			return interpolationMode;
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetInterpolationMode(nativeObject, value));
		}
	}

	public bool IsClipEmpty
	{
		get
		{
			bool result = false;
			GDIPlus.CheckStatus(GDIPlus.GdipIsClipEmpty(nativeObject, out result));
			return result;
		}
	}

	public bool IsVisibleClipEmpty
	{
		get
		{
			bool result = false;
			GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleClipEmpty(nativeObject, out result));
			return result;
		}
	}

	public float PageScale
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPageScale(nativeObject, out var scale));
			return scale;
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetPageScale(nativeObject, value));
		}
	}

	public GraphicsUnit PageUnit
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPageUnit(nativeObject, out var unit));
			return unit;
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetPageUnit(nativeObject, value));
		}
	}

	[System.MonoTODO("This property does not do anything when used with libgdiplus.")]
	public PixelOffsetMode PixelOffsetMode
	{
		get
		{
			PixelOffsetMode pixelOffsetMode = PixelOffsetMode.Invalid;
			GDIPlus.CheckStatus(GDIPlus.GdipGetPixelOffsetMode(nativeObject, out pixelOffsetMode));
			return pixelOffsetMode;
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetPixelOffsetMode(nativeObject, value));
		}
	}

	public Point RenderingOrigin
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetRenderingOrigin(nativeObject, out var x, out var y));
			return new Point(x, y);
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetRenderingOrigin(nativeObject, value.X, value.Y));
		}
	}

	public SmoothingMode SmoothingMode
	{
		get
		{
			SmoothingMode smoothingMode = SmoothingMode.Invalid;
			GDIPlus.CheckStatus(GDIPlus.GdipGetSmoothingMode(nativeObject, out smoothingMode));
			return smoothingMode;
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetSmoothingMode(nativeObject, value));
		}
	}

	[System.MonoTODO("This property does not do anything when used with libgdiplus.")]
	public int TextContrast
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetTextContrast(nativeObject, out var contrast));
			return contrast;
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetTextContrast(nativeObject, value));
		}
	}

	public TextRenderingHint TextRenderingHint
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetTextRenderingHint(nativeObject, out var mode));
			return mode;
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetTextRenderingHint(nativeObject, value));
		}
	}

	public Matrix Transform
	{
		get
		{
			Matrix matrix = new Matrix();
			GDIPlus.CheckStatus(GDIPlus.GdipGetWorldTransform(nativeObject, matrix.nativeMatrix));
			return matrix;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetWorldTransform(nativeObject, value.nativeMatrix));
		}
	}

	public RectangleF VisibleClipBounds
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetVisibleClipBounds(nativeObject, out var rect));
			return rect;
		}
	}

	internal Graphics(IntPtr nativeGraphics)
	{
		nativeObject = IntPtr.Zero;
		base._002Ector();
		nativeObject = nativeGraphics;
	}

	internal Graphics(IntPtr nativeGraphics, Image image)
		: this(nativeGraphics)
	{
		if (image is Metafile metafile)
		{
			_metafileHolder = metafile.AddMetafileHolder();
		}
	}

	~Graphics()
	{
		Dispose();
	}

	[System.MonoTODO("Metafiles, both WMF and EMF formats, aren't supported.")]
	public void AddMetafileComment(byte[] data)
	{
		throw new NotImplementedException();
	}

	public GraphicsContainer BeginContainer()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipBeginContainer2(nativeObject, out var state));
		return new GraphicsContainer(state);
	}

	[System.MonoTODO("The rectangles and unit parameters aren't supported in libgdiplus")]
	public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipBeginContainerI(nativeObject, ref dstrect, ref srcrect, unit, out var state));
		return new GraphicsContainer(state);
	}

	[System.MonoTODO("The rectangles and unit parameters aren't supported in libgdiplus")]
	public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipBeginContainer(nativeObject, ref dstrect, ref srcrect, unit, out var state));
		return new GraphicsContainer(state);
	}

	public void Clear(Color color)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGraphicsClear(nativeObject, color.ToArgb()));
	}

	[System.MonoLimitation("Works on Win32 and on X11 (but not on Cocoa and Quartz)")]
	public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize)
	{
		CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, CopyPixelOperation.SourceCopy);
	}

	[System.MonoLimitation("Works on Win32 and (for CopyPixelOperation.SourceCopy only) on X11 but not on Cocoa and Quartz")]
	public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
	{
		CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, copyPixelOperation);
	}

	[System.MonoLimitation("Works on Win32 and on X11 (but not on Cocoa and Quartz)")]
	public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
	{
		CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize, CopyPixelOperation.SourceCopy);
	}

	[System.MonoLimitation("Works on Win32 and (for CopyPixelOperation.SourceCopy only) on X11 but not on Cocoa and Quartz")]
	public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
	{
		if (!Enum.IsDefined(typeof(CopyPixelOperation), copyPixelOperation))
		{
			throw new InvalidEnumArgumentException(global::Locale.GetText("Enum argument value '{0}' is not valid for CopyPixelOperation", copyPixelOperation));
		}
		if (GDIPlus.UseX11Drawable)
		{
			CopyFromScreenX11(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
		}
		else if (GDIPlus.UseCarbonDrawable)
		{
			CopyFromScreenMac(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
		}
		else if (GDIPlus.UseCocoaDrawable)
		{
			CopyFromScreenMac(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
		}
		else
		{
			CopyFromScreenWin32(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
		}
	}

	private void CopyFromScreenWin32(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
	{
		IntPtr dC = GDIPlus.GetDC(GDIPlus.GetDesktopWindow());
		IntPtr hdc = GetHdc();
		GDIPlus.BitBlt(hdc, destinationX, destinationY, blockRegionSize.Width, blockRegionSize.Height, dC, sourceX, sourceY, (int)copyPixelOperation);
		GDIPlus.ReleaseDC(IntPtr.Zero, dC);
		ReleaseHdc(hdc);
	}

	private void CopyFromScreenMac(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
	{
		throw new NotImplementedException();
	}

	private void CopyFromScreenX11(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
	{
		int pane = -1;
		int nitems = 0;
		if (copyPixelOperation != CopyPixelOperation.SourceCopy)
		{
			throw new NotImplementedException("Operation not implemented under X11");
		}
		if (GDIPlus.Display == IntPtr.Zero)
		{
			GDIPlus.Display = GDIPlus.XOpenDisplay(IntPtr.Zero);
		}
		IntPtr drawable = GDIPlus.XRootWindow(GDIPlus.Display, 0);
		IntPtr visual = GDIPlus.XDefaultVisual(GDIPlus.Display, 0);
		XVisualInfo vinfo_template = new XVisualInfo
		{
			visualid = GDIPlus.XVisualIDFromVisual(visual)
		};
		IntPtr intPtr = GDIPlus.XGetVisualInfo(GDIPlus.Display, 1, ref vinfo_template, ref nitems);
		vinfo_template = (XVisualInfo)Marshal.PtrToStructure(intPtr, typeof(XVisualInfo));
		IntPtr intPtr2 = GDIPlus.XGetImage(GDIPlus.Display, drawable, sourceX, sourceY, blockRegionSize.Width, blockRegionSize.Height, pane, 2);
		if (intPtr2 == IntPtr.Zero)
		{
			throw new InvalidOperationException($"XGetImage returned NULL when asked to for a {blockRegionSize.Width}x{blockRegionSize.Height} region block");
		}
		Bitmap bitmap = new Bitmap(blockRegionSize.Width, blockRegionSize.Height);
		int num = (int)vinfo_template.red_mask;
		int num2 = (int)vinfo_template.blue_mask;
		int num3 = (int)vinfo_template.green_mask;
		for (int i = 0; i < blockRegionSize.Height; i++)
		{
			for (int j = 0; j < blockRegionSize.Width; j++)
			{
				int num4 = GDIPlus.XGetPixel(intPtr2, j, i);
				int red;
				int green;
				int blue;
				switch (vinfo_template.depth)
				{
				case 16u:
					red = ((num4 & num) >> 8) & 0xFF;
					green = ((num4 & num3) >> 3) & 0xFF;
					blue = ((num4 & num2) << 3) & 0xFF;
					break;
				case 24u:
				case 32u:
					red = ((num4 & num) >> 16) & 0xFF;
					green = ((num4 & num3) >> 8) & 0xFF;
					blue = num4 & num2 & 0xFF;
					break;
				default:
					throw new NotImplementedException(global::Locale.GetText("{0}bbp depth not supported.", vinfo_template.depth));
				}
				bitmap.SetPixel(j, i, Color.FromArgb(255, red, green, blue));
			}
		}
		DrawImage(bitmap, destinationX, destinationY);
		bitmap.Dispose();
		GDIPlus.XDestroyImage(intPtr2);
		GDIPlus.XFree(intPtr);
	}

	public void Dispose()
	{
		if (!disposed)
		{
			if (deviceContextHdc != IntPtr.Zero)
			{
				ReleaseHdc();
			}
			if (GDIPlus.UseCarbonDrawable || GDIPlus.UseCocoaDrawable)
			{
				Flush();
				if (maccontext != null)
				{
					maccontext.Release();
				}
			}
			Status status = GDIPlus.GdipDeleteGraphics(nativeObject);
			nativeObject = IntPtr.Zero;
			GDIPlus.CheckStatus(status);
			if (_metafileHolder != null)
			{
				Metafile.MetafileHolder metafileHolder = _metafileHolder;
				_metafileHolder = null;
				metafileHolder.GraphicsDisposed();
			}
			disposed = true;
		}
		GC.SuppressFinalize(this);
	}

	public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
	{
		DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
	}

	public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
	{
		DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
	}

	public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawArc(nativeObject, pen.NativePen, x, y, width, height, startAngle, sweepAngle));
	}

	public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawArcI(nativeObject, pen.NativePen, x, y, width, height, startAngle, sweepAngle));
	}

	public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawBezier(nativeObject, pen.NativePen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
	}

	public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawBezierI(nativeObject, pen.NativePen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
	}

	public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawBezier(nativeObject, pen.NativePen, x1, y1, x2, y2, x3, y3, x4, y4));
	}

	public void DrawBeziers(Pen pen, Point[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		int num = points.Length;
		if (num >= 4)
		{
			for (int i = 0; i < num - 1; i += 3)
			{
				Point point = points[i];
				Point point2 = points[i + 1];
				Point point3 = points[i + 2];
				Point point4 = points[i + 3];
				GDIPlus.CheckStatus(GDIPlus.GdipDrawBezier(nativeObject, pen.NativePen, point.X, point.Y, point2.X, point2.Y, point3.X, point3.Y, point4.X, point4.Y));
			}
		}
	}

	public void DrawBeziers(Pen pen, PointF[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		int num = points.Length;
		if (num >= 4)
		{
			for (int i = 0; i < num - 1; i += 3)
			{
				PointF pointF = points[i];
				PointF pointF2 = points[i + 1];
				PointF pointF3 = points[i + 2];
				PointF pointF4 = points[i + 3];
				GDIPlus.CheckStatus(GDIPlus.GdipDrawBezier(nativeObject, pen.NativePen, pointF.X, pointF.Y, pointF2.X, pointF2.Y, pointF3.X, pointF3.Y, pointF4.X, pointF4.Y));
			}
		}
	}

	public void DrawClosedCurve(Pen pen, PointF[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawClosedCurve(nativeObject, pen.NativePen, points, points.Length));
	}

	public void DrawClosedCurve(Pen pen, Point[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawClosedCurveI(nativeObject, pen.NativePen, points, points.Length));
	}

	public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawClosedCurve2I(nativeObject, pen.NativePen, points, points.Length, tension));
	}

	public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawClosedCurve2(nativeObject, pen.NativePen, points, points.Length, tension));
	}

	public void DrawCurve(Pen pen, Point[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawCurveI(nativeObject, pen.NativePen, points, points.Length));
	}

	public void DrawCurve(Pen pen, PointF[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve(nativeObject, pen.NativePen, points, points.Length));
	}

	public void DrawCurve(Pen pen, PointF[] points, float tension)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve2(nativeObject, pen.NativePen, points, points.Length, tension));
	}

	public void DrawCurve(Pen pen, Point[] points, float tension)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve2I(nativeObject, pen.NativePen, points, points.Length, tension));
	}

	public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve3(nativeObject, pen.NativePen, points, points.Length, offset, numberOfSegments, 0.5f));
	}

	public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve3I(nativeObject, pen.NativePen, points, points.Length, offset, numberOfSegments, tension));
	}

	public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawCurve3(nativeObject, pen.NativePen, points, points.Length, offset, numberOfSegments, tension));
	}

	public void DrawEllipse(Pen pen, Rectangle rect)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void DrawEllipse(Pen pen, RectangleF rect)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void DrawEllipse(Pen pen, int x, int y, int width, int height)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawEllipseI(nativeObject, pen.NativePen, x, y, width, height));
	}

	public void DrawEllipse(Pen pen, float x, float y, float width, float height)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawEllipse(nativeObject, pen.NativePen, x, y, width, height));
	}

	public void DrawIcon(Icon icon, Rectangle targetRect)
	{
		if (icon == null)
		{
			throw new ArgumentNullException("icon");
		}
		DrawImage(icon.GetInternalBitmap(), targetRect);
	}

	public void DrawIcon(Icon icon, int x, int y)
	{
		if (icon == null)
		{
			throw new ArgumentNullException("icon");
		}
		DrawImage(icon.GetInternalBitmap(), x, y);
	}

	public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
	{
		if (icon == null)
		{
			throw new ArgumentNullException("icon");
		}
		DrawImageUnscaled(icon.GetInternalBitmap(), targetRect);
	}

	public void DrawImage(Image image, RectangleF rect)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRect(nativeObject, image.NativeObject, rect.X, rect.Y, rect.Width, rect.Height));
	}

	public void DrawImage(Image image, PointF point)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImage(nativeObject, image.NativeObject, point.X, point.Y));
	}

	public void DrawImage(Image image, Point[] destPoints)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsI(nativeObject, image.NativeObject, destPoints, destPoints.Length));
	}

	public void DrawImage(Image image, Point point)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		DrawImage(image, point.X, point.Y);
	}

	public void DrawImage(Image image, Rectangle rect)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void DrawImage(Image image, PointF[] destPoints)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePoints(nativeObject, image.NativeObject, destPoints, destPoints.Length));
	}

	public void DrawImage(Image image, int x, int y)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageI(nativeObject, image.NativeObject, x, y));
	}

	public void DrawImage(Image image, float x, float y)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImage(nativeObject, image.NativeObject, x, y));
	}

	public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRectI(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
	}

	public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
	}

	public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRectI(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
	}

	public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRect(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
	}

	public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRectI(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.nativeImageAttributes ?? IntPtr.Zero, null, IntPtr.Zero));
	}

	public void DrawImage(Image image, float x, float y, float width, float height)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRect(nativeObject, image.NativeObject, x, y, width, height));
	}

	public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRect(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.nativeImageAttributes ?? IntPtr.Zero, null, IntPtr.Zero));
	}

	public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointRectI(nativeObject, image.NativeObject, x, y, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit));
	}

	public void DrawImage(Image image, int x, int y, int width, int height)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectI(nativeObject, image.nativeObject, x, y, width, height));
	}

	public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointRect(nativeObject, image.nativeObject, x, y, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit));
	}

	public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRect(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.nativeImageAttributes ?? IntPtr.Zero, callback, IntPtr.Zero));
	}

	public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRectI(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.nativeImageAttributes ?? IntPtr.Zero, callback, IntPtr.Zero));
	}

	public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRectI(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.nativeImageAttributes ?? IntPtr.Zero, callback, (IntPtr)callbackData));
	}

	public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
	}

	public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImagePointsRect(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.nativeImageAttributes ?? IntPtr.Zero, callback, (IntPtr)callbackData));
	}

	public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRectI(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, IntPtr.Zero, null, IntPtr.Zero));
	}

	public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs?.nativeImageAttributes ?? IntPtr.Zero, null, IntPtr.Zero));
	}

	public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRectI(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr?.nativeImageAttributes ?? IntPtr.Zero, null, IntPtr.Zero));
	}

	public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRectI(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr?.nativeImageAttributes ?? IntPtr.Zero, callback, IntPtr.Zero));
	}

	public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs?.nativeImageAttributes ?? IntPtr.Zero, callback, IntPtr.Zero));
	}

	public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs?.nativeImageAttributes ?? IntPtr.Zero, callback, callbackData));
	}

	public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs?.nativeImageAttributes ?? IntPtr.Zero, callback, callbackData));
	}

	public void DrawImageUnscaled(Image image, Point point)
	{
		DrawImageUnscaled(image, point.X, point.Y);
	}

	public void DrawImageUnscaled(Image image, Rectangle rect)
	{
		DrawImageUnscaled(image, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void DrawImageUnscaled(Image image, int x, int y)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		DrawImage(image, x, y, image.Width, image.Height);
	}

	public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (width <= 0 || height <= 0)
		{
			return;
		}
		using Image image2 = new Bitmap(width, height);
		using Graphics graphics = FromImage(image2);
		graphics.DrawImage(image, 0, 0, image.Width, image.Height);
		DrawImage(image2, x, y, width, height);
	}

	public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		int width = ((image.Width > rect.Width) ? rect.Width : image.Width);
		int height = ((image.Height > rect.Height) ? rect.Height : image.Height);
		DrawImageUnscaled(image, rect.X, rect.Y, width, height);
	}

	public void DrawLine(Pen pen, PointF pt1, PointF pt2)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawLine(nativeObject, pen.NativePen, pt1.X, pt1.Y, pt2.X, pt2.Y));
	}

	public void DrawLine(Pen pen, Point pt1, Point pt2)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawLineI(nativeObject, pen.NativePen, pt1.X, pt1.Y, pt2.X, pt2.Y));
	}

	public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawLineI(nativeObject, pen.NativePen, x1, y1, x2, y2));
	}

	public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (!float.IsNaN(x1) && !float.IsNaN(y1) && !float.IsNaN(x2) && !float.IsNaN(y2))
		{
			GDIPlus.CheckStatus(GDIPlus.GdipDrawLine(nativeObject, pen.NativePen, x1, y1, x2, y2));
		}
	}

	public void DrawLines(Pen pen, PointF[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawLines(nativeObject, pen.NativePen, points, points.Length));
	}

	public void DrawLines(Pen pen, Point[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawLinesI(nativeObject, pen.NativePen, points, points.Length));
	}

	public void DrawPath(Pen pen, GraphicsPath path)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawPath(nativeObject, pen.NativePen, path.nativePath));
	}

	public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
	}

	public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
	}

	public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawPie(nativeObject, pen.NativePen, x, y, width, height, startAngle, sweepAngle));
	}

	public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawPieI(nativeObject, pen.NativePen, x, y, width, height, startAngle, sweepAngle));
	}

	public void DrawPolygon(Pen pen, Point[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawPolygonI(nativeObject, pen.NativePen, points, points.Length));
	}

	public void DrawPolygon(Pen pen, PointF[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawPolygon(nativeObject, pen.NativePen, points, points.Length));
	}

	public void DrawRectangle(Pen pen, Rectangle rect)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
	}

	public void DrawRectangle(Pen pen, float x, float y, float width, float height)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawRectangle(nativeObject, pen.NativePen, x, y, width, height));
	}

	public void DrawRectangle(Pen pen, int x, int y, int width, int height)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawRectangleI(nativeObject, pen.NativePen, x, y, width, height));
	}

	public void DrawRectangles(Pen pen, RectangleF[] rects)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("image");
		}
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawRectangles(nativeObject, pen.NativePen, rects, rects.Length));
	}

	public void DrawRectangles(Pen pen, Rectangle[] rects)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("image");
		}
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipDrawRectanglesI(nativeObject, pen.NativePen, rects, rects.Length));
	}

	public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
	{
		DrawString(s, font, brush, layoutRectangle, null);
	}

	public void DrawString(string s, Font font, Brush brush, PointF point)
	{
		DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), null);
	}

	public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
	{
		DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), format);
	}

	public void DrawString(string s, Font font, Brush brush, float x, float y)
	{
		DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), null);
	}

	public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
	{
		DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), format);
	}

	public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
	{
		if (font == null)
		{
			throw new ArgumentNullException("font");
		}
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (s != null && s.Length != 0)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipDrawString(nativeObject, s, s.Length, font.NativeObject, ref layoutRectangle, format?.NativeObject ?? IntPtr.Zero, brush.NativeBrush));
		}
	}

	public void EndContainer(GraphicsContainer container)
	{
		if (container == null)
		{
			throw new ArgumentNullException("container");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipEndContainer(nativeObject, container.NativeObject));
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	public void ExcludeClip(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetClipRectI(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Exclude));
	}

	public void ExcludeClip(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipSetClipRegion(nativeObject, region.NativeObject, CombineMode.Exclude));
	}

	public void FillClosedCurve(Brush brush, PointF[] points)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillClosedCurve(nativeObject, brush.NativeBrush, points, points.Length));
	}

	public void FillClosedCurve(Brush brush, Point[] points)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillClosedCurveI(nativeObject, brush.NativeBrush, points, points.Length));
	}

	public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		FillClosedCurve(brush, points, fillmode, 0.5f);
	}

	public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		FillClosedCurve(brush, points, fillmode, 0.5f);
	}

	public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillClosedCurve2(nativeObject, brush.NativeBrush, points, points.Length, tension, fillmode));
	}

	public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillClosedCurve2I(nativeObject, brush.NativeBrush, points, points.Length, tension, fillmode));
	}

	public void FillEllipse(Brush brush, Rectangle rect)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void FillEllipse(Brush brush, RectangleF rect)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void FillEllipse(Brush brush, float x, float y, float width, float height)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillEllipse(nativeObject, brush.NativeBrush, x, y, width, height));
	}

	public void FillEllipse(Brush brush, int x, int y, int width, int height)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillEllipseI(nativeObject, brush.NativeBrush, x, y, width, height));
	}

	public void FillPath(Brush brush, GraphicsPath path)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillPath(nativeObject, brush.NativeBrush, path.nativePath));
	}

	public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillPie(nativeObject, brush.NativeBrush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle));
	}

	public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillPieI(nativeObject, brush.NativeBrush, x, y, width, height, startAngle, sweepAngle));
	}

	public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillPie(nativeObject, brush.NativeBrush, x, y, width, height, startAngle, sweepAngle));
	}

	public void FillPolygon(Brush brush, PointF[] points)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillPolygon2(nativeObject, brush.NativeBrush, points, points.Length));
	}

	public void FillPolygon(Brush brush, Point[] points)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillPolygon2I(nativeObject, brush.NativeBrush, points, points.Length));
	}

	public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillPolygonI(nativeObject, brush.NativeBrush, points, points.Length, fillMode));
	}

	public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillPolygon(nativeObject, brush.NativeBrush, points, points.Length, fillMode));
	}

	public void FillRectangle(Brush brush, RectangleF rect)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
	}

	public void FillRectangle(Brush brush, Rectangle rect)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
	}

	public void FillRectangle(Brush brush, int x, int y, int width, int height)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillRectangleI(nativeObject, brush.NativeBrush, x, y, width, height));
	}

	public void FillRectangle(Brush brush, float x, float y, float width, float height)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillRectangle(nativeObject, brush.NativeBrush, x, y, width, height));
	}

	public void FillRectangles(Brush brush, Rectangle[] rects)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillRectanglesI(nativeObject, brush.NativeBrush, rects, rects.Length));
	}

	public void FillRectangles(Brush brush, RectangleF[] rects)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillRectangles(nativeObject, brush.NativeBrush, rects, rects.Length));
	}

	public void FillRegion(Brush brush, Region region)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipFillRegion(nativeObject, brush.NativeBrush, region.NativeObject));
	}

	public void Flush()
	{
		Flush(FlushIntention.Flush);
	}

	public void Flush(FlushIntention intention)
	{
		if (!(nativeObject == IntPtr.Zero))
		{
			GDIPlus.CheckStatus(GDIPlus.GdipFlush(nativeObject, intention));
			if (maccontext != null)
			{
				maccontext.Synchronize();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Graphics FromHdc(IntPtr hdc)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateFromHDC(hdc, out var graphics));
		return new Graphics(graphics);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[System.MonoTODO]
	public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
	{
		throw new NotImplementedException();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Graphics FromHdcInternal(IntPtr hdc)
	{
		GDIPlus.Display = hdc;
		return null;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Graphics FromHwnd(IntPtr hwnd)
	{
		IntPtr graphics;
		if (GDIPlus.UseCocoaDrawable)
		{
			if (hwnd == IntPtr.Zero)
			{
				throw new NotSupportedException("Opening display graphics is not supported");
			}
			CocoaContext cGContextForNSView = MacSupport.GetCGContextForNSView(hwnd);
			GDIPlus.GdipCreateFromContext_macosx(cGContextForNSView.ctx, cGContextForNSView.width, cGContextForNSView.height, out graphics);
			return new Graphics(graphics)
			{
				maccontext = cGContextForNSView
			};
		}
		if (GDIPlus.UseCarbonDrawable)
		{
			CarbonContext cGContextForView = MacSupport.GetCGContextForView(hwnd);
			GDIPlus.GdipCreateFromContext_macosx(cGContextForView.ctx, cGContextForView.width, cGContextForView.height, out graphics);
			return new Graphics(graphics)
			{
				maccontext = cGContextForView
			};
		}
		if (GDIPlus.UseX11Drawable)
		{
			if (GDIPlus.Display == IntPtr.Zero)
			{
				GDIPlus.Display = GDIPlus.XOpenDisplay(IntPtr.Zero);
				if (GDIPlus.Display == IntPtr.Zero)
				{
					throw new NotSupportedException("Could not open display (X-Server required. Check your DISPLAY environment variable)");
				}
			}
			if (hwnd == IntPtr.Zero)
			{
				hwnd = GDIPlus.XRootWindow(GDIPlus.Display, GDIPlus.XDefaultScreen(GDIPlus.Display));
			}
			return FromXDrawable(hwnd, GDIPlus.Display);
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCreateFromHWND(hwnd, out graphics));
		return new Graphics(graphics);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Graphics FromHwndInternal(IntPtr hwnd)
	{
		return FromHwnd(hwnd);
	}

	public static Graphics FromImage(Image image)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if ((image.PixelFormat & PixelFormat.Indexed) != PixelFormat.Undefined)
		{
			throw new Exception(global::Locale.GetText("Cannot create Graphics from an indexed bitmap."));
		}
		GDIPlus.CheckStatus(GDIPlus.GdipGetImageGraphicsContext(image.nativeObject, out var graphics));
		Graphics graphics2 = new Graphics(graphics, image);
		if (GDIPlus.RunningOnUnix())
		{
			Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
			GDIPlus.GdipSetVisibleClip_linux(graphics2.NativeObject, ref rect);
		}
		return graphics2;
	}

	internal static Graphics FromXDrawable(IntPtr drawable, IntPtr display)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateFromXDrawable_linux(drawable, display, out var graphics));
		return new Graphics(graphics);
	}

	[System.MonoTODO]
	public static IntPtr GetHalftonePalette()
	{
		throw new NotImplementedException();
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public IntPtr GetHdc()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetDC(nativeObject, out deviceContextHdc));
		return deviceContextHdc;
	}

	public Color GetNearestColor(Color color)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetNearestColor(nativeObject, out var argb));
		return Color.FromArgb(argb);
	}

	public void IntersectClip(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipSetClipRegion(nativeObject, region.NativeObject, CombineMode.Intersect));
	}

	public void IntersectClip(RectangleF rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetClipRect(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Intersect));
	}

	public void IntersectClip(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetClipRectI(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Intersect));
	}

	public bool IsVisible(Point point)
	{
		bool result = false;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisiblePointI(nativeObject, point.X, point.Y, out result));
		return result;
	}

	public bool IsVisible(RectangleF rect)
	{
		bool result = false;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRect(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, out result));
		return result;
	}

	public bool IsVisible(PointF point)
	{
		bool result = false;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisiblePoint(nativeObject, point.X, point.Y, out result));
		return result;
	}

	public bool IsVisible(Rectangle rect)
	{
		bool result = false;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRectI(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, out result));
		return result;
	}

	public bool IsVisible(float x, float y)
	{
		return IsVisible(new PointF(x, y));
	}

	public bool IsVisible(int x, int y)
	{
		return IsVisible(new Point(x, y));
	}

	public bool IsVisible(float x, float y, float width, float height)
	{
		return IsVisible(new RectangleF(x, y, width, height));
	}

	public bool IsVisible(int x, int y, int width, int height)
	{
		return IsVisible(new Rectangle(x, y, width, height));
	}

	public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
	{
		if (text == null || text.Length == 0)
		{
			return new Region[0];
		}
		if (font == null)
		{
			throw new ArgumentNullException("font");
		}
		if (stringFormat == null)
		{
			throw new ArgumentException("stringFormat");
		}
		int measurableCharacterRangeCount = stringFormat.GetMeasurableCharacterRangeCount();
		if (measurableCharacterRangeCount == 0)
		{
			return new Region[0];
		}
		IntPtr[] array = new IntPtr[measurableCharacterRangeCount];
		Region[] array2 = new Region[measurableCharacterRangeCount];
		for (int i = 0; i < measurableCharacterRangeCount; i++)
		{
			array2[i] = new Region();
			array[i] = array2[i].NativeObject;
		}
		GDIPlus.CheckStatus(GDIPlus.GdipMeasureCharacterRanges(nativeObject, text, text.Length, font.NativeObject, ref layoutRect, stringFormat.NativeObject, measurableCharacterRangeCount, out array[0]));
		return array2;
	}

	private unsafe SizeF GdipMeasureString(IntPtr graphics, string text, Font font, ref RectangleF layoutRect, IntPtr stringFormat)
	{
		if (text == null || text.Length == 0)
		{
			return SizeF.Empty;
		}
		if (font == null)
		{
			throw new ArgumentNullException("font");
		}
		RectangleF boundingBox = default(RectangleF);
		GDIPlus.CheckStatus(GDIPlus.GdipMeasureString(nativeObject, text, text.Length, font.NativeObject, ref layoutRect, stringFormat, out boundingBox, null, null));
		return new SizeF(boundingBox.Width, boundingBox.Height);
	}

	public SizeF MeasureString(string text, Font font)
	{
		return MeasureString(text, font, SizeF.Empty);
	}

	public SizeF MeasureString(string text, Font font, SizeF layoutArea)
	{
		RectangleF layoutRect = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
		return GdipMeasureString(nativeObject, text, font, ref layoutRect, IntPtr.Zero);
	}

	public SizeF MeasureString(string text, Font font, int width)
	{
		RectangleF layoutRect = new RectangleF(0f, 0f, width, 2.1474836E+09f);
		return GdipMeasureString(nativeObject, text, font, ref layoutRect, IntPtr.Zero);
	}

	public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
	{
		RectangleF layoutRect = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
		IntPtr stringFormat2 = stringFormat?.NativeObject ?? IntPtr.Zero;
		return GdipMeasureString(nativeObject, text, font, ref layoutRect, stringFormat2);
	}

	public SizeF MeasureString(string text, Font font, int width, StringFormat format)
	{
		RectangleF layoutRect = new RectangleF(0f, 0f, width, 2.1474836E+09f);
		IntPtr stringFormat = format?.NativeObject ?? IntPtr.Zero;
		return GdipMeasureString(nativeObject, text, font, ref layoutRect, stringFormat);
	}

	public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
	{
		RectangleF layoutRect = new RectangleF(origin.X, origin.Y, 0f, 0f);
		IntPtr stringFormat2 = stringFormat?.NativeObject ?? IntPtr.Zero;
		return GdipMeasureString(nativeObject, text, font, ref layoutRect, stringFormat2);
	}

	public unsafe SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
	{
		charactersFitted = 0;
		linesFilled = 0;
		if (text == null || text.Length == 0)
		{
			return SizeF.Empty;
		}
		if (font == null)
		{
			throw new ArgumentNullException("font");
		}
		RectangleF boundingBox = default(RectangleF);
		RectangleF layoutRect = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
		IntPtr stringFormat2 = stringFormat?.NativeObject ?? IntPtr.Zero;
		fixed (int* codepointsFitted = &charactersFitted)
		{
			fixed (int* linesFilled2 = &linesFilled)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipMeasureString(nativeObject, text, text.Length, font.NativeObject, ref layoutRect, stringFormat2, out boundingBox, codepointsFitted, linesFilled2));
			}
		}
		return new SizeF(boundingBox.Width, boundingBox.Height);
	}

	public void MultiplyTransform(Matrix matrix)
	{
		MultiplyTransform(matrix, MatrixOrder.Prepend);
	}

	public void MultiplyTransform(Matrix matrix, MatrixOrder order)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipMultiplyWorldTransform(nativeObject, matrix.nativeMatrix, order));
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void ReleaseHdc(IntPtr hdc)
	{
		ReleaseHdcInternal(hdc);
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public void ReleaseHdc()
	{
		ReleaseHdcInternal(deviceContextHdc);
	}

	[System.MonoLimitation("Can only be used when hdc was provided by Graphics.GetHdc() method")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ReleaseHdcInternal(IntPtr hdc)
	{
		Status status = Status.InvalidParameter;
		if (hdc == deviceContextHdc)
		{
			status = GDIPlus.GdipReleaseDC(nativeObject, deviceContextHdc);
			deviceContextHdc = IntPtr.Zero;
		}
		GDIPlus.CheckStatus(status);
	}

	public void ResetClip()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipResetClip(nativeObject));
	}

	public void ResetTransform()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipResetWorldTransform(nativeObject));
	}

	public void Restore(GraphicsState gstate)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipRestoreGraphics(nativeObject, (uint)gstate.nativeState));
	}

	public void RotateTransform(float angle)
	{
		RotateTransform(angle, MatrixOrder.Prepend);
	}

	public void RotateTransform(float angle, MatrixOrder order)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipRotateWorldTransform(nativeObject, angle, order));
	}

	public GraphicsState Save()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSaveGraphics(nativeObject, out var state));
		return new GraphicsState((int)state);
	}

	public void ScaleTransform(float sx, float sy)
	{
		ScaleTransform(sx, sy, MatrixOrder.Prepend);
	}

	public void ScaleTransform(float sx, float sy, MatrixOrder order)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipScaleWorldTransform(nativeObject, sx, sy, order));
	}

	public void SetClip(RectangleF rect)
	{
		SetClip(rect, CombineMode.Replace);
	}

	public void SetClip(GraphicsPath path)
	{
		SetClip(path, CombineMode.Replace);
	}

	public void SetClip(Rectangle rect)
	{
		SetClip(rect, CombineMode.Replace);
	}

	public void SetClip(Graphics g)
	{
		SetClip(g, CombineMode.Replace);
	}

	public void SetClip(Graphics g, CombineMode combineMode)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipSetClipGraphics(nativeObject, g.NativeObject, combineMode));
	}

	public void SetClip(Rectangle rect, CombineMode combineMode)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetClipRectI(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, combineMode));
	}

	public void SetClip(RectangleF rect, CombineMode combineMode)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetClipRect(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, combineMode));
	}

	public void SetClip(Region region, CombineMode combineMode)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipSetClipRegion(nativeObject, region.NativeObject, combineMode));
	}

	public void SetClip(GraphicsPath path, CombineMode combineMode)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipSetClipPath(nativeObject, path.nativePath, combineMode));
	}

	public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		IntPtr intPtr = GDIPlus.FromPointToUnManagedMemory(pts);
		GDIPlus.CheckStatus(GDIPlus.GdipTransformPoints(nativeObject, destSpace, srcSpace, intPtr, pts.Length));
		GDIPlus.FromUnManagedMemoryToPoint(intPtr, pts);
	}

	public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		IntPtr intPtr = GDIPlus.FromPointToUnManagedMemoryI(pts);
		GDIPlus.CheckStatus(GDIPlus.GdipTransformPointsI(nativeObject, destSpace, srcSpace, intPtr, pts.Length));
		GDIPlus.FromUnManagedMemoryToPointI(intPtr, pts);
	}

	public void TranslateClip(int dx, int dy)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipTranslateClipI(nativeObject, dx, dy));
	}

	public void TranslateClip(float dx, float dy)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipTranslateClip(nativeObject, dx, dy));
	}

	public void TranslateTransform(float dx, float dy)
	{
		TranslateTransform(dx, dy, MatrixOrder.Prepend);
	}

	public void TranslateTransform(float dx, float dy, MatrixOrder order)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipTranslateWorldTransform(nativeObject, dx, dy, order));
	}

	[System.MonoTODO]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public object GetContextInfo()
	{
		throw new NotImplementedException();
	}

	internal Graphics()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
