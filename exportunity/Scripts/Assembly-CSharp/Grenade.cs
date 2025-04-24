using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Grenade : NetworkBehaviour
{
	public bool okk;

	public GameObject tron;

	public float force;

	public float tgCham;

	public int mauMat;

	public int tamNo;

	public int doi;

	private Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddForce(base.transform.forward * force, ForceMode.Impulse);
	}

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		PlayerStat component = other.transform.GetComponent<PlayerStat>();
		if (component != null)
		{
			if (component.team == doi)
			{
				return;
			}
			RaycastHit[] array = Physics.SphereCastAll(base.transform.position, tamNo, base.transform.forward);
			for (int i = 0; i < array.Length; i++)
			{
				PlayerStat component2 = array[i].transform.GetComponent<PlayerStat>();
				if (component2 != null && component2.team != doi)
				{
					if (tgCham > 0f)
					{
						component2.LamCham(tgCham);
					}
					if (mauMat > 0)
					{
						component2.MatMau(mauMat);
					}
				}
			}
			TuHuy();
		}
		else
		{
			if (!other.transform.CompareTag("matDat") && !other.transform.CompareTag("tuong"))
			{
				return;
			}
			RaycastHit[] array2 = Physics.SphereCastAll(base.transform.position, tamNo, base.transform.forward, 1f);
			for (int j = 0; j < array2.Length; j++)
			{
				PlayerStat component3 = array2[j].transform.GetComponent<PlayerStat>();
				if (component3 != null && component3.team != doi)
				{
					if (tgCham > 0f)
					{
						component3.LamCham(tgCham);
					}
					if (mauMat > 0)
					{
						component3.MatMau(mauMat);
					}
				}
			}
			TuHuy();
		}
	}

	[Command(requiresAuthority = false)]
	private void TuHuy()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Grenade::TuHuy()", -1750173086, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_TuHuy()
	{
		GameObject gameObject = Object.Instantiate(tron, base.transform.position, tron.transform.rotation);
		if (!okk)
		{
			gameObject.transform.localScale = new Vector3(tamNo * 2, tamNo * 2, tamNo * 2);
		}
		NetworkServer.Spawn(gameObject);
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
			((Grenade)obj).UserCode_TuHuy();
		}
	}

	static Grenade()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Grenade), "System.Void Grenade::TuHuy()", InvokeUserCode_TuHuy, requiresAuthority: false);
	}
}
