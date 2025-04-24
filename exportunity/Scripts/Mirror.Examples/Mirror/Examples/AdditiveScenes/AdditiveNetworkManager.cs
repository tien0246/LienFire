using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror.Examples.AdditiveScenes;

[AddComponentMenu("")]
public class AdditiveNetworkManager : NetworkManager
{
	[Tooltip("Trigger Zone Prefab")]
	public GameObject Zone;

	[Scene]
	[Tooltip("Add all sub-scenes to this list")]
	public string[] subScenes;

	public new static AdditiveNetworkManager singleton { get; private set; }

	public override void Awake()
	{
		base.Awake();
		singleton = this;
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		StartCoroutine(LoadSubScenes());
		Object.Instantiate(Zone);
	}

	public override void OnStopServer()
	{
		StartCoroutine(UnloadScenes());
	}

	public override void OnStopClient()
	{
		if (base.mode == NetworkManagerMode.Offline)
		{
			StartCoroutine(UnloadScenes());
		}
	}

	private IEnumerator LoadSubScenes()
	{
		Debug.Log("Loading Scenes");
		string[] array = subScenes;
		foreach (string sceneName in array)
		{
			yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		}
	}

	private IEnumerator UnloadScenes()
	{
		Debug.Log("Unloading Subscenes");
		string[] array = subScenes;
		foreach (string text in array)
		{
			if (SceneManager.GetSceneByName(text).IsValid() || SceneManager.GetSceneByPath(text).IsValid())
			{
				yield return SceneManager.UnloadSceneAsync(text);
			}
		}
		yield return Resources.UnloadUnusedAssets();
	}
}
