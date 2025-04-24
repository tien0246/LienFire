using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel;

[Serializable]
[UsedByNativeCode]
internal struct MarkPositionAdjustment
{
	[SerializeField]
	[NativeName("xCoordinate")]
	private float m_XPositionAdjustment;

	[NativeName("yCoordinate")]
	[SerializeField]
	private float m_YPositionAdjustment;

	public float xPositionAdjustment
	{
		get
		{
			return m_XPositionAdjustment;
		}
		set
		{
			m_XPositionAdjustment = value;
		}
	}

	public float yPositionAdjustment
	{
		get
		{
			return m_YPositionAdjustment;
		}
		set
		{
			m_YPositionAdjustment = value;
		}
	}

	public MarkPositionAdjustment(float x, float y)
	{
		m_XPositionAdjustment = x;
		m_YPositionAdjustment = y;
	}
}
