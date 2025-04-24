using System;
using System.Runtime.CompilerServices;

namespace Mirror;

public static class NetworkReaderPool
{
	private static readonly Pool<NetworkReaderPooled> Pool = new Pool<NetworkReaderPooled>(() => new NetworkReaderPooled(new byte[0]), 1000);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NetworkReaderPooled Get(byte[] bytes)
	{
		NetworkReaderPooled networkReaderPooled = Pool.Get();
		networkReaderPooled.SetBuffer(bytes);
		return networkReaderPooled;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NetworkReaderPooled Get(ArraySegment<byte> segment)
	{
		NetworkReaderPooled networkReaderPooled = Pool.Get();
		networkReaderPooled.SetBuffer(segment);
		return networkReaderPooled;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Return(NetworkReaderPooled reader)
	{
		Pool.Return(reader);
	}
}
