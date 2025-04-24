using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.UI;

public class ChonTuong : NetworkBehaviour
{
	[SyncVar]
	public int tuong;

	[SyncVar]
	public float doNhay;

	[SyncVar]
	public int boTro;

	public GameObject[] tuongG;

	public Slider sli;

	public Button[] nutChon;

	public Button[] nutBT;

	public int Networktuong
	{
		get
		{
			return tuong;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref tuong, 1uL, null);
		}
	}

	public float NetworkdoNhay
	{
		get
		{
			return doNhay;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref doNhay, 2uL, null);
		}
	}

	public int NetworkboTro
	{
		get
		{
			return boTro;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref boTro, 4uL, null);
		}
	}

	private void Start()
	{
		if (!base.isLocalPlayer)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void VaoGame(int doi)
	{
		NetworkdoNhay = sli.value;
		VaoGameCmd(doi, tuong, boTro, doNhay);
	}

	[Command]
	private void VaoGameCmd(int doi, int tuo, int bt, float doN)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(doi);
		writer.WriteInt(tuo);
		writer.WriteInt(bt);
		writer.WriteFloat(doN);
		SendCommandInternal("System.Void ChonTuong::VaoGameCmd(System.Int32,System.Int32,System.Int32,System.Single)", -1896422470, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public void Chon(int t)
	{
		Networktuong = t;
		for (int i = 0; i < nutChon.Length; i++)
		{
			if (i == t)
			{
				nutChon[i].interactable = false;
			}
			else
			{
				nutChon[i].interactable = true;
			}
		}
	}

	public void ChonBT(int b)
	{
		NetworkboTro = b;
		for (int i = 0; i < nutBT.Length; i++)
		{
			if (i == b)
			{
				nutBT[i].interactable = false;
			}
			else
			{
				nutBT[i].interactable = true;
			}
		}
	}

	public void BTru()
	{
		if (base.isLocalPlayer)
		{
			Object.FindObjectOfType<BatTru>().Sponu();
		}
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_VaoGameCmd__Int32__Int32__Int32__Single(int doi, int tuo, int bt, float doN)
	{
		GameObject gameObject = Object.Instantiate(tuongG[tuo]);
		gameObject.GetComponent<PlayerStat>().Networkteam = doi;
		gameObject.GetComponent<PlayerAtack>().NetworkboTro = bt;
		gameObject.GetComponent<PlayerLook>().NetworkdoNhay = doN;
		NetworkServer.ReplacePlayerForConnection(base.connectionToClient, gameObject, keepAuthority: true);
		Object.Destroy(base.gameObject, 0.05f);
	}

	protected static void InvokeUserCode_VaoGameCmd__Int32__Int32__Int32__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command VaoGameCmd called on client.");
		}
		else
		{
			((ChonTuong)obj).UserCode_VaoGameCmd__Int32__Int32__Int32__Single(reader.ReadInt(), reader.ReadInt(), reader.ReadInt(), reader.ReadFloat());
		}
	}

	static ChonTuong()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(ChonTuong), "System.Void ChonTuong::VaoGameCmd(System.Int32,System.Int32,System.Int32,System.Single)", InvokeUserCode_VaoGameCmd__Int32__Int32__Int32__Single, requiresAuthority: true);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(tuong);
			writer.WriteFloat(doNhay);
			writer.WriteInt(boTro);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(tuong);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteFloat(doNhay);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteInt(boTro);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref tuong, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref doNhay, null, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref boTro, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref tuong, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref doNhay, null, reader.ReadFloat());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref boTro, null, reader.ReadInt());
		}
	}
}
