using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("")]
[RequireComponent(typeof(NetworkIdentity))]
[HelpURL("https://mirror-networking.gitbook.io/docs/guides/networkbehaviour")]
public abstract class NetworkBehaviour : MonoBehaviour
{
	[Tooltip("Server Authority calls OnSerialize on the server and syncs it to clients.\n\nClient Authority calls OnSerialize on the owning client, syncs it to server, which then broadcasts it to all other clients.\n\nUse server authority for cheat safety.")]
	[HideInInspector]
	public SyncDirection syncDirection;

	[Tooltip("By default synced data is sent from the server to all Observers of the object.\nChange this to Owner to only have the server update the client that has ownership authority for this object")]
	[HideInInspector]
	public SyncMode syncMode;

	[Tooltip("Time in seconds until next change is synchronized to the client. '0' means send immediately if changed. '0.5' means only send changes every 500ms.\n(This is for state synchronization like SyncVars, SyncLists, OnSerialize. Not for Cmds, Rpcs, etc.)")]
	[Range(0f, 2f)]
	[HideInInspector]
	public float syncInterval;

	internal double lastSyncTime;

	protected readonly List<SyncObject> syncObjects = new List<SyncObject>();

	internal ulong syncObjectDirtyBits;

	private ulong syncVarHookGuard;

	public bool isServer => netIdentity.isServer;

	public bool isClient => netIdentity.isClient;

	public bool isLocalPlayer => netIdentity.isLocalPlayer;

	public bool isServerOnly => netIdentity.isServerOnly;

	public bool isClientOnly => netIdentity.isClientOnly;

	public bool isOwned => netIdentity.isOwned;

	[Obsolete(".hasAuthority was renamed to .isOwned. This is easier to understand and prepares for SyncDirection, where there is a difference betwen isOwned and authority.")]
	public bool hasAuthority => isOwned;

	public bool authority
	{
		get
		{
			if (!isClient)
			{
				return syncDirection == SyncDirection.ServerToClient;
			}
			if (syncDirection == SyncDirection.ClientToServer)
			{
				return isOwned;
			}
			return false;
		}
	}

	public uint netId => netIdentity.netId;

	public NetworkConnection connectionToServer => netIdentity.connectionToServer;

	public NetworkConnectionToClient connectionToClient => netIdentity.connectionToClient;

	public NetworkIdentity netIdentity { get; internal set; }

	public byte ComponentIndex { get; internal set; }

	protected ulong syncVarDirtyBits { get; private set; }

	internal bool HasSyncObjects()
	{
		return syncObjects.Count > 0;
	}

	protected bool GetSyncVarHookGuard(ulong dirtyBit)
	{
		return (syncVarHookGuard & dirtyBit) != 0;
	}

