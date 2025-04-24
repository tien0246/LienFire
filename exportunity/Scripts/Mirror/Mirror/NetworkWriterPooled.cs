using System;

namespace Mirror;

public class NetworkWriterPooled : NetworkWriter, IDisposable
{
	public void Dispose()
	{
		NetworkWriterPool.Return(this);
	}
}
