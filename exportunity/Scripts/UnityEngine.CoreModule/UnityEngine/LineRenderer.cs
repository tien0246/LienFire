using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Graphics/LineRenderer.h")]
public sealed class LineRenderer : Renderer
{
	public extern float startWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float endWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float widthMultiplier
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern int numCornerVertices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern int numCapVertices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool useWorldSpace
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool loop
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Color startColor
	{
		get
		{
			get_startColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_startColor_Injected(ref value);
		}
	}

	public Color endColor
	{
		get
		{
			get_endColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_endColor_Injected(ref value);
		}
	}

	[NativeProperty("PositionsCount")]
	public extern int positionCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float shadowBias
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool generateLightingData
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern LineTextureMode textureMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern LineAlignment alignment
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public AnimationCurve widthCurve
	{
		get
		{
			return GetWidthCurveCopy();
		}
		set
		{
			SetWidthCurve(value);
		}
	}

	public Gradient colorGradient
	{
		get
		{
			return GetColorGradientCopy();
		}
		set
		{
			SetColorGradient(value);
		}
	}

	[Obsolete("Use positionCount instead (UnityUpgradable) -> positionCount", false)]
	public int numPositions
	{
		get
		{
			return positionCount;
		}
		set
		{
			positionCount = value;
		}
	}

	public void SetPosition(int index, Vector3 position)
	{
		SetPosition_Injected(index, ref position);
	}

	public Vector3 GetPosition(int index)
	{
		GetPosition_Injected(index, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void Simplify(float tolerance);

	public void BakeMesh(Mesh mesh, bool useTransform = false)
	{
		BakeMesh(mesh, Camera.main, useTransform);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void BakeMesh([NotNull("ArgumentNullException")] Mesh mesh, [NotNull("ArgumentNullException")] Camera camera, bool useTransform = false);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern AnimationCurve GetWidthCurveCopy();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetWidthCurve([NotNull("ArgumentNullException")] AnimationCurve curve);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Gradient GetColorGradientCopy();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetColorGradient([NotNull("ArgumentNullException")] Gradient curve);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "LineRendererScripting::GetPositions", HasExplicitThis = true)]
	public extern int GetPositions([Out][NotNull("ArgumentNullException")] Vector3[] positions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "LineRendererScripting::SetPositions", HasExplicitThis = true)]
	public extern void SetPositions([NotNull("ArgumentNullException")] Vector3[] positions);

	public unsafe void SetPositions(NativeArray<Vector3> positions)
	{
		SetPositionsWithNativeContainer((IntPtr)positions.GetUnsafeReadOnlyPtr(), positions.Length);
	}

	public unsafe void SetPositions(NativeSlice<Vector3> positions)
	{
		SetPositionsWithNativeContainer((IntPtr)positions.GetUnsafeReadOnlyPtr(), positions.Length);
	}

	public unsafe int GetPositions([Out] NativeArray<Vector3> positions)
	{
		return GetPositionsWithNativeContainer((IntPtr)positions.GetUnsafePtr(), positions.Length);
	}

	public unsafe int GetPositions([Out] NativeSlice<Vector3> positions)
	{
		return GetPositionsWithNativeContainer((IntPtr)positions.GetUnsafePtr(), positions.Length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "LineRendererScripting::SetPositionsWithNativeContainer", HasExplicitThis = true)]
	private extern void SetPositionsWithNativeContainer(IntPtr positions, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "LineRendererScripting::GetPositionsWithNativeContainer", HasExplicitThis = true)]
	private extern int GetPositionsWithNativeContainer(IntPtr positions, int length);

	[Obsolete("Use startWidth, endWidth or widthCurve instead.", false)]
	public void SetWidth(float start, float end)
	{
		startWidth = start;
		endWidth = end;
	}

	[Obsolete("Use startColor, endColor or colorGradient instead.", false)]
	public void SetColors(Color start, Color end)
	{
		startColor = start;
		endColor = end;
	}

	[Obsolete("Use positionCount instead.", false)]
	public void SetVertexCount(int count)
	{
		positionCount = count;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_startColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_startColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_endColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_endColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetPosition_Injected(int index, ref Vector3 position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetPosition_Injected(int index, out Vector3 ret);
}
