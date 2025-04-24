using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[UsedByNativeCode]
public struct ShadowSplitData : IEquatable<ShadowSplitData>
{
	private const int k_MaximumCullingPlaneCount = 10;

	public static readonly int maximumCullingPlaneCount = 10;

	private int m_CullingPlaneCount;

	internal unsafe fixed byte m_CullingPlanes[160];

	private Vector4 m_CullingSphere;

	private float m_ShadowCascadeBlendCullingFactor;

	private float m_CullingNearPlane;

	public int cullingPlaneCount
	{
		get
		{
			return m_CullingPlaneCount;
		}
		set
		{
			if (value < 0 || value > 10)
			{
				throw new ArgumentException($"Value should range from {0} to ShadowSplitData.maximumCullingPlaneCount ({10}), but was {value}.");
			}
			m_CullingPlaneCount = value;
		}
	}

	public Vector4 cullingSphere
	{
		get
		{
			return m_CullingSphere;
		}
		set
		{
			m_CullingSphere = value;
		}
	}

	public float shadowCascadeBlendCullingFactor
	{
		get
		{
			return m_ShadowCascadeBlendCullingFactor;
		}
		set
		{
			if (value < 0f || value > 1f)
			{
				throw new ArgumentException($"Value should range from {0} to {1}, but was {value}.");
			}
			m_ShadowCascadeBlendCullingFactor = value;
		}
	}

	public unsafe Plane GetCullingPlane(int index)
	{
		if (index < 0 || index >= cullingPlaneCount)
		{
			throw new ArgumentException("index", $"Index should be at least {0} and less than cullingPlaneCount ({cullingPlaneCount}), but was {index}.");
		}
		fixed (byte* cullingPlanes = m_CullingPlanes)
		{
			Plane* ptr = (Plane*)cullingPlanes;
			return ptr[index];
		}
	}

	public unsafe void SetCullingPlane(int index, Plane plane)
	{
		if (index < 0 || index >= cullingPlaneCount)
		{
			throw new ArgumentException("index", $"Index should be at least {0} and less than cullingPlaneCount ({cullingPlaneCount}), but was {index}.");
		}
		fixed (byte* cullingPlanes = m_CullingPlanes)
		{
			Plane* ptr = (Plane*)cullingPlanes;
			ptr[index] = plane;
		}
	}

	public bool Equals(ShadowSplitData other)
	{
		if (m_CullingPlaneCount != other.m_CullingPlaneCount)
		{
			return false;
		}
		for (int i = 0; i < cullingPlaneCount; i++)
		{
			if (!GetCullingPlane(i).Equals(other.GetCullingPlane(i)))
			{
				return false;
			}
		}
		return m_CullingSphere.Equals(other.m_CullingSphere);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return obj is ShadowSplitData && Equals((ShadowSplitData)obj);
	}

	public override int GetHashCode()
	{
		return (m_CullingPlaneCount * 397) ^ m_CullingSphere.GetHashCode();
	}

	public static bool operator ==(ShadowSplitData left, ShadowSplitData right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ShadowSplitData left, ShadowSplitData right)
	{
		return !left.Equals(right);
	}
}
