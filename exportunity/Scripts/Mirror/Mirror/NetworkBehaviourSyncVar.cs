using System;

namespace Mirror;

public struct NetworkBehaviourSyncVar : IEquatable<NetworkBehaviourSyncVar>
{
	public uint netId;

	public byte componentIndex;

	public NetworkBehaviourSyncVar(uint netId, int componentIndex)
	{
		this = default(NetworkBehaviourSyncVar);
		this.netId = netId;
		this.componentIndex = (byte)componentIndex;
	}

	public bool Equals(NetworkBehaviourSyncVar other)
	{
		if (other.netId == netId)
		{
			return other.componentIndex == componentIndex;
		}
		return false;
	}

	public bool Equals(uint netId, int componentIndex)
	{
		if (this.netId == netId)
		{
			return this.componentIndex == componentIndex;
		}
		return false;
	}

	public override string ToString()
	{
		return $"[netId:{netId} compIndex:{componentIndex}]";
	}
}
