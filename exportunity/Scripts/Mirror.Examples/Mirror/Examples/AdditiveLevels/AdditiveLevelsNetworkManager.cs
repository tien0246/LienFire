using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror.Examples.AdditiveLevels;

[AddComponentMenu("")]
public class AdditiveLevelsNetworkManager : NetworkManager
{
	[Header("Additive Scenes - First is start scene")]
	[Scene]
	[Tooltip("Add additive scenes here.\nFirst entry will be players' start scene")]
	public string[] additiveScenes;

	[Header("Fade Control - See child FadeCanvas")]
	[Tooltip("Reference to FadeInOut script on child FadeCanvas")]
	public FadeInOut fadeInOut;

	private bool subscenesLoaded;

	private bool isInTransition;

	public new static AdditiveLevelsNetworkManager singleton { get; private set; }

	public override void Awake()
	{
		base.Awake();
		singleton = this;
	}

	public override void OnServerSceneChanged(string sceneName)
	{
		if (sceneName == onlineScene)
		{
			StartCoroutine(ServerLoadSubScenes());
		}
	}

	private IEnumerator ServerLoadSubScenes()
	{
		string[] array = additiveScenes;
		foreach (string sceneName in array)
		{
			yield return SceneManager.LoadSceneAsync(sceneName, new LoadSceneParameters
			{
				loadSceneMode = LoadSceneMode.Additive,
				localPhysicsMode = LocalPhysicsMode.Physics3D
			});
		}
		subscenesLoaded = true;
	}

	public override void OnClientChangeScene(string sceneName, SceneOperation sceneOperation, bool customHandling)
	{
		if (sceneOperation == SceneOperation.UnloadAdditive)
		{
			StartCoroutine(UnloadAdditive(sceneName));
		}
		if (sceneOperation == SceneOperation.LoadAdditive)
		{
			StartCoroutine(LoadAdditive(sceneName));
		}
	}

	private IEnumerator LoadAdditive(string sceneName)
	{
		isInTransition = true;
		yield return fadeInOut.FadeIn();
		if (base.mode == NetworkManagerMode.ClientOnly)
		{
			NetworkManager.loadingSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			while (NetworkManager.loadingSceneAsync != null && !NetworkManager.loadingSceneAsync.isDone)
			{
				yield return null;
			}
		}
		NetworkClient.isLoadingScene = false;
		isInTransition = false;
		OnClientSceneChanged();
		yield return fadeInOut.FadeOut();
	}

	private IEnumerator UnloadAdditive(string sceneName)
	{
		isInTransition = true;
		yield return fadeInOut.FadeIn();
		if (base.mode == NetworkManagerMode.ClientOnly)
		{
			yield return SceneManager.UnloadSceneAsync(sceneName);
			yield return Resources.UnloadUnusedAssets();
		}
		NetworkClient.isLoadingScene = false;
		isInTransition = false;
		OnClientSceneChanged();
	}

	public override void OnClientSceneChanged()
	{
		if (!isInTransition)
		{
			base.OnClientSceneChanged();
		}
	}

	public override void OnServerReady(NetworkConnectionToClient conn)
	{
		base.OnServerReady(conn);
		if (conn.identity == null)
		{
			StartCoroutine(AddPlayerDelayed(conn));
		}
	}

	private IEnumerator AddPlayerDelayed(NetworkConnectionToClient conn)
	{
		while (!subscenesLoaded)
		{
			yield return null;
		}
		conn.Send(new SceneMessage
		{
			sceneName = additiveScenes[0],
			sceneOperation = SceneOperation.LoadAdditive,
			customHandling = true
		});
		Transform startPosition = GetStartPosition();
		GameObject player = Object.Instantiate(playerPrefab, startPosition);
		player.transform.SetParent(null);
		yield return new WaitForEndOfFrame();
		NetworkServer.AddPlayerForConnection(conn, player);
	}
}
