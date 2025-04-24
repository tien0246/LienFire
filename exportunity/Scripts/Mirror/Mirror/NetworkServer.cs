using System;
using System.Collections.Generic;
using System.Linq;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror;

public static class NetworkServer
{
	private enum DestroyMode
	{
		Destroy = 0,
		Reset = 1
	}

	private static bool initialized;

	public static int maxConnections;

	public static int tickRate = 30;

	private static double lastSendTime;

	public static Dictionary<int, NetworkConnectionToClient> connections = new Dictionary<int, NetworkConnectionToClient>();

	internal static Dictionary<ushort, NetworkMessageDelegate> handlers = new Dictionary<ushort, NetworkMessageDelegate>();

	public static readonly Dictionary<uint, NetworkIdentity> spawned = new Dictionary<uint, NetworkIdentity>();

	public static bool dontListen;

	public static bool isLoadingScene;

	public static InterestManagementBase aoi;

	public static Action<NetworkConnectionToClient> OnConnectedEvent;

	public static Action<NetworkConnectionToClient> OnDisconnectedEvent;

	public static Action<NetworkConnectionToClient, TransportError, string> OnErrorEvent;

	public static int actualTickRate;

	private static double actualTickRateStart;

	private static int actualTickRateCounter;

	public static TimeSample earlyUpdateDuration;

	public static TimeSample lateUpdateDuration;

	public static TimeSample fullUpdateDuration;

	internal static readonly List<NetworkConnectionToClient> connectionsCopy = new List<NetworkConnectionToClient>();

	public static float tickInterval
	{
		get
		{
			if (tickRate >= int.MaxValue)
			{
				return 0f;
			}
			return 1f / (float)tickRate;
		}
	}

	public static int sendRate => tickRate;

	public static float sendInterval
	{
		get
		{
			if (sendRate >= int.MaxValue)
			{
				return 0f;
			}
			return 1f / (float)sendRate;
		}
	}

	public static LocalConnectionToClient localConnection { get; private set; }

	[Obsolete("NetworkServer.localClientActive was renamed to .activeHost to be more obvious")]
	public static bool localClientActive => activeHost;

	public static bool active { get; internal set; }

	public static bool activeHost => localConnection != null;

	public static void Listen(int maxConns)
	{
		Initialize();
		maxConnections = maxConns;
		if (!dontListen)
		{
			Transport.active.ServerStart();
		}
		active = true;
		RegisterMessageHandlers();
	}

	private static void Initialize()
	{
		if (!initialized)
		{
			connections.Clear();
			if (aoi != null)
			{
				aoi.Reset();
			}
			NetworkTime.ResetStatics();
			AddTransportHandlers();
			initialized = true;
			earlyUpdateDuration = new TimeSample(sendRate);
			lateUpdateDuration = new TimeSample(sendRate);
			fullUpdateDuration = new TimeSample(sendRate);
		}
	}