	protected void SetSyncVarHookGuard(ulong dirtyBit, bool value)
	{
		if (value)
		{
			syncVarHookGuard |= dirtyBit;
		}
		else
		{
			syncVarHookGuard &= ~dirtyBit;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetSyncObjectDirtyBit(ulong dirtyBit)
	{
		syncObjectDirtyBits |= dirtyBit;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetSyncVarDirtyBit(ulong dirtyBit)
	{
		syncVarDirtyBits |= dirtyBit;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetDirty()
	{
		SetSyncVarDirtyBit(ulong.MaxValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsDirty()
	{
		if ((syncVarDirtyBits | syncObjectDirtyBits) != 0L)
		{
			return NetworkTime.localTime - lastSyncTime >= (double)syncInterval;
		}
		return false;
	}

	public void ClearAllDirtyBits()
	{
		lastSyncTime = NetworkTime.localTime;
		syncVarDirtyBits = 0uL;
		syncObjectDirtyBits = 0uL;
		for (int i = 0; i < syncObjects.Count; i++)
		{
			syncObjects[i].ClearChanges();
		}
	}

	protected void InitSyncObject(SyncObject syncObject)
	{
		if (syncObject == null)
		{
			Debug.LogError("Uninitialized SyncObject. Manually call the constructor on your SyncList, SyncSet, SyncDictionary or SyncField<T>");
			return;
		}
		int count = syncObjects.Count;
		syncObjects.Add(syncObject);
		ulong nthBit = (ulong)(1L << count);
		syncObject.OnDirty = delegate
		{
			SetSyncObjectDirtyBit(nthBit);
		};
		syncObject.IsWritable = delegate
		{
			if (NetworkServer.active && NetworkClient.active)
			{
				if (syncDirection != SyncDirection.ServerToClient)
				{
					return isOwned;
				}
				return true;
			}
			if (NetworkServer.active)
			{
				return syncDirection == SyncDirection.ServerToClient;
			}
			if (NetworkClient.active)
			{
				if (netId != 0)
				{
					if (syncDirection == SyncDirection.ClientToServer)
					{
						return isOwned;
					}
					return false;
				}
				return true;
			}
			throw new Exception("InitSyncObject: IsWritable: neither NetworkServer nor NetworkClient are active.");
		};
		syncObject.IsRecording = delegate
		{
			if (isServer && isClient)
			{
				return netIdentity.observers.Count > 0;
			}
			if (isServer)
			{
				return netIdentity.observers.Count > 0;
			}
			return isClient && syncDirection == SyncDirection.ClientToServer && isOwned;
		};
	}

	protected void SendCommandInternal(string functionFullName, int functionHashCode, NetworkWriter writer, int channelId, bool requiresAuthority = true)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command Function " + functionFullName + " called on " + base.name + " without an active client.", base.gameObject);
		}
		else if (!NetworkClient.ready)
		{
			if (channelId == 0)
			{
				Debug.LogWarning("Command Function " + functionFullName + " called on " + base.name + " while NetworkClient is not ready.\nThis may be ignored if client intentionally set NotReady.", base.gameObject);
			}
		}
		else if (requiresAuthority && !isLocalPlayer && !isOwned)
		{
			Debug.LogWarning("Command Function " + functionFullName + " called on " + base.name + " without authority.", base.gameObject);
		}
		else if (NetworkClient.connection == null)
		{
			Debug.LogError("Command Function " + functionFullName + " called on " + base.name + " with no client running.", base.gameObject);
		}
		else
		{
			CommandMessage message = new CommandMessage
			{
				netId = netId,
				componentIndex = ComponentIndex,
				functionHash = (ushort)functionHashCode,
				payload = writer.ToArraySegment()
			};
			NetworkClient.connection.Send(message, channelId);
		}
	}

	protected void SendRPCInternal(string functionFullName, int functionHashCode, NetworkWriter writer, int channelId, bool includeOwner)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function " + functionFullName + " called on Client.", base.gameObject);
			return;
		}
		if (!isServer)
		{
			Debug.LogWarning("ClientRpc " + functionFullName + " called on un-spawned object: " + base.name, base.gameObject);
			return;
		}
		RpcMessage rpcMessage = new RpcMessage
		{
			netId = netId,
			componentIndex = ComponentIndex,
			functionHash = (ushort)functionHashCode,
			payload = writer.ToArraySegment()
		};
		if (netIdentity.observers == null || netIdentity.observers.Count == 0)
		{
			return;
		}
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.Write(rpcMessage);
		foreach (NetworkConnectionToClient value in netIdentity.observers.Values)
		{
			if ((value != netIdentity.connectionToClient || includeOwner) && value.isReady)
			{
				value.BufferRpc(rpcMessage, channelId);
			}
		}
	}

	protected void SendTargetRPCInternal(NetworkConnection conn, string functionFullName, int functionHashCode, NetworkWriter writer, int channelId)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("TargetRPC " + functionFullName + " was called on " + base.name + " when server not active.", base.gameObject);
			return;
		}
		if (!isServer)
		{
			Debug.LogWarning("TargetRpc " + functionFullName + " called on " + base.name + " but that object has not been spawned or has been unspawned.", base.gameObject);
			return;
		}
		if (conn == null)
		{
			conn = connectionToClient;
		}
		if (conn == null)
		{
			Debug.LogError("TargetRPC " + functionFullName + " can't be sent because it was given a null connection. Make sure " + base.name + " is owned by a connection, or if you pass a connection manually then make sure it's not null. For example, TargetRpcs can be called on Player/Pet which are owned by a connection. However, they can not be called on Monsters/Npcs which don't have an owner connection.", base.gameObject);
		}
		else if (!(conn is NetworkConnectionToClient networkConnectionToClient))
		{
			Debug.LogError("TargetRPC " + functionFullName + " called on " + base.name + " requires a NetworkConnectionToClient but was given " + conn.GetType().Name, base.gameObject);
		}
		else
		{
			RpcMessage message = new RpcMessage
			{
				netId = netId,
				componentIndex = ComponentIndex,
				functionHash = (ushort)functionHashCode,
				payload = writer.ToArraySegment()
			};
			networkConnectionToClient.BufferRpc(message, channelId);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GeneratedSyncVarSetter<T>(T value, ref T field, ulong dirtyBit, Action<T, T> OnChanged)
	{
		if (!SyncVarEqual(value, ref field))
		{
			T arg = field;
			SetSyncVar(value, ref field, dirtyBit);
			if (OnChanged != null && NetworkServer.activeHost && !GetSyncVarHookGuard(dirtyBit))
			{
				SetSyncVarHookGuard(dirtyBit, value: true);
				OnChanged(arg, value);
				SetSyncVarHookGuard(dirtyBit, value: false);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GeneratedSyncVarSetter_GameObject(GameObject value, ref GameObject field, ulong dirtyBit, Action<GameObject, GameObject> OnChanged, ref uint netIdField)
	{
		if (!SyncVarGameObjectEqual(value, netIdField))
		{
			GameObject arg = field;
			SetSyncVarGameObject(value, ref field, dirtyBit, ref netIdField);
			if (OnChanged != null && NetworkServer.activeHost && !GetSyncVarHookGuard(dirtyBit))
			{
				SetSyncVarHookGuard(dirtyBit, value: true);
				OnChanged(arg, value);
				SetSyncVarHookGuard(dirtyBit, value: false);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GeneratedSyncVarSetter_NetworkIdentity(NetworkIdentity value, ref NetworkIdentity field, ulong dirtyBit, Action<NetworkIdentity, NetworkIdentity> OnChanged, ref uint netIdField)
	{
		if (!SyncVarNetworkIdentityEqual(value, netIdField))
		{
			NetworkIdentity arg = field;
			SetSyncVarNetworkIdentity(value, ref field, dirtyBit, ref netIdField);
			if (OnChanged != null && NetworkServer.activeHost && !GetSyncVarHookGuard(dirtyBit))
			{
				SetSyncVarHookGuard(dirtyBit, value: true);
				OnChanged(arg, value);
				SetSyncVarHookGuard(dirtyBit, value: false);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GeneratedSyncVarSetter_NetworkBehaviour<T>(T value, ref T field, ulong dirtyBit, Action<T, T> OnChanged, ref NetworkBehaviourSyncVar netIdField) where T : NetworkBehaviour
	{
		if (!SyncVarNetworkBehaviourEqual(value, netIdField))
		{
			T arg = field;
			SetSyncVarNetworkBehaviour(value, ref field, dirtyBit, ref netIdField);
			if (OnChanged != null && NetworkServer.activeHost && !GetSyncVarHookGuard(dirtyBit))
			{
				SetSyncVarHookGuard(dirtyBit, value: true);
				OnChanged(arg, value);
				SetSyncVarHookGuard(dirtyBit, value: false);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool SyncVarGameObjectEqual(GameObject newGameObject, uint netIdField)
	{
		uint num = 0u;
		if (newGameObject != null && newGameObject.TryGetComponent<NetworkIdentity>(out var component))
		{
			num = component.netId;
			if (num == 0)
			{
				Debug.LogWarning($"SetSyncVarGameObject GameObject {newGameObject} has a zero netId. Maybe it is not spawned yet?");
			}
		}
		return num == netIdField;
	}

	protected void SetSyncVarGameObject(GameObject newGameObject, ref GameObject gameObjectField, ulong dirtyBit, ref uint netIdField)
	{
		if (GetSyncVarHookGuard(dirtyBit))
		{
			return;
		}
		uint num = 0u;
		if (newGameObject != null && newGameObject.TryGetComponent<NetworkIdentity>(out var component))
		{
			num = component.netId;
			if (num == 0)
			{
				Debug.LogWarning($"SetSyncVarGameObject GameObject {newGameObject} has a zero netId. Maybe it is not spawned yet?");
			}
		}
		SetSyncVarDirtyBit(dirtyBit);
		gameObjectField = newGameObject;
		netIdField = num;
	}

	protected GameObject GetSyncVarGameObject(uint netId, ref GameObject gameObjectField)
	{
		if (isServer)
		{
			return gameObjectField;
		}
		if (NetworkClient.spawned.TryGetValue(netId, out var value) && value != null)
		{
			return gameObjectField = value.gameObject;
		}
		return null;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool SyncVarNetworkIdentityEqual(NetworkIdentity newIdentity, uint netIdField)
	{
		uint num = 0u;
		if (newIdentity != null)
		{
			num = newIdentity.netId;
			if (num == 0)
			{
				Debug.LogWarning($"SetSyncVarNetworkIdentity NetworkIdentity {newIdentity} has a zero netId. Maybe it is not spawned yet?");
			}
		}
		return num == netIdField;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GeneratedSyncVarDeserialize<T>(ref T field, Action<T, T> OnChanged, T value)
	{
		T val = field;
		field = value;
		if (OnChanged != null && !SyncVarEqual(val, ref field))
		{
			OnChanged(val, field);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GeneratedSyncVarDeserialize_GameObject(ref GameObject field, Action<GameObject, GameObject> OnChanged, NetworkReader reader, ref uint netIdField)
	{
		uint value = netIdField;
		GameObject arg = field;
		netIdField = reader.ReadUInt();
		field = GetSyncVarGameObject(netIdField, ref field);
		if (OnChanged != null && !SyncVarEqual(value, ref netIdField))
		{
			OnChanged(arg, field);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GeneratedSyncVarDeserialize_NetworkIdentity(ref NetworkIdentity field, Action<NetworkIdentity, NetworkIdentity> OnChanged, NetworkReader reader, ref uint netIdField)
	{
		uint value = netIdField;
		NetworkIdentity arg = field;
		netIdField = reader.ReadUInt();
		field = GetSyncVarNetworkIdentity(netIdField, ref field);
		if (OnChanged != null && !SyncVarEqual(value, ref netIdField))
		{
			OnChanged(arg, field);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GeneratedSyncVarDeserialize_NetworkBehaviour<T>(ref T field, Action<T, T> OnChanged, NetworkReader reader, ref NetworkBehaviourSyncVar netIdField) where T : NetworkBehaviour
	{
		NetworkBehaviourSyncVar value = netIdField;
		T arg = field;
		netIdField = reader.ReadNetworkBehaviourSyncVar();
		field = GetSyncVarNetworkBehaviour(netIdField, ref field);
		if (OnChanged != null && !SyncVarEqual(value, ref netIdField))
		{
			OnChanged(arg, field);
		}
	}

	protected void SetSyncVarNetworkIdentity(NetworkIdentity newIdentity, ref NetworkIdentity identityField, ulong dirtyBit, ref uint netIdField)
	{
		if (GetSyncVarHookGuard(dirtyBit))
		{
			return;
		}
		uint num = 0u;
		if (newIdentity != null)
		{
			num = newIdentity.netId;
			if (num == 0)
			{
				Debug.LogWarning($"SetSyncVarNetworkIdentity NetworkIdentity {newIdentity} has a zero netId. Maybe it is not spawned yet?");
			}
		}
		SetSyncVarDirtyBit(dirtyBit);
		netIdField = num;
		identityField = newIdentity;
	}

	protected NetworkIdentity GetSyncVarNetworkIdentity(uint netId, ref NetworkIdentity identityField)
	{
		if (isServer)
		{
			return identityField;
		}
		NetworkClient.spawned.TryGetValue(netId, out identityField);
		return identityField;
	}

	protected static bool SyncVarNetworkBehaviourEqual<T>(T newBehaviour, NetworkBehaviourSyncVar syncField) where T : NetworkBehaviour
	{
		uint num = 0u;
		byte componentIndex = 0;
		if (newBehaviour != null)
		{
			num = newBehaviour.netId;
			componentIndex = newBehaviour.ComponentIndex;
			if (num == 0)
			{
				Debug.LogWarning($"SetSyncVarNetworkIdentity NetworkIdentity {newBehaviour} has a zero netId. Maybe it is not spawned yet?");
			}
		}
		return syncField.Equals(num, componentIndex);
	}

	protected void SetSyncVarNetworkBehaviour<T>(T newBehaviour, ref T behaviourField, ulong dirtyBit, ref NetworkBehaviourSyncVar syncField) where T : NetworkBehaviour
	{
		if (GetSyncVarHookGuard(dirtyBit))
		{
			return;
		}
		uint num = 0u;
		byte componentIndex = 0;
		if (newBehaviour != null)
		{
			num = newBehaviour.netId;
			componentIndex = newBehaviour.ComponentIndex;
			if (num == 0)
			{
				Debug.LogWarning(string.Format("{0} NetworkIdentity {1} has a zero netId. Maybe it is not spawned yet?", "SetSyncVarNetworkBehaviour", newBehaviour));
			}
		}
		syncField = new NetworkBehaviourSyncVar(num, componentIndex);
		SetSyncVarDirtyBit(dirtyBit);
		behaviourField = newBehaviour;
	}

	protected T GetSyncVarNetworkBehaviour<T>(NetworkBehaviourSyncVar syncNetBehaviour, ref T behaviourField) where T : NetworkBehaviour
	{
		if (isServer)
		{
			return behaviourField;
		}
		if (!NetworkClient.spawned.TryGetValue(syncNetBehaviour.netId, out var value))
		{
			return null;
		}
		behaviourField = value.NetworkBehaviours[syncNetBehaviour.componentIndex] as T;
		return behaviourField;
	}

	protected static bool SyncVarEqual<T>(T value, ref T fieldValue)
	{
		return EqualityComparer<T>.Default.Equals(value, fieldValue);
	}

	protected void SetSyncVar<T>(T value, ref T fieldValue, ulong dirtyBit)
	{
		SetSyncVarDirtyBit(dirtyBit);
		fieldValue = value;
	}

	public virtual void OnSerialize(NetworkWriter writer, bool initialState)
	{
		SerializeSyncObjects(writer, initialState);
		SerializeSyncVars(writer, initialState);
	}

	public virtual void OnDeserialize(NetworkReader reader, bool initialState)
	{
		DeserializeSyncObjects(reader, initialState);
		DeserializeSyncVars(reader, initialState);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SerializeSyncObjects(NetworkWriter writer, bool initialState)
	{
		if (initialState)
		{
			SerializeObjectsAll(writer);
		}
		else
		{
			SerializeObjectsDelta(writer);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void DeserializeSyncObjects(NetworkReader reader, bool initialState)
	{
		if (initialState)
		{
			DeserializeObjectsAll(reader);
		}
		else
		{
			DeserializeObjectsDelta(reader);
		}
	}

	protected virtual void SerializeSyncVars(NetworkWriter writer, bool initialState)
	{
	}

	protected virtual void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
	}

	public void SerializeObjectsAll(NetworkWriter writer)
	{
		for (int i = 0; i < syncObjects.Count; i++)
		{
			syncObjects[i].OnSerializeAll(writer);
		}
	}

	public void SerializeObjectsDelta(NetworkWriter writer)
	{
		writer.WriteULong(syncObjectDirtyBits);
		for (int i = 0; i < syncObjects.Count; i++)
		{
			SyncObject syncObject = syncObjects[i];
			if ((syncObjectDirtyBits & (ulong)(1L << i)) != 0L)
			{
				syncObject.OnSerializeDelta(writer);
			}
		}
	}

	internal void DeserializeObjectsAll(NetworkReader reader)
	{
		for (int i = 0; i < syncObjects.Count; i++)
		{
			syncObjects[i].OnDeserializeAll(reader);
		}
	}

	internal void DeserializeObjectsDelta(NetworkReader reader)
	{
		ulong num = reader.ReadULong();
		for (int i = 0; i < syncObjects.Count; i++)
		{
			SyncObject syncObject = syncObjects[i];
			if ((num & (ulong)(1L << i)) != 0L)
			{
				syncObject.OnDeserializeDelta(reader);
			}
		}
	}

	internal void Serialize(NetworkWriter writer, bool initialState)
	{
		int position = writer.Position;
		writer.WriteByte(0);
		int position2 = writer.Position;
		try
		{
			OnSerialize(writer, initialState);
		}
		catch (Exception ex)
		{
			Debug.LogError($"OnSerialize failed for: object={base.name} component={GetType()} sceneId={netIdentity.sceneId:X}\n\n{ex}");
		}
		int position3 = writer.Position;
		writer.Position = position;
		byte value = (byte)((position3 - position2) & 0xFF);
		writer.WriteByte(value);
		writer.Position = position3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int ErrorCorrection(int size, byte safety)
	{
		return (size & -256) | safety;
	}

	internal bool Deserialize(NetworkReader reader, bool initialState)
	{
		bool result = true;
		byte b = reader.ReadByte();
		int position = reader.Position;
		try
		{
			OnDeserialize(reader, initialState);
		}
		catch (Exception ex)
		{
			Debug.LogError($"OnDeserialize failed Exception={ex.GetType()} (see below) object={base.name} component={GetType()} netId={netId}. Possible Reasons:\n" + $"  * Do {GetType()}'s OnSerialize and OnDeserialize calls write the same amount of data? \n" + $"  * Was there an exception in {GetType()}'s OnSerialize/OnDeserialize code?\n" + "  * Are the server and client the exact same project?\n  * Maybe this OnDeserialize call was meant for another GameObject? The sceneIds can easily get out of sync if the Hierarchy was modified only in the client OR the server. Try rebuilding both.\n\n" + $"Exception {ex}");
			result = false;
		}
		int num = reader.Position - position;
		byte b2 = (byte)(num & 0xFF);
		if (b2 != b)
		{
			Debug.LogWarning($"{base.name} (netId={netId}): {GetType()} OnDeserialize size mismatch. It read {num} bytes, which caused a size hash mismatch of {b2:X2} vs. {b:X2}. Make sure that OnSerialize and OnDeserialize write/read the same amount of data in all cases.");
			int num2 = ErrorCorrection(num, b);
			reader.Position = position + num2;
			result = false;
		}
		return result;
	}

	internal void ResetSyncObjects()
	{
		foreach (SyncObject syncObject in syncObjects)
		{
			syncObject.Reset();
		}
	}

	public virtual void OnStartServer()
	{
	}

	public virtual void OnStopServer()
	{
	}

	public virtual void OnStartClient()
	{
	}

	public virtual void OnStopClient()
	{
	}

	public virtual void OnStartLocalPlayer()
	{
	}

	public virtual void OnStopLocalPlayer()
	{
	}

	public virtual void OnStartAuthority()
	{
	}

	public virtual void OnStopAuthority()
	{
	}
}
