using Mirror;
using UnityEngine;

public class ProjNo : NetworkBehaviour
{
	public GameObject tron;

	public float tgChoang;

	public float tgCham;

	public int mauMat;

	public int tamNo;

	public int doi;

	public bool w;

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		PlayerStat component = other.GetComponent<PlayerStat>();
		if (component != null)
		{
			if (component.team == doi)
			{
				return;
			}
			RaycastHit[] array = Physics.SphereCastAll(base.transform.position, tamNo, base.transform.forward, 1f);
			for (int i = 0; i < array.Length; i++)
			{
				PlayerStat component2 = array[i].transform.GetComponent<PlayerStat>();
				if (component2 != null && component2.team != doi)
				{
					if (tgChoang > 0f)
					{
						component2.DungYen(tgChoang);
					}
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
			if ((!other.transform.CompareTag("matDat") && !other.transform.CompareTag("tuong")) || !w)
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

	private void TuHuy()
	{
		GameObject obj = Object.Instantiate(tron, base.transform.position, base.transform.rotation);
		obj.transform.localScale = new Vector3(tamNo * 2, tamNo * 2, tamNo * 2);
		NetworkServer.Spawn(obj);
		NetworkServer.Destroy(base.gameObject);
	}

	private void MirrorProcessed()
	{
	}
}
