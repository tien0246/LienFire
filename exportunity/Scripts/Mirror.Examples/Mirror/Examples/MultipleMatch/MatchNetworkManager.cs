using System.Collections;
using UnityEngine;

namespace Mirror.Examples.MultipleMatch;

[AddComponentMenu("")]
public class MatchNetworkManager : NetworkManager
{
	[Header("Match GUI")]
	public GameObject canvas;

	public CanvasController canvasController;

	public new static MatchNetworkManager singleton { get; private set; }

	public override void Awake()
	{
		base.Awake();
		singleton = this;
		canvasController.InitializeData();
	}

	public override void OnServerReady(NetworkConnectionToClient conn)
	{
		base.OnServerReady(conn);
		canvasController.OnServerReady(conn);
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		StartCoroutine(DoServerDisconnect(conn));
	}

	private IEnumerator DoServerDisconnect(NetworkConnectionToClient conn)
	{
		yield return canvasController.OnServerDisconnect(conn);
		base.OnServerDisconnect(conn);
	}

	public override void OnClientConnect()
	{
		base.OnClientConnect();
		canvasController.OnClientConnect();
	}

	public override void OnClientDisconnect()
	{
		canvasController.OnClientDisconnect();
		base.OnClientDisconnect();
	}

	public override void OnStartServer()
	{
		if (base.mode == NetworkManagerMode.ServerOnly)
		{
			canvas.SetActive(value: true);
		}
		canvasController.OnStartServer();
	}

	public override void OnStartClient()
	{
		canvas.SetActive(value: true);
		canvasController.OnStartClient();
	}

	public override void OnStopServer()
	{
		canvasController.OnStopServer();
		canvas.SetActive(value: false);
	}

	public override void OnStopClient()
	{
		canvasController.OnStopClient();
	}
}
