using System.Runtime.InteropServices;
using UnityEngine;

namespace Mirror.Examples.SyncDir;

public class Player : NetworkBehaviour
{
	public TextMesh textMesh;

	public Color localColor = Color.white;

	[SyncVar]
	public int health;

	private readonly SyncList<int> list = new SyncList<int>();

	public int Networkhealth
	{
		get
		{
			return health;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref health, 1uL, null);
		}
	}

	public override void OnStartLocalPlayer()
	{
		textMesh.color = localColor;
	}

	private void Update()
	{
		textMesh.text = $"{health} / {list.Count}";
		if (base.isLocalPlayer)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Networkhealth = health + 1;
			}
			if (Input.GetKeyDown(KeyCode.L))
			{
				list.Add(list.Count);
			}
		}
	}

	private void OnGUI()
	{
		if (base.isLocalPlayer)
		{
			int num = 250;
			int num2 = 50;
			GUI.color = localColor;
			GUI.Label(new Rect(Screen.width / 2 - num / 2, Screen.height / 2 - num2 / 2, num, num2), "Press Space to increase your own health!\nPress L to add to your SyncList!");
		}
	}

	public Player()
	{
		InitSyncObject(list);
	}

	private void MirrorProcessed()
	{
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(health);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(health);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref health, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref health, null, reader.ReadInt());
		}
	}
}
