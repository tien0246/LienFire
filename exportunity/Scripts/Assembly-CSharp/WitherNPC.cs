using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.AI;

public class WitherNPC : NetworkBehaviour
{
	public float tamDanh;

	public float doLon;

	private NavMeshAgent nav;

	public AudioClip danhA;

	private float tgDuocBan;

	public float tdBan;

	public GameObject rocket;

	public Transform vtBan;

	private void Start()
	{
		nav = GetComponent<NavMeshAgent>();
	}

	[ServerCallback]
	private void Update()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (GetComponent<PlayerStat>().dungYen)
		{
			nav.SetDestination(base.transform.position);
			return;
		}
		Vector3 position = base.transform.position;
		float num = 100000f;
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLon, base.transform.forward, 1f);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].transform.CompareTag("Player"))
			{
				float num2 = Vector3.Distance(array[i].transform.position, base.transform.position);
				if (num2 < num)
				{
					position = array[i].transform.position;
					num = num2;
				}
			}
		}
		if (num > tamDanh)
		{
			nav.isStopped = false;
			nav.SetDestination(position);
			return;
		}
		nav.isStopped = true;
		if (Time.time > tgDuocBan)
		{
			tgDuocBan = Time.time + tdBan;
			At(position);
		}
	}

	private void At(Vector3 vt)
	{
		C2Rpc();
		Quaternion rotation = Quaternion.LookRotation(vt - vtBan.transform.position, Vector3.up);
		NetworkServer.Spawn(Object.Instantiate(rocket, vtBan.transform.position, rotation));
	}

	[ClientRpc]
	private void C2Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void WitherNPC::C2Rpc()", -1128412563, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_C2Rpc()
	{
		GetComponent<AudioSource>().PlayOneShot(danhA);
	}

	protected static void InvokeUserCode_C2Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C2Rpc called on server.");
		}
		else
		{
			((WitherNPC)obj).UserCode_C2Rpc();
		}
	}

	static WitherNPC()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(WitherNPC), "System.Void WitherNPC::C2Rpc()", InvokeUserCode_C2Rpc);
	}
}
