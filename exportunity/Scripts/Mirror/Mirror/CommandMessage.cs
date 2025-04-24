using System;

namespace Mirror;

public struct CommandMessage : NetworkMessage
{
	public uint netId;

	public byte componentIndex;

	public ushort functionHash;

	public ArraySegment<byte> payload;
}
