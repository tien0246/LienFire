using Mirror;
using UnityEngine;

public class TaoHoa : NetworkBehaviour
{
	public GameObject baHoa;

	public int doi;

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		doi = GetComponent<ProjHit>().doi;
		PlayerStat component = other.GetComponent<PlayerStat>();
		if (component != null && component.team != doi)
		{
			for (int i = 0; i < 3; i++)
			{
				GameObject obj = Object.Instantiate(baHoa, component.transform.position, Quaternion.Euler(0f, base.transform.rotation.y + (float)(i * 120), 0f));
				obj.GetComponent<NhatHoa>().Networkdoi = doi;
				NetworkServer.Spawn(obj);
			}
		}
	}

	private void MirrorProcessed()
	{
	}
}
