using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[NativeHeader("Runtime/Shaders/GraphicsBuffer.h")]
[NativeHeader("Runtime/Export/Graphics/GraphicsBuffer.bindings.h")]
public sealed class GraphicsBuffer : IDisposable
{
	[Flags]
	public enum Target
	{
		Vertex = 1,
		Index = 2,
		CopySource = 4,
		CopyDestination = 8,
		Structured = 0x10,
		Raw = 0x20,
		Append = 0x40,
		Counter = 0x80,
		IndirectArguments = 0x100,
		Constant = 0x200
	}

	public struct IndirectDrawArgs
	{
		public const int size = 16;

		public uint vertexCountPerInstance { get; set; }

		public uint instanceCount { get; set; }

		public uint startVertex { get; set; }

		public uint startInstance { get; set; }
	}

	public struct IndirectDrawIndexedArgs
	{
		public const int size = 20;

		public uint indexCountPerInstance { get; set; }

		public uint instanceCount { get; set; }

		public uint startIndex { get; set; }

		public uint baseVertexIndex { get; set; }

		public uint startInstance { get; set; }
	}

	internal IntPtr m_Ptr;

	public extern int count
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public extern int stride
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public extern Target target
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public string name
	{
		set
		{
			SetName(value);
		}
	}

	~GraphicsBuffer()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			DestroyBuffer(this);
		}
		else if (m_Ptr != IntPtr.Zero)
		{
			Debug.LogWarning("GarbageCollector disposing of GraphicsBuffer. Please use GraphicsBuffer.Release() or .Dispose() to manually release the buffer.");
		}
		m_Ptr = IntPtr.Zero;
	}

	private static bool RequiresCompute(Target target)
	{
		Target target2 = Target.Structured | Target.Raw | Target.Append | Target.Counter | Target.IndirectArguments;
		return (target & target2) != 0;
	}

	private static bool IsVertexIndexOrCopyOnly(Target target)
	{
		Target target2 = Target.Vertex | Target.Index | Target.CopySource | Target.CopyDestination;
		return (target & target2) == target;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsBuffer_Bindings::InitBuffer")]
	private static extern IntPtr InitBuffer(Target target, int count, int stride);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsBuffer_Bindings::DestroyBuffer")]
	private static extern void DestroyBuffer(GraphicsBuffer buf);

	public GraphicsBuffer(Target target, int count, int stride)
	{
		if (RequiresCompute(target) && !SystemInfo.supportsComputeShaders)
		{
			throw new ArgumentException("Attempting to create a graphics buffer that requires compute shader support, but compute shaders are not supported on this platform. Target: " + target);
		}
		if (count <= 0)
		{
			throw new ArgumentException("Attempting to create a zero length graphics buffer", "count");
		}
		if (stride <= 0)
		{
			throw new ArgumentException("Attempting to create a graphics buffer with a negative or null stride", "stride");
		}
		if ((target & Target.Index) != 0 && stride != 2 && stride != 4)
		{
			throw new ArgumentException("Attempting to create an index buffer with an invalid stride: " + stride, "stride");
		}
		if (!IsVertexIndexOrCopyOnly(target) && stride % 4 != 0)
		{
			throw new ArgumentException("Stride must be a multiple of 4 unless the buffer is only used as a vertex buffer and/or index buffer ", "stride");
		}
		long num = (long)count * (long)stride;
		long maxGraphicsBufferSize = SystemInfo.maxGraphicsBufferSize;
		if (num > maxGraphicsBufferSize)
		{
			throw new ArgumentException($"The total size of the graphics buffer ({num} bytes) exceeds the maximum buffer size. Maximum supported buffer size: {maxGraphicsBufferSize} bytes.");
		}
		m_Ptr = InitBuffer(target, count, stride);
	}

	public void Release()
	{
		Dispose();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsBuffer_Bindings::IsValidBuffer")]
	private static extern bool IsValidBuffer(GraphicsBuffer buf);

	public bool IsValid()
	{
		return m_Ptr != IntPtr.Zero && IsValidBuffer(this);
	}

	[SecuritySafeCritical]
	public void SetData(Array data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException($"Array passed to GraphicsBuffer.SetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
		}
		InternalSetData(data, 0, 0, data.Length, UnsafeUtility.SizeOf(data.GetType().GetElementType()));
	}

	[SecuritySafeCritical]
	public void SetData<T>(List<T> data) where T : struct
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsGenericListBlittable<T>())
		{
			throw new ArgumentException($"List<{typeof(T)}> passed to GraphicsBuffer.SetData(List<>) must be blittable.\n{UnsafeUtility.GetReasonForGenericListNonBlittable<T>()}");
		}
		InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), 0, 0, NoAllocHelpers.SafeLength(data), Marshal.SizeOf(typeof(T)));
	}

	[SecuritySafeCritical]
	public unsafe void SetData<T>(NativeArray<T> data) where T : struct
	{
		InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr(), 0, 0, data.Length, UnsafeUtility.SizeOf<T>());
	}

	[SecuritySafeCritical]
	public void SetData(Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException($"Array passed to GraphicsBuffer.SetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
		}
		if (managedBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (managedBufferStartIndex:{managedBufferStartIndex} graphicsBufferStartIndex:{graphicsBufferStartIndex} count:{count})");
		}
		InternalSetData(data, managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[SecuritySafeCritical]
	public void SetData<T>(List<T> data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsGenericListBlittable<T>())
		{
			throw new ArgumentException($"List<{typeof(T)}> passed to GraphicsBuffer.SetData(List<>) must be blittable.\n{UnsafeUtility.GetReasonForGenericListNonBlittable<T>()}");
		}
		if (managedBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Count)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (managedBufferStartIndex:{managedBufferStartIndex} graphicsBufferStartIndex:{graphicsBufferStartIndex} count:{count})");
		}
		InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(typeof(T)));
	}

	[SecuritySafeCritical]
	public unsafe void SetData<T>(NativeArray<T> data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
	{
		if (nativeBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || nativeBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (nativeBufferStartIndex:{nativeBufferStartIndex} graphicsBufferStartIndex:{graphicsBufferStartIndex} count:{count})");
		}
		InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr(), nativeBufferStartIndex, graphicsBufferStartIndex, count, UnsafeUtility.SizeOf<T>());
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetNativeData", HasExplicitThis = true, ThrowsException = true)]
	[SecurityCritical]
	private extern void InternalSetNativeData(IntPtr data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetData", HasExplicitThis = true, ThrowsException = true)]
	[SecurityCritical]
	private extern void InternalSetData(Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

	[SecurityCritical]
	public void GetData(Array data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException($"Array passed to GraphicsBuffer.GetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
		}
		InternalGetData(data, 0, 0, data.Length, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[SecurityCritical]
	public void GetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException($"Array passed to GraphicsBuffer.GetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
		}
		if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count argument (managedBufferStartIndex:{managedBufferStartIndex} computeBufferStartIndex:{computeBufferStartIndex} count:{count})");
		}
		InternalGetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SecurityCritical]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalGetData", HasExplicitThis = true, ThrowsException = true)]
	private extern void InternalGetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalGetNativeBufferPtr", HasExplicitThis = true)]
	public extern IntPtr GetNativeBufferPtr();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::SetName", HasExplicitThis = true)]
	private extern void SetName(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetCounterValue(uint counterValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
	private static extern void CopyCountCC(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
	private static extern void CopyCountGC(GraphicsBuffer src, ComputeBuffer dst, int dstOffsetBytes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
	private static extern void CopyCountCG(ComputeBuffer src, GraphicsBuffer dst, int dstOffsetBytes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
	private static extern void CopyCountGG(GraphicsBuffer src, GraphicsBuffer dst, int dstOffsetBytes);

	public static void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes)
	{
		CopyCountCC(src, dst, dstOffsetBytes);
	}

	public static void CopyCount(GraphicsBuffer src, ComputeBuffer dst, int dstOffsetBytes)
	{
		CopyCountGC(src, dst, dstOffsetBytes);
	}

	public static void CopyCount(ComputeBuffer src, GraphicsBuffer dst, int dstOffsetBytes)
	{
		CopyCountCG(src, dst, dstOffsetBytes);
	}

	public static void CopyCount(GraphicsBuffer src, GraphicsBuffer dst, int dstOffsetBytes)
	{
		CopyCountGG(src, dst, dstOffsetBytes);
	}
}
