using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.MultipleMatch;

[RequireComponent(typeof(NetworkMatch))]
public class MatchController : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CServerEndMatch_003Ed__32 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public MatchController _003C_003E4__this;

		public bool disconnected;

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
		public _003CServerEndMatch_003Ed__32(int _003C_003E1__state)
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
			MatchController matchController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				matchController.RpcExitGame();
				matchController.canvasController.OnPlayerDisconnected -= matchController.OnPlayerDisconnected;
				_003C_003E2__current = new WaitForSeconds(0.1f);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (!disconnected)
				{
					NetworkServer.RemovePlayerForConnection(matchController.player1.connectionToClient, destroyServerObject: true);
					CanvasController.waitingConnections.Add(matchController.player1.connectionToClient);
					NetworkServer.RemovePlayerForConnection(matchController.player2.connectionToClient, destroyServerObject: true);
					CanvasController.waitingConnections.Add(matchController.player2.connectionToClient);
				}
				else if (conn == matchController.player1.connectionToClient)
				{
					NetworkServer.RemovePlayerForConnection(matchController.player2.connectionToClient, destroyServerObject: true);
					CanvasController.waitingConnections.Add(matchController.player2.connectionToClient);
				}
				else if (conn == matchController.player2.connectionToClient)
				{
					NetworkServer.RemovePlayerForConnection(matchController.player1.connectionToClient, destroyServerObject: true);
					CanvasController.waitingConnections.Add(matchController.player1.connectionToClient);
				}
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				matchController.canvasController.SendMatchList();
				NetworkServer.Destroy(matchController.gameObject);
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

	internal readonly SyncDictionary<NetworkIdentity, MatchPlayerData> matchPlayerData = new SyncDictionary<NetworkIdentity, MatchPlayerData>();

	internal readonly Dictionary<CellValue, CellGUI> MatchCells = new Dictionary<CellValue, CellGUI>();

	private CellValue boardScore;

	private bool playAgain;

	[Header("GUI References")]
	public CanvasGroup canvasGroup;

	public Text gameText;

	public Button exitButton;

	public Button playAgainButton;

	public Text winCountLocal;

	public Text winCountOpponent;

	[Header("Diagnostics - Do Not Modify")]
	public CanvasController canvasController;

	public NetworkIdentity player1;

	public NetworkIdentity player2;

	public NetworkIdentity startingPlayer;

	[SyncVar(hook = "UpdateGameUI")]
	public NetworkIdentity currentPlayer;

	protected uint ___currentPlayerNetId;

	public NetworkIdentity NetworkcurrentPlayer
	{
		get
		{
			return GetSyncVarNetworkIdentity(___currentPlayerNetId, ref currentPlayer);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkIdentity(value, ref currentPlayer, 1uL, UpdateGameUI, ref ___currentPlayerNetId);
		}
	}

	private void Awake()
	{
		canvasController = UnityEngine.Object.FindObjectOfType<CanvasController>();
	}

	public override void OnStartServer()
	{
		StartCoroutine(AddPlayersToMatchController());
	}

	private IEnumerator AddPlayersToMatchController()
	{
		yield return null;
		matchPlayerData.Add(player1, new MatchPlayerData
		{
			playerIndex = CanvasController.playerInfos[player1.connectionToClient].playerIndex
		});
		matchPlayerData.Add(player2, new MatchPlayerData
		{
			playerIndex = CanvasController.playerInfos[player2.connectionToClient].playerIndex
		});
	}

	public override void OnStartClient()
	{
		matchPlayerData.Callback += UpdateWins;
		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		exitButton.gameObject.SetActive(value: false);
		playAgainButton.gameObject.SetActive(value: false);
	}

	[ClientCallback]
	public void UpdateGameUI(NetworkIdentity _, NetworkIdentity newPlayerTurn)
	{
		if (NetworkClient.active && (bool)newPlayerTurn)
		{
			if (newPlayerTurn.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
			{
				gameText.text = "Your Turn";
				gameText.color = Color.blue;
			}
			else
			{
				gameText.text = "Their Turn";
				gameText.color = Color.red;
			}
		}
	}

	[ClientCallback]
	public void UpdateWins(SyncIDictionary<NetworkIdentity, MatchPlayerData>.Operation op, NetworkIdentity key, MatchPlayerData matchPlayerData)
	{
		if (NetworkClient.active)
		{
			if (key.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
			{
				winCountLocal.text = $"Player {matchPlayerData.playerIndex}\n{matchPlayerData.wins}";
			}
			else
			{
				winCountOpponent.text = $"Player {matchPlayerData.playerIndex}\n{matchPlayerData.wins}";
			}
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdMakePlay(CellValue cellValue, NetworkConnectionToClient sender = null)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_Mirror_002EExamples_002EMultipleMatch_002ECellValue(writer, cellValue);
		SendCommandInternal("System.Void Mirror.Examples.MultipleMatch.MatchController::CmdMakePlay(Mirror.Examples.MultipleMatch.CellValue,Mirror.NetworkConnectionToClient)", 1735971840, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	private bool CheckWinner(CellValue currentScore)
	{
		if (!NetworkServer.active)
		{
			return default(bool);
		}
		if ((currentScore & CellValue.TopRow) == CellValue.TopRow)
		{
			return true;
		}
		if ((currentScore & CellValue.MidRow) == CellValue.MidRow)
		{
			return true;
		}
		if ((currentScore & CellValue.BotRow) == CellValue.BotRow)
		{
			return true;
		}
		if ((currentScore & CellValue.LeftCol) == CellValue.LeftCol)
		{
			return true;
		}
		if ((currentScore & CellValue.MidCol) == CellValue.MidCol)
		{
			return true;
		}
		if ((currentScore & CellValue.RightCol) == CellValue.RightCol)
		{
			return true;
		}
		if ((currentScore & CellValue.Diag1) == CellValue.Diag1)
		{
			return true;
		}
		if ((currentScore & CellValue.Diag2) == CellValue.Diag2)
		{
			return true;
		}
		return false;
	}

	[ClientRpc]
	public void RpcUpdateCell(CellValue cellValue, NetworkIdentity player)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_Mirror_002EExamples_002EMultipleMatch_002ECellValue(writer, cellValue);
		writer.WriteNetworkIdentity(player);
		SendRPCInternal("System.Void Mirror.Examples.MultipleMatch.MatchController::RpcUpdateCell(Mirror.Examples.MultipleMatch.CellValue,Mirror.NetworkIdentity)", 1108510426, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void RpcShowWinner(NetworkIdentity winner)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkIdentity(winner);
		SendRPCInternal("System.Void Mirror.Examples.MultipleMatch.MatchController::RpcShowWinner(Mirror.NetworkIdentity)", 405402071, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void RequestPlayAgain()
	{
		if (NetworkClient.active)
		{
			playAgainButton.gameObject.SetActive(value: false);
			CmdPlayAgain();
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdPlayAgain(NetworkConnectionToClient sender = null)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Mirror.Examples.MultipleMatch.MatchController::CmdPlayAgain(Mirror.NetworkConnectionToClient)", -231214242, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void RestartGame()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (CellGUI value2 in MatchCells.Values)
		{
			value2.SetPlayer(null);
		}
		boardScore = CellValue.None;
		NetworkIdentity[] array = new NetworkIdentity[matchPlayerData.Keys.Count];
		matchPlayerData.Keys.CopyTo(array, 0);
		NetworkIdentity[] array2 = array;
		foreach (NetworkIdentity i2 in array2)
		{
			MatchPlayerData value = matchPlayerData[i2];
			value.currentScore = CellValue.None;
			matchPlayerData[i2] = value;
		}
		RpcRestartGame();
		startingPlayer = ((startingPlayer == player1) ? player2 : player1);
		NetworkcurrentPlayer = startingPlayer;
	}

	[ClientRpc]
	public void RpcRestartGame()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Mirror.Examples.MultipleMatch.MatchController::RpcRestartGame()", -339873653, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Client]
	public void RequestExitGame()
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogWarning("[Client] function 'System.Void Mirror.Examples.MultipleMatch.MatchController::RequestExitGame()' called when client was not active");
			return;
		}
		exitButton.gameObject.SetActive(value: false);
		playAgainButton.gameObject.SetActive(value: false);
		CmdRequestExitGame();
	}

	[Command(requiresAuthority = false)]
	public void CmdRequestExitGame(NetworkConnectionToClient sender = null)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Mirror.Examples.MultipleMatch.MatchController::CmdRequestExitGame(Mirror.NetworkConnectionToClient)", 535875825, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void OnPlayerDisconnected(NetworkConnectionToClient conn)
	{
		if (NetworkServer.active && (player1 == conn.identity || player2 == conn.identity))
		{
			StartCoroutine(ServerEndMatch(conn, disconnected: true));
		}
	}

	[IteratorStateMachine(typeof(_003CServerEndMatch_003Ed__32))]
	[ServerCallback]
	public IEnumerator ServerEndMatch(NetworkConnectionToClient conn, bool disconnected)
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CServerEndMatch_003Ed__32(0)
		{
			_003C_003E4__this = this,
			conn = conn,
			disconnected = disconnected
		};
	}

	[ClientRpc]
	public void RpcExitGame()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Mirror.Examples.MultipleMatch.MatchController::RpcExitGame()", 1034181288, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public MatchController()
	{
		InitSyncObject(matchPlayerData);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdMakePlay__CellValue__NetworkConnectionToClient(CellValue cellValue, NetworkConnectionToClient sender)
	{
		if (!(sender.identity != NetworkcurrentPlayer) && !(MatchCells[cellValue].playerIdentity != null))
		{
			MatchCells[cellValue].playerIdentity = NetworkcurrentPlayer;
			RpcUpdateCell(cellValue, NetworkcurrentPlayer);
			MatchPlayerData value = matchPlayerData[NetworkcurrentPlayer];
			value.currentScore |= cellValue;
			matchPlayerData[NetworkcurrentPlayer] = value;
			boardScore |= cellValue;
			if (CheckWinner(value.currentScore))
			{
				value.wins++;
				matchPlayerData[NetworkcurrentPlayer] = value;
				RpcShowWinner(NetworkcurrentPlayer);
				NetworkcurrentPlayer = null;
			}
			else if (boardScore == CellValue.Full)
			{
				RpcShowWinner(null);
				NetworkcurrentPlayer = null;
			}
			else
			{
				NetworkcurrentPlayer = ((NetworkcurrentPlayer == player1) ? player2 : player1);
			}
		}
	}

	protected static void InvokeUserCode_CmdMakePlay__CellValue__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdMakePlay called on client.");
		}
		else
		{
			((MatchController)obj).UserCode_CmdMakePlay__CellValue__NetworkConnectionToClient(GeneratedNetworkCode._Read_Mirror_002EExamples_002EMultipleMatch_002ECellValue(reader), senderConnection);
		}
	}

	protected void UserCode_RpcUpdateCell__CellValue__NetworkIdentity(CellValue cellValue, NetworkIdentity player)
	{
		MatchCells[cellValue].SetPlayer(player);
	}

	protected static void InvokeUserCode_RpcUpdateCell__CellValue__NetworkIdentity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcUpdateCell called on server.");
		}
		else
		{
			((MatchController)obj).UserCode_RpcUpdateCell__CellValue__NetworkIdentity(GeneratedNetworkCode._Read_Mirror_002EExamples_002EMultipleMatch_002ECellValue(reader), reader.ReadNetworkIdentity());
		}
	}

	protected void UserCode_RpcShowWinner__NetworkIdentity(NetworkIdentity winner)
	{
		foreach (CellGUI value in MatchCells.Values)
		{
			value.GetComponent<Button>().interactable = false;
		}
		if (winner == null)
		{
			gameText.text = "Draw!";
			gameText.color = Color.yellow;
		}
		else if (winner.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
		{
			gameText.text = "Winner!";
			gameText.color = Color.blue;
		}
		else
		{
			gameText.text = "Loser!";
			gameText.color = Color.red;
		}
		exitButton.gameObject.SetActive(value: true);
		playAgainButton.gameObject.SetActive(value: true);
	}

	protected static void InvokeUserCode_RpcShowWinner__NetworkIdentity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcShowWinner called on server.");
		}
		else
		{
			((MatchController)obj).UserCode_RpcShowWinner__NetworkIdentity(reader.ReadNetworkIdentity());
		}
	}

	protected void UserCode_CmdPlayAgain__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!playAgain)
		{
			playAgain = true;
			return;
		}
		playAgain = false;
		RestartGame();
	}

	protected static void InvokeUserCode_CmdPlayAgain__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdPlayAgain called on client.");
		}
		else
		{
			((MatchController)obj).UserCode_CmdPlayAgain__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcRestartGame()
	{
		foreach (CellGUI value in MatchCells.Values)
		{
			value.SetPlayer(null);
		}
		exitButton.gameObject.SetActive(value: false);
		playAgainButton.gameObject.SetActive(value: false);
	}

	protected static void InvokeUserCode_RpcRestartGame(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcRestartGame called on server.");
		}
		else
		{
			((MatchController)obj).UserCode_RpcRestartGame();
		}
	}

	protected void UserCode_CmdRequestExitGame__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		StartCoroutine(ServerEndMatch(sender, disconnected: false));
	}

	protected static void InvokeUserCode_CmdRequestExitGame__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdRequestExitGame called on client.");
		}
		else
		{
			((MatchController)obj).UserCode_CmdRequestExitGame__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcExitGame()
	{
		canvasController.OnMatchEnded();
	}

	protected static void InvokeUserCode_RpcExitGame(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcExitGame called on server.");
		}
		else
		{
			((MatchController)obj).UserCode_RpcExitGame();
		}
	}

	static MatchController()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(MatchController), "System.Void Mirror.Examples.MultipleMatch.MatchController::CmdMakePlay(Mirror.Examples.MultipleMatch.CellValue,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdMakePlay__CellValue__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(MatchController), "System.Void Mirror.Examples.MultipleMatch.MatchController::CmdPlayAgain(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayAgain__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(MatchController), "System.Void Mirror.Examples.MultipleMatch.MatchController::CmdRequestExitGame(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdRequestExitGame__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(MatchController), "System.Void Mirror.Examples.MultipleMatch.MatchController::RpcUpdateCell(Mirror.Examples.MultipleMatch.CellValue,Mirror.NetworkIdentity)", InvokeUserCode_RpcUpdateCell__CellValue__NetworkIdentity);
		RemoteProcedureCalls.RegisterRpc(typeof(MatchController), "System.Void Mirror.Examples.MultipleMatch.MatchController::RpcShowWinner(Mirror.NetworkIdentity)", InvokeUserCode_RpcShowWinner__NetworkIdentity);
		RemoteProcedureCalls.RegisterRpc(typeof(MatchController), "System.Void Mirror.Examples.MultipleMatch.MatchController::RpcRestartGame()", InvokeUserCode_RpcRestartGame);
		RemoteProcedureCalls.RegisterRpc(typeof(MatchController), "System.Void Mirror.Examples.MultipleMatch.MatchController::RpcExitGame()", InvokeUserCode_RpcExitGame);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteNetworkIdentity(NetworkcurrentPlayer);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkIdentity(NetworkcurrentPlayer);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkIdentity(ref currentPlayer, UpdateGameUI, reader, ref ___currentPlayerNetId);
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkIdentity(ref currentPlayer, UpdateGameUI, reader, ref ___currentPlayerNetId);
		}
	}
}
