using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Drawing.Printing;

[Serializable]
[TypeConverter(typeof(MarginsConverter))]
public class Margins : ICloneable
{
	private int _left;

	private int _right;

	private int _bottom;

	private int _top;

	[OptionalField]
	private double _doubleLeft;

	[OptionalField]
	private double _doubleRight;

	[OptionalField]
	private double _doubleTop;

	[OptionalField]
	private double _doubleBottom;

	public int Left
	{
		get
		{
			return _left;
		}
		set
		{
			CheckMargin(value, "Left");
			_left = value;
			_doubleLeft = value;
		}
	}

	public int Right
	{
		get
		{
			return _right;
		}
		set
		{
			CheckMargin(value, "Right");
			_right = value;
			_doubleRight = value;
		}
	}

	public int Top
	{
		get
		{
			return _top;
		}
		set
		{
			CheckMargin(value, "Top");
			_top = value;
			_doubleTop = value;
		}
	}

	public int Bottom
	{
		get
		{
			return _bottom;
		}
		set
		{
			CheckMargin(value, "Bottom");
			_bottom = value;
			_doubleBottom = value;
		}
	}

	internal double DoubleLeft
	{
		get
		{
			return _doubleLeft;
		}
		set
		{
			Left = (int)Math.Round(value);
			_doubleLeft = value;
		}
	}

	internal double DoubleRight
	{
		get
		{
			return _doubleRight;
		}
		set
		{
			Right = (int)Math.Round(value);
			_doubleRight = value;
		}
	}

	internal double DoubleTop
	{
		get
		{
			return _doubleTop;
		}
		set
		{
			Top = (int)Math.Round(value);
			_doubleTop = value;
		}
	}

	internal double DoubleBottom
	{
		get
		{
			return _doubleBottom;
		}
		set
		{
			Bottom = (int)Math.Round(value);
			_doubleBottom = value;
		}
	}

	[OnDeserialized]
	private void OnDeserializedMethod(StreamingContext context)
	{
		if (_doubleLeft == 0.0 && _left != 0)
		{
			_doubleLeft = _left;
		}
		if (_doubleRight == 0.0 && _right != 0)
		{
			_doubleRight = _right;
		}
		if (_doubleTop == 0.0 && _top != 0)
		{
			_doubleTop = _top;
		}
		if (_doubleBottom == 0.0 && _bottom != 0)
		{
			_doubleBottom = _bottom;
		}
	}

	public Margins()
		: this(100, 100, 100, 100)
	{
	}

	public Margins(int left, int right, int top, int bottom)
	{
		CheckMargin(left, "left");
		CheckMargin(right, "right");
		CheckMargin(top, "top");
		CheckMargin(bottom, "bottom");
		_left = left;
		_right = right;
		_top = top;
		_bottom = bottom;
		_doubleLeft = left;
		_doubleRight = right;
		_doubleTop = top;
		_doubleBottom = bottom;
	}

	private void CheckMargin(int margin, string name)
	{
		if (margin < 0)
		{
			throw new ArgumentException(global::SR.Format("Value of '{1}' is not valid for '{0}'. '{0}' must be greater than or equal to {2}.", name, margin, "0"));
		}
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public override bool Equals(object obj)
	{
		Margins margins = obj as Margins;
		if (margins == this)
		{
			return true;
		}
		if (margins == null)
		{
			return false;
		}
		if (margins.Left == Left && margins.Right == Right && margins.Top == Top)
		{
			return margins.Bottom == Bottom;
		}
		return false;
	}

	public override int GetHashCode()
	{
		int left = Left;
		uint right = (uint)Right;
		uint top = (uint)Top;
		uint bottom = (uint)Bottom;
		return (int)((uint)left ^ ((right << 13) | (right >> 19)) ^ ((top << 26) | (top >> 6)) ^ ((bottom << 7) | (bottom >> 25)));
	}

	public static bool operator ==(Margins m1, Margins m2)
	{
		if ((object)m1 == null != ((object)m2 == null))
		{
			return false;
		}
		if ((object)m1 != null)
		{
			if (m1.Left == m2.Left && m1.Top == m2.Top && m1.Right == m2.Right)
			{
				return m1.Bottom == m2.Bottom;
			}
			return false;
		}
		return true;
	}

	public static bool operator !=(Margins m1, Margins m2)
	{
		return !(m1 == m2);
	}

	public override string ToString()
	{
		return "[Margins Left=" + Left.ToString(CultureInfo.InvariantCulture) + " Right=" + Right.ToString(CultureInfo.InvariantCulture) + " Top=" + Top.ToString(CultureInfo.InvariantCulture) + " Bottom=" + Bottom.ToString(CultureInfo.InvariantCulture) + "]";
	}
}
