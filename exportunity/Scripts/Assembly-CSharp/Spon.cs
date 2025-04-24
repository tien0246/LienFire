using Mirror;
using UnityEngine;

public class Spon : NetworkBehaviour
{
	public GameObject ht;

	public GameObject spon;

	public float tgSpon;

	private bool daChet;

	[ServerCallback]
	private void Update()
	{
		if (NetworkServer.active && ht == null && !daChet)
		{
			daChet = true;
			Invoke("SpawnE", tgSpon);
		}
	}

	private void SpawnE()
	{
		daChet = false;
		GameObject obj = Object.Instantiate(spon, base.transform.position, base.transform.rotation);
		NetworkServer.Spawn(obj);
		ht = obj;
	}

	private void MirrorProcessed()
	{
	}
}
