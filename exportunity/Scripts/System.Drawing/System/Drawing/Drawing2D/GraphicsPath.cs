using System.ComponentModel;

namespace System.Drawing.Drawing2D;

public sealed class GraphicsPath : MarshalByRefObject, ICloneable, IDisposable
{
	private const float FlatnessDefault = 0.25f;

	internal IntPtr nativePath = IntPtr.Zero;

	public FillMode FillMode
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPathFillMode(nativePath, out var fillMode));
			return fillMode;
		}
		set
		{
			if (value < FillMode.Alternate || value > FillMode.Winding)
			{
				throw new InvalidEnumArgumentException("FillMode", (int)value, typeof(FillMode));
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetPathFillMode(nativePath, value));
		}
	}

	public PathData PathData
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(nativePath, out var count));
			PointF[] points = new PointF[count];
			byte[] types = new byte[count];
			if (count > 0)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipGetPathPoints(nativePath, points, count));
				GDIPlus.CheckStatus(GDIPlus.GdipGetPathTypes(nativePath, types, count));
			}
			return new PathData
			{
				Points = points,
				Types = types
			};
		}
	}

	public PointF[] PathPoints
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(nativePath, out var count));
			if (count == 0)
			{
				throw new ArgumentException("PathPoints");
			}
			PointF[] array = new PointF[count];
			GDIPlus.CheckStatus(GDIPlus.GdipGetPathPoints(nativePath, array, count));
			return array;
		}
	}

	public byte[] PathTypes
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(nativePath, out var count));
			if (count == 0)
			{
				throw new ArgumentException("PathTypes");
			}
			byte[] array = new byte[count];
			GDIPlus.CheckStatus(GDIPlus.GdipGetPathTypes(nativePath, array, count));
			return array;
		}
	}

	public int PointCount
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPointCount(nativePath, out var count));
			return count;
		}
	}

	internal IntPtr NativeObject
	{
		get
		{
			return nativePath;
		}
		set
		{
			nativePath = value;
		}
	}

	private GraphicsPath(IntPtr ptr)
	{
		nativePath = ptr;
	}

	public GraphicsPath()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreatePath(FillMode.Alternate, out nativePath));
	}

	public GraphicsPath(FillMode fillMode)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreatePath(fillMode, out nativePath));
	}

	public GraphicsPath(Point[] pts, byte[] types)
		: this(pts, types, FillMode.Alternate)
	{
	}

	public GraphicsPath(PointF[] pts, byte[] types)
		: this(pts, types, FillMode.Alternate)
	{
	}

	public GraphicsPath(Point[] pts, byte[] types, FillMode fillMode)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		if (pts.Length != types.Length)
		{
			throw new ArgumentException("Invalid parameter passed. Number of points and types must be same.");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCreatePath2I(pts, types, pts.Length, fillMode, out nativePath));
	}

	public GraphicsPath(PointF[] pts, byte[] types, FillMode fillMode)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		if (pts.Length != types.Length)
		{
			throw new ArgumentException("Invalid parameter passed. Number of points and types must be same.");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCreatePath2(pts, types, pts.Length, fillMode, out nativePath));
	}

	public object Clone()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipClonePath(nativePath, out var clonePath));
		return new GraphicsPath(clonePath);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~GraphicsPath()
	{
		Dispose(disposing: false);
	}

	private void Dispose(bool disposing)
	{
		if (nativePath != IntPtr.Zero)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipDeletePath(nativePath));
			nativePath = IntPtr.Zero;
		}
	}

	public void AddArc(Rectangle rect, float startAngle, float sweepAngle)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathArcI(nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle));
	}

	public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathArc(nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle));
	}

	public void AddArc(int x, int y, int width, int height, float startAngle, float sweepAngle)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathArcI(nativePath, x, y, width, height, startAngle, sweepAngle));
	}

	public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathArc(nativePath, x, y, width, height, startAngle, sweepAngle));
	}

	public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezierI(nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
	}

	public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezier(nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
	}

	public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezierI(nativePath, x1, y1, x2, y2, x3, y3, x4, y4));
	}

	public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathBezier(nativePath, x1, y1, x2, y2, x3, y3, x4, y4));
	}

	public void AddBeziers(params Point[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathBeziersI(nativePath, points, points.Length));
	}

	public void AddBeziers(PointF[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathBeziers(nativePath, points, points.Length));
	}

	public void AddEllipse(RectangleF rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipse(nativePath, rect.X, rect.Y, rect.Width, rect.Height));
	}

	public void AddEllipse(float x, float y, float width, float height)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipse(nativePath, x, y, width, height));
	}

	public void AddEllipse(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipseI(nativePath, rect.X, rect.Y, rect.Width, rect.Height));
	}

	public void AddEllipse(int x, int y, int width, int height)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathEllipseI(nativePath, x, y, width, height));
	}

	public void AddLine(Point pt1, Point pt2)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathLineI(nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y));
	}

	public void AddLine(PointF pt1, PointF pt2)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine(nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y));
	}

	public void AddLine(int x1, int y1, int x2, int y2)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathLineI(nativePath, x1, y1, x2, y2));
	}

	public void AddLine(float x1, float y1, float x2, float y2)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine(nativePath, x1, y1, x2, y2));
	}

	public void AddLines(Point[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (points.Length == 0)
		{
			throw new ArgumentException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine2I(nativePath, points, points.Length));
	}

	public void AddLines(PointF[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (points.Length == 0)
		{
			throw new ArgumentException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathLine2(nativePath, points, points.Length));
	}

	public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathPie(nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle));
	}

	public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathPieI(nativePath, x, y, width, height, startAngle, sweepAngle));
	}

	public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathPie(nativePath, x, y, width, height, startAngle, sweepAngle));
	}

	public void AddPolygon(Point[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathPolygonI(nativePath, points, points.Length));
	}

	public void AddPolygon(PointF[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathPolygon(nativePath, points, points.Length));
	}

	public void AddRectangle(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectangleI(nativePath, rect.X, rect.Y, rect.Width, rect.Height));
	}

	public void AddRectangle(RectangleF rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectangle(nativePath, rect.X, rect.Y, rect.Width, rect.Height));
	}

	public void AddRectangles(Rectangle[] rects)
	{
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		if (rects.Length == 0)
		{
			throw new ArgumentException("rects");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectanglesI(nativePath, rects, rects.Length));
	}

	public void AddRectangles(RectangleF[] rects)
	{
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		if (rects.Length == 0)
		{
			throw new ArgumentException("rects");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathRectangles(nativePath, rects, rects.Length));
	}

	public void AddPath(GraphicsPath addingPath, bool connect)
	{
		if (addingPath == null)
		{
			throw new ArgumentNullException("addingPath");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathPath(nativePath, addingPath.nativePath, connect));
	}

	public PointF GetLastPoint()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetPathLastPoint(nativePath, out var lastPoint));
		return lastPoint;
	}

	public void AddClosedCurve(Point[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurveI(nativePath, points, points.Length));
	}

	public void AddClosedCurve(PointF[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurve(nativePath, points, points.Length));
	}

	public void AddClosedCurve(Point[] points, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurve2I(nativePath, points, points.Length, tension));
	}

	public void AddClosedCurve(PointF[] points, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathClosedCurve2(nativePath, points, points.Length, tension));
	}

	public void AddCurve(Point[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurveI(nativePath, points, points.Length));
	}

	public void AddCurve(PointF[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve(nativePath, points, points.Length));
	}

	public void AddCurve(Point[] points, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve2I(nativePath, points, points.Length, tension));
	}

	public void AddCurve(PointF[] points, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve2(nativePath, points, points.Length, tension));
	}

	public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve3I(nativePath, points, points.Length, offset, numberOfSegments, tension));
	}

	public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathCurve3(nativePath, points, points.Length, offset, numberOfSegments, tension));
	}

	public void Reset()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipResetPath(nativePath));
	}

	public void Reverse()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipReversePath(nativePath));
	}

	public void Transform(Matrix matrix)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipTransformPath(nativePath, matrix.nativeMatrix));
	}

	[System.MonoTODO("The StringFormat parameter is ignored when using libgdiplus.")]
	public void AddString(string s, FontFamily family, int style, float emSize, Point origin, StringFormat format)
	{
		AddString(s, family, style, emSize, new Rectangle
		{
			X = origin.X,
			Y = origin.Y
		}, format);
	}

	[System.MonoTODO("The StringFormat parameter is ignored when using libgdiplus.")]
	public void AddString(string s, FontFamily family, int style, float emSize, PointF origin, StringFormat format)
	{
		AddString(s, family, style, emSize, new RectangleF
		{
			X = origin.X,
			Y = origin.Y
		}, format);
	}

	[System.MonoTODO("The layoutRect and StringFormat parameters are ignored when using libgdiplus.")]
	public void AddString(string s, FontFamily family, int style, float emSize, Rectangle layoutRect, StringFormat format)
	{
		if (family == null)
		{
			throw new ArgumentException("family");
		}
		IntPtr format2 = format?.NativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathStringI(nativePath, s, s.Length, family.NativeFamily, style, emSize, ref layoutRect, format2));
	}

	[System.MonoTODO("The layoutRect and StringFormat parameters are ignored when using libgdiplus.")]
	public void AddString(string s, FontFamily family, int style, float emSize, RectangleF layoutRect, StringFormat format)
	{
		if (family == null)
		{
			throw new ArgumentException("family");
		}
		IntPtr format2 = format?.NativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipAddPathString(nativePath, s, s.Length, family.NativeFamily, style, emSize, ref layoutRect, format2));
	}

	public void ClearMarkers()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipClearPathMarkers(nativePath));
	}

	public void CloseAllFigures()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipClosePathFigures(nativePath));
	}

	public void CloseFigure()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipClosePathFigure(nativePath));
	}

	public void Flatten()
	{
		Flatten(null, 0.25f);
	}

	public void Flatten(Matrix matrix)
	{
		Flatten(matrix, 0.25f);
	}

	public void Flatten(Matrix matrix, float flatness)
	{
		IntPtr matrix2 = matrix?.nativeMatrix ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipFlattenPath(nativePath, matrix2, flatness));
	}

	public RectangleF GetBounds()
	{
		return GetBounds(null, null);
	}

	public RectangleF GetBounds(Matrix matrix)
	{
		return GetBounds(matrix, null);
	}

	public RectangleF GetBounds(Matrix matrix, Pen pen)
	{
		IntPtr matrix2 = matrix?.nativeMatrix ?? IntPtr.Zero;
		IntPtr pen2 = pen?.NativePen ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipGetPathWorldBounds(nativePath, out var bounds, matrix2, pen2));
		return bounds;
	}

	public bool IsOutlineVisible(Point point, Pen pen)
	{
		return IsOutlineVisible(point.X, point.Y, pen, null);
	}

	public bool IsOutlineVisible(PointF point, Pen pen)
	{
		return IsOutlineVisible(point.X, point.Y, pen, null);
	}

	public bool IsOutlineVisible(int x, int y, Pen pen)
	{
		return IsOutlineVisible(x, y, pen, null);
	}

	public bool IsOutlineVisible(float x, float y, Pen pen)
	{
		return IsOutlineVisible(x, y, pen, null);
	}

	public bool IsOutlineVisible(Point pt, Pen pen, Graphics graphics)
	{
		return IsOutlineVisible(pt.X, pt.Y, pen, graphics);
	}

	public bool IsOutlineVisible(PointF pt, Pen pen, Graphics graphics)
	{
		return IsOutlineVisible(pt.X, pt.Y, pen, graphics);
	}

	public bool IsOutlineVisible(int x, int y, Pen pen, Graphics graphics)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		IntPtr graphics2 = graphics?.nativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsOutlineVisiblePathPointI(nativePath, x, y, pen.NativePen, graphics2, out var result));
		return result;
	}

	public bool IsOutlineVisible(float x, float y, Pen pen, Graphics graphics)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		IntPtr graphics2 = graphics?.nativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsOutlineVisiblePathPoint(nativePath, x, y, pen.NativePen, graphics2, out var result));
		return result;
	}

	public bool IsVisible(Point point)
	{
		return IsVisible(point.X, point.Y, null);
	}

	public bool IsVisible(PointF point)
	{
		return IsVisible(point.X, point.Y, null);
	}

	public bool IsVisible(int x, int y)
	{
		return IsVisible(x, y, null);
	}

	public bool IsVisible(float x, float y)
	{
		return IsVisible(x, y, null);
	}

	public bool IsVisible(Point pt, Graphics graphics)
	{
		return IsVisible(pt.X, pt.Y, graphics);
	}

	public bool IsVisible(PointF pt, Graphics graphics)
	{
		return IsVisible(pt.X, pt.Y, graphics);
	}

	public bool IsVisible(int x, int y, Graphics graphics)
	{
		IntPtr graphics2 = graphics?.nativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisiblePathPointI(nativePath, x, y, graphics2, out var result));
		return result;
	}

	public bool IsVisible(float x, float y, Graphics graphics)
	{
		IntPtr graphics2 = graphics?.nativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisiblePathPoint(nativePath, x, y, graphics2, out var result));
		return result;
	}

	public void SetMarkers()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetPathMarker(nativePath));
	}

	public void StartFigure()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipStartPathFigure(nativePath));
	}

	[System.MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
	public void Warp(PointF[] destPoints, RectangleF srcRect)
	{
		Warp(destPoints, srcRect, null, WarpMode.Perspective, 0.25f);
	}

	[System.MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
	public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix)
	{
		Warp(destPoints, srcRect, matrix, WarpMode.Perspective, 0.25f);
	}

	[System.MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
	public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode)
	{
		Warp(destPoints, srcRect, matrix, warpMode, 0.25f);
	}

	[System.MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
	public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode, float flatness)
	{
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		IntPtr matrix2 = matrix?.nativeMatrix ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipWarpPath(nativePath, matrix2, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, warpMode, flatness));
	}

	[System.MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
	public void Widen(Pen pen)
	{
		Widen(pen, null, 0.25f);
	}

	[System.MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
	public void Widen(Pen pen, Matrix matrix)
	{
		Widen(pen, matrix, 0.25f);
	}

	[System.MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
	public void Widen(Pen pen, Matrix matrix, float flatness)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (PointCount != 0)
		{
			IntPtr matrix2 = matrix?.nativeMatrix ?? IntPtr.Zero;
			GDIPlus.CheckStatus(GDIPlus.GdipWidenPath(nativePath, pen.NativePen, matrix2, flatness));
		}
	}
}
