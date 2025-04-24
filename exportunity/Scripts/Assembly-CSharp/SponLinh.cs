using Mirror;
using UnityEngine;

public class SponLinh : MonoBehaviour
{
	public GameObject linh;

	public Transform[] vtMoc;

	private int team;

	public int soSpon;

	public float tgSpon;

	public void Spon()
	{
		team = GetComponentInParent<PlayerStat>().team;
		for (int i = 0; i < soSpon; i++)
		{
			Invoke("Ok", tgSpon * (float)i);
		}
	}

	private void Ok()
	{
		GameObject obj = Object.Instantiate(linh, base.transform.position, base.transform.rotation);
		obj.GetComponent<PlayerStat>().Networkteam = team;
		obj.GetComponent<Linh>().vtDen = vtMoc;
		NetworkServer.Spawn(obj);
	}
}
