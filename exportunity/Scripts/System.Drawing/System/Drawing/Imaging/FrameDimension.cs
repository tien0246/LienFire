namespace System.Drawing.Imaging;

public sealed class FrameDimension
{
	private static FrameDimension s_time = new FrameDimension(new Guid("{6aedbd6d-3fb5-418a-83a6-7f45229dc872}"));

	private static FrameDimension s_resolution = new FrameDimension(new Guid("{84236f7b-3bd3-428f-8dab-4ea1439ca315}"));

	private static FrameDimension s_page = new FrameDimension(new Guid("{7462dc86-6180-4c7e-8e3f-ee7333a7a483}"));

	private Guid _guid;

	public Guid Guid => _guid;

	public static FrameDimension Time => s_time;

	public static FrameDimension Resolution => s_resolution;

	public static FrameDimension Page => s_page;

	public FrameDimension(Guid guid)
	{
		_guid = guid;
	}

	public override bool Equals(object o)
	{
		if (!(o is FrameDimension frameDimension))
		{
			return false;
		}
		return _guid == frameDimension._guid;
	}

	public override int GetHashCode()
	{
		return _guid.GetHashCode();
	}

	public override string ToString()
	{
		if (this == s_time)
		{
			return "Time";
		}
		if (this == s_resolution)
		{
			return "Resolution";
		}
		if (this == s_page)
		{
			return "Page";
		}
		Guid guid = _guid;
		return "[FrameDimension: " + guid.ToString() + "]";
	}
}
