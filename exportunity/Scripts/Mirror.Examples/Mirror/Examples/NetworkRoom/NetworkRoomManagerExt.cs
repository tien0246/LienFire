using UnityEngine;

namespace Mirror.Examples.NetworkRoom;

[AddComponentMenu("")]
public class NetworkRoomManagerExt : NetworkRoomManager
{
	[Header("Spawner Setup")]
	[Tooltip("Reward Prefab for the Spawner")]
	public GameObject rewardPrefab;

	private bool showStartButton;

	public new static NetworkRoomManagerExt singleton { get; private set; }

	public override void Awake()
	{
		base.Awake();
		singleton = this;
	}

	public override void OnRoomServerSceneChanged(string sceneName)
	{
		if (sceneName == GameplayScene)
		{
			Spawner.InitialSpawn();
		}
	}

	public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
	{
		gamePlayer.GetComponent<PlayerScore>().Networkindex = roomPlayer.GetComponent<NetworkRoomPlayer>().index;
		return true;
	}

	public override void OnRoomStopClient()
	{
		base.OnRoomStopClient();
	}

	public override void OnRoomStopServer()
	{
		base.OnRoomStopServer();
	}

	public override void OnRoomServerPlayersReady()
	{
		showStartButton = true;
	}

	public override void OnGUI()
	{
		base.OnGUI();
		if (base.allPlayersReady && showStartButton && GUI.Button(new Rect(150f, 300f, 120f, 20f), "START GAME"))
		{
			showStartButton = false;
			ServerChangeScene(GameplayScene);
		}
	}
}
