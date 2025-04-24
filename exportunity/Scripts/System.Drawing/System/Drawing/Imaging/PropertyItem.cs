namespace System.Drawing.Imaging;

public sealed class PropertyItem
{
	private int _id;

	private int _len;

	private short _type;

	private byte[] _value;

	public int Id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public int Len
	{
		get
		{
			return _len;
		}
		set
		{
			_len = value;
		}
	}

	public short Type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
		}
	}

	public byte[] Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	internal PropertyItem()
	{
	}
}
