using Mirror;
using UnityEngine;

public class SponTru : NetworkBehaviour
{
	public GameObject tru;

	public void Spon()
	{
		NetworkServer.Spawn(Object.Instantiate(tru, base.transform.position, base.transform.rotation));
	}

	private void MirrorProcessed()
	{
	}
}
