using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror;

[AddComponentMenu("Network/ Interest Management/ Scene/Scene Interest Management")]
public class SceneInterestManagement : InterestManagement
{
	private readonly Dictionary<Scene, HashSet<NetworkIdentity>> sceneObjects = new Dictionary<Scene, HashSet<NetworkIdentity>>();

	private readonly Dictionary<NetworkIdentity, Scene> lastObjectScene = new Dictionary<NetworkIdentity, Scene>();

	private HashSet<Scene> dirtyScenes = new HashSet<Scene>();

	[ServerCallback]
	public override void OnSpawned(NetworkIdentity identity)
	{
		if (NetworkServer.active)
		{
			Scene scene = identity.gameObject.scene;
			lastObjectScene[identity] = scene;
			if (!sceneObjects.TryGetValue(scene, out var value))
			{
				value = new HashSet<NetworkIdentity>();
				sceneObjects.Add(scene, value);
			}
			value.Add(identity);
		}
	}

	[ServerCallback]
	public override void OnDestroyed(NetworkIdentity identity)
	{
		if (NetworkServer.active && lastObjectScene.TryGetValue(identity, out var value))
		{
			lastObjectScene.Remove(identity);
			if (sceneObjects.TryGetValue(value, out var value2) && value2.Remove(identity))
			{
				dirtyScenes.Add(value);
			}
		}
	}

	[ServerCallback]
	internal void Update()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (NetworkIdentity value2 in NetworkServer.spawned.Values)
		{
			if (!lastObjectScene.TryGetValue(value2, out var value))
			{
				continue;
			}
			Scene scene = value2.gameObject.scene;
			if (!(scene == value))
			{
				dirtyScenes.Add(value);
				dirtyScenes.Add(scene);
				sceneObjects[value].Remove(value2);
				lastObjectScene[value2] = scene;
				if (!sceneObjects.ContainsKey(scene))
				{
					sceneObjects.Add(scene, new HashSet<NetworkIdentity>());
				}
				sceneObjects[scene].Add(value2);
			}
		}
		foreach (Scene dirtyScene in dirtyScenes)
		{
			RebuildSceneObservers(dirtyScene);
		}
		dirtyScenes.Clear();
	}

	private void RebuildSceneObservers(Scene scene)
	{
		foreach (NetworkIdentity item in sceneObjects[scene])
		{
			if (item != null)
			{
				NetworkServer.RebuildObservers(item, initialize: false);
			}
		}
	}

	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		return identity.gameObject.scene == newObserver.identity.gameObject.scene;
	}

	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		if (!sceneObjects.TryGetValue(identity.gameObject.scene, out var value))
		{
			return;
		}
		foreach (NetworkIdentity item in value)
		{
			if (item != null && item.connectionToClient != null)
			{
				newObservers.Add(item.connectionToClient);
			}
		}
	}
}
