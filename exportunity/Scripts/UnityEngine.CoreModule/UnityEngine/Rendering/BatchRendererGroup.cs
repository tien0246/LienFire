using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
[NativeHeader("Runtime/Math/Matrix4x4.h")]
public class BatchRendererGroup : IDisposable
{
	public delegate JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext);

	private IntPtr m_GroupHandle = IntPtr.Zero;

	private OnPerformCulling m_PerformCulling;

	public BatchRendererGroup(OnPerformCulling cullingCallback)
	{
		m_PerformCulling = cullingCallback;
		m_GroupHandle = Create(this);
	}

	public void Dispose()
	{
		Destroy(m_GroupHandle);
		m_GroupHandle = IntPtr.Zero;
	}

	public int AddBatch(Mesh mesh, int subMeshIndex, Material material, int layer, ShadowCastingMode castShadows, bool receiveShadows, bool invertCulling, Bounds bounds, int instanceCount, MaterialPropertyBlock customProps, GameObject associatedSceneObject, ulong sceneCullingMask = 9223372036854775808uL, uint renderingLayerMask = uint.MaxValue)
	{
		return AddBatch_Injected(mesh, subMeshIndex, material, layer, castShadows, receiveShadows, invertCulling, ref bounds, instanceCount, customProps, associatedSceneObject, sceneCullingMask, renderingLayerMask);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetBatchFlags(int batchIndex, ulong flags);

	public unsafe void SetBatchPropertyMetadata(int batchIndex, NativeArray<int> cbufferLengths, NativeArray<int> cbufferMetadata)
	{
		InternalSetBatchPropertyMetadata(batchIndex, (IntPtr)cbufferLengths.GetUnsafeReadOnlyPtr(), cbufferLengths.Length, (IntPtr)cbufferMetadata.GetUnsafeReadOnlyPtr(), cbufferMetadata.Length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void InternalSetBatchPropertyMetadata(int batchIndex, IntPtr cbufferLengths, int cbufferLengthsCount, IntPtr cbufferMetadata, int cbufferMetadataCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetInstancingData(int batchIndex, int instanceCount, MaterialPropertyBlock customProps);

	public unsafe NativeArray<Matrix4x4> GetBatchMatrices(int batchIndex)
	{
		int matrixCount = 0;
		void* batchMatrices = GetBatchMatrices(batchIndex, out matrixCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(batchMatrices, matrixCount, Allocator.Invalid);
	}

	public unsafe NativeArray<int> GetBatchScalarArrayInt(int batchIndex, string propertyName)
	{
		int elementCount = 0;
		void* batchScalarArray = GetBatchScalarArray(batchIndex, propertyName, out elementCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(batchScalarArray, elementCount, Allocator.Invalid);
	}

	public unsafe NativeArray<float> GetBatchScalarArray(int batchIndex, string propertyName)
	{
		int elementCount = 0;
		void* batchScalarArray = GetBatchScalarArray(batchIndex, propertyName, out elementCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(batchScalarArray, elementCount, Allocator.Invalid);
	}

	public unsafe NativeArray<int> GetBatchVectorArrayInt(int batchIndex, string propertyName)
	{
		int elementCount = 0;
		void* batchVectorArray = GetBatchVectorArray(batchIndex, propertyName, out elementCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(batchVectorArray, elementCount, Allocator.Invalid);
	}

	public unsafe NativeArray<Vector4> GetBatchVectorArray(int batchIndex, string propertyName)
	{
		int elementCount = 0;
		void* batchVectorArray = GetBatchVectorArray(batchIndex, propertyName, out elementCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(batchVectorArray, elementCount, Allocator.Invalid);
	}

	public unsafe NativeArray<Matrix4x4> GetBatchMatrixArray(int batchIndex, string propertyName)
	{
		int elementCount = 0;
		void* batchMatrixArray = GetBatchMatrixArray(batchIndex, propertyName, out elementCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(batchMatrixArray, elementCount, Allocator.Invalid);
	}

	public unsafe NativeArray<int> GetBatchScalarArrayInt(int batchIndex, int propertyName)
	{
		int elementCount = 0;
		void* batchScalarArray_Internal = GetBatchScalarArray_Internal(batchIndex, propertyName, out elementCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(batchScalarArray_Internal, elementCount, Allocator.Invalid);
	}

	public unsafe NativeArray<float> GetBatchScalarArray(int batchIndex, int propertyName)
	{
		int elementCount = 0;
		void* batchScalarArray_Internal = GetBatchScalarArray_Internal(batchIndex, propertyName, out elementCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(batchScalarArray_Internal, elementCount, Allocator.Invalid);
	}

	public unsafe NativeArray<int> GetBatchVectorArrayInt(int batchIndex, int propertyName)
	{
		int elementCount = 0;
		void* batchVectorArray_Internal = GetBatchVectorArray_Internal(batchIndex, propertyName, out elementCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(batchVectorArray_Internal, elementCount, Allocator.Invalid);
	}

	public unsafe NativeArray<Vector4> GetBatchVectorArray(int batchIndex, int propertyName)
	{
		int elementCount = 0;
		void* batchVectorArray_Internal = GetBatchVectorArray_Internal(batchIndex, propertyName, out elementCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(batchVectorArray_Internal, elementCount, Allocator.Invalid);
	}

	public unsafe NativeArray<Matrix4x4> GetBatchMatrixArray(int batchIndex, int propertyName)
	{
		int elementCount = 0;
		void* batchMatrixArray_Internal = GetBatchMatrixArray_Internal(batchIndex, propertyName, out elementCount);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(batchMatrixArray_Internal, elementCount, Allocator.Invalid);
	}

	public void SetBatchBounds(int batchIndex, Bounds bounds)
	{
		SetBatchBounds_Injected(batchIndex, ref bounds);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern int GetNumBatches();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void RemoveBatch(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe extern void* GetBatchMatrices(int batchIndex, out int matrixCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe extern void* GetBatchScalarArray(int batchIndex, string propertyName, out int elementCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe extern void* GetBatchVectorArray(int batchIndex, string propertyName, out int elementCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe extern void* GetBatchMatrixArray(int batchIndex, string propertyName, out int elementCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetBatchScalarArray")]
	private unsafe extern void* GetBatchScalarArray_Internal(int batchIndex, int propertyName, out int elementCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetBatchVectorArray")]
	private unsafe extern void* GetBatchVectorArray_Internal(int batchIndex, int propertyName, out int elementCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetBatchMatrixArray")]
	private unsafe extern void* GetBatchMatrixArray_Internal(int batchIndex, int propertyName, out int elementCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void EnableVisibleIndicesYArray(bool enabled);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create(BatchRendererGroup group);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Destroy(IntPtr groupHandle);

	[RequiredByNativeCode]
	private unsafe static void InvokeOnPerformCulling(BatchRendererGroup group, ref BatchRendererCullingOutput context, ref LODParameters lodParameters)
	{
		NativeArray<Plane> inCullingPlanes = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Plane>(context.cullingPlanes, context.cullingPlanesCount, Allocator.Invalid);
		NativeArray<BatchVisibility> inOutBatchVisibility = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<BatchVisibility>(context.batchVisibility, context.batchVisibilityCount, Allocator.Invalid);
		NativeArray<int> outVisibleIndices = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(context.visibleIndices, context.visibleIndicesCount, Allocator.Invalid);
		NativeArray<int> outVisibleIndicesY = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(context.visibleIndicesY, context.visibleIndicesCount, Allocator.Invalid);
		try
		{
			context.cullingJobsFence = group.m_PerformCulling(group, new BatchCullingContext(inCullingPlanes, inOutBatchVisibility, outVisibleIndices, outVisibleIndicesY, lodParameters, context.cullingMatrix, context.nearPlane));
		}
		finally
		{
			JobHandle.ScheduleBatchedJobs();
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int AddBatch_Injected(Mesh mesh, int subMeshIndex, Material material, int layer, ShadowCastingMode castShadows, bool receiveShadows, bool invertCulling, ref Bounds bounds, int instanceCount, MaterialPropertyBlock customProps, GameObject associatedSceneObject, ulong sceneCullingMask = 9223372036854775808uL, uint renderingLayerMask = uint.MaxValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetBatchBounds_Injected(int batchIndex, ref Bounds bounds);
}