	private static void AddTransportHandlers()
	{
		Transport transport = Transport.active;
		transport.OnServerConnected = (Action<int>)Delegate.Combine(transport.OnServerConnected, new Action<int>(OnTransportConnected));
		Transport transport2 = Transport.active;
		transport2.OnServerDataReceived = (Action<int, ArraySegment<byte>, int>)Delegate.Combine(transport2.OnServerDataReceived, new Action<int, ArraySegment<byte>, int>(OnTransportData));
		Transport transport3 = Transport.active;
		transport3.OnServerDisconnected = (Action<int>)Delegate.Combine(transport3.OnServerDisconnected, new Action<int>(OnTransportDisconnected));
		Transport transport4 = Transport.active;
		transport4.OnServerError = (Action<int, TransportError, string>)Delegate.Combine(transport4.OnServerError, new Action<int, TransportError, string>(OnTransportError));
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Shutdown()
	{
		if (initialized)
		{
			DisconnectAll();
			Transport.active.ServerStop();
			RemoveTransportHandlers();
			initialized = false;
		}
		dontListen = false;
		isLoadingScene = false;
		lastSendTime = 0.0;
		actualTickRate = 0;
		localConnection = null;
		connections.Clear();
		connectionsCopy.Clear();
		handlers.Clear();
		CleanupSpawned();
		active = false;
		NetworkIdentity.ResetStatics();
		OnConnectedEvent = null;
		OnDisconnectedEvent = null;
		OnErrorEvent = null;
		if (aoi != null)
		{
			aoi.Reset();
		}
	}

	private static void RemoveTransportHandlers()
	{
		Transport transport = Transport.active;
		transport.OnServerConnected = (Action<int>)Delegate.Remove(transport.OnServerConnected, new Action<int>(OnTransportConnected));
		Transport transport2 = Transport.active;
		transport2.OnServerDataReceived = (Action<int, ArraySegment<byte>, int>)Delegate.Remove(transport2.OnServerDataReceived, new Action<int, ArraySegment<byte>, int>(OnTransportData));
		Transport transport3 = Transport.active;
		transport3.OnServerDisconnected = (Action<int>)Delegate.Remove(transport3.OnServerDisconnected, new Action<int>(OnTransportDisconnected));
		Transport transport4 = Transport.active;
		transport4.OnServerError = (Action<int, TransportError, string>)Delegate.Remove(transport4.OnServerError, new Action<int, TransportError, string>(OnTransportError));
	}

	private static void CleanupSpawned()
	{
		foreach (NetworkIdentity item in spawned.Values.ToList())
		{
			if (item != null)
			{
				if (item.sceneId != 0L)
				{
					DestroyObject(item, DestroyMode.Reset);
					item.gameObject.SetActive(value: false);
				}
				else
				{
					DestroyObject(item, DestroyMode.Destroy);
				}
			}
		}
		spawned.Clear();
	}

	internal static void RegisterMessageHandlers()
	{
		RegisterHandler<ReadyMessage>(OnClientReadyMessage);
		RegisterHandler<CommandMessage>(OnCommandMessage);
		RegisterHandler<NetworkPingMessage>(NetworkTime.OnServerPing, requireAuthentication: false);
		RegisterHandler<EntityStateMessage>(OnEntityStateMessage);
		RegisterHandler<TimeSnapshotMessage>(OnTimeSnapshotMessage);
	}

	private static void OnClientReadyMessage(NetworkConnectionToClient conn, ReadyMessage msg)
	{
		SetClientReady(conn);
	}

	private static void OnCommandMessage(NetworkConnectionToClient conn, CommandMessage msg, int channelId)
	{
		if (!conn.isReady)
		{
			if (channelId == 0)
			{
				Debug.LogWarning("Command received while client is not ready.\nThis may be ignored if client intentionally set NotReady.");
			}
			return;
		}
		if (!spawned.TryGetValue(msg.netId, out var value))
		{
			if (channelId == 0)
			{
				Debug.LogWarning($"Spawned object not found when handling Command message [netId={msg.netId}]");
			}
			return;
		}
		if (RemoteProcedureCalls.CommandRequiresAuthority(msg.functionHash) && value.connectionToClient != conn)
		{
			Debug.LogWarning($"Command for object without authority [netId={msg.netId}]");
			return;
		}
		using NetworkReaderPooled reader = NetworkReaderPool.Get(msg.payload);
		value.HandleRemoteCall(msg.componentIndex, msg.functionHash, RemoteCallType.Command, reader, conn);
	}

	private static void OnEntityStateMessage(NetworkConnectionToClient connection, EntityStateMessage message)
	{
		if (!spawned.TryGetValue(message.netId, out var value) || !(value != null))
		{
			return;
		}
		if (value.connectionToClient == connection)
		{
			using (NetworkReaderPooled reader = NetworkReaderPool.Get(message.payload))
			{
				if (!value.DeserializeServer(reader))
				{
					Debug.LogWarning($"Server failed to deserialize client state for {value.name} with netId={value.netId}, Disconnecting.");
					connection.Disconnect();
				}
				return;
			}
		}
		Debug.LogWarning($"EntityStateMessage from {connection} for {value} without authority.");
	}

	private static void OnTimeSnapshotMessage(NetworkConnectionToClient connection, TimeSnapshotMessage _)
	{
		connection.OnTimeSnapshot(new TimeSnapshot(connection.remoteTimeStamp, NetworkTime.localTime));
	}

	public static bool AddConnection(NetworkConnectionToClient conn)
	{
		if (!connections.ContainsKey(conn.connectionId))
		{
			connections[conn.connectionId] = conn;
			return true;
		}
		return false;
	}

	public static bool RemoveConnection(int connectionId)
	{
		return connections.Remove(connectionId);
	}

	internal static void SetLocalConnection(LocalConnectionToClient conn)
	{
		if (localConnection != null)
		{
			Debug.LogError("Local Connection already exists");
		}
		else
		{
			localConnection = conn;
		}
	}

	internal static void RemoveLocalConnection()
	{
		if (localConnection != null)
		{
			localConnection.Disconnect();
			localConnection = null;
		}
		RemoveConnection(0);
	}

	public static bool HasExternalConnections()
	{
		if (connections.Count > 0)
		{
			if (connections.Count == 1 && localConnection != null)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public static void SendToAll<T>(T message, int channelId = 0, bool sendToReadyOnly = false) where T : struct, NetworkMessage
	{
		if (!active)
		{
			Debug.LogWarning("Can not send using NetworkServer.SendToAll<T>(T msg) because NetworkServer is not active");
			return;
		}
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		NetworkMessages.Pack(message, networkWriterPooled);
		ArraySegment<byte> segment = networkWriterPooled.ToArraySegment();
		int num = 0;
		foreach (NetworkConnectionToClient value in connections.Values)
		{
			if (!sendToReadyOnly || value.isReady)
			{
				num++;
				value.Send(segment, channelId);
			}
		}
		NetworkDiagnostics.OnSend(message, channelId, segment.Count, num);
	}

	public static void SendToReady<T>(T message, int channelId = 0) where T : struct, NetworkMessage
	{
		if (!active)
		{
			Debug.LogWarning("Can not send using NetworkServer.SendToReady<T>(T msg) because NetworkServer is not active");
		}
		else
		{
			SendToAll(message, channelId, sendToReadyOnly: true);
		}
	}

	private static void SendToObservers<T>(NetworkIdentity identity, T message, int channelId = 0) where T : struct, NetworkMessage
	{
		if (identity == null || identity.observers.Count == 0)
		{
			return;
		}
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		NetworkMessages.Pack(message, networkWriterPooled);
		ArraySegment<byte> segment = networkWriterPooled.ToArraySegment();
		foreach (NetworkConnectionToClient value in identity.observers.Values)
		{
			value.Send(segment, channelId);
		}
		NetworkDiagnostics.OnSend(message, channelId, segment.Count, identity.observers.Count);
	}

	public static void SendToReadyObservers<T>(NetworkIdentity identity, T message, bool includeOwner = true, int channelId = 0) where T : struct, NetworkMessage
	{
		if (identity == null || identity.observers.Count == 0)
		{
			return;
		}
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		NetworkMessages.Pack(message, networkWriterPooled);
		ArraySegment<byte> segment = networkWriterPooled.ToArraySegment();
		int num = 0;
		foreach (NetworkConnectionToClient value in identity.observers.Values)
		{
			if ((value != identity.connectionToClient || includeOwner) && value.isReady)
			{
				num++;
				value.Send(segment, channelId);
			}
		}
		NetworkDiagnostics.OnSend(message, channelId, segment.Count, num);
	}

	public static void SendToReadyObservers<T>(NetworkIdentity identity, T message, int channelId) where T : struct, NetworkMessage
	{
		SendToReadyObservers(identity, message, includeOwner: true, channelId);
	}

	private static void OnTransportConnected(int connectionId)
	{
		if (connectionId == 0)
		{
			Debug.LogError($"Server.HandleConnect: invalid connectionId: {connectionId} . Needs to be != 0, because 0 is reserved for local player.");
			Transport.active.ServerDisconnect(connectionId);
		}
		else if (connections.ContainsKey(connectionId))
		{
			Transport.active.ServerDisconnect(connectionId);
		}
		else if (connections.Count < maxConnections)
		{
			OnConnected(new NetworkConnectionToClient(connectionId));
		}
		else
		{
			Transport.active.ServerDisconnect(connectionId);
		}
	}

	internal static void OnConnected(NetworkConnectionToClient conn)
	{
		AddConnection(conn);
		OnConnectedEvent?.Invoke(conn);
	}

	private static bool UnpackAndInvoke(NetworkConnectionToClient connection, NetworkReader reader, int channelId)
	{
		if (NetworkMessages.UnpackId(reader, out var messageId))
		{
			if (handlers.TryGetValue(messageId, out var value))
			{
				value(connection, reader, channelId);
				connection.lastMessageTime = Time.time;
				return true;
			}
			Debug.LogWarning($"Unknown message id: {messageId} for connection: {connection}. This can happen if no handler was registered for this message.");
			return false;
		}
		Debug.LogWarning($"Invalid message header for connection: {connection}.");
		return false;
	}

	internal static void OnTransportData(int connectionId, ArraySegment<byte> data, int channelId)
	{
		if (connections.TryGetValue(connectionId, out var value))
		{
			if (!value.unbatcher.AddBatch(data))
			{
				Debug.LogWarning("NetworkServer: received Message was too short (messages should start with message id)");
				value.Disconnect();
				return;
			}
			NetworkReader message;
			double remoteTimeStamp;
			while (!isLoadingScene && value.unbatcher.GetNextMessage(out message, out remoteTimeStamp))
			{
				if (message.Remaining >= 2)
				{
					value.remoteTimeStamp = remoteTimeStamp;
					if (!UnpackAndInvoke(value, message, channelId))
					{
						Debug.LogWarning($"NetworkServer: failed to unpack and invoke message. Disconnecting {connectionId}.");
						value.Disconnect();
						return;
					}
					continue;
				}
				Debug.LogWarning($"NetworkServer: received Message was too short (messages should start with message id). Disconnecting {connectionId}");
				value.Disconnect();
				return;
			}
			if (!isLoadingScene && value.unbatcher.BatchesCount > 0)
			{
				Debug.LogError($"Still had {value.unbatcher.BatchesCount} batches remaining after processing, even though processing was not interrupted by a scene change. This should never happen, as it would cause ever growing batches.\nPossible reasons:\n* A message didn't deserialize as much as it serialized\n*There was no message handler for a message id, so the reader wasn't read until the end.");
			}
		}
		else
		{
			Debug.LogError($"HandleData Unknown connectionId:{connectionId}");
		}
	}

	internal static void OnTransportDisconnected(int connectionId)
	{
		if (connections.TryGetValue(connectionId, out var value))
		{
			RemoveConnection(connectionId);
			if (OnDisconnectedEvent != null)
			{
				OnDisconnectedEvent(value);
			}
			else
			{
				DestroyPlayerForConnection(value);
			}
		}
	}

	private static void OnTransportError(int connectionId, TransportError error, string reason)
	{
		Debug.LogWarning($"Server Transport Error for connId={connectionId}: {error}: {reason}. This is fine.");
		connections.TryGetValue(connectionId, out var value);
		OnErrorEvent?.Invoke(value, error, reason);
	}

	public static void DestroyPlayerForConnection(NetworkConnectionToClient conn)
	{
		conn.DestroyOwnedObjects();
		conn.RemoveFromObservingsObservers();
		conn.identity = null;
	}

	public static void RegisterHandler<T>(Action<NetworkConnectionToClient, T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage
	{
		ushort id = NetworkMessageId<T>.Id;
		if (handlers.ContainsKey(id))
		{
			Debug.LogWarning($"NetworkServer.RegisterHandler replacing handler for {typeof(T).FullName}, id={id}. If replacement is intentional, use ReplaceHandler instead to avoid this warning.");
		}
		handlers[id] = NetworkMessages.WrapHandler(handler, requireAuthentication);
	}

	public static void RegisterHandler<T>(Action<NetworkConnectionToClient, T, int> handler, bool requireAuthentication = true) where T : struct, NetworkMessage
	{
		ushort id = NetworkMessageId<T>.Id;
		if (handlers.ContainsKey(id))
		{
			Debug.LogWarning($"NetworkServer.RegisterHandler replacing handler for {typeof(T).FullName}, id={id}. If replacement is intentional, use ReplaceHandler instead to avoid this warning.");
		}
		handlers[id] = NetworkMessages.WrapHandler(handler, requireAuthentication);
	}

	public static void ReplaceHandler<T>(Action<T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage
	{
		ReplaceHandler(delegate(NetworkConnectionToClient _, T value)
		{
			handler(value);
		}, requireAuthentication);
	}

	public static void ReplaceHandler<T>(Action<NetworkConnectionToClient, T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage
	{
		ushort id = NetworkMessageId<T>.Id;
		handlers[id] = NetworkMessages.WrapHandler(handler, requireAuthentication);
	}

	public static void UnregisterHandler<T>() where T : struct, NetworkMessage
	{
		ushort id = NetworkMessageId<T>.Id;
		handlers.Remove(id);
	}

	public static void ClearHandlers()
	{
		handlers.Clear();
	}

	internal static bool GetNetworkIdentity(GameObject go, out NetworkIdentity identity)
	{
		if (!go.TryGetComponent<NetworkIdentity>(out identity))
		{
			Debug.LogError("GameObject " + go.name + " doesn't have NetworkIdentity.");
			return false;
		}
		return true;
	}

	public static void DisconnectAll()
	{
		foreach (NetworkConnectionToClient item in connections.Values.ToList())
		{
			item.Disconnect();
			if (item.connectionId != 0)
			{
				OnTransportDisconnected(item.connectionId);
			}
		}
		connections.Clear();
		localConnection = null;
	}

	public static bool AddPlayerForConnection(NetworkConnectionToClient conn, GameObject player, uint assetId)
	{
		if (GetNetworkIdentity(player, out var identity))
		{
			identity.assetId = assetId;
		}
		return AddPlayerForConnection(conn, player);
	}

	public static bool AddPlayerForConnection(NetworkConnectionToClient conn, GameObject player)
	{
		if (!player.TryGetComponent<NetworkIdentity>(out var component))
		{
			Debug.LogWarning($"AddPlayer: playerGameObject has no NetworkIdentity. Please add a NetworkIdentity to {player}");
			return false;
		}
		if (conn.identity != null)
		{
			Debug.Log("AddPlayer: player object already exists");
			return false;
		}
		conn.identity = component;
		component.SetClientOwner(conn);
		if (conn is LocalConnectionToClient)
		{
			component.isOwned = true;
			NetworkClient.InternalAddPlayer(component);
		}
		SetClientReady(conn);
		Respawn(component);
		return true;
	}

	public static bool ReplacePlayerForConnection(NetworkConnectionToClient conn, GameObject player, bool keepAuthority = false)
	{
		if (!player.TryGetComponent<NetworkIdentity>(out var component))
		{
			Debug.LogError($"ReplacePlayer: playerGameObject has no NetworkIdentity. Please add a NetworkIdentity to {player}");
			return false;
		}
		if (component.connectionToClient != null && component.connectionToClient != conn)
		{
			Debug.LogError($"Cannot replace player for connection. New player is already owned by a different connection{player}");
			return false;
		}
		NetworkIdentity identity = conn.identity;
		conn.identity = component;
		component.SetClientOwner(conn);
		if (conn is LocalConnectionToClient)
		{
			component.isOwned = true;
			NetworkClient.InternalAddPlayer(component);
		}
		SpawnObserversForConnection(conn);
		Respawn(component);
		if (keepAuthority)
		{
			SendChangeOwnerMessage(identity, conn);
		}
		else
		{
			identity.RemoveClientAuthority();
		}
		return true;
	}

	public static bool ReplacePlayerForConnection(NetworkConnectionToClient conn, GameObject player, uint assetId, bool keepAuthority = false)
	{
		if (GetNetworkIdentity(player, out var identity))
		{
			identity.assetId = assetId;
		}
		return ReplacePlayerForConnection(conn, player, keepAuthority);
	}

	public static void RemovePlayerForConnection(NetworkConnection conn, bool destroyServerObject)
	{
		if (conn.identity != null)
		{
			if (destroyServerObject)
			{
				Destroy(conn.identity.gameObject);
			}
			else
			{
				UnSpawn(conn.identity.gameObject);
			}
			conn.identity = null;
		}
	}

	public static void SetClientReady(NetworkConnectionToClient conn)
	{
		conn.isReady = true;
		if (conn.identity != null)
		{
			SpawnObserversForConnection(conn);
		}
	}

	private static void SpawnObserversForConnection(NetworkConnectionToClient conn)
	{
		if (!conn.isReady)
		{
			return;
		}
		conn.Send(default(ObjectSpawnStartedMessage));
		foreach (NetworkIdentity value in spawned.Values)
		{
			if (!value.gameObject.activeSelf)
			{
				continue;
			}
			if (value.visible == Visibility.ForceShown)
			{
				value.AddObserver(conn);
			}
			else
			{
				if (value.visible == Visibility.ForceHidden || value.visible != Visibility.Default)
				{
					continue;
				}
				if (aoi != null)
				{
					if (aoi.OnCheckObserver(value, conn))
					{
						value.AddObserver(conn);
					}
				}
				else
				{
					value.AddObserver(conn);
				}
			}
		}
		conn.Send(default(ObjectSpawnFinishedMessage));
	}

	public static void SetClientNotReady(NetworkConnectionToClient conn)
	{
		conn.isReady = false;
		conn.RemoveFromObservingsObservers();
		conn.Send(default(NotReadyMessage));
	}

	public static void SetAllClientsNotReady()
	{
		foreach (NetworkConnectionToClient value in connections.Values)
		{
			SetClientNotReady(value);
		}
	}

	internal static void ShowForConnection(NetworkIdentity identity, NetworkConnection conn)
	{
		if (conn.isReady)
		{
			SendSpawnMessage(identity, conn);
		}
	}

	internal static void HideForConnection(NetworkIdentity identity, NetworkConnection conn)
	{
		ObjectHideMessage message = new ObjectHideMessage
		{
			netId = identity.netId
		};
		conn.Send(message);
	}

	internal static void SendSpawnMessage(NetworkIdentity identity, NetworkConnection conn)
	{
		if (identity.serverOnly)
		{
			return;
		}
		using NetworkWriterPooled ownerWriter = NetworkWriterPool.Get();
		using NetworkWriterPooled observersWriter = NetworkWriterPool.Get();
		bool isOwner = identity.connectionToClient == conn;
		ArraySegment<byte> payload = CreateSpawnMessagePayload(isOwner, identity, ownerWriter, observersWriter);
		SpawnMessage message = new SpawnMessage
		{
			netId = identity.netId,
			isLocalPlayer = (conn.identity == identity),
			isOwner = isOwner,
			sceneId = identity.sceneId,
			assetId = identity.assetId,
			position = identity.transform.localPosition,
			rotation = identity.transform.localRotation,
			scale = identity.transform.localScale,
			payload = payload
		};
		conn.Send(message);
	}

	private static ArraySegment<byte> CreateSpawnMessagePayload(bool isOwner, NetworkIdentity identity, NetworkWriterPooled ownerWriter, NetworkWriterPooled observersWriter)
	{
		if (identity.NetworkBehaviours.Length == 0)
		{
			return default(ArraySegment<byte>);
		}
		identity.SerializeServer(initialState: true, ownerWriter, observersWriter);
		ArraySegment<byte> result = ownerWriter.ToArraySegment();
		ArraySegment<byte> result2 = observersWriter.ToArraySegment();
		if (!isOwner)
		{
			return result2;
		}
		return result;
	}

	internal static void SendChangeOwnerMessage(NetworkIdentity identity, NetworkConnectionToClient conn)
	{
		if (identity.netId != 0 && !identity.serverOnly && conn.observing.Contains(identity))
		{
			conn.Send(new ChangeOwnerMessage
			{
				netId = identity.netId,
				isOwner = (identity.connectionToClient == conn),
				isLocalPlayer = (conn.identity == identity)
			});
		}
	}

	private static bool ValidParent(NetworkIdentity identity)
	{
		if (!(identity.transform.parent == null))
		{
			return identity.transform.parent.gameObject.activeInHierarchy;
		}
		return true;
	}

	public static bool SpawnObjects()
	{
		if (!active)
		{
			return false;
		}
		NetworkIdentity[] array = Resources.FindObjectsOfTypeAll<NetworkIdentity>();
		NetworkIdentity[] array2 = array;
		foreach (NetworkIdentity networkIdentity in array2)
		{
			if (Utils.IsSceneObject(networkIdentity) && networkIdentity.netId == 0)
			{
				networkIdentity.gameObject.SetActive(value: true);
			}
		}
		array2 = array;
		foreach (NetworkIdentity networkIdentity2 in array2)
		{
			if (Utils.IsSceneObject(networkIdentity2) && networkIdentity2.netId == 0 && ValidParent(networkIdentity2))
			{
				Spawn(networkIdentity2.gameObject, networkIdentity2.connectionToClient);
			}
		}
		return true;
	}

	public static void Spawn(GameObject obj, GameObject ownerPlayer)
	{
		if (!ownerPlayer.TryGetComponent<NetworkIdentity>(out var component))
		{
			Debug.LogError("Player object has no NetworkIdentity");
		}
		else if (component.connectionToClient == null)
		{
			Debug.LogError("Player object is not a player.");
		}
		else
		{
			Spawn(obj, component.connectionToClient);
		}
	}

	private static void Respawn(NetworkIdentity identity)
	{
		if (identity.netId == 0)
		{
			Spawn(identity.gameObject, identity.connectionToClient);
		}
		else
		{
			SendSpawnMessage(identity, identity.connectionToClient);
		}
	}

	public static void Spawn(GameObject obj, NetworkConnection ownerConnection = null)
	{
		SpawnObject(obj, ownerConnection);
	}

	public static void Spawn(GameObject obj, uint assetId, NetworkConnection ownerConnection = null)
	{
		if (GetNetworkIdentity(obj, out var identity))
		{
			identity.assetId = assetId;
		}
		SpawnObject(obj, ownerConnection);
	}

	private static void SpawnObject(GameObject obj, NetworkConnection ownerConnection)
	{
		NetworkIdentity component;
		if (Utils.IsPrefab(obj))
		{
			Debug.LogError("GameObject " + obj.name + " is a prefab, it can't be spawned. Instantiate it first.", obj);
		}
		else if (!active)
		{
			Debug.LogError($"SpawnObject for {obj}, NetworkServer is not active. Cannot spawn objects without an active server.", obj);
		}
		else if (!obj.TryGetComponent<NetworkIdentity>(out component))
		{
			Debug.LogError($"SpawnObject {obj} has no NetworkIdentity. Please add a NetworkIdentity to {obj}", obj);
		}
		else
		{
			if (component.SpawnedFromInstantiate)
			{
				return;
			}
			if (spawned.ContainsKey(component.netId))
			{
				Debug.LogWarning($"{component} with netId={component.netId} was already spawned.", component.gameObject);
				return;
			}
			component.connectionToClient = (NetworkConnectionToClient)ownerConnection;
			if (ownerConnection is LocalConnectionToClient)
			{
				component.isOwned = true;
			}
			if (!component.isServer && component.netId == 0)
			{
				component.isLocalPlayer = NetworkClient.localPlayer == component;
				component.isClient = NetworkClient.active;
				component.isServer = true;
				component.netId = NetworkIdentity.GetNextNetworkId();
				spawned[component.netId] = component;
				component.OnStartServer();
			}
			if ((bool)aoi)
			{
				try
				{
					aoi.OnSpawned(component);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			RebuildObservers(component, initialize: true);
		}
	}

	public static void UnSpawn(GameObject obj)
	{
		DestroyObject(obj, DestroyMode.Reset);
	}

	public static void Destroy(GameObject obj)
	{
		DestroyObject(obj, DestroyMode.Destroy);
	}

	private static void DestroyObject(GameObject obj, DestroyMode mode)
	{
		NetworkIdentity identity;
		if (obj == null)
		{
			Debug.Log("NetworkServer DestroyObject is null");
		}
		else if (GetNetworkIdentity(obj, out identity))
		{
			DestroyObject(identity, mode);
		}
	}

	private static void DestroyObject(NetworkIdentity identity, DestroyMode mode)
	{
		if (active && (bool)aoi)
		{
			try
			{
				aoi.OnDestroyed(identity);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		spawned.Remove(identity.netId);
		identity.connectionToClient?.RemoveOwnedObject(identity);
		SendToObservers(identity, new ObjectDestroyMessage
		{
			netId = identity.netId
		});
		identity.ClearObservers();
		if (NetworkClient.active && activeHost)
		{
			if (identity.isLocalPlayer)
			{
				identity.OnStopLocalPlayer();
			}
			identity.OnStopClient();
			identity.isOwned = false;
			identity.NotifyAuthority();
			NetworkClient.connection.owned.Remove(identity);
			NetworkClient.spawned.Remove(identity.netId);
		}
		identity.OnStopServer();
		switch (mode)
		{
		case DestroyMode.Destroy:
			identity.destroyCalled = true;
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(identity.gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(identity.gameObject);
			}
			break;
		case DestroyMode.Reset:
			identity.Reset();
			break;
		}
	}

	private static void RebuildObserversDefault(NetworkIdentity identity, bool initialize)
	{
		if (initialize && identity.visible != Visibility.ForceHidden)
		{
			AddAllReadyServerConnectionsToObservers(identity);
		}
	}

	internal static void AddAllReadyServerConnectionsToObservers(NetworkIdentity identity)
	{
		foreach (NetworkConnectionToClient value in connections.Values)
		{
			if (value.isReady)
			{
				identity.AddObserver(value);
			}
		}
		if (localConnection != null && localConnection.isReady)
		{
			identity.AddObserver(localConnection);
		}
	}

	public static void RebuildObservers(NetworkIdentity identity, bool initialize)
	{
		if (aoi == null || identity.visible == Visibility.ForceShown)
		{
			RebuildObserversDefault(identity, initialize);
		}
		else
		{
			aoi.Rebuild(identity, initialize);
		}
	}

	private static NetworkWriter SerializeForConnection(NetworkIdentity identity, NetworkConnectionToClient connection)
	{
		NetworkIdentitySerialization serverSerializationAtTick = identity.GetServerSerializationAtTick(Time.frameCount);
		if (identity.connectionToClient == connection)
		{
			if (serverSerializationAtTick.ownerWriter.Position > 0)
			{
				return serverSerializationAtTick.ownerWriter;
			}
		}
		else if (serverSerializationAtTick.observersWriter.Position > 0)
		{
			return serverSerializationAtTick.observersWriter;
		}
		return null;
	}

	private static void BroadcastToConnection(NetworkConnectionToClient connection)
	{
		foreach (NetworkIdentity item in connection.observing)
		{
			if (item != null)
			{
				NetworkWriter networkWriter = SerializeForConnection(item, connection);
				if (networkWriter != null)
				{
					EntityStateMessage message = new EntityStateMessage
					{
						netId = item.netId,
						payload = networkWriter.ToArraySegment()
					};
					connection.Send(message);
				}
			}
			else
			{
				Debug.LogWarning($"Found 'null' entry in observing list for connectionId={connection.connectionId}. Please call NetworkServer.Destroy to destroy networked objects. Don't use GameObject.Destroy.");
			}
		}
	}

	private static void Broadcast()
	{
		connectionsCopy.Clear();
		connections.Values.CopyTo(connectionsCopy);
		foreach (NetworkConnectionToClient item in connectionsCopy)
		{
			if (item.isReady)
			{
				item.Send(default(TimeSnapshotMessage), 1);
				BroadcastToConnection(item);
			}
			item.Update();
		}
	}

	internal static void NetworkEarlyUpdate()
	{
		if (active)
		{
			earlyUpdateDuration.Begin();
			fullUpdateDuration.Begin();
		}
		if (Transport.active != null)
		{
			Transport.active.ServerEarlyUpdate();
		}
		foreach (NetworkConnectionToClient value in connections.Values)
		{
			value.UpdateTimeInterpolation();
		}
		if (active)
		{
			earlyUpdateDuration.End();
		}
	}

	internal static void NetworkLateUpdate()
	{
		if (active)
		{
			lateUpdateDuration.Begin();
			if (!Application.isPlaying || AccurateInterval.Elapsed(NetworkTime.localTime, sendInterval, ref lastSendTime))
			{
				Broadcast();
			}
		}
		if (Transport.active != null)
		{
			Transport.active.ServerLateUpdate();
		}
		if (active)
		{
			actualTickRateCounter++;
			if (NetworkTime.localTime >= actualTickRateStart + 1.0)
			{
				float num = (float)(NetworkTime.localTime - actualTickRateStart);
				actualTickRate = Mathf.RoundToInt((float)actualTickRateCounter / num);
				actualTickRateStart = NetworkTime.localTime;
				actualTickRateCounter = 0;
			}
			lateUpdateDuration.End();
			fullUpdateDuration.End();
		}
	}

	[Obsolete("NetworkServer.ActivateHostScene was moved to HostMode.ActivateHostScene")]
	public static void ActivateHostScene()
	{
		HostMode.ActivateHostScene();
	}
}
