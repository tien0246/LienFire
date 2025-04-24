using System;

namespace Mirror;

public class LocalConnectionToClient : NetworkConnectionToClient
{
	internal LocalConnectionToServer connectionToServer;

	public override string address => "localhost";

	public LocalConnectionToClient()
		: base(0)
	{
	}

	internal override void Send(ArraySegment<byte> segment, int channelId = 0)
	{
		NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.WriteBytes(segment.Array, segment.Offset, segment.Count);
		connectionToServer.queue.Enqueue(networkWriterPooled);
	}

	internal override bool IsAlive(float timeout)
	{
		return true;
	}

	internal void DisconnectInternal()
	{
		isReady = false;
		RemoveFromObservingsObservers();
	}

	public override void Disconnect()
	{
		DisconnectInternal();
		connectionToServer.DisconnectInternal();
	}
}
