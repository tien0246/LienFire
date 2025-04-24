using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[AddComponentMenu("Network/Network Manager HUD")]
[RequireComponent(typeof(NetworkManager))]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-manager-hud")]
public class NetworkManagerHUD : MonoBehaviour
{
	private NetworkManager manager;

	public int offsetX;

	public int offsetY;

	private void Awake()
	{
		manager = GetComponent<NetworkManager>();
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 250f, 9999f));
		if (!NetworkClient.isConnected && !NetworkServer.active)
		{
			StartButtons();
		}
		else
		{
			StatusLabels();
		}
		if (NetworkClient.isConnected && !NetworkClient.ready && GUILayout.Button("Client Ready"))
		{
			NetworkClient.Ready();
			if (NetworkClient.localPlayer == null)
			{
				NetworkClient.AddPlayer();
			}
		}
		StopButtons();
		GUILayout.EndArea();
	}

	private void StartButtons()
	{
		if (!NetworkClient.active)
		{
			if (Application.platform != RuntimePlatform.WebGLPlayer && GUILayout.Button("Host (Server + Client)"))
			{
				manager.StartHost();
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Client"))
			{
				manager.StartClient();
			}
			manager.networkAddress = GUILayout.TextField(manager.networkAddress);
			GUILayout.EndHorizontal();
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				GUILayout.Box("(  WebGL cannot be server  )");
			}
			else if (GUILayout.Button("Server Only"))
			{
				manager.StartServer();
			}
		}
		else
		{
			GUILayout.Label("Connecting to " + manager.networkAddress + "..");
			if (GUILayout.Button("Cancel Connection Attempt"))
			{
				manager.StopClient();
			}
		}
	}

	private void StatusLabels()
	{
		if (NetworkServer.active && NetworkClient.active)
		{
			GUILayout.Label($"<b>Host</b>: running via {Transport.active}");
		}
		else if (NetworkServer.active)
		{
			GUILayout.Label($"<b>Server</b>: running via {Transport.active}");
		}
		else if (NetworkClient.isConnected)
		{
			GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.active}");
		}
	}

	private void StopButtons()
	{
		if (NetworkServer.active && NetworkClient.isConnected)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Stop Host"))
			{
				manager.StopHost();
			}
			if (GUILayout.Button("Stop Client"))
			{
				manager.StopClient();
			}
			GUILayout.EndHorizontal();
		}
		else if (NetworkClient.isConnected)
		{
			if (GUILayout.Button("Stop Client"))
			{
				manager.StopClient();
			}
		}
		else if (NetworkServer.active && GUILayout.Button("Stop Server"))
		{
			manager.StopServer();
		}
	}
}
