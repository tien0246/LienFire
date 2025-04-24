using System;
using System.Globalization;

namespace UnityEngine.UIElements;

public struct Angle : IEquatable<Angle>
{
	private enum Unit
	{
		Degree = 0,
		Gradian = 1,
		Radian = 2,
		Turn = 3,
		None = 4
	}

	private float m_Value;

	private Unit m_Unit;

	public float value
	{
		get
		{
			return m_Value;
		}
		set
		{
			m_Value = value;
		}
	}

	public AngleUnit unit
	{
		get
		{
			return (AngleUnit)m_Unit;
		}
		set
		{
			m_Unit = (Unit)value;
		}
	}

	public static Angle Degrees(float value)
	{
		return new Angle(value, AngleUnit.Degree);
	}

	internal static Angle None()
	{
		return new Angle(0f, Unit.None);
	}

	internal bool IsNone()
	{
		return m_Unit == Unit.None;
	}

	public Angle(float value)
		: this(value, Unit.Degree)
	{
	}

	public Angle(float value, AngleUnit unit)
		: this(value, (Unit)unit)
	{
	}

	private Angle(float value, Unit unit)
	{
		m_Value = value;
		m_Unit = unit;
	}

	public float ToDegrees()
	{
		return m_Unit switch
		{
			Unit.Degree => m_Value, 
			Unit.Gradian => m_Value * 360f / 400f, 
			Unit.Radian => m_Value * 180f / (float)Math.PI, 
			Unit.Turn => m_Value * 360f, 
			Unit.None => 0f, 
			_ => 0f, 
		};
	}

	public static implicit operator Angle(float value)
	{
		return new Angle(value, AngleUnit.Degree);
	}

	public static bool operator ==(Angle lhs, Angle rhs)
	{
		return lhs.m_Value == rhs.m_Value && lhs.m_Unit == rhs.m_Unit;
	}

	public static bool operator !=(Angle lhs, Angle rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(Angle other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is Angle other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (m_Value.GetHashCode() * 397) ^ (int)m_Unit;
	}

	public override string ToString()
	{
		string text = value.ToString(CultureInfo.InvariantCulture.NumberFormat);
		string text2 = string.Empty;
		switch (m_Unit)
		{
		case Unit.Degree:
			if (!Mathf.Approximately(0f, value))
			{
				text2 = "deg";
			}
			break;
		case Unit.Gradian:
			text2 = "grad";
			break;
		case Unit.Radian:
			text2 = "rad";
			break;
		case Unit.Turn:
			text2 = "turn";
			break;
		case Unit.None:
			text = "";
			break;
		}
		return text + text2;
	}
}
