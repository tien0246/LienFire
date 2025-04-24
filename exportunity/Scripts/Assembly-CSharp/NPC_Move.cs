using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Move : NetworkBehaviour
{
	public float tamDanh;

	private Vector3 vtCanh;

	public float doLon;

	private NavMeshAgent nav;

	private float tgDuocBan;

	public float tdBan;

	public int dam;

	private void Start()
	{
		vtCanh = base.transform.position;
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
		Vector3 position = vtCanh;
		float num = 10000f;
		PlayerStat playerStat = null;
		RaycastHit[] array = Physics.SphereCastAll(vtCanh, doLon, base.transform.forward, 1f);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].transform.CompareTag("Player"))
			{
				float num2 = Vector3.Distance(array[i].transform.position, base.transform.position);
				if (num2 < num)
				{
					playerStat = array[i].transform.GetComponent<PlayerStat>();
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
		if (playerStat != null && Time.time > tgDuocBan)
		{
			tgDuocBan = Time.time + tdBan;
			playerStat.MatMau(dam);
		}
	}

	private void MirrorProcessed()
	{
	}
}
