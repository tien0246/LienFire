using System;
using UnityEngine;

namespace Mirror.Examples.CCU;

[AddComponentMenu("")]
public class CCUNetworkManager : NetworkManager
{
	[Header("Spawns")]
	public int spawnAmount = 10000;

	public float interleave = 1f;

	public GameObject spawnPrefab;

	[Range(0f, 1f)]
	public float spawnPositionRatio = 0.01f;

	private System.Random random = new System.Random(42);

	private void SpawnAll()
	{
		foreach (Transform startPosition in NetworkManager.startPositions)
		{
			UnityEngine.Object.Destroy(startPosition.gameObject);
		}
		NetworkManager.startPositions.Clear();
		float num = Mathf.Sqrt(spawnAmount);
		float num2 = (0f - num) / 2f * interleave;
		int num3 = 0;
		for (int i = 0; (float)i < num; i++)
		{
			for (int j = 0; (float)j < num; j++)
			{
				if (num3 < spawnAmount)
				{
					GameObject obj = UnityEngine.Object.Instantiate(spawnPrefab);
					float x = num2 + (float)i * interleave;
					float z = num2 + (float)j * interleave;
					Vector3 position = new Vector3(x, 0f, z);
					obj.transform.position = position;
					NetworkServer.Spawn(obj);
					num3++;
					if (random.NextDouble() <= (double)spawnPositionRatio)
					{
						GameObject obj2 = new GameObject("Spawn");
						obj2.transform.position = position;
						obj2.AddComponent<NetworkStartPosition>();
					}
				}
			}
		}
	}

	public override Transform GetStartPosition()
	{
		NetworkManager.startPositions.RemoveAll((Transform t) => t == null);
		if (NetworkManager.startPositions.Count == 0)
		{
			return null;
		}
		int index = random.Next(0, NetworkManager.startPositions.Count);
		return NetworkManager.startPositions[index];
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		SpawnAll();
	}
}
