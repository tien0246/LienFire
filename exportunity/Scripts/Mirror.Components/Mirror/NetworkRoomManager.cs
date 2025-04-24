using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Mirror;

[AddComponentMenu("Network/Network Room Manager")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-room-manager")]
public class NetworkRoomManager : NetworkManager
{
	public struct PendingPlayer
	{
		public NetworkConnectionToClient conn;

		public GameObject roomPlayer;
	}

	[Header("Room Settings")]
	[FormerlySerializedAs("m_ShowRoomGUI")]
	[SerializeField]
	[Tooltip("This flag controls whether the default UI is shown for the room")]
	public bool showRoomGUI = true;

	[FormerlySerializedAs("m_MinPlayers")]
	[SerializeField]
	[Tooltip("Minimum number of players to auto-start the game")]
	public int minPlayers = 1;

	[FormerlySerializedAs("m_RoomPlayerPrefab")]
	[SerializeField]
	[Tooltip("Prefab to use for the Room Player")]
	public NetworkRoomPlayer roomPlayerPrefab;

	[Scene]
	public string RoomScene;

	[Scene]
	public string GameplayScene;

	[FormerlySerializedAs("m_PendingPlayers")]
	public List<PendingPlayer> pendingPlayers = new List<PendingPlayer>();

	[Header("Diagnostics")]
	[Tooltip("Diagnostic flag indicating all players are ready to play")]
	[FormerlySerializedAs("allPlayersReady")]
	[SerializeField]
	private bool _allPlayersReady;

	[Tooltip("List of Room Player objects")]
	public List<NetworkRoomPlayer> roomSlots = new List<NetworkRoomPlayer>();

	public int clientIndex;

	public bool allPlayersReady
	{
		get
		{
			return _allPlayersReady;
		}
		set
		{
			bool num = _allPlayersReady;
			bool flag = value;
			if (num != flag)
			{
				_allPlayersReady = value;
				if (flag)
				{
					OnRoomServerPlayersReady();
				}
				else
				{
					OnRoomServerPlayersNotReady();
				}
			}
		}
	}

	public override void OnValidate()
	{
		base.OnValidate();
		minPlayers = Mathf.Min(minPlayers, maxConnections);
		minPlayers = Mathf.Max(minPlayers, 0);
		if (roomPlayerPrefab != null && roomPlayerPrefab.GetComponent<NetworkIdentity>() == null)
		{
			roomPlayerPrefab = null;
			Debug.LogError("RoomPlayer prefab must have a NetworkIdentity component.");
		}
	}

	private void SceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
	{
		Debug.Log($"NetworkRoom SceneLoadedForPlayer scene: {SceneManager.GetActiveScene().path} {conn}");
		if (Utils.IsSceneActive(RoomScene))
		{
			PendingPlayer item = default(PendingPlayer);
			item.conn = conn;
			item.roomPlayer = roomPlayer;
			pendingPlayers.Add(item);
			return;
		}
		GameObject gameObject = OnRoomServerCreateGamePlayer(conn, roomPlayer);
		if (gameObject == null)
		{
			Transform startPosition = GetStartPosition();
			gameObject = ((startPosition != null) ? Object.Instantiate(playerPrefab, startPosition.position, startPosition.rotation) : Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity));
		}
		if (OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gameObject))
		{
			NetworkServer.ReplacePlayerForConnection(conn, gameObject, keepAuthority: true);
		}
	}

	internal void CallOnClientEnterRoom()
	{
		OnRoomClientEnter();
		foreach (NetworkRoomPlayer roomSlot in roomSlots)
		{
			if (roomSlot != null)
			{
				roomSlot.OnClientEnterRoom();
			}
		}
	}

	internal void CallOnClientExitRoom()
	{
		OnRoomClientExit();
		foreach (NetworkRoomPlayer roomSlot in roomSlots)
		{
			if (roomSlot != null)
			{
				roomSlot.OnClientExitRoom();
			}
		}
	}

	public void CheckReadyToBegin()
	{
		if (Utils.IsSceneActive(RoomScene))
		{
			NetworkRoomPlayer component;
			int num = NetworkServer.connections.Count((KeyValuePair<int, NetworkConnectionToClient> conn) => conn.Value != null && conn.Value.identity != null && conn.Value.identity.TryGetComponent<NetworkRoomPlayer>(out component) && component.readyToBegin);
			if (minPlayers <= 0 || num >= minPlayers)
			{
				pendingPlayers.Clear();
				allPlayersReady = true;
			}
			else
			{
				allPlayersReady = false;
			}
		}
	}

	public override void OnServerConnect(NetworkConnectionToClient conn)
	{
		if (!Utils.IsSceneActive(RoomScene))
		{
			Debug.Log($"Not in Room scene...disconnecting {conn}");
			conn.Disconnect();
		}
		else
		{
			base.OnServerConnect(conn);
			OnRoomServerConnect(conn);
		}
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		if (conn.identity != null)
		{
			NetworkRoomPlayer component = conn.identity.GetComponent<NetworkRoomPlayer>();
			if (component != null)
			{
				roomSlots.Remove(component);
			}
			foreach (NetworkIdentity item in conn.owned)
			{
				component = item.GetComponent<NetworkRoomPlayer>();
				if (component != null)
				{
					roomSlots.Remove(component);
				}
			}
		}
		allPlayersReady = false;
		foreach (NetworkRoomPlayer roomSlot in roomSlots)
		{
			if (roomSlot != null)
			{
				roomSlot.GetComponent<NetworkRoomPlayer>().NetworkreadyToBegin = false;
			}
		}
		if (Utils.IsSceneActive(RoomScene))
		{
			RecalculateRoomPlayerIndices();
		}
		OnRoomServerDisconnect(conn);
		base.OnServerDisconnect(conn);
	}

	public override void OnServerReady(NetworkConnectionToClient conn)
	{
		Debug.Log($"NetworkRoomManager OnServerReady {conn}");
		base.OnServerReady(conn);
		if (conn != null && conn.identity != null)
		{
			GameObject gameObject = conn.identity.gameObject;
			if (gameObject != null && gameObject.GetComponent<NetworkRoomPlayer>() != null)
			{
				SceneLoadedForPlayer(conn, gameObject);
			}
		}
	}

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		clientIndex++;
		if (Utils.IsSceneActive(RoomScene))
		{
			allPlayersReady = false;
			GameObject gameObject = OnRoomServerCreateRoomPlayer(conn);
			if (gameObject == null)
			{
				gameObject = Object.Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
			}
			NetworkServer.AddPlayerForConnection(conn, gameObject);
		}
		else
		{
			Debug.Log($"Not in Room scene...disconnecting {conn}");
			conn.Disconnect();
		}
	}

	[Server]
	public void RecalculateRoomPlayerIndices()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Mirror.NetworkRoomManager::RecalculateRoomPlayerIndices()' called when server was not active");
		}
		else if (roomSlots.Count > 0)
		{
			for (int i = 0; i < roomSlots.Count; i++)
			{
				roomSlots[i].Networkindex = i;
			}
		}
	}

	public override void ServerChangeScene(string newSceneName)
	{
		if (newSceneName == RoomScene)
		{
			foreach (NetworkRoomPlayer roomSlot in roomSlots)
			{
				if (!(roomSlot == null))
				{
					NetworkIdentity component = roomSlot.GetComponent<NetworkIdentity>();
					if (NetworkServer.active)
					{
						roomSlot.GetComponent<NetworkRoomPlayer>().NetworkreadyToBegin = false;
						NetworkServer.ReplacePlayerForConnection(component.connectionToClient, roomSlot.gameObject);
					}
				}
			}
			allPlayersReady = false;
		}
		base.ServerChangeScene(newSceneName);
	}

	public override void OnServerSceneChanged(string sceneName)
	{
		if (sceneName != RoomScene)
		{
			foreach (PendingPlayer pendingPlayer in pendingPlayers)
			{
				SceneLoadedForPlayer(pendingPlayer.conn, pendingPlayer.roomPlayer);
			}
			pendingPlayers.Clear();
		}
		OnRoomServerSceneChanged(sceneName);
	}

	public override void OnStartServer()
	{
		if (string.IsNullOrWhiteSpace(RoomScene))
		{
			Debug.LogError("NetworkRoomManager RoomScene is empty. Set the RoomScene in the inspector for the NetworkRoomManager");
		}
		else if (string.IsNullOrWhiteSpace(GameplayScene))
		{
			Debug.LogError("NetworkRoomManager PlayScene is empty. Set the PlayScene in the inspector for the NetworkRoomManager");
		}
		else
		{
			OnRoomStartServer();
		}
	}

	public override void OnStartHost()
	{
		OnRoomStartHost();
	}

	public override void OnStopServer()
	{
		roomSlots.Clear();
		OnRoomStopServer();
	}

	public override void OnStopHost()
	{
		OnRoomStopHost();
	}

	public override void OnStartClient()
	{
		if (roomPlayerPrefab == null || roomPlayerPrefab.gameObject == null)
		{
			Debug.LogError("NetworkRoomManager no RoomPlayer prefab is registered. Please add a RoomPlayer prefab.");
		}
		else
		{
			NetworkClient.RegisterPrefab(roomPlayerPrefab.gameObject);
		}
		if (playerPrefab == null)
		{
			Debug.LogError("NetworkRoomManager no GamePlayer prefab is registered. Please add a GamePlayer prefab.");
		}
		OnRoomStartClient();
	}

	public override void OnClientConnect()
	{
		OnRoomClientConnect();
		base.OnClientConnect();
	}

	public override void OnClientDisconnect()
	{
		OnRoomClientDisconnect();
		base.OnClientDisconnect();
	}

	public override void OnStopClient()
	{
		OnRoomStopClient();
		CallOnClientExitRoom();
		roomSlots.Clear();
	}

	public override void OnClientSceneChanged()
	{
		if (Utils.IsSceneActive(RoomScene))
		{
			if (NetworkClient.isConnected)
			{
				CallOnClientEnterRoom();
			}
		}
		else
		{
			CallOnClientExitRoom();
		}
		base.OnClientSceneChanged();
		OnRoomClientSceneChanged();
	}

	public virtual void OnRoomStartHost()
	{
	}

	public virtual void OnRoomStopHost()
	{
	}

	public virtual void OnRoomStartServer()
	{
	}

	public virtual void OnRoomStopServer()
	{
	}

	public virtual void OnRoomServerConnect(NetworkConnectionToClient conn)
	{
	}

	public virtual void OnRoomServerDisconnect(NetworkConnectionToClient conn)
	{
	}

	public virtual void OnRoomServerSceneChanged(string sceneName)
	{
	}

	public virtual GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
	{
		return null;
	}

	public virtual GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
	{
		return null;
	}

	public virtual void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
	{
		base.OnServerAddPlayer(conn);
	}

	public virtual bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
	{
		return true;
	}

	public virtual void ReadyStatusChanged()
	{
		int num = 0;
		int num2 = 0;
		foreach (NetworkRoomPlayer roomSlot in roomSlots)
		{
			if (roomSlot != null)
			{
				num++;
				if (roomSlot.readyToBegin)
				{
					num2++;
				}
			}
		}
		if (num == num2)
		{
			CheckReadyToBegin();
		}
		else
		{
			allPlayersReady = false;
		}
	}

	public virtual void OnRoomServerPlayersReady()
	{
		ServerChangeScene(GameplayScene);
	}

	public virtual void OnRoomServerPlayersNotReady()
	{
	}

	public virtual void OnRoomClientEnter()
	{
	}

	public virtual void OnRoomClientExit()
	{
	}

	public virtual void OnRoomClientConnect()
	{
	}

	public virtual void OnRoomClientDisconnect()
	{
	}

	public virtual void OnRoomStartClient()
	{
	}

	public virtual void OnRoomStopClient()
	{
	}

	public virtual void OnRoomClientSceneChanged()
	{
	}

	public virtual void OnGUI()
	{
		if (!showRoomGUI)
		{
			return;
		}
		if (NetworkServer.active && Utils.IsSceneActive(GameplayScene))
		{
			GUILayout.BeginArea(new Rect((float)Screen.width - 150f, 10f, 140f, 30f));
			if (GUILayout.Button("Return to Room"))
			{
				ServerChangeScene(RoomScene);
			}
			GUILayout.EndArea();
		}
		if (Utils.IsSceneActive(RoomScene))
		{
			GUI.Box(new Rect(10f, 180f, 520f, 150f), "PLAYERS");
		}
	}
}
