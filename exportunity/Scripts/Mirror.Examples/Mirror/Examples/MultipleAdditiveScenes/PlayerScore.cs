using System.Runtime.InteropServices;
using UnityEngine;

namespace Mirror.Examples.MultipleAdditiveScenes;

public class PlayerScore : NetworkBehaviour
{
	[SyncVar]
	public int playerNumber;

	[SyncVar]
	public int scoreIndex;

	[SyncVar]
	public int matchIndex;

	[SyncVar]
	public uint score;

	public int clientMatchIndex = -1;

	public int NetworkplayerNumber
	{
		get
		{
			return playerNumber;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref playerNumber, 1uL, null);
		}
	}

	public int NetworkscoreIndex
	{
		get
		{
			return scoreIndex;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref scoreIndex, 2uL, null);
		}
	}

	public int NetworkmatchIndex
	{
		get
		{
			return matchIndex;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref matchIndex, 4uL, null);
		}
	}

	public uint Networkscore
	{
		get
		{
			return score;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref score, 8uL, null);
		}
	}

	private void OnGUI()
	{
		if (!base.isServerOnly && !base.isLocalPlayer && clientMatchIndex < 0)
		{
			clientMatchIndex = NetworkClient.connection.identity.GetComponent<PlayerScore>().matchIndex;
		}
		if (base.isLocalPlayer || matchIndex == clientMatchIndex)
		{
			GUI.Box(new Rect(10f + (float)(scoreIndex * 110), 10f, 100f, 25f), $"P{playerNumber}: {score}");
		}
	}

	private void MirrorProcessed()
	{
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(playerNumber);
			writer.WriteInt(scoreIndex);
			writer.WriteInt(matchIndex);
			writer.WriteUInt(score);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(playerNumber);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteInt(scoreIndex);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteInt(matchIndex);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteUInt(score);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref playerNumber, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref scoreIndex, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref matchIndex, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref score, null, reader.ReadUInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref playerNumber, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref scoreIndex, null, reader.ReadInt());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref matchIndex, null, reader.ReadInt());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref score, null, reader.ReadUInt());
		}
	}
}
