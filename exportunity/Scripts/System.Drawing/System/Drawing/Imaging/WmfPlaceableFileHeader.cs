using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
public sealed class WmfPlaceableFileHeader
{
	private int _key = -1698247209;

	private short _hmf;

	private short _bboxLeft;

	private short _bboxTop;

	private short _bboxRight;

	private short _bboxBottom;

	private short _inch;

	private int _reserved;

	private short _checksum;

	public int Key
	{
		get
		{
			return _key;
		}
		set
		{
			_key = value;
		}
	}

	public short Hmf
	{
		get
		{
			return _hmf;
		}
		set
		{
			_hmf = value;
		}
	}

	public short BboxLeft
	{
		get
		{
			return _bboxLeft;
		}
		set
		{
			_bboxLeft = value;
		}
	}

	public short BboxTop
	{
		get
		{
			return _bboxTop;
		}
		set
		{
			_bboxTop = value;
		}
	}

	public short BboxRight
	{
		get
		{
			return _bboxRight;
		}
		set
		{
			_bboxRight = value;
		}
	}

	public short BboxBottom
	{
		get
		{
			return _bboxBottom;
		}
		set
		{
			_bboxBottom = value;
		}
	}

	public short Inch
	{
		get
		{
			return _inch;
		}
		set
		{
			_inch = value;
		}
	}

	public int Reserved
	{
		get
		{
			return _reserved;
		}
		set
		{
			_reserved = value;
		}
	}

	public short Checksum
	{
		get
		{
			return _checksum;
		}
		set
		{
			_checksum = value;
		}
	}
}
