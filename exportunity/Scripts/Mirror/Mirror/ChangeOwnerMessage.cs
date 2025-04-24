namespace Mirror;

public struct ChangeOwnerMessage : NetworkMessage
{
	public uint netId;

	public bool isOwner;

	public bool isLocalPlayer;
}
