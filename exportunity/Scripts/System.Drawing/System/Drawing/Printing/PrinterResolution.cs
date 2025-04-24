using System.ComponentModel;
using System.Globalization;

namespace System.Drawing.Printing;

[Serializable]
public class PrinterResolution
{
	private int _x;

	private int _y;

	private PrinterResolutionKind _kind;

	public PrinterResolutionKind Kind
	{
		get
		{
			return _kind;
		}
		set
		{
			if (value < PrinterResolutionKind.High || value > PrinterResolutionKind.Custom)
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(PrinterResolutionKind));
			}
			_kind = value;
		}
	}

	public int X
	{
		get
		{
			return _x;
		}
		set
		{
			_x = value;
		}
	}

	public int Y
	{
		get
		{
			return _y;
		}
		set
		{
			_y = value;
		}
	}

	public PrinterResolution()
	{
		_kind = PrinterResolutionKind.Custom;
	}

	internal PrinterResolution(PrinterResolutionKind kind, int x, int y)
	{
		_kind = kind;
		_x = x;
		_y = y;
	}

	public override string ToString()
	{
		if (_kind != PrinterResolutionKind.Custom)
		{
			return "[PrinterResolution " + Kind.ToString() + "]";
		}
		return "[PrinterResolution X=" + X.ToString(CultureInfo.InvariantCulture) + " Y=" + Y.ToString(CultureInfo.InvariantCulture) + "]";
	}
}
