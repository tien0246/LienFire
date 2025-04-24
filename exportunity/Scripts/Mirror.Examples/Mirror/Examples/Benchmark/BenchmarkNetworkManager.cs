using UnityEngine;

namespace Mirror.Examples.Benchmark;

[AddComponentMenu("")]
public class BenchmarkNetworkManager : NetworkManager
{
	[Header("Spawns")]
	public GameObject spawnPrefab;

	public int spawnAmount = 5000;

	public float interleave = 1f;

	private void SpawnAll()
	{
		float num = Mathf.Sqrt(spawnAmount);
		float num2 = (0f - num) / 2f * interleave;
		int num3 = 0;
		for (int i = 0; (float)i < num; i++)
		{
			for (int j = 0; (float)j < num; j++)
			{
				if (num3 < spawnAmount)
				{
					GameObject obj = Object.Instantiate(spawnPrefab);
					float x = num2 + (float)i * interleave;
					float z = num2 + (float)j * interleave;
					obj.transform.position = new Vector3(x, 0f, z);
					NetworkServer.Spawn(obj);
					num3++;
				}
			}
		}
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		SpawnAll();
	}
}
