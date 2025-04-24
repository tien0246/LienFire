using UnityEngine;

namespace Mirror.Examples.NetworkRoom;

internal class Spawner
{
	[ServerCallback]
	internal static void InitialSpawn()
	{
		if (NetworkServer.active)
		{
			for (int i = 0; i < 10; i++)
			{
				SpawnReward();
			}
		}
	}

	[ServerCallback]
	internal static void SpawnReward()
	{
		if (NetworkServer.active)
		{
			NetworkServer.Spawn(Object.Instantiate(position: new Vector3(Random.Range(-19, 20), 1f, Random.Range(-19, 20)), original: NetworkRoomManagerExt.singleton.rewardPrefab, rotation: Quaternion.identity));
		}
	}
}
