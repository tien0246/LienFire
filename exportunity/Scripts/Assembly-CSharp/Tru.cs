using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Tru : NetworkBehaviour
{
	public AudioClip banA;

	public Transform vtBan;

	public int dam;

	public float doLon;

	public GameObject line;

	private float tgDuocBan;

	public float tdBan;

	private bool coL;

	private PlayerStat pSS;

	private void Start()
	{
		pSS = GetComponent<PlayerStat>();
	}

	[ServerCallback]
	private void Update()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		coL = false;
		float num = 100000f;
		PlayerStat playerStat = null;
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLon, base.transform.forward, 1f);
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].transform.CompareTag("linh"))
			{
				continue;
			}
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component.team != pSS.team)
			{
				if (!coL)
				{
					coL = true;
					pSS.NetworkdangKhieng = false;
				}
				float num2 = Vector3.Distance(array[i].transform.position, base.transform.position);
				if (num2 < num)
				{
					playerStat = component;
					num = num2;
				}
			}
		}
		if (!coL)
		{
			pSS.NetworkdangKhieng = true;
			for (int j = 0; j < array.Length; j++)
			{
				if (!array[j].transform.CompareTag("Player"))
				{
					continue;
				}
				PlayerStat component2 = array[j].transform.GetComponent<PlayerStat>();
				if (component2.team != pSS.team)
				{
					float num3 = Vector3.Distance(array[j].transform.position, base.transform.position);
					if (num3 < num)
					{
						playerStat = component2;
						num = num3;
					}
				}
			}
		}
		if (playerStat != null && Time.time > tgDuocBan)
		{
			tgDuocBan = Time.time + tdBan;
			DrawLineRPC(playerStat.transform.position);
			playerStat.MatMau(dam);
		}
	}

	[ClientRpc]
	private void DrawLineRPC(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendRPCInternal("System.Void Tru::DrawLineRPC(UnityEngine.Vector3)", 977191806, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_DrawLineRPC__Vector3(Vector3 vt)
	{
		Object.Instantiate(line, vtBan.position, vtBan.rotation).GetComponent<Line>().SetLine(vtBan.position, vt);
		base.gameObject.GetComponent<AudioSource>().PlayOneShot(banA);
	}

	protected static void InvokeUserCode_DrawLineRPC__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DrawLineRPC called on server.");
		}
		else
		{
			((Tru)obj).UserCode_DrawLineRPC__Vector3(reader.ReadVector3());
		}
	}

	static Tru()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(Tru), "System.Void Tru::DrawLineRPC(UnityEngine.Vector3)", InvokeUserCode_DrawLineRPC__Vector3);
	}
}
