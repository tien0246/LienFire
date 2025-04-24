using System.Runtime.CompilerServices;

namespace Mirror;

public static class DeltaCompression
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Compress(NetworkWriter writer, long last, long current)
	{
		Compression.CompressVarInt(writer, current - last);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Decompress(NetworkReader reader, long last)
	{
		return last + Compression.DecompressVarInt(reader);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Compress(NetworkWriter writer, Vector3Long last, Vector3Long current)
	{
		Compress(writer, last.x, current.x);
		Compress(writer, last.y, current.y);
		Compress(writer, last.z, current.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Long Decompress(NetworkReader reader, Vector3Long last)
	{
		long x = Decompress(reader, last.x);
		long y = Decompress(reader, last.y);
		long z = Decompress(reader, last.z);
		return new Vector3Long(x, y, z);
	}
}
