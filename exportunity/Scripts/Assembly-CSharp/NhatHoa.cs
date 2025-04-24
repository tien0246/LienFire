using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class NhatHoa : NetworkBehaviour
{
	[SyncVar]
	public int doi;

	public MeshRenderer me;

	public float tg;

	public int Networkdoi
	{
		get
		{
			return doi;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref doi, 1uL, null);
		}
	}

	private void Start()
	{
		Invoke("TuHuy", tg);
		if (doi == 1)
		{
			me.material.color = new Color(1f, 0f, 0f, 0.2f);
		}
		else if (doi == 2)
		{
			me.material.color = new Color(0f, 0f, 1f, 0.2f);
		}
	}

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active)
		{
			Florentino component = other.GetComponent<Florentino>();
			PlayerStat component2 = other.GetComponent<PlayerStat>();
			if (component != null && component2 != null && component2.team == doi)
			{
				component.NhatHoa();
				TuHuy();
			}
		}
	}

	[Command(requiresAuthority = false)]
	private void TuHuy()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void NhatHoa::TuHuy()", 604580099, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_TuHuy()
	{
		NetworkServer.Destroy(base.gameObject);
	}

	protected static void InvokeUserCode_TuHuy(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command TuHuy called on client.");
		}
		else
		{
			((NhatHoa)obj).UserCode_TuHuy();
		}
	}

	static NhatHoa()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(NhatHoa), "System.Void NhatHoa::TuHuy()", InvokeUserCode_TuHuy, requiresAuthority: false);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(doi);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(doi);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref doi, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref doi, null, reader.ReadInt());
		}
	}
}
