using System;

namespace Mirror;

public struct RpcBufferMessage : NetworkMessage
{
	public ArraySegment<byte> payload;
}
