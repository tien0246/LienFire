using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.AI;

public class AngSeng : NetworkBehaviour
{
	[SyncVar]
	private bool chay;

	public float tamDanh;

	public float td;

	public float tgBT;

	public float tgH;

	public AudioClip no;

	private NavMeshAgent nav;

	private AudioSource aS;

	public bool Networkchay
	{
		get
		{
			return chay;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref chay, 1uL, null);
		}
	}

	private void Awake()
	{
		nav = GetComponent<NavMeshAgent>();
		aS = GetComponent<AudioSource>();
	}

	[ServerCallback]
	private void Start()
	{
		if (NetworkServer.active)
		{
			Invoke("Chay", tgBT);
			Invoke("Het", tgH);
		}
	}

	[ServerCallback]
	private void Update()
	{
		if (!NetworkServer.active || !chay)
		{
			return;
		}
		Vector3 position = base.transform.position;
		float num = 10000f;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].GetComponent<PlayerStat>();
			if (component != null && component.heath > 0)
			{
				float num2 = Vector3.Distance(array[i].transform.position, base.transform.position);
				if (num2 < num)
				{
					position = array[i].transform.position;
					num = num2;
				}
				if (num2 < tamDanh)
				{
					component.MatMau(1000000);
					NoRpc();
				}
			}
		}
		nav.SetDestination(position);
	}

	[ClientRpc]
	private void NoRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void AngSeng::NoRpc()", 334483556, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chay()
	{
		Networkchay = true;
	}

	private void Het()
	{
		NetworkServer.Destroy(base.gameObject);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_NoRpc()
	{
		aS.PlayOneShot(no);
	}

	protected static void InvokeUserCode_NoRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC NoRpc called on server.");
		}
		else
		{
			((AngSeng)obj).UserCode_NoRpc();
		}
	}

	static AngSeng()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(AngSeng), "System.Void AngSeng::NoRpc()", InvokeUserCode_NoRpc);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(chay);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(chay);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref chay, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref chay, null, reader.ReadBool());
		}
	}
}
