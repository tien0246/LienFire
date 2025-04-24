using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror.Examples.MultipleAdditiveScenes;

[AddComponentMenu("")]
public class MultiSceneNetManager : NetworkManager
{
	[Header("Spawner Setup")]
	[Tooltip("Reward Prefab for the Spawner")]
	public GameObject rewardPrefab;

	[Header("MultiScene Setup")]
	public int instances = 3;

	[Scene]
	public string gameScene;

	private bool subscenesLoaded;

	private readonly List<Scene> subScenes = new List<Scene>();

	private int clientIndex;

	public new static MultiSceneNetManager singleton { get; private set; }

	public override void Awake()
	{
		base.Awake();
		singleton = this;
	}

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		StartCoroutine(OnServerAddPlayerDelayed(conn));
	}

	private IEnumerator OnServerAddPlayerDelayed(NetworkConnectionToClient conn)
	{
		while (!subscenesLoaded)
		{
			yield return null;
		}
		conn.Send(new SceneMessage
		{
			sceneName = gameScene,
			sceneOperation = SceneOperation.LoadAdditive
		});
		yield return new WaitForEndOfFrame();
		base.OnServerAddPlayer(conn);
		PlayerScore component = conn.identity.GetComponent<PlayerScore>();
		component.NetworkplayerNumber = clientIndex;
		component.NetworkscoreIndex = clientIndex / subScenes.Count;
		component.NetworkmatchIndex = clientIndex % subScenes.Count;
		if (subScenes.Count > 0)
		{
			SceneManager.MoveGameObjectToScene(conn.identity.gameObject, subScenes[clientIndex % subScenes.Count]);
		}
		clientIndex++;
	}

	public override void OnStartServer()
	{
		StartCoroutine(ServerLoadSubScenes());
	}

	private IEnumerator ServerLoadSubScenes()
	{
		for (int index = 1; index <= instances; index++)
		{
			yield return SceneManager.LoadSceneAsync(gameScene, new LoadSceneParameters
			{
				loadSceneMode = LoadSceneMode.Additive,
				localPhysicsMode = LocalPhysicsMode.Physics3D
			});
			Scene sceneAt = SceneManager.GetSceneAt(index);
			subScenes.Add(sceneAt);
			Spawner.InitialSpawn(sceneAt);
		}
		subscenesLoaded = true;
	}

	public override void OnStopServer()
	{
		NetworkServer.SendToAll(new SceneMessage
		{
			sceneName = gameScene,
			sceneOperation = SceneOperation.UnloadAdditive
		});
		StartCoroutine(ServerUnloadSubScenes());
		clientIndex = 0;
	}

	private IEnumerator ServerUnloadSubScenes()
	{
		for (int index = 0; index < subScenes.Count; index++)
		{
			if (subScenes[index].IsValid())
			{
				yield return SceneManager.UnloadSceneAsync(subScenes[index]);
			}
		}
		subScenes.Clear();
		subscenesLoaded = false;
		yield return Resources.UnloadUnusedAssets();
	}

	public override void OnStopClient()
	{
		if (base.mode == NetworkManagerMode.Offline)
		{
			StartCoroutine(ClientUnloadSubScenes());
		}
	}

	private IEnumerator ClientUnloadSubScenes()
	{
		for (int index = 0; index < SceneManager.sceneCount; index++)
		{
			if (SceneManager.GetSceneAt(index) != SceneManager.GetActiveScene())
			{
				yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(index));
			}
		}
	}
}
