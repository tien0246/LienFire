using Mirror;
using UnityEngine;

public class BuaDo : NetworkBehaviour
{
	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active && other.CompareTag("Player"))
		{
			NC(other.GetComponent<PlayerStat>());
			NetworkServer.Destroy(base.gameObject);
		}
	}

	private void NC(PlayerStat pS)
	{
		pS.BDo();
	}

	private void MirrorProcessed()
	{
	}
}
