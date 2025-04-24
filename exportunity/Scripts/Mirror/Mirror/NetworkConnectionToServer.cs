using System;
using System.Runtime.CompilerServices;

namespace Mirror;

public class NetworkConnectionToServer : NetworkConnection
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override void SendToTransport(ArraySegment<byte> segment, int channelId = 0)
	{
		Transport.active.ClientSend(segment, channelId);
	}

	public override void Disconnect()
	{
		isReady = false;
		NetworkClient.ready = false;
		Transport.active.ClientDisconnect();
	}
}
