using System.Drawing.Internal;
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D;

public sealed class GraphicsPathIterator : MarshalByRefObject, IDisposable
{
	internal IntPtr nativeIter;

	public int Count
	{
		get
		{
			int count;
			int num = GDIPlus.GdipPathIterGetCount(new HandleRef(this, nativeIter), out count);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return count;
		}
	}

	public int SubpathCount
	{
		get
		{
			int count;
			int num = GDIPlus.GdipPathIterGetSubpathCount(new HandleRef(this, nativeIter), out count);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return count;
		}
	}

	public GraphicsPathIterator(GraphicsPath path)
	{
		IntPtr iterator = IntPtr.Zero;
		int num = GDIPlus.GdipCreatePathIter(out iterator, new HandleRef(path, path?.nativePath ?? IntPtr.Zero));
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		nativeIter = iterator;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!(nativeIter != IntPtr.Zero))
		{
			return;
		}
		try
		{
			GDIPlus.GdipDeletePathIter(new HandleRef(this, nativeIter));
		}
		catch (Exception ex)
		{
			if (ClientUtils.IsSecurityOrCriticalException(ex))
			{
				throw;
			}
		}
		finally
		{
			nativeIter = IntPtr.Zero;
		}
	}

	~GraphicsPathIterator()
	{
		Dispose(disposing: false);
	}

	public int NextSubpath(out int startIndex, out int endIndex, out bool isClosed)
	{
		int resultCount;
		int startIndex2;
		int endIndex2;
		int num = GDIPlus.GdipPathIterNextSubpath(new HandleRef(this, nativeIter), out resultCount, out startIndex2, out endIndex2, out isClosed);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		startIndex = startIndex2;
		endIndex = endIndex2;
		return resultCount;
	}

	public int NextSubpath(GraphicsPath path, out bool isClosed)
	{
		int resultCount;
		int num = GDIPlus.GdipPathIterNextSubpathPath(new HandleRef(this, nativeIter), out resultCount, new HandleRef(path, path?.nativePath ?? IntPtr.Zero), out isClosed);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		return resultCount;
	}

	public int NextPathType(out byte pathType, out int startIndex, out int endIndex)
	{
		int resultCount;
		int num = GDIPlus.GdipPathIterNextPathType(new HandleRef(this, nativeIter), out resultCount, out pathType, out startIndex, out endIndex);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		return resultCount;
	}

	public int NextMarker(out int startIndex, out int endIndex)
	{
		int resultCount;
		int num = GDIPlus.GdipPathIterNextMarker(new HandleRef(this, nativeIter), out resultCount, out startIndex, out endIndex);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		return resultCount;
	}

	public int NextMarker(GraphicsPath path)
	{
		int resultCount;
		int num = GDIPlus.GdipPathIterNextMarkerPath(new HandleRef(this, nativeIter), out resultCount, new HandleRef(path, path?.nativePath ?? IntPtr.Zero));
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		return resultCount;
	}

	public bool HasCurve()
	{
		bool curve;
		int num = GDIPlus.GdipPathIterHasCurve(new HandleRef(this, nativeIter), out curve);
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
		return curve;
	}

	public void Rewind()
	{
		int num = GDIPlus.GdipPathIterRewind(new HandleRef(this, nativeIter));
		if (num != 0)
		{
			throw SafeNativeMethods.Gdip.StatusException(num);
		}
	}

	public unsafe int Enumerate(ref PointF[] points, ref byte[] types)
	{
		if (points.Length != types.Length)
		{
			throw SafeNativeMethods.Gdip.StatusException(2);
		}
		int resultCount = 0;
		int num = Marshal.SizeOf(typeof(GPPOINTF));
		int num2 = points.Length;
		byte[] array = new byte[num2];
		IntPtr intPtr = Marshal.AllocHGlobal(checked(num2 * num));
		try
		{
			int num3 = GDIPlus.GdipPathIterEnumerate(new HandleRef(this, nativeIter), out resultCount, intPtr, array, num2);
			if (num3 != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num3);
			}
			if (resultCount < num2)
			{
				SafeNativeMethods.ZeroMemory((byte*)checked((long)intPtr + resultCount * num), (ulong)((num2 - resultCount) * num));
			}
			points = SafeNativeMethods.Gdip.ConvertGPPOINTFArrayF(intPtr, num2);
			array.CopyTo(types, 0);
			return resultCount;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public unsafe int CopyData(ref PointF[] points, ref byte[] types, int startIndex, int endIndex)
	{
		if (points.Length != types.Length || endIndex - startIndex + 1 > points.Length)
		{
			throw SafeNativeMethods.Gdip.StatusException(2);
		}
		int resultCount = 0;
		int num = Marshal.SizeOf(typeof(GPPOINTF));
		int num2 = points.Length;
		byte[] array = new byte[num2];
		IntPtr intPtr = Marshal.AllocHGlobal(checked(num2 * num));
		try
		{
			int num3 = GDIPlus.GdipPathIterCopyData(new HandleRef(this, nativeIter), out resultCount, intPtr, array, startIndex, endIndex);
			if (num3 != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num3);
			}
			if (resultCount < num2)
			{
				SafeNativeMethods.ZeroMemory((byte*)checked((long)intPtr + resultCount * num), (ulong)((num2 - resultCount) * num));
			}
			points = SafeNativeMethods.Gdip.ConvertGPPOINTFArrayF(intPtr, num2);
			array.CopyTo(types, 0);
			return resultCount;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}
}
