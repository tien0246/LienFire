using UnityEngine;

namespace Mirror.Examples.AdditiveScenes;

public class ZoneHandler : MonoBehaviour
{
	[Scene]
	[Tooltip("Assign the sub-scene to load for this zone")]
	public string subScene;

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active && other.CompareTag("Player") && other.TryGetComponent<NetworkIdentity>(out var component))
		{
			SceneMessage message = new SceneMessage
			{
				sceneName = subScene,
				sceneOperation = SceneOperation.LoadAdditive
			};
			component.connectionToClient.Send(message);
		}
	}

	[ServerCallback]
	private void OnTriggerExit(Collider other)
	{
		if (NetworkServer.active && other.CompareTag("Player") && other.TryGetComponent<NetworkIdentity>(out var component))
		{
			SceneMessage message = new SceneMessage
			{
				sceneName = subScene,
				sceneOperation = SceneOperation.UnloadAdditive
			};
			component.connectionToClient.Send(message);
		}
	}
}
