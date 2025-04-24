using Mirror;
using UnityEngine;

public class KetThuc : NetworkBehaviour
{
	[ServerCallback]
	private void Start()
	{
		if (NetworkServer.active)
		{
			Object.FindObjectOfType<BatTru>().TatSponTru();
		}
	}

	private void MirrorProcessed()
	{
	}
}
