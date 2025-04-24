using Mirror;
using UnityEngine;

public class BuaXanh : NetworkBehaviour
{
	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active && other.CompareTag("Player"))
		{
			NC(other.GetComponent<PlayerAtack>());
			NetworkServer.Destroy(base.gameObject);
		}
	}

	private void NC(PlayerAtack pS)
	{
		pS.BXanh();
	}

	private void MirrorProcessed()
	{
	}
}
