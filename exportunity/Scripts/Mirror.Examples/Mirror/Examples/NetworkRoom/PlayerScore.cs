using System.Runtime.InteropServices;
using UnityEngine;

namespace Mirror.Examples.NetworkRoom;

public class PlayerScore : NetworkBehaviour
{
	[SyncVar]
	public int index;

	[SyncVar]
	public uint score;

	public int Networkindex
	{
		get
		{
			return index;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref index, 1uL, null);
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
			GeneratedSyncVarSetter(value, ref score, 2uL, null);
		}
	}

	private void OnGUI()
	{
		GUI.Box(new Rect(10f + (float)(index * 110), 10f, 100f, 25f), $"P{index}: {score:0000000}");
	}

	private void MirrorProcessed()
	{
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(index);
			writer.WriteUInt(score);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(index);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteUInt(score);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref index, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref score, null, reader.ReadUInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref index, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref score, null, reader.ReadUInt());
		}
	}
}
