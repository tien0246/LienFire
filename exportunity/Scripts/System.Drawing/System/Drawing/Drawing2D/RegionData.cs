using Unity;

namespace System.Drawing.Drawing2D;

public sealed class RegionData
{
	public byte[] Data { get; set; }

	internal RegionData(byte[] data)
	{
		Data = data;
	}

	internal RegionData()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
