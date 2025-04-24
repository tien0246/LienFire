using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class SponMau : NetworkBehaviour
{
	[SyncVar]
	private bool hoi = true;

	public AudioClip eatS;

	[SyncVar]
	private float tgHoi;

	public float tgHoiT;

	public GameObject model;

	public bool Networkhoi
	{
		get
		{
			return hoi;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref hoi, 1uL, null);
		}
	}

	public float NetworktgHoi
	{
		get
		{
			return tgHoi;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref tgHoi, 2uL, null);
		}
	}

	[ServerCallback]
	private void Update()
	{
		if (NetworkServer.active)
		{
			if (tgHoi > 0f)
			{
				NetworktgHoi = tgHoi - Time.deltaTime;
			}
			else if (!hoi)
			{
				Networkhoi = true;
				Model(t: true);
			}
		}
	}

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active)
		{
			PlayerStat component = other.GetComponent<PlayerStat>();
			if (component != null && hoi && other.CompareTag("Player"))
			{
				NetworktgHoi = tgHoiT;
				Networkhoi = false;
				component.HoiMau(component.maxHeath);
				Model(t: false);
			}
		}
	}

	[ClientRpc]
	private void Model(bool t)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(t);
		SendRPCInternal("System.Void SponMau::Model(System.Boolean)", 189857120, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_Model__Boolean(bool t)
	{
		model.SetActive(t);
		if (!t)
		{
			GetComponent<AudioSource>().PlayOneShot(eatS);
		}
	}

	protected static void InvokeUserCode_Model__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Model called on server.");
		}
		else
		{
			((SponMau)obj).UserCode_Model__Boolean(reader.ReadBool());
		}
	}

	static SponMau()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(SponMau), "System.Void SponMau::Model(System.Boolean)", InvokeUserCode_Model__Boolean);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(hoi);
			writer.WriteFloat(tgHoi);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(hoi);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteFloat(tgHoi);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref hoi, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref tgHoi, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref hoi, null, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref tgHoi, null, reader.ReadFloat());
		}
	}
}
