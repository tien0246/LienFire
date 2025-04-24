using System;

namespace Mirror;

public struct RpcMessage : NetworkMessage
{
	public uint netId;

	public byte componentIndex;

	public ushort functionHash;

	public ArraySegment<byte> payload;
}
