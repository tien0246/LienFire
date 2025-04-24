using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.MultipleMatch;

public class CanvasController : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003COnServerDisconnect_003Ed__34 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public CanvasController _003C_003E4__this;

		public NetworkConnectionToClient conn;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003COnServerDisconnect_003Ed__34(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			CanvasController canvasController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				canvasController.OnPlayerDisconnected?.Invoke(conn);
				if (playerMatches.TryGetValue(conn, out var value))
				{
					playerMatches.Remove(conn);
					openMatches.Remove(value);
					foreach (NetworkConnectionToClient item in matchConnections[value])
					{
						PlayerInfo value2 = CanvasController.playerInfos[item];
						value2.ready = false;
						value2.matchId = Guid.Empty;
						CanvasController.playerInfos[item] = value2;
						item.Send(new ClientMatchMessage
						{
							clientMatchOperation = ClientMatchOperation.Departed
						});
					}
				}
				foreach (KeyValuePair<Guid, HashSet<NetworkConnectionToClient>> matchConnection in matchConnections)
				{
					matchConnection.Value.Remove(conn);
				}
				PlayerInfo playerInfo = CanvasController.playerInfos[conn];
				if (playerInfo.matchId != Guid.Empty)
				{
					if (openMatches.TryGetValue(playerInfo.matchId, out var value3))
					{
						value3.players--;
						openMatches[playerInfo.matchId] = value3;
					}
					if (matchConnections.TryGetValue(playerInfo.matchId, out var value4))
					{
						PlayerInfo[] playerInfos = value4.Select((NetworkConnectionToClient playerConn) => CanvasController.playerInfos[playerConn]).ToArray();
						foreach (NetworkConnectionToClient item2 in matchConnections[playerInfo.matchId])
						{
							if (item2 != conn)
							{
								item2.Send(new ClientMatchMessage
								{
									clientMatchOperation = ClientMatchOperation.UpdateRoom,
									playerInfos = playerInfos
								});
							}
						}
					}
				}
				canvasController.SendMatchList();
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			case 1:
				_003C_003E1__state = -1;
				return false;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	internal static readonly Dictionary<NetworkConnectionToClient, Guid> playerMatches = new Dictionary<NetworkConnectionToClient, Guid>();

	internal static readonly Dictionary<Guid, MatchInfo> openMatches = new Dictionary<Guid, MatchInfo>();

	internal static readonly Dictionary<Guid, HashSet<NetworkConnectionToClient>> matchConnections = new Dictionary<Guid, HashSet<NetworkConnectionToClient>>();

	internal static readonly Dictionary<NetworkConnection, PlayerInfo> playerInfos = new Dictionary<NetworkConnection, PlayerInfo>();

	internal static readonly List<NetworkConnectionToClient> waitingConnections = new List<NetworkConnectionToClient>();

	internal Guid localPlayerMatch = Guid.Empty;

	internal Guid localJoinedMatch = Guid.Empty;

	internal Guid selectedMatch = Guid.Empty;

	private int playerIndex = 1;

	[Header("GUI References")]
	public GameObject matchList;

	public GameObject matchPrefab;

	public GameObject matchControllerPrefab;

	public Button createButton;

	public Button joinButton;

	public GameObject lobbyView;

	public GameObject roomView;

	public RoomGUI roomGUI;

	public ToggleGroup toggleGroup;

	public event Action<NetworkConnectionToClient> OnPlayerDisconnected;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void ResetStatics()
	{
		playerMatches.Clear();
		openMatches.Clear();
		matchConnections.Clear();
		playerInfos.Clear();
		waitingConnections.Clear();
	}

	internal void InitializeData()
	{
		playerMatches.Clear();
		openMatches.Clear();
		matchConnections.Clear();
		waitingConnections.Clear();
		localPlayerMatch = Guid.Empty;
		localJoinedMatch = Guid.Empty;
	}

	private void ResetCanvas()
	{
		InitializeData();
		lobbyView.SetActive(value: false);
		roomView.SetActive(value: false);
		base.gameObject.SetActive(value: false);
	}

	[ClientCallback]
	public void SelectMatch(Guid matchId)
	{
		if (NetworkClient.active)
		{
			if (matchId == Guid.Empty)
			{
				selectedMatch = Guid.Empty;
				joinButton.interactable = false;
			}
			else if (!openMatches.Keys.Contains(matchId))
			{
				joinButton.interactable = false;
			}
			else
			{
				selectedMatch = matchId;
				MatchInfo matchInfo = openMatches[matchId];
				joinButton.interactable = matchInfo.players < matchInfo.maxPlayers;
			}
		}
	}

	[ClientCallback]
	public void RequestCreateMatch()
	{
		if (NetworkClient.active)
		{
			NetworkClient.Send(new ServerMatchMessage
			{
				serverMatchOperation = ServerMatchOperation.Create
			});
		}
	}

	[ClientCallback]
	public void RequestJoinMatch()
	{
		if (NetworkClient.active && !(selectedMatch == Guid.Empty))
		{
			NetworkClient.Send(new ServerMatchMessage
			{
				serverMatchOperation = ServerMatchOperation.Join,
				matchId = selectedMatch
			});
		}
	}

	[ClientCallback]
	public void RequestLeaveMatch()
	{
		if (NetworkClient.active && !(localJoinedMatch == Guid.Empty))
		{
			NetworkClient.Send(new ServerMatchMessage
			{
				serverMatchOperation = ServerMatchOperation.Leave,
				matchId = localJoinedMatch
			});
		}
	}

	[ClientCallback]
	public void RequestCancelMatch()
	{
		if (NetworkClient.active && !(localPlayerMatch == Guid.Empty))
		{
			NetworkClient.Send(new ServerMatchMessage
			{
				serverMatchOperation = ServerMatchOperation.Cancel
			});
		}
	}

	[ClientCallback]
	public void RequestReadyChange()
	{
		if (NetworkClient.active && (!(localPlayerMatch == Guid.Empty) || !(localJoinedMatch == Guid.Empty)))
		{
			Guid matchId = ((localPlayerMatch == Guid.Empty) ? localJoinedMatch : localPlayerMatch);
			NetworkClient.Send(new ServerMatchMessage
			{
				serverMatchOperation = ServerMatchOperation.Ready,
				matchId = matchId
			});
		}
	}

	[ClientCallback]
	public void RequestStartMatch()
	{
		if (NetworkClient.active && !(localPlayerMatch == Guid.Empty))
		{
			NetworkClient.Send(new ServerMatchMessage
			{
				serverMatchOperation = ServerMatchOperation.Start
			});
		}
	}

	[ClientCallback]
	public void OnMatchEnded()
	{
		if (NetworkClient.active)
		{
			localPlayerMatch = Guid.Empty;
			localJoinedMatch = Guid.Empty;
			ShowLobbyView();
		}
	}

	[ServerCallback]
	internal void OnStartServer()
	{
		if (NetworkServer.active)
		{
			InitializeData();
			NetworkServer.RegisterHandler<ServerMatchMessage>(OnServerMatchMessage);
		}
	}

	[ServerCallback]
	internal void OnServerReady(NetworkConnectionToClient conn)
	{
		if (NetworkServer.active)
		{
			waitingConnections.Add(conn);
			playerInfos.Add(conn, new PlayerInfo
			{
				playerIndex = playerIndex,
				ready = false
			});
			playerIndex++;
			SendMatchList();
		}
	}

	[IteratorStateMachine(typeof(_003COnServerDisconnect_003Ed__34))]
	[ServerCallback]
	internal IEnumerator OnServerDisconnect(NetworkConnectionToClient conn)
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003COnServerDisconnect_003Ed__34(0)
		{
			_003C_003E4__this = this,
			conn = conn
		};
	}

	[ServerCallback]
	internal void OnStopServer()
	{
		if (NetworkServer.active)
		{
			ResetCanvas();
		}
	}

	[ClientCallback]
	internal void OnClientConnect()
	{
		if (NetworkClient.active)
		{
			playerInfos.Add(NetworkClient.connection, new PlayerInfo
			{
				playerIndex = playerIndex,
				ready = false
			});
		}
	}

	[ClientCallback]
	internal void OnStartClient()
	{
		if (NetworkClient.active)
		{
			InitializeData();
			ShowLobbyView();
			createButton.gameObject.SetActive(value: true);
			joinButton.gameObject.SetActive(value: true);
			NetworkClient.RegisterHandler<ClientMatchMessage>(OnClientMatchMessage);
		}
	}

	[ClientCallback]
	internal void OnClientDisconnect()
	{
		if (NetworkClient.active)
		{
			InitializeData();
		}
	}

	[ClientCallback]
	internal void OnStopClient()
	{
		if (NetworkClient.active)
		{
			ResetCanvas();
		}
	}

	[ServerCallback]
	private void OnServerMatchMessage(NetworkConnectionToClient conn, ServerMatchMessage msg)
	{
		if (NetworkServer.active)
		{
			switch (msg.serverMatchOperation)
			{
			case ServerMatchOperation.None:
				UnityEngine.Debug.LogWarning("Missing ServerMatchOperation");
				break;
			case ServerMatchOperation.Create:
				OnServerCreateMatch(conn);
				break;
			case ServerMatchOperation.Cancel:
				OnServerCancelMatch(conn);
				break;
			case ServerMatchOperation.Start:
				OnServerStartMatch(conn);
				break;
			case ServerMatchOperation.Join:
				OnServerJoinMatch(conn, msg.matchId);
				break;
			case ServerMatchOperation.Leave:
				OnServerLeaveMatch(conn, msg.matchId);
				break;
			case ServerMatchOperation.Ready:
				OnServerPlayerReady(conn, msg.matchId);
				break;
			}
		}
	}

	[ServerCallback]
	private void OnServerPlayerReady(NetworkConnectionToClient conn, Guid matchId)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		PlayerInfo value = playerInfos[conn];
		value.ready = !value.ready;
		playerInfos[conn] = value;
		PlayerInfo[] array = matchConnections[matchId].Select((NetworkConnectionToClient playerConn) => playerInfos[playerConn]).ToArray();
		foreach (NetworkConnectionToClient item in matchConnections[matchId])
		{
			item.Send(new ClientMatchMessage
			{
				clientMatchOperation = ClientMatchOperation.UpdateRoom,
				playerInfos = array
			});
		}
	}

	[ServerCallback]
	private void OnServerLeaveMatch(NetworkConnectionToClient conn, Guid matchId)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		MatchInfo value = openMatches[matchId];
		value.players--;
		openMatches[matchId] = value;
		PlayerInfo value2 = playerInfos[conn];
		value2.ready = false;
		value2.matchId = Guid.Empty;
		playerInfos[conn] = value2;
		foreach (KeyValuePair<Guid, HashSet<NetworkConnectionToClient>> matchConnection in matchConnections)
		{
			matchConnection.Value.Remove(conn);
		}
		PlayerInfo[] array = matchConnections[matchId].Select((NetworkConnectionToClient playerConn) => playerInfos[playerConn]).ToArray();
		foreach (NetworkConnectionToClient item in matchConnections[matchId])
		{
			item.Send(new ClientMatchMessage
			{
				clientMatchOperation = ClientMatchOperation.UpdateRoom,
				playerInfos = array
			});
		}
		SendMatchList();
		conn.Send(new ClientMatchMessage
		{
			clientMatchOperation = ClientMatchOperation.Departed
		});
	}

	[ServerCallback]
	private void OnServerCreateMatch(NetworkConnectionToClient conn)
	{
		if (NetworkServer.active && !playerMatches.ContainsKey(conn))
		{
			Guid guid = Guid.NewGuid();
			matchConnections.Add(guid, new HashSet<NetworkConnectionToClient>());
			matchConnections[guid].Add(conn);
			playerMatches.Add(conn, guid);
			openMatches.Add(guid, new MatchInfo
			{
				matchId = guid,
				maxPlayers = 2,
				players = 1
			});
			PlayerInfo value = playerInfos[conn];
			value.ready = false;
			value.matchId = guid;
			playerInfos[conn] = value;
			PlayerInfo[] array = matchConnections[guid].Select((NetworkConnectionToClient playerConn) => playerInfos[playerConn]).ToArray();
			conn.Send(new ClientMatchMessage
			{
				clientMatchOperation = ClientMatchOperation.Created,
				matchId = guid,
				playerInfos = array
			});
			SendMatchList();
		}
	}

	[ServerCallback]
	private void OnServerCancelMatch(NetworkConnectionToClient conn)
	{
		if (!NetworkServer.active || !playerMatches.ContainsKey(conn))
		{
			return;
		}
		conn.Send(new ClientMatchMessage
		{
			clientMatchOperation = ClientMatchOperation.Cancelled
		});
		if (!playerMatches.TryGetValue(conn, out var value))
		{
			return;
		}
		playerMatches.Remove(conn);
		openMatches.Remove(value);
		foreach (NetworkConnectionToClient item in matchConnections[value])
		{
			PlayerInfo value2 = playerInfos[item];
			value2.ready = false;
			value2.matchId = Guid.Empty;
			playerInfos[item] = value2;
			item.Send(new ClientMatchMessage
			{
				clientMatchOperation = ClientMatchOperation.Departed
			});
		}
		SendMatchList();
	}

	[ServerCallback]
	private void OnServerStartMatch(NetworkConnectionToClient conn)
	{
		if (!NetworkServer.active || !playerMatches.ContainsKey(conn) || !playerMatches.TryGetValue(conn, out var value))
		{
			return;
		}
		GameObject obj = UnityEngine.Object.Instantiate(matchControllerPrefab);
		obj.GetComponent<NetworkMatch>().matchId = value;
		NetworkServer.Spawn(obj);
		MatchController component = obj.GetComponent<MatchController>();
		foreach (NetworkConnectionToClient item in matchConnections[value])
		{
			item.Send(new ClientMatchMessage
			{
				clientMatchOperation = ClientMatchOperation.Started
			});
			GameObject gameObject = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
			gameObject.GetComponent<NetworkMatch>().matchId = value;
			NetworkServer.AddPlayerForConnection(item, gameObject);
			if (component.player1 == null)
			{
				component.player1 = item.identity;
			}
			else
			{
				component.player2 = item.identity;
			}
			PlayerInfo value2 = playerInfos[item];
			value2.ready = false;
			playerInfos[item] = value2;
		}
		component.startingPlayer = component.player1;
		component.NetworkcurrentPlayer = component.player1;
		playerMatches.Remove(conn);
		openMatches.Remove(value);
		matchConnections.Remove(value);
		SendMatchList();
		OnPlayerDisconnected += component.OnPlayerDisconnected;
	}

	[ServerCallback]
	private void OnServerJoinMatch(NetworkConnectionToClient conn, Guid matchId)
	{
		if (!NetworkServer.active || !matchConnections.ContainsKey(matchId) || !openMatches.ContainsKey(matchId))
		{
			return;
		}
		MatchInfo value = openMatches[matchId];
		value.players++;
		openMatches[matchId] = value;
		matchConnections[matchId].Add(conn);
		PlayerInfo value2 = playerInfos[conn];
		value2.ready = false;
		value2.matchId = matchId;
		playerInfos[conn] = value2;
		PlayerInfo[] array = matchConnections[matchId].Select((NetworkConnectionToClient playerConn) => playerInfos[playerConn]).ToArray();
		SendMatchList();
		conn.Send(new ClientMatchMessage
		{
			clientMatchOperation = ClientMatchOperation.Joined,
			matchId = matchId,
			playerInfos = array
		});
		foreach (NetworkConnectionToClient item in matchConnections[matchId])
		{
			item.Send(new ClientMatchMessage
			{
				clientMatchOperation = ClientMatchOperation.UpdateRoom,
				playerInfos = array
			});
		}
	}

	[ServerCallback]
	internal void SendMatchList(NetworkConnectionToClient conn = null)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (conn != null)
		{
			conn.Send(new ClientMatchMessage
			{
				clientMatchOperation = ClientMatchOperation.List,
				matchInfos = openMatches.Values.ToArray()
			});
			return;
		}
		foreach (NetworkConnectionToClient waitingConnection in waitingConnections)
		{
			waitingConnection.Send(new ClientMatchMessage
			{
				clientMatchOperation = ClientMatchOperation.List,
				matchInfos = openMatches.Values.ToArray()
			});
		}
	}

	[ClientCallback]
	private void OnClientMatchMessage(ClientMatchMessage msg)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		switch (msg.clientMatchOperation)
		{
		case ClientMatchOperation.None:
			UnityEngine.Debug.LogWarning("Missing ClientMatchOperation");
			break;
		case ClientMatchOperation.List:
		{
			openMatches.Clear();
			MatchInfo[] matchInfos = msg.matchInfos;
			for (int i = 0; i < matchInfos.Length; i++)
			{
				MatchInfo value = matchInfos[i];
				openMatches.Add(value.matchId, value);
			}
			RefreshMatchList();
			break;
		}
		case ClientMatchOperation.Created:
			localPlayerMatch = msg.matchId;
			ShowRoomView();
			roomGUI.RefreshRoomPlayers(msg.playerInfos);
			roomGUI.SetOwner(owner: true);
			break;
		case ClientMatchOperation.Cancelled:
			localPlayerMatch = Guid.Empty;
			ShowLobbyView();
			break;
		case ClientMatchOperation.Joined:
			localJoinedMatch = msg.matchId;
			ShowRoomView();
			roomGUI.RefreshRoomPlayers(msg.playerInfos);
			roomGUI.SetOwner(owner: false);
			break;
		case ClientMatchOperation.Departed:
			localJoinedMatch = Guid.Empty;
			ShowLobbyView();
			break;
		case ClientMatchOperation.UpdateRoom:
			roomGUI.RefreshRoomPlayers(msg.playerInfos);
			break;
		case ClientMatchOperation.Started:
			lobbyView.SetActive(value: false);
			roomView.SetActive(value: false);
			break;
		}
	}

	[ClientCallback]
	private void ShowLobbyView()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		lobbyView.SetActive(value: true);
		roomView.SetActive(value: false);
		foreach (Transform item in matchList.transform)
		{
			if (item.gameObject.GetComponent<MatchGUI>().GetMatchId() == selectedMatch)
			{
				item.gameObject.GetComponent<Toggle>().isOn = true;
			}
		}
	}

	[ClientCallback]
	private void ShowRoomView()
	{
		if (NetworkClient.active)
		{
			lobbyView.SetActive(value: false);
			roomView.SetActive(value: true);
		}
	}

	[ClientCallback]
	private void RefreshMatchList()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		foreach (Transform item in matchList.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		joinButton.interactable = false;
		foreach (MatchInfo value in openMatches.Values)
		{
			GameObject obj = UnityEngine.Object.Instantiate(matchPrefab, Vector3.zero, Quaternion.identity);
			obj.transform.SetParent(matchList.transform, worldPositionStays: false);
			obj.GetComponent<MatchGUI>().SetMatchInfo(value);
			Toggle component = obj.GetComponent<Toggle>();
			component.group = toggleGroup;
			if (value.matchId == selectedMatch)
			{
				component.isOn = true;
			}
		}
	}
}
