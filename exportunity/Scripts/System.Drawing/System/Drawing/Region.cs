using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Drawing;

public sealed class Region : MarshalByRefObject, IDisposable
{
	private IntPtr nativeRegion = IntPtr.Zero;

	internal IntPtr NativeObject
	{
		get
		{
			return nativeRegion;
		}
		set
		{
			nativeRegion = value;
		}
	}

	public Region()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateRegion(out nativeRegion));
	}

	internal Region(IntPtr native)
	{
		nativeRegion = native;
	}

	public Region(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCreateRegionPath(path.nativePath, out nativeRegion));
	}

	public Region(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateRegionRectI(ref rect, out nativeRegion));
	}

	public Region(RectangleF rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateRegionRect(ref rect, out nativeRegion));
	}

	public Region(RegionData rgnData)
	{
		if (rgnData == null)
		{
			throw new ArgumentNullException("rgnData");
		}
		if (rgnData.Data.Length == 0)
		{
			throw new ArgumentException("rgnData");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCreateRegionRgnData(rgnData.Data, rgnData.Data.Length, out nativeRegion));
	}

	public void Union(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionPath(nativeRegion, path.nativePath, CombineMode.Union));
	}

	public void Union(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRectI(nativeRegion, ref rect, CombineMode.Union));
	}

	public void Union(RectangleF rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRect(nativeRegion, ref rect, CombineMode.Union));
	}

	public void Union(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRegion(nativeRegion, region.NativeObject, CombineMode.Union));
	}

	public void Intersect(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionPath(nativeRegion, path.nativePath, CombineMode.Intersect));
	}

	public void Intersect(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRectI(nativeRegion, ref rect, CombineMode.Intersect));
	}

	public void Intersect(RectangleF rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRect(nativeRegion, ref rect, CombineMode.Intersect));
	}

	public void Intersect(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRegion(nativeRegion, region.NativeObject, CombineMode.Intersect));
	}

	public void Complement(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionPath(nativeRegion, path.nativePath, CombineMode.Complement));
	}

	public void Complement(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRectI(nativeRegion, ref rect, CombineMode.Complement));
	}

	public void Complement(RectangleF rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRect(nativeRegion, ref rect, CombineMode.Complement));
	}

	public void Complement(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRegion(nativeRegion, region.NativeObject, CombineMode.Complement));
	}

	public void Exclude(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionPath(nativeRegion, path.nativePath, CombineMode.Exclude));
	}

	public void Exclude(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRectI(nativeRegion, ref rect, CombineMode.Exclude));
	}

	public void Exclude(RectangleF rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRect(nativeRegion, ref rect, CombineMode.Exclude));
	}

	public void Exclude(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRegion(nativeRegion, region.NativeObject, CombineMode.Exclude));
	}

	public void Xor(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionPath(nativeRegion, path.nativePath, CombineMode.Xor));
	}

	public void Xor(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRectI(nativeRegion, ref rect, CombineMode.Xor));
	}

	public void Xor(RectangleF rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRect(nativeRegion, ref rect, CombineMode.Xor));
	}

	public void Xor(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCombineRegionRegion(nativeRegion, region.NativeObject, CombineMode.Xor));
	}

	public RectangleF GetBounds(Graphics g)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		RectangleF rect = default(Rectangle);
		GDIPlus.CheckStatus(GDIPlus.GdipGetRegionBounds(nativeRegion, g.NativeObject, ref rect));
		return rect;
	}

	public void Translate(int dx, int dy)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipTranslateRegionI(nativeRegion, dx, dy));
	}

	public void Translate(float dx, float dy)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipTranslateRegion(nativeRegion, dx, dy));
	}

	public bool IsVisible(int x, int y, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPointI(nativeRegion, x, y, graphics, out var result));
		return result;
	}

	public bool IsVisible(int x, int y, int width, int height)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRectI(nativeRegion, x, y, width, height, IntPtr.Zero, out var result));
		return result;
	}

	public bool IsVisible(int x, int y, int width, int height, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRectI(nativeRegion, x, y, width, height, graphics, out var result));
		return result;
	}

	public bool IsVisible(Point point)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPointI(nativeRegion, point.X, point.Y, IntPtr.Zero, out var result));
		return result;
	}

	public bool IsVisible(PointF point)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPoint(nativeRegion, point.X, point.Y, IntPtr.Zero, out var result));
		return result;
	}

	public bool IsVisible(Point point, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPointI(nativeRegion, point.X, point.Y, graphics, out var result));
		return result;
	}

	public bool IsVisible(PointF point, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPoint(nativeRegion, point.X, point.Y, graphics, out var result));
		return result;
	}

	public bool IsVisible(Rectangle rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRectI(nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, IntPtr.Zero, out var result));
		return result;
	}

	public bool IsVisible(RectangleF rect)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRect(nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, IntPtr.Zero, out var result));
		return result;
	}

	public bool IsVisible(Rectangle rect, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRectI(nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, graphics, out var result));
		return result;
	}

	public bool IsVisible(RectangleF rect, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRect(nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, graphics, out var result));
		return result;
	}

	public bool IsVisible(float x, float y)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPoint(nativeRegion, x, y, IntPtr.Zero, out var result));
		return result;
	}

	public bool IsVisible(float x, float y, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionPoint(nativeRegion, x, y, graphics, out var result));
		return result;
	}

	public bool IsVisible(float x, float y, float width, float height)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRect(nativeRegion, x, y, width, height, IntPtr.Zero, out var result));
		return result;
	}

	public bool IsVisible(float x, float y, float width, float height, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipIsVisibleRegionRect(nativeRegion, x, y, width, height, graphics, out var result));
		return result;
	}

	public bool IsEmpty(Graphics g)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipIsEmptyRegion(nativeRegion, g.NativeObject, out var result));
		return result;
	}

	public bool IsInfinite(Graphics g)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipIsInfiniteRegion(nativeRegion, g.NativeObject, out var result));
		return result;
	}

	public void MakeEmpty()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetEmpty(nativeRegion));
	}

	public void MakeInfinite()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetInfinite(nativeRegion));
	}

	public bool Equals(Region region, Graphics g)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipIsEqualRegion(nativeRegion, region.NativeObject, g.NativeObject, out var result));
		return result;
	}

	public static Region FromHrgn(IntPtr hrgn)
	{
		if (hrgn == IntPtr.Zero)
		{
			throw new ArgumentException("hrgn");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCreateRegionHrgn(hrgn, out var region));
		return new Region(region);
	}

	public IntPtr GetHrgn(Graphics g)
	{
		if (g == null)
		{
			return nativeRegion;
		}
		IntPtr hRgn = IntPtr.Zero;
		GDIPlus.CheckStatus(GDIPlus.GdipGetRegionHRgn(nativeRegion, g.NativeObject, ref hRgn));
		return hRgn;
	}

	public RegionData GetRegionData()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetRegionDataSize(nativeRegion, out var bufferSize));
		byte[] array = new byte[bufferSize];
		GDIPlus.CheckStatus(GDIPlus.GdipGetRegionData(nativeRegion, array, bufferSize, out var _));
		return new RegionData(array);
	}

	public RectangleF[] GetRegionScans(Matrix matrix)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipGetRegionScansCount(nativeRegion, out var count, matrix.NativeObject));
		if (count == 0)
		{
			return new RectangleF[0];
		}
		RectangleF[] array = new RectangleF[count];
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(array[0]) * count);
		try
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetRegionScans(nativeRegion, intPtr, out count, matrix.NativeObject));
		}
		finally
		{
			GDIPlus.FromUnManagedMemoryToRectangles(intPtr, array);
		}
		return array;
	}

	public void Transform(Matrix matrix)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipTransformRegion(nativeRegion, matrix.NativeObject));
	}

	public Region Clone()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCloneRegion(nativeRegion, out var cloned));
		return new Region(cloned);
	}

	public void Dispose()
	{
		DisposeHandle();
		GC.SuppressFinalize(this);
	}

	private void DisposeHandle()
	{
		if (nativeRegion != IntPtr.Zero)
		{
			GDIPlus.GdipDeleteRegion(nativeRegion);
			nativeRegion = IntPtr.Zero;
		}
	}

	~Region()
	{
		DisposeHandle();
	}

	public void ReleaseHrgn(IntPtr regionHandle)
	{
		if (regionHandle == IntPtr.Zero)
		{
			throw new ArgumentNullException("regionHandle");
		}
		Status status = Status.Ok;
		if (GDIPlus.RunningOnUnix())
		{
			status = GDIPlus.GdipDeleteRegion(regionHandle);
		}
		else if (!GDIPlus.DeleteObject(regionHandle))
		{
			status = Status.InvalidParameter;
		}
		GDIPlus.CheckStatus(status);
	}
}
