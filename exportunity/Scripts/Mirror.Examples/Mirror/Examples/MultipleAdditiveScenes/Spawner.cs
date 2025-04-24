using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror.Examples.MultipleAdditiveScenes;

internal class Spawner
{
	[ServerCallback]
	internal static void InitialSpawn(Scene scene)
	{
		if (NetworkServer.active)
		{
			for (int i = 0; i < 10; i++)
			{
				SpawnReward(scene);
			}
		}
	}

	[ServerCallback]
	internal static void SpawnReward(Scene scene)
	{
		if (NetworkServer.active)
		{
			GameObject obj = Object.Instantiate(position: new Vector3(Random.Range(-19, 20), 1f, Random.Range(-19, 20)), original: ((MultiSceneNetManager)NetworkManager.singleton).rewardPrefab, rotation: Quaternion.identity);
			SceneManager.MoveGameObjectToScene(obj, scene);
			NetworkServer.Spawn(obj);
		}
	}
}
