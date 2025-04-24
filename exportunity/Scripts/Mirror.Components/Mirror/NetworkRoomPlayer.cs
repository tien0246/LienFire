using System.Runtime.InteropServices;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[AddComponentMenu("Network/Network Room Player")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-room-player")]
public class NetworkRoomPlayer : NetworkBehaviour
{
	[Tooltip("This flag controls whether the default UI is shown for the room player")]
	public bool showRoomGUI = true;

	[Header("Diagnostics")]
	[Tooltip("Diagnostic flag indicating whether this player is ready for the game to begin")]
	[SyncVar(hook = "ReadyStateChanged")]
	public bool readyToBegin;

	[Tooltip("Diagnostic index of the player, e.g. Player1, Player2, etc.")]
	[SyncVar(hook = "IndexChanged")]
	public int index;

	public bool NetworkreadyToBegin
	{
		get
		{
			return readyToBegin;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref readyToBegin, 1uL, ReadyStateChanged);
		}
	}

	public int Networkindex
	{
		get
		{
			return index;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref index, 2uL, IndexChanged);
		}
	}

	public void Start()
	{
		if (NetworkManager.singleton is NetworkRoomManager networkRoomManager)
		{
			if (networkRoomManager.dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			networkRoomManager.roomSlots.Add(this);
			if (NetworkServer.active)
			{
				networkRoomManager.RecalculateRoomPlayerIndices();
			}
			if (NetworkClient.active)
			{
				networkRoomManager.CallOnClientEnterRoom();
			}
		}
		else
		{
			Debug.LogError("RoomPlayer could not find a NetworkRoomManager. The RoomPlayer requires a NetworkRoomManager object to function. Make sure that there is one in the scene.");
		}
	}

	public virtual void OnDisable()
	{
		if (NetworkClient.active && NetworkManager.singleton is NetworkRoomManager networkRoomManager)
		{
			networkRoomManager.roomSlots.Remove(this);
			networkRoomManager.CallOnClientExitRoom();
		}
	}

	[Command]
	public void CmdChangeReadyState(bool readyState)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(readyState);
		SendCommandInternal("System.Void Mirror.NetworkRoomPlayer::CmdChangeReadyState(System.Boolean)", -485050255, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public virtual void IndexChanged(int oldIndex, int newIndex)
	{
	}

	public virtual void ReadyStateChanged(bool oldReadyState, bool newReadyState)
	{
	}

	public virtual void OnClientEnterRoom()
	{
	}

	public virtual void OnClientExitRoom()
	{
	}

	public virtual void OnGUI()
	{
		if (showRoomGUI)
		{
			NetworkRoomManager networkRoomManager = NetworkManager.singleton as NetworkRoomManager;
			if ((bool)networkRoomManager && networkRoomManager.showRoomGUI && Utils.IsSceneActive(networkRoomManager.RoomScene))
			{
				DrawPlayerReadyState();
				DrawPlayerReadyButton();
			}
		}
	}

	private void DrawPlayerReadyState()
	{
		GUILayout.BeginArea(new Rect(20f + (float)(index * 100), 200f, 90f, 130f));
		GUILayout.Label($"Player [{index + 1}]");
		if (readyToBegin)
		{
			GUILayout.Label("Ready");
		}
		else
		{
			GUILayout.Label("Not Ready");
		}
		if (((base.isServer && index > 0) || base.isServerOnly) && GUILayout.Button("REMOVE"))
		{
			GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
		}
		GUILayout.EndArea();
	}

	private void DrawPlayerReadyButton()
	{
		if (!NetworkClient.active || !base.isLocalPlayer)
		{
			return;
		}
		GUILayout.BeginArea(new Rect(20f, 300f, 120f, 20f));
		if (readyToBegin)
		{
			if (GUILayout.Button("Cancel"))
			{
				CmdChangeReadyState(readyState: false);
			}
		}
		else if (GUILayout.Button("Ready"))
		{
			CmdChangeReadyState(readyState: true);
		}
		GUILayout.EndArea();
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdChangeReadyState__Boolean(bool readyState)
	{
		NetworkreadyToBegin = readyState;
		NetworkRoomManager networkRoomManager = NetworkManager.singleton as NetworkRoomManager;
		if (networkRoomManager != null)
		{
			networkRoomManager.ReadyStatusChanged();
		}
	}

	protected static void InvokeUserCode_CmdChangeReadyState__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdChangeReadyState called on client.");
		}
		else
		{
			((NetworkRoomPlayer)obj).UserCode_CmdChangeReadyState__Boolean(reader.ReadBool());
		}
	}

	static NetworkRoomPlayer()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRoomPlayer), "System.Void Mirror.NetworkRoomPlayer::CmdChangeReadyState(System.Boolean)", InvokeUserCode_CmdChangeReadyState__Boolean, requiresAuthority: true);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(readyToBegin);
			writer.WriteInt(index);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(readyToBegin);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteInt(index);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref readyToBegin, ReadyStateChanged, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref index, IndexChanged, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref readyToBegin, ReadyStateChanged, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref index, IndexChanged, reader.ReadInt());
		}
	}
}
