using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.AI;

public class Bot_Move : NetworkBehaviour
{
	public GameObject model;

	public Transform vtBan1;

	public GameObject line;

	public AudioClip banA;

	public float tamDanh;

	public float doLon;

	private NavMeshAgent nav;

	private float tgDuocBan;

	private Transform vt;

	private PlayerStat pS;

	private bool phatHien;

	public float tdBan;

	public int dam;

	private AudioSource aS;

	private Animator ani;

	private void Start()
	{
		nav = GetComponent<NavMeshAgent>();
		aS = GetComponent<AudioSource>();
		ani = GetComponent<Animator>();
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
		if (!phatHien)
		{
			RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLon, base.transform.forward, 1f);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].transform.CompareTag("Player"))
				{
					vt = array[i].transform;
					pS = array[i].transform.GetComponent<PlayerStat>();
					phatHien = true;
				}
			}
			return;
		}
		Vector3 vector = vt.position - base.transform.position;
		if (Physics.Raycast(base.transform.position, vector, out var hitInfo, tamDanh))
		{
			if (hitInfo.transform == vt.transform)
			{
				ani.SetBool("Chay", value: false);
				nav.isStopped = true;
				base.transform.forward = vector;
				if (Time.time > tgDuocBan)
				{
					tgDuocBan = Time.time + tdBan;
					pS.MatMau(dam);
					DrawLineRPC(hitInfo.point);
				}
			}
			else
			{
				nav.isStopped = false;
				nav.SetDestination(vt.position);
				ani.SetBool("Chay", value: true);
			}
		}
		else
		{
			nav.isStopped = false;
			ani.SetBool("Chay", value: true);
			nav.SetDestination(vt.position);
		}
	}

	[ClientRpc]
	private void DrawLineRPC(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendRPCInternal("System.Void Bot_Move::DrawLineRPC(UnityEngine.Vector3)", -359181896, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_DrawLineRPC__Vector3(Vector3 vt)
	{
		if (aS != null)
		{
			aS.PlayOneShot(banA);
		}
		GameObject gameObject = Object.Instantiate(line, vtBan1.position, vtBan1.rotation);
		if (gameObject.GetComponent<Line>() != null)
		{
			gameObject.GetComponent<Line>().SetLine(vtBan1.position, vt);
		}
	}

	protected static void InvokeUserCode_DrawLineRPC__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DrawLineRPC called on server.");
		}
		else
		{
			((Bot_Move)obj).UserCode_DrawLineRPC__Vector3(reader.ReadVector3());
		}
	}

	static Bot_Move()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(Bot_Move), "System.Void Bot_Move::DrawLineRPC(UnityEngine.Vector3)", InvokeUserCode_DrawLineRPC__Vector3);
	}
}
