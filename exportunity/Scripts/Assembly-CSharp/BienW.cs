using Mirror;
using UnityEngine;

public class BienW : NetworkBehaviour
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
		pS.Wither();
	}

	private void MirrorProcessed()
	{
	}
}
