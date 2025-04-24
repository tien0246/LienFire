using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class Linh : NetworkBehaviour
{
	public Transform[] vtDen;

	private int vtHt;

	private NavMeshAgent nav;

	private PlayerStat pS;

	private AudioSource aS;

	private Animator ani;

	private float tgDuocBan;

	public float tdBan;

	public int dam;

	public GameObject gDuoi;

	private bool duoi;

	public float tam;

	public float tamDanh;

	private void Start()
	{
		pS = GetComponent<PlayerStat>();
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
		if (gDuoi != null && duoi)
		{
			PlayerStat component = GetComponent<PlayerStat>();
			float num = Vector3.Distance(base.transform.position, gDuoi.transform.position);
			if (num < tam && !component.chet)
			{
				if (num > tamDanh)
				{
					nav.isStopped = false;
					if (!ani.GetBool("Chay"))
					{
						ani.SetBool("Chay", value: true);
					}
					nav.SetDestination(gDuoi.transform.position);
				}
				else
				{
					nav.isStopped = true;
					if (ani.GetBool("Chay"))
					{
						ani.SetBool("Chay", value: false);
					}
					if (Time.time > tgDuocBan)
					{
						ani.SetTrigger("DT");
						tgDuocBan = Time.time + tdBan;
						gDuoi.GetComponent<PlayerStat>().MatMau(dam);
					}
				}
			}
			else
			{
				nav.isStopped = false;
				duoi = false;
			}
		}
		if (duoi && !(gDuoi == null))
		{
			return;
		}
		float num2 = 100000f;
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, tam - 1f, base.transform.forward, 1f);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component2 = array[i].transform.GetComponent<PlayerStat>();
			if (component2 != null && component2.team != pS.team)
			{
				duoi = true;
				float num3 = Vector3.Distance(array[i].transform.position, base.transform.position);
				if (num3 < num2)
				{
					gDuoi = component2.gameObject;
					num2 = num3;
				}
			}
		}
		nav.isStopped = false;
		nav.SetDestination(vtDen[vtHt].transform.position);
		if (!ani.GetBool("Chay"))
		{
			ani.SetBool("Chay", value: true);
		}
	}

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active && other.transform == vtDen[vtHt] && vtHt < vtDen.Length - 1)
		{
			vtHt++;
		}
	}

	private void MirrorProcessed()
	{
	}
}
