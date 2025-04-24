using System.Runtime.CompilerServices;

namespace Mirror;

public static class NetworkWriterPool
{
	private static readonly Pool<NetworkWriterPooled> Pool = new Pool<NetworkWriterPooled>(() => new NetworkWriterPooled(), 1000);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NetworkWriterPooled Get()
	{
		NetworkWriterPooled networkWriterPooled = Pool.Get();
		networkWriterPooled.Reset();
		return networkWriterPooled;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Return(NetworkWriterPooled writer)
	{
		Pool.Return(writer);
	}
}
