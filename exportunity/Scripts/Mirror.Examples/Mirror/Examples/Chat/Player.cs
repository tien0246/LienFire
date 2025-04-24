using System.Runtime.InteropServices;

namespace Mirror.Examples.Chat;

public class Player : NetworkBehaviour
{
	[SyncVar]
	public string playerName;

	public string NetworkplayerName
	{
		get
		{
			return playerName;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref playerName, 1uL, null);
		}
	}

	public override void OnStartServer()
	{
		NetworkplayerName = (string)base.connectionToClient.authenticationData;
	}

	public override void OnStartLocalPlayer()
	{
		ChatUI.localPlayerName = playerName;
	}

	private void MirrorProcessed()
	{
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteString(playerName);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteString(playerName);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref playerName, null, reader.ReadString());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref playerName, null, reader.ReadString());
		}
	}
}
