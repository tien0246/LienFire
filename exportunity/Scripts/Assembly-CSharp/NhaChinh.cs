using Mirror;
using UnityEngine;

public class NhaChinh : NetworkBehaviour
{
	private SponLinh[] sl;

	private float tgDuocSpon;

	public float tdSpon;

	private void Start()
	{
		sl = GetComponentsInChildren<SponLinh>();
	}

	[ServerCallback]
	private void Update()
	{
		if (NetworkServer.active && Time.time > tgDuocSpon)
		{
			tgDuocSpon = Time.time + tdSpon;
			for (int i = 0; i < sl.Length; i++)
			{
				sl[i].Spon();
			}
		}
	}

	private void MirrorProcessed()
	{
	}
}
