using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
public sealed class MetaHeader
{
	private WmfMetaHeader wmf;

	public short HeaderSize
	{
		get
		{
			return wmf.header_size;
		}
		set
		{
			wmf.header_size = value;
		}
	}

	public int MaxRecord
	{
		get
		{
			return wmf.max_record_size;
		}
		set
		{
			wmf.max_record_size = value;
		}
	}

	public short NoObjects
	{
		get
		{
			return wmf.num_of_objects;
		}
		set
		{
			wmf.num_of_objects = value;
		}
	}

	public short NoParameters
	{
		get
		{
			return wmf.num_of_params;
		}
		set
		{
			wmf.num_of_params = value;
		}
	}

	public int Size
	{
		get
		{
			if (BitConverter.IsLittleEndian)
			{
				return (wmf.file_size_high << 16) | wmf.file_size_low;
			}
			return (wmf.file_size_low << 16) | wmf.file_size_high;
		}
		set
		{
			if (BitConverter.IsLittleEndian)
			{
				wmf.file_size_high = (ushort)(value >> 16);
				wmf.file_size_low = (ushort)value;
			}
			else
			{
				wmf.file_size_high = (ushort)value;
				wmf.file_size_low = (ushort)(value >> 16);
			}
		}
	}

	public short Type
	{
		get
		{
			return wmf.file_type;
		}
		set
		{
			wmf.file_type = value;
		}
	}

	public short Version
	{
		get
		{
			return wmf.version;
		}
		set
		{
			wmf.version = value;
		}
	}

	public MetaHeader()
	{
	}

	internal MetaHeader(WmfMetaHeader header)
	{
		wmf.file_type = header.file_type;
		wmf.header_size = header.header_size;
		wmf.version = header.version;
		wmf.file_size_low = header.file_size_low;
		wmf.file_size_high = header.file_size_high;
		wmf.num_of_objects = header.num_of_objects;
		wmf.max_record_size = header.max_record_size;
		wmf.num_of_params = header.num_of_params;
	}
}
