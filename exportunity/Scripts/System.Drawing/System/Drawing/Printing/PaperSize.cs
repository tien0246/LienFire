using System.Globalization;

namespace System.Drawing.Printing;

[Serializable]
public class PaperSize
{
	private PaperKind _kind;

	private string _name;

	private int _width;

	private int _height;

	private bool _createdByDefaultConstructor;

	public int Height
	{
		get
		{
			return _height;
		}
		set
		{
			if (_kind != PaperKind.Custom && !_createdByDefaultConstructor)
			{
				throw new ArgumentException(global::SR.Format("PaperSize cannot be changed unless the Kind property is set to Custom."));
			}
			_height = value;
		}
	}

	public PaperKind Kind
	{
		get
		{
			if (_kind <= PaperKind.PrcEnvelopeNumber10Rotated && _kind != (PaperKind)48 && _kind != (PaperKind)49)
			{
				return _kind;
			}
			return PaperKind.Custom;
		}
	}

	public string PaperName
	{
		get
		{
			return _name;
		}
		set
		{
			if (_kind != PaperKind.Custom && !_createdByDefaultConstructor)
			{
				throw new ArgumentException(global::SR.Format("PaperSize cannot be changed unless the Kind property is set to Custom."));
			}
			_name = value;
		}
	}

	public int RawKind
	{
		get
		{
			return (int)_kind;
		}
		set
		{
			_kind = (PaperKind)value;
		}
	}

	public int Width
	{
		get
		{
			return _width;
		}
		set
		{
			if (_kind != PaperKind.Custom && !_createdByDefaultConstructor)
			{
				throw new ArgumentException(global::SR.Format("PaperSize cannot be changed unless the Kind property is set to Custom."));
			}
			_width = value;
		}
	}

	public PaperSize()
	{
		_kind = PaperKind.Custom;
		_name = string.Empty;
		_createdByDefaultConstructor = true;
	}

	internal PaperSize(PaperKind kind, string name, int width, int height)
	{
		_kind = kind;
		_name = name;
		_width = width;
		_height = height;
	}

	public PaperSize(string name, int width, int height)
	{
		_kind = PaperKind.Custom;
		_name = name;
		_width = width;
		_height = height;
	}

	public override string ToString()
	{
		return "[PaperSize " + PaperName + " Kind=" + Kind.ToString() + " Height=" + Height.ToString(CultureInfo.InvariantCulture) + " Width=" + Width.ToString(CultureInfo.InvariantCulture) + "]";
	}
}
