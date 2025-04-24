using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mirror.Examples.Basic;

public class Player : NetworkBehaviour
{
	private static readonly List<Player> playersList = new List<Player>();

	[Header("Player UI")]
	public GameObject playerUIPrefab;

	private GameObject playerUIObject;

	private PlayerUI playerUI;

	[Header("SyncVars")]
	[SyncVar(hook = "PlayerNumberChanged")]
	public byte playerNumber;

	[SyncVar(hook = "PlayerColorChanged")]
	public Color32 playerColor = Color.white;

	[SyncVar(hook = "PlayerDataChanged")]
	public ushort playerData;

	public byte NetworkplayerNumber
	{
		get
		{
			return playerNumber;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref playerNumber, 1uL, PlayerNumberChanged);
		}
	}

	public Color32 NetworkplayerColor
	{
		get
		{
			return playerColor;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref playerColor, 2uL, PlayerColorChanged);
		}
	}

	public ushort NetworkplayerData
	{
		get
		{
			return playerData;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref playerData, 4uL, PlayerDataChanged);
		}
	}

	public event Action<byte> OnPlayerNumberChanged;

	public event Action<Color32> OnPlayerColorChanged;

	public event Action<ushort> OnPlayerDataChanged;

	private void PlayerNumberChanged(byte _, byte newPlayerNumber)
	{
		this.OnPlayerNumberChanged?.Invoke(newPlayerNumber);
	}

	private void PlayerColorChanged(Color32 _, Color32 newPlayerColor)
	{
		this.OnPlayerColorChanged?.Invoke(newPlayerColor);
	}

	private void PlayerDataChanged(ushort _, ushort newPlayerData)
	{
		this.OnPlayerDataChanged?.Invoke(newPlayerData);
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		playersList.Add(this);
		NetworkplayerColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.9f, 0.9f, 1f, 1f);
		NetworkplayerData = (ushort)UnityEngine.Random.Range(100, 1000);
		InvokeRepeating("UpdateData", 1f, 1f);
	}

	[ServerCallback]
	internal static void ResetPlayerNumbers()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		byte b = 0;
		foreach (Player players in playersList)
		{
			players.NetworkplayerNumber = b++;
		}
	}

	[ServerCallback]
	private void UpdateData()
	{
		if (NetworkServer.active)
		{
			NetworkplayerData = (ushort)UnityEngine.Random.Range(100, 1000);
		}
	}

	public override void OnStopServer()
	{
		CancelInvoke();
		playersList.Remove(this);
	}

	public override void OnStartClient()
	{
		playerUIObject = UnityEngine.Object.Instantiate(playerUIPrefab, CanvasUI.GetPlayersPanel());
		playerUI = playerUIObject.GetComponent<PlayerUI>();
		this.OnPlayerNumberChanged = playerUI.OnPlayerNumberChanged;
		this.OnPlayerColorChanged = playerUI.OnPlayerColorChanged;
		this.OnPlayerDataChanged = playerUI.OnPlayerDataChanged;
		this.OnPlayerNumberChanged(playerNumber);
		this.OnPlayerColorChanged(playerColor);
		this.OnPlayerDataChanged(playerData);
	}

	public override void OnStartLocalPlayer()
	{
		playerUI.SetLocalPlayer();
		CanvasUI.SetActive(active: true);
	}

	public override void OnStopLocalPlayer()
	{
		CanvasUI.SetActive(active: false);
	}

	public override void OnStopClient()
	{
		this.OnPlayerNumberChanged = null;
		this.OnPlayerColorChanged = null;
		this.OnPlayerDataChanged = null;
		UnityEngine.Object.Destroy(playerUIObject);
	}

	private void MirrorProcessed()
	{
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			NetworkWriterExtensions.WriteByte(writer, playerNumber);
			writer.WriteColor32(playerColor);
			writer.WriteUShort(playerData);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			NetworkWriterExtensions.WriteByte(writer, playerNumber);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteColor32(playerColor);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteUShort(playerData);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref playerNumber, PlayerNumberChanged, NetworkReaderExtensions.ReadByte(reader));
			GeneratedSyncVarDeserialize(ref playerColor, PlayerColorChanged, reader.ReadColor32());
			GeneratedSyncVarDeserialize(ref playerData, PlayerDataChanged, reader.ReadUShort());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref playerNumber, PlayerNumberChanged, NetworkReaderExtensions.ReadByte(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref playerColor, PlayerColorChanged, reader.ReadColor32());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref playerData, PlayerDataChanged, reader.ReadUShort());
		}
	}
}
