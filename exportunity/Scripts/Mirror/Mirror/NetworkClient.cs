using System;
using System.Collections.Generic;
using System.Linq;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror;

public static class NetworkClient
{
	private static double lastSendTime;

	internal static readonly Dictionary<ushort, NetworkMessageDelegate> handlers = new Dictionary<ushort, NetworkMessageDelegate>();

	public static readonly Dictionary<uint, NetworkIdentity> spawned = new Dictionary<uint, NetworkIdentity>();

	public static bool ready;

	internal static ConnectState connectState = ConnectState.None;

	public static Action OnConnectedEvent;

	public static Action OnDisconnectedEvent;

	public static Action<TransportError, string> OnErrorEvent;

	public static readonly Dictionary<uint, GameObject> prefabs = new Dictionary<uint, GameObject>();

	internal static readonly Dictionary<uint, SpawnHandlerDelegate> spawnHandlers = new Dictionary<uint, SpawnHandlerDelegate>();

	internal static readonly Dictionary<uint, UnSpawnDelegate> unspawnHandlers = new Dictionary<uint, UnSpawnDelegate>();

	internal static bool isSpawnFinished;

	internal static readonly Dictionary<ulong, NetworkIdentity> spawnableObjects = new Dictionary<ulong, NetworkIdentity>();

	private static Unbatcher unbatcher = new Unbatcher();

	public static InterestManagementBase aoi;

	public static bool isLoadingScene;

	public static SnapshotInterpolationSettings snapshotSettings = new SnapshotInterpolationSettings();

	public static SortedList<double, TimeSnapshot> snapshots = new SortedList<double, TimeSnapshot>();

	internal static double localTimeline;

	internal static double localTimescale = 1.0;

	private static ExponentialMovingAverage driftEma;

	[Header("Snapshot Interpolation: Dynamic Adjustment")]
	[Tooltip("Automatically adjust bufferTimeMultiplier for smooth results.\nSets a low multiplier on stable connections, and a high multiplier on jittery connections.")]
	public static bool dynamicAdjustment = true;

	[Tooltip("Safety buffer that is always added to the dynamic bufferTimeMultiplier adjustment.")]
	public static float dynamicAdjustmentTolerance = 1f;

	[Tooltip("Dynamic adjustment is computed over n-second exponential moving average standard deviation.")]
	public static int deliveryTimeEmaDuration = 2;

	private static ExponentialMovingAverage deliveryTimeEma;

	public static int sendRate => NetworkServer.sendRate;

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

	public static NetworkConnection connection { get; internal set; }

	public static NetworkIdentity localPlayer { get; internal set; }

	public static bool active
	{
		get
		{
			if (connectState != ConnectState.Connecting)
			{
				return connectState == ConnectState.Connected;
			}
			return true;
		}
	}

	public static bool activeHost => connection is LocalConnectionToServer;

	public static bool isConnecting => connectState == ConnectState.Connecting;

	public static bool isConnected => connectState == ConnectState.Connected;

	[Obsolete("NetworkClient.isHostClient was renamed to .activeHost to be more obvious")]
	public static bool isHostClient => activeHost;

	[Obsolete("NetworkClient snapshot interpolation settings were moved to NetworkClient.snapshotSettings.*")]
	public static double bufferTimeMultiplier => snapshotSettings.bufferTimeMultiplier;

	[Obsolete("NetworkClient snapshot interpolation settings were moved to NetworkClient.snapshotSettings.*")]
	public static float catchupNegativeThreshold => snapshotSettings.catchupNegativeThreshold;

	[Obsolete("NetworkClient snapshot interpolation settings were moved to NetworkClient.snapshotSettings.*")]
	public static float catchupPositiveThreshold => snapshotSettings.catchupPositiveThreshold;

	[Obsolete("NetworkClient snapshot interpolation settings were moved to NetworkClient.snapshotSettings.*")]
	public static double catchupSpeed => snapshotSettings.catchupSpeed;

	[Obsolete("NetworkClient snapshot interpolation settings were moved to NetworkClient.snapshotSettings.*")]
	public static double slowdownSpeed => snapshotSettings.slowdownSpeed;

	[Obsolete("NetworkClient snapshot interpolation settings were moved to NetworkClient.snapshotSettings.*")]
	public static int driftEmaDuration => snapshotSettings.driftEmaDuration;

	public static double bufferTime => (double)NetworkServer.sendInterval * snapshotSettings.bufferTimeMultiplier;

	private static void AddTransportHandlers()
	{
		RemoveTransportHandlers();
		Transport transport = Transport.active;
		transport.OnClientConnected = (Action)Delegate.Combine(transport.OnClientConnected, new Action(OnTransportConnected));
		Transport transport2 = Transport.active;
		transport2.OnClientDataReceived = (Action<ArraySegment<byte>, int>)Delegate.Combine(transport2.OnClientDataReceived, new Action<ArraySegment<byte>, int>(OnTransportData));
		Transport transport3 = Transport.active;
		transport3.OnClientDisconnected = (Action)Delegate.Combine(transport3.OnClientDisconnected, new Action(OnTransportDisconnected));
		Transport transport4 = Transport.active;
		transport4.OnClientError = (Action<TransportError, string>)Delegate.Combine(transport4.OnClientError, new Action<TransportError, string>(OnTransportError));
	}

	private static void RemoveTransportHandlers()
	{
		Transport transport = Transport.active;
		transport.OnClientConnected = (Action)Delegate.Remove(transport.OnClientConnected, new Action(OnTransportConnected));
		Transport transport2 = Transport.active;
		transport2.OnClientDataReceived = (Action<ArraySegment<byte>, int>)Delegate.Remove(transport2.OnClientDataReceived, new Action<ArraySegment<byte>, int>(OnTransportData));
		Transport transport3 = Transport.active;
		transport3.OnClientDisconnected = (Action)Delegate.Remove(transport3.OnClientDisconnected, new Action(OnTransportDisconnected));
		Transport transport4 = Transport.active;
		transport4.OnClientError = (Action<TransportError, string>)Delegate.Remove(transport4.OnClientError, new Action<TransportError, string>(OnTransportError));
	}

	private static void Initialize(bool hostMode)
	{
		InitTimeInterpolation();
		RegisterMessageHandlers(hostMode);
		Transport.active.enabled = true;
	}

	public static void Connect(string address)
	{
		Initialize(hostMode: false);
		AddTransportHandlers();
		connectState = ConnectState.Connecting;
		Transport.active.ClientConnect(address);
		connection = new NetworkConnectionToServer();
	}

	public static void Connect(Uri uri)
	{
		Initialize(hostMode: false);
		AddTransportHandlers();
		connectState = ConnectState.Connecting;
		Transport.active.ClientConnect(uri);
		connection = new NetworkConnectionToServer();
	}

	public static void ConnectHost()
	{
		Initialize(hostMode: true);
		connectState = ConnectState.Connected;
		HostMode.SetupConnections();
	}

	[Obsolete("NetworkClient.ConnectLocalServer was moved to HostMode.InvokeOnConnected")]
	public static void ConnectLocalServer()
	{
		HostMode.InvokeOnConnected();
	}

	public static void Disconnect()
	{
		if (connectState == ConnectState.Connecting || connectState == ConnectState.Connected)
		{
			connectState = ConnectState.Disconnecting;
			ready = false;
			connection?.Disconnect();
		}
	}

	private static void OnTransportConnected()
	{
		if (connection != null)
		{
			NetworkTime.ResetStatics();
			unbatcher = new Unbatcher();
			connectState = ConnectState.Connected;
			NetworkTime.UpdateClient();
			OnConnectedEvent?.Invoke();
		}
		else
		{
			Debug.LogError("Skipped Connect message handling because connection is null.");
		}
	}

	private static bool UnpackAndInvoke(NetworkReader reader, int channelId)
	{
		if (NetworkMessages.UnpackId(reader, out var messageId))
		{
			if (handlers.TryGetValue(messageId, out var value))
			{
				value(connection, reader, channelId);
				if (connection != null)
				{
					connection.lastMessageTime = Time.time;
				}
				return true;
			}
			Debug.LogWarning($"Unknown message id: {messageId}. This can happen if no handler was registered for this message.");
			return false;
		}
		Debug.LogWarning("Invalid message header.");
		return false;
	}

	internal static void OnTransportData(ArraySegment<byte> data, int channelId)
	{
		if (connection != null)
		{
			if (!unbatcher.AddBatch(data))
			{
				Debug.LogWarning("NetworkClient: failed to add batch, disconnecting.");
				connection.Disconnect();
				return;
			}
			NetworkReader message;
			double remoteTimeStamp;
			while (!isLoadingScene && unbatcher.GetNextMessage(out message, out remoteTimeStamp))
			{
				if (message.Remaining >= 2)
				{
					connection.remoteTimeStamp = remoteTimeStamp;
					if (!UnpackAndInvoke(message, channelId))
					{
						Debug.LogWarning("NetworkClient: failed to unpack and invoke message. Disconnecting.");
						connection.Disconnect();
						return;
					}
					continue;
				}
				Debug.LogWarning("NetworkClient: received Message was too short (messages should start with message id)");
				connection.Disconnect();
				return;
			}
			if (!isLoadingScene && unbatcher.BatchesCount > 0)
			{
				Debug.LogError($"Still had {unbatcher.BatchesCount} batches remaining after processing, even though processing was not interrupted by a scene change. This should never happen, as it would cause ever growing batches.\nPossible reasons:\n* A message didn't deserialize as much as it serialized\n*There was no message handler for a message id, so the reader wasn't read until the end.");
			}
		}
		else
		{
			Debug.LogError("Skipped Data message handling because connection is null.");
		}
	}

	internal static void OnTransportDisconnected()
	{
		if (connectState != ConnectState.Disconnected)
		{
			OnDisconnectedEvent?.Invoke();
			connectState = ConnectState.Disconnected;
			ready = false;
			snapshots.Clear();
			localTimeline = 0.0;
			connection = null;
			RemoveTransportHandlers();
		}
	}

	private static void OnTransportError(TransportError error, string reason)
	{
		Debug.LogWarning($"Client Transport Error: {error}: {reason}. This is fine.");
		OnErrorEvent?.Invoke(error, reason);
	}

	public static void Send<T>(T message, int channelId = 0) where T : struct, NetworkMessage
	{
		if (connection != null)
		{
			if (connectState == ConnectState.Connected)
			{
				connection.Send(message, channelId);
			}
			else
			{
				Debug.LogError("NetworkClient Send when not connected to a server");
			}
		}
		else
		{
			Debug.LogError("NetworkClient Send with no connection");
		}
	}

	internal static void RegisterMessageHandlers(bool hostMode)
	{
		if (hostMode)
		{
			RegisterHandler<ObjectDestroyMessage>(OnHostClientObjectDestroy);
			RegisterHandler<ObjectHideMessage>(OnHostClientObjectHide);
			RegisterHandler<NetworkPongMessage>(delegate
			{
			}, requireAuthentication: false);
			RegisterHandler<SpawnMessage>(OnHostClientSpawn);
			RegisterHandler<ObjectSpawnStartedMessage>(delegate
			{
			});
			RegisterHandler<ObjectSpawnFinishedMessage>(delegate
			{
			});
			RegisterHandler<EntityStateMessage>(delegate
			{
			});
		}
		else
		{
			RegisterHandler<ObjectDestroyMessage>(OnObjectDestroy);
			RegisterHandler<ObjectHideMessage>(OnObjectHide);
			RegisterHandler<NetworkPongMessage>(NetworkTime.OnClientPong, requireAuthentication: false);
			RegisterHandler<SpawnMessage>(OnSpawn);
			RegisterHandler<ObjectSpawnStartedMessage>(OnObjectSpawnStarted);
			RegisterHandler<ObjectSpawnFinishedMessage>(OnObjectSpawnFinished);
			RegisterHandler<EntityStateMessage>(OnEntityStateMessage);
		}
		RegisterHandler<TimeSnapshotMessage>(OnTimeSnapshotMessage);
		RegisterHandler<ChangeOwnerMessage>(OnChangeOwner);
		RegisterHandler<RpcBufferMessage>(OnRPCBufferMessage);
	}

	public static void RegisterHandler<T>(Action<T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage
	{
		ushort id = NetworkMessageId<T>.Id;
		if (handlers.ContainsKey(id))
		{
			Debug.LogWarning($"NetworkClient.RegisterHandler replacing handler for {typeof(T).FullName}, id={id}. If replacement is intentional, use ReplaceHandler instead to avoid this warning.");
		}
		handlers[id] = NetworkMessages.WrapHandler<T, NetworkConnection>(HandlerWrapped, requireAuthentication);
		void HandlerWrapped(NetworkConnection _, T value)
		{
			handler(value);
		}
	}

	public static void ReplaceHandler<T>(Action<NetworkConnection, T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage
	{
		ushort id = NetworkMessageId<T>.Id;
		handlers[id] = NetworkMessages.WrapHandler(handler, requireAuthentication);
	}

	public static void ReplaceHandler<T>(Action<T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage
	{
		ReplaceHandler(delegate(NetworkConnection _, T value)
		{
			handler(value);
		}, requireAuthentication);
	}

	public static bool UnregisterHandler<T>() where T : struct, NetworkMessage
	{
		ushort id = NetworkMessageId<T>.Id;
		return handlers.Remove(id);
	}

	public static bool GetPrefab(uint assetId, out GameObject prefab)
	{
		prefab = null;
		if (assetId != 0 && prefabs.TryGetValue(assetId, out prefab))
		{
			return prefab != null;
		}
		return false;
	}

	private static void RegisterPrefabIdentity(NetworkIdentity prefab)
	{
		if (prefab.assetId == 0)
		{
			Debug.LogError("Can not Register '" + prefab.name + "' because it had empty assetid. If this is a scene Object use RegisterSpawnHandler instead");
			return;
		}
		if (prefab.sceneId != 0L)
		{
			Debug.LogError("Can not Register '" + prefab.name + "' because it has a sceneId, make sure you are passing in the original prefab and not an instance in the scene.");
			return;
		}
		if (prefab.GetComponentsInChildren<NetworkIdentity>().Length > 1)
		{
			Debug.LogError("Prefab '" + prefab.name + "' has multiple NetworkIdentity components. There should only be one NetworkIdentity on a prefab, and it must be on the root object.");
		}
		if (prefabs.ContainsKey(prefab.assetId))
		{
			GameObject gameObject = prefabs[prefab.assetId];
			Debug.LogWarning($"Replacing existing prefab with assetId '{prefab.assetId}'. Old prefab '{gameObject.name}', New prefab '{prefab.name}'");
		}
		if (spawnHandlers.ContainsKey(prefab.assetId) || unspawnHandlers.ContainsKey(prefab.assetId))
		{
			Debug.LogWarning($"Adding prefab '{prefab.name}' with assetId '{prefab.assetId}' when spawnHandlers with same assetId already exists. If you want to use custom spawn handling, then remove the prefab from NetworkManager's registered prefabs first.");
		}
		prefabs[prefab.assetId] = prefab.gameObject;
	}

	public static void RegisterPrefab(GameObject prefab, uint newAssetId)
	{
		if (prefab == null)
		{
			Debug.LogError("Could not register prefab because it was null");
			return;
		}
		if (newAssetId == 0)
		{
			Debug.LogError("Could not register '" + prefab.name + "' with new assetId because the new assetId was empty");
			return;
		}
		if (!prefab.TryGetComponent<NetworkIdentity>(out var component))
		{
			Debug.LogError("Could not register '" + prefab.name + "' since it contains no NetworkIdentity component");
			return;
		}
		if (component.assetId != 0 && component.assetId != newAssetId)
		{
			Debug.LogError($"Could not register '{prefab.name}' to {newAssetId} because it already had an AssetId, Existing assetId {component.assetId}");
			return;
		}
		component.assetId = newAssetId;
		RegisterPrefabIdentity(component);
	}

	public static void RegisterPrefab(GameObject prefab)
	{
		NetworkIdentity component;
		if (prefab == null)
		{
			Debug.LogError("Could not register prefab because it was null");
		}
		else if (!prefab.TryGetComponent<NetworkIdentity>(out component))
		{
			Debug.LogError("Could not register '" + prefab.name + "' since it contains no NetworkIdentity component");
		}
		else
		{
			RegisterPrefabIdentity(component);
		}
	}

	public static void RegisterPrefab(GameObject prefab, uint newAssetId, SpawnDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
	{
		if (spawnHandler == null)
		{
			Debug.LogError($"Can not Register null SpawnHandler for {newAssetId}");
			return;
		}
		RegisterPrefab(prefab, newAssetId, (SpawnMessage msg) => spawnHandler(msg.position, msg.assetId), unspawnHandler);
	}

	public static void RegisterPrefab(GameObject prefab, SpawnDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
	{
		if (prefab == null)
		{
			Debug.LogError("Could not register handler for prefab because the prefab was null");
			return;
		}
		if (!prefab.TryGetComponent<NetworkIdentity>(out var component))
		{
			Debug.LogError("Could not register handler for '" + prefab.name + "' since it contains no NetworkIdentity component");
			return;
		}
		if (component.sceneId != 0L)
		{
			Debug.LogError("Can not Register '" + prefab.name + "' because it has a sceneId, make sure you are passing in the original prefab and not an instance in the scene.");
			return;
		}
		if (component.assetId == 0)
		{
			Debug.LogError("Can not Register handler for '" + prefab.name + "' because it had empty assetid. If this is a scene Object use RegisterSpawnHandler instead");
			return;
		}
		if (spawnHandler == null)
		{
			Debug.LogError($"Can not Register null SpawnHandler for {component.assetId}");
			return;
		}
		RegisterPrefab(prefab, (SpawnMessage msg) => spawnHandler(msg.position, msg.assetId), unspawnHandler);
	}

	public static void RegisterPrefab(GameObject prefab, uint newAssetId, SpawnHandlerDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
	{
		if (newAssetId == 0)
		{
			Debug.LogError("Could not register handler for '" + prefab.name + "' with new assetId because the new assetId was empty");
			return;
		}
		if (prefab == null)
		{
			Debug.LogError("Could not register handler for prefab because the prefab was null");
			return;
		}
		if (!prefab.TryGetComponent<NetworkIdentity>(out var component))
		{
			Debug.LogError("Could not register handler for '" + prefab.name + "' since it contains no NetworkIdentity component");
			return;
		}
		if (component.assetId != 0 && component.assetId != newAssetId)
		{
			Debug.LogError($"Could not register Handler for '{prefab.name}' to {newAssetId} because it already had an AssetId, Existing assetId {component.assetId}");
			return;
		}
		if (component.sceneId != 0L)
		{
			Debug.LogError("Can not Register '" + prefab.name + "' because it has a sceneId, make sure you are passing in the original prefab and not an instance in the scene.");
			return;
		}
		component.assetId = newAssetId;
		uint assetId = component.assetId;
		if (spawnHandler == null)
		{
			Debug.LogError($"Can not Register null SpawnHandler for {assetId}");
			return;
		}
		if (unspawnHandler == null)
		{
			Debug.LogError($"Can not Register null UnSpawnHandler for {assetId}");
			return;
		}
		if (spawnHandlers.ContainsKey(assetId) || unspawnHandlers.ContainsKey(assetId))
		{
			Debug.LogWarning($"Replacing existing spawnHandlers for prefab '{prefab.name}' with assetId '{assetId}'");
		}
		if (prefabs.ContainsKey(assetId))
		{
			Debug.LogError($"assetId '{assetId}' is already used by prefab '{prefabs[assetId].name}', unregister the prefab first before trying to add handler");
		}
		if (prefab.GetComponentsInChildren<NetworkIdentity>().Length > 1)
		{
			Debug.LogError("Prefab '" + prefab.name + "' has multiple NetworkIdentity components. There should only be one NetworkIdentity on a prefab, and it must be on the root object.");
		}
		spawnHandlers[assetId] = spawnHandler;
		unspawnHandlers[assetId] = unspawnHandler;
	}

	public static void RegisterPrefab(GameObject prefab, SpawnHandlerDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
	{
		if (prefab == null)
		{
			Debug.LogError("Could not register handler for prefab because the prefab was null");
			return;
		}
		if (!prefab.TryGetComponent<NetworkIdentity>(out var component))
		{
			Debug.LogError("Could not register handler for '" + prefab.name + "' since it contains no NetworkIdentity component");
			return;
		}
		if (component.sceneId != 0L)
		{
			Debug.LogError("Can not Register '" + prefab.name + "' because it has a sceneId, make sure you are passing in the original prefab and not an instance in the scene.");
			return;
		}
		uint assetId = component.assetId;
		if (assetId == 0)
		{
			Debug.LogError("Can not Register handler for '" + prefab.name + "' because it had empty assetid. If this is a scene Object use RegisterSpawnHandler instead");
			return;
		}
		if (spawnHandler == null)
		{
			Debug.LogError($"Can not Register null SpawnHandler for {assetId}");
			return;
		}
		if (unspawnHandler == null)
		{
			Debug.LogError($"Can not Register null UnSpawnHandler for {assetId}");
			return;
		}
		if (spawnHandlers.ContainsKey(assetId) || unspawnHandlers.ContainsKey(assetId))
		{
			Debug.LogWarning($"Replacing existing spawnHandlers for prefab '{prefab.name}' with assetId '{assetId}'");
		}
		if (prefabs.ContainsKey(assetId))
		{
			Debug.LogError($"assetId '{assetId}' is already used by prefab '{prefabs[assetId].name}', unregister the prefab first before trying to add handler");
		}
		if (prefab.GetComponentsInChildren<NetworkIdentity>().Length > 1)
		{
			Debug.LogError("Prefab '" + prefab.name + "' has multiple NetworkIdentity components. There should only be one NetworkIdentity on a prefab, and it must be on the root object.");
		}
		spawnHandlers[assetId] = spawnHandler;
		unspawnHandlers[assetId] = unspawnHandler;
	}

	public static void UnregisterPrefab(GameObject prefab)
	{
		if (prefab == null)
		{
			Debug.LogError("Could not unregister prefab because it was null");
			return;
		}
		if (!prefab.TryGetComponent<NetworkIdentity>(out var component))
		{
			Debug.LogError("Could not unregister '" + prefab.name + "' since it contains no NetworkIdentity component");
			return;
		}
		uint assetId = component.assetId;
		prefabs.Remove(assetId);
		spawnHandlers.Remove(assetId);
		unspawnHandlers.Remove(assetId);
	}

	public static void RegisterSpawnHandler(uint assetId, SpawnDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
	{
		if (spawnHandler == null)
		{
			Debug.LogError($"Can not Register null SpawnHandler for {assetId}");
			return;
		}
		RegisterSpawnHandler(assetId, (SpawnMessage msg) => spawnHandler(msg.position, msg.assetId), unspawnHandler);
	}

	public static void RegisterSpawnHandler(uint assetId, SpawnHandlerDelegate spawnHandler, UnSpawnDelegate unspawnHandler)
	{
		if (spawnHandler == null)
		{
			Debug.LogError($"Can not Register null SpawnHandler for {assetId}");
			return;
		}
		if (unspawnHandler == null)
		{
			Debug.LogError($"Can not Register null UnSpawnHandler for {assetId}");
			return;
		}
		if (assetId == 0)
		{
			Debug.LogError("Can not Register SpawnHandler for empty assetId");
			return;
		}
		if (spawnHandlers.ContainsKey(assetId) || unspawnHandlers.ContainsKey(assetId))
		{
			Debug.LogWarning($"Replacing existing spawnHandlers for {assetId}");
		}
		if (prefabs.ContainsKey(assetId))
		{
			Debug.LogError($"assetId '{assetId}' is already used by prefab '{prefabs[assetId].name}'");
		}
		spawnHandlers[assetId] = spawnHandler;
		unspawnHandlers[assetId] = unspawnHandler;
	}

	public static void UnregisterSpawnHandler(uint assetId)
	{
		spawnHandlers.Remove(assetId);
		unspawnHandlers.Remove(assetId);
	}

	public static void ClearSpawners()
	{
		prefabs.Clear();
		spawnHandlers.Clear();
		unspawnHandlers.Clear();
	}

	internal static bool InvokeUnSpawnHandler(uint assetId, GameObject obj)
	{
		if (unspawnHandlers.TryGetValue(assetId, out var value) && value != null)
		{
			value(obj);
			return true;
		}
		return false;
	}

	public static bool Ready()
	{
		if (ready)
		{
			Debug.LogError("NetworkClient is already ready. It shouldn't be called twice.");
			return false;
		}
		if (connection == null)
		{
			Debug.LogError("Ready() called with invalid connection object: conn=null");
			return false;
		}
		ready = true;
		connection.isReady = true;
		connection.Send(default(ReadyMessage));
		return true;
	}

	internal static void InternalAddPlayer(NetworkIdentity identity)
	{
		localPlayer = identity;
		if (ready && connection != null)
		{
			connection.identity = identity;
		}
		else
		{
			Debug.LogWarning("NetworkClient can't AddPlayer before being ready. Please call NetworkClient.Ready() first. Clients are considered ready after joining the game world.");
		}
	}

	public static bool AddPlayer()
	{
		if (connection == null)
		{
			Debug.LogError("AddPlayer requires a valid NetworkClient.connection.");
			return false;
		}
		if (!ready)
		{
			Debug.LogError("AddPlayer requires a ready NetworkClient.");
			return false;
		}
		if (connection.identity != null)
		{
			Debug.LogError("NetworkClient.AddPlayer: a PlayerController was already added. Did you call AddPlayer twice?");
			return false;
		}
		connection.Send(default(AddPlayerMessage));
		return true;
	}

	internal static void ApplySpawnPayload(NetworkIdentity identity, SpawnMessage message)
	{
		if (message.assetId != 0)
		{
			identity.assetId = message.assetId;
		}
		if (!identity.gameObject.activeSelf)
		{
			identity.gameObject.SetActive(value: true);
		}
		identity.transform.localPosition = message.position;
		identity.transform.localRotation = message.rotation;
		identity.transform.localScale = message.scale;
		identity.isOwned = message.isOwner;
		identity.netId = message.netId;
		if (message.isLocalPlayer)
		{
			InternalAddPlayer(identity);
		}
		InitializeIdentityFlags(identity);
		if (message.payload.Count > 0)
		{
			using NetworkReaderPooled reader = NetworkReaderPool.Get(message.payload);
			identity.DeserializeClient(reader, initialState: true);
		}
		spawned[message.netId] = identity;
		if (identity.isOwned)
		{
			connection?.owned.Add(identity);
		}
		if (isSpawnFinished)
		{
			InvokeIdentityCallbacks(identity);
		}
	}

	internal static bool FindOrSpawnObject(SpawnMessage message, out NetworkIdentity identity)
	{
		identity = GetExistingObject(message.netId);
		if (identity != null)
		{
			return true;
		}
		if (message.assetId == 0 && message.sceneId == 0L)
		{
			Debug.LogError($"OnSpawn message with netId '{message.netId}' has no AssetId or sceneId");
			return false;
		}
		identity = ((message.sceneId == 0L) ? SpawnPrefab(message) : SpawnSceneObject(message.sceneId));
		if (identity == null)
		{
			Debug.LogError($"Could not spawn assetId={message.assetId} scene={message.sceneId:X} netId={message.netId}");
			return false;
		}
		return true;
	}

	private static NetworkIdentity GetExistingObject(uint netid)
	{
		spawned.TryGetValue(netid, out var value);
		return value;
	}

	private static NetworkIdentity SpawnPrefab(SpawnMessage message)
	{
		if (spawnHandlers.TryGetValue(message.assetId, out var value))
		{
			GameObject gameObject = value(message);
			if (gameObject == null)
			{
				Debug.LogError($"Spawn Handler returned null, Handler assetId '{message.assetId}'");
				return null;
			}
			if (!gameObject.TryGetComponent<NetworkIdentity>(out var component))
			{
				Debug.LogError($"Object Spawned by handler did not have a NetworkIdentity, Handler assetId '{message.assetId}'");
				return null;
			}
			return component;
		}
		if (GetPrefab(message.assetId, out var prefab))
		{
			return UnityEngine.Object.Instantiate(prefab, message.position, message.rotation).GetComponent<NetworkIdentity>();
		}
		Debug.LogError($"Failed to spawn server object, did you forget to add it to the NetworkManager? assetId={message.assetId} netId={message.netId}");
		return null;
	}

	private static NetworkIdentity SpawnSceneObject(ulong sceneId)
	{
		NetworkIdentity andRemoveSceneObject = GetAndRemoveSceneObject(sceneId);
		if (andRemoveSceneObject == null)
		{
			Debug.LogError($"Spawn scene object not found for {sceneId:X}. Make sure that client and server use exactly the same project. This only happens if the hierarchy gets out of sync.");
		}
		return andRemoveSceneObject;
	}

	private static NetworkIdentity GetAndRemoveSceneObject(ulong sceneId)
	{
		if (spawnableObjects.TryGetValue(sceneId, out var value))
		{
			spawnableObjects.Remove(sceneId);
			return value;
		}
		return null;
	}

	public static void PrepareToSpawnSceneObjects()
	{
		spawnableObjects.Clear();
		NetworkIdentity[] array = Resources.FindObjectsOfTypeAll<NetworkIdentity>();
		foreach (NetworkIdentity networkIdentity in array)
		{
			if (Utils.IsSceneObject(networkIdentity) && !networkIdentity.gameObject.activeSelf)
			{
				if (spawnableObjects.TryGetValue(networkIdentity.sceneId, out var value))
				{
					Debug.LogWarning($"NetworkClient: Duplicate sceneId {networkIdentity.sceneId} detected on {networkIdentity.gameObject.name} and {value.gameObject.name}\n" + "This can happen if a networked object is persisted in DontDestroyOnLoad through loading / changing to the scene where it originated,\n" + $"otherwise you may need to open and re-save the {networkIdentity.gameObject.scene} to reset scene id's.", networkIdentity.gameObject);
				}
				else
				{
					spawnableObjects.Add(networkIdentity.sceneId, networkIdentity);
				}
			}
		}
	}

	internal static void OnObjectSpawnStarted(ObjectSpawnStartedMessage _)
	{
		PrepareToSpawnSceneObjects();
		isSpawnFinished = false;
	}

	internal static void OnObjectSpawnFinished(ObjectSpawnFinishedMessage _)
	{
		foreach (NetworkIdentity item in spawned.Values.OrderBy((NetworkIdentity uv) => uv.netId))
		{
			if (item != null)
			{
				BootstrapIdentity(item);
			}
			else
			{
				Debug.LogWarning("Found null entry in NetworkClient.spawned. This is unexpected. Was the NetworkIdentity not destroyed properly?");
			}
		}
		isSpawnFinished = true;
	}

	private static void OnHostClientObjectDestroy(ObjectDestroyMessage message)
	{
		if (spawned.TryGetValue(message.netId, out var value))
		{
			connection.owned.Remove(value);
		}
		spawned.Remove(message.netId);
	}

	private static void OnHostClientObjectHide(ObjectHideMessage message)
	{
		if (spawned.TryGetValue(message.netId, out var value) && value != null && aoi != null)
		{
			aoi.SetHostVisibility(value, visible: false);
		}
	}

	internal static void OnHostClientSpawn(SpawnMessage message)
	{
		if (NetworkServer.spawned.TryGetValue(message.netId, out var value) && value != null)
		{
			spawned[message.netId] = value;
			if (message.isOwner)
			{
				connection.owned.Add(value);
			}
			if (message.isLocalPlayer)
			{
				InternalAddPlayer(value);
			}
			if (aoi != null)
			{
				aoi.SetHostVisibility(value, visible: true);
			}
			value.isOwned = message.isOwner;
			BootstrapIdentity(value);
		}
	}

	private static void OnEntityStateMessage(EntityStateMessage message)
	{
		if (spawned.TryGetValue(message.netId, out var value) && value != null)
		{
			using (NetworkReaderPooled reader = NetworkReaderPool.Get(message.payload))
			{
				value.DeserializeClient(reader, initialState: false);
				return;
			}
		}
		Debug.LogWarning($"Did not find target for sync message for {message.netId} . Note: this can be completely normal because UDP messages may arrive out of order, so this message might have arrived after a Destroy message.");
	}

	private static void OnRPCMessage(RpcMessage message)
	{
		if (spawned.TryGetValue(message.netId, out var value))
		{
			using (NetworkReaderPooled reader = NetworkReaderPool.Get(message.payload))
			{
				value.HandleRemoteCall(message.componentIndex, message.functionHash, RemoteCallType.ClientRpc, reader);
			}
		}
	}

	private static void OnRPCBufferMessage(RpcBufferMessage message)
	{
		using NetworkReaderPooled networkReaderPooled = NetworkReaderPool.Get(message.payload);
		while (networkReaderPooled.Remaining > 0)
		{
			OnRPCMessage(networkReaderPooled.Read<RpcMessage>());
		}
	}

	private static void OnObjectHide(ObjectHideMessage message)
	{
		DestroyObject(message.netId);
	}

	internal static void OnObjectDestroy(ObjectDestroyMessage message)
	{
		DestroyObject(message.netId);
	}

	internal static void OnSpawn(SpawnMessage message)
	{
		if (FindOrSpawnObject(message, out var identity))
		{
			ApplySpawnPayload(identity, message);
		}
	}

	internal static void OnChangeOwner(ChangeOwnerMessage message)
	{
		NetworkIdentity existingObject = GetExistingObject(message.netId);
		if (existingObject != null)
		{
			ChangeOwner(existingObject, message);
		}
		else
		{
			Debug.LogError($"OnChangeOwner: Could not find object with netId {message.netId}");
		}
	}

	internal static void ChangeOwner(NetworkIdentity identity, ChangeOwnerMessage message)
	{
		if (identity.isLocalPlayer && !message.isLocalPlayer)
		{
			identity.OnStopLocalPlayer();
		}
		identity.isOwned = message.isOwner;
		if (identity.isOwned)
		{
			connection?.owned.Add(identity);
		}
		else
		{
			connection?.owned.Remove(identity);
		}
		identity.NotifyAuthority();
		identity.isLocalPlayer = message.isLocalPlayer;
		if (identity.isLocalPlayer)
		{
			localPlayer = identity;
			identity.connectionToServer = connection;
			identity.OnStartLocalPlayer();
		}
		else if (localPlayer == identity)
		{
			localPlayer = null;
		}
	}

	private static void InitializeIdentityFlags(NetworkIdentity identity)
	{
		identity.isClient = true;
		identity.isLocalPlayer = localPlayer == identity;
		if (identity.isLocalPlayer)
		{
			identity.connectionToServer = connection;
		}
	}

	private static void InvokeIdentityCallbacks(NetworkIdentity identity)
	{
		identity.NotifyAuthority();
		identity.OnStartClient();
		if (identity.isLocalPlayer)
		{
			identity.OnStartLocalPlayer();
		}
	}

	private static void BootstrapIdentity(NetworkIdentity identity)
	{
		InitializeIdentityFlags(identity);
		InvokeIdentityCallbacks(identity);
	}

	private static void BroadcastTimeSnapshot()
	{
		Send(default(TimeSnapshotMessage), 1);
	}

	private static void Broadcast()
	{
		if (!connection.isReady || NetworkServer.active)
		{
			return;
		}
		BroadcastTimeSnapshot();
		foreach (NetworkIdentity item in connection.owned)
		{
			if (item != null)
			{
				using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
				item.SerializeClient(networkWriterPooled);
				if (networkWriterPooled.Position > 0)
				{
					Send(new EntityStateMessage
					{
						netId = item.netId,
						payload = networkWriterPooled.ToArraySegment()
					});
					item.ClearDirtyComponentsDirtyBits();
				}
			}
			else
			{
				Debug.LogWarning("Found 'null' entry in owned list for client. This is unexpected behaviour.");
			}
		}
	}

	internal static void NetworkEarlyUpdate()
	{
		if (Transport.active != null)
		{
			Transport.active.ClientEarlyUpdate();
		}
		UpdateTimeInterpolation();
	}

	internal static void NetworkLateUpdate()
	{
		if (active && (!Application.isPlaying || AccurateInterval.Elapsed(NetworkTime.localTime, sendInterval, ref lastSendTime)))
		{
			Broadcast();
		}
		if (connection is LocalConnectionToServer localConnectionToServer)
		{
			localConnectionToServer.Update();
		}
		else if (connection is NetworkConnectionToServer networkConnectionToServer && active && connectState == ConnectState.Connected)
		{
			NetworkTime.UpdateClient();
			networkConnectionToServer.Update();
		}
		if (Transport.active != null)
		{
			Transport.active.ClientLateUpdate();
		}
	}

	public static void DestroyAllClientObjects()
	{
		try
		{
			foreach (NetworkIdentity value in spawned.Values)
			{
				if (!(value != null) || !(value.gameObject != null))
				{
					continue;
				}
				if (value.isLocalPlayer)
				{
					value.OnStopLocalPlayer();
				}
				value.OnStopClient();
				bool flag = value.connectionToServer is LocalConnectionToServer;
				if (!value.isServer || flag)
				{
					if (InvokeUnSpawnHandler(value.assetId, value.gameObject))
					{
						value.Reset();
					}
					else if (value.sceneId != 0L)
					{
						value.Reset();
						value.gameObject.SetActive(value: false);
					}
					else
					{
						UnityEngine.Object.Destroy(value.gameObject);
					}
				}
			}
			spawned.Clear();
			connection?.owned.Clear();
		}
		catch (InvalidOperationException exception)
		{
			Debug.LogException(exception);
			Debug.LogError("Could not DestroyAllClientObjects because spawned list was modified during loop, make sure you are not modifying NetworkIdentity.spawned by calling NetworkServer.Destroy or NetworkServer.Spawn in OnDestroy or OnDisable.");
		}
	}

	private static void DestroyObject(uint netId)
	{
		if (spawned.TryGetValue(netId, out var value) && value != null)
		{
			if (value.isLocalPlayer)
			{
				value.OnStopLocalPlayer();
			}
			value.OnStopClient();
			if (InvokeUnSpawnHandler(value.assetId, value.gameObject))
			{
				value.Reset();
			}
			else if (value.sceneId == 0L)
			{
				UnityEngine.Object.Destroy(value.gameObject);
			}
			else
			{
				value.gameObject.SetActive(value: false);
				spawnableObjects[value.sceneId] = value;
				value.Reset();
			}
			connection.owned.Remove(value);
			spawned.Remove(netId);
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Shutdown()
	{
		DestroyAllClientObjects();
		ClearSpawners();
		spawned.Clear();
		connection?.owned.Clear();
		handlers.Clear();
		spawnableObjects.Clear();
		NetworkIdentity.ResetClientStatics();
		if (Transport.active != null)
		{
			Transport.active.ClientDisconnect();
		}
		connectState = ConnectState.None;
		connection = null;
		localPlayer = null;
		ready = false;
		isSpawnFinished = false;
		isLoadingScene = false;
		lastSendTime = 0.0;
		unbatcher = new Unbatcher();
		OnConnectedEvent = null;
		OnDisconnectedEvent = null;
		OnErrorEvent = null;
	}

	public static void OnGUI()
	{
		if (ready)
		{
			GUILayout.BeginArea(new Rect(10f, 5f, 800f, 50f));
			GUILayout.BeginHorizontal("Box");
			GUILayout.Label("Snapshot Interp.:");
			if (localTimescale > 1.0)
			{
				GUI.color = Color.green;
			}
			else if (localTimescale < 1.0)
			{
				GUI.color = Color.red;
			}
			else
			{
				GUI.color = Color.white;
			}
			GUILayout.Box($"timeline: {localTimeline:F2}");
			GUILayout.Box($"buffer: {snapshots.Count}");
			GUILayout.Box($"DriftEMA: {driftEma.Value:F2}");
			GUILayout.Box($"DelTimeEMA: {deliveryTimeEma.Value:F2}");
			GUILayout.Box($"timescale: {localTimescale:F2}");
			GUILayout.Box($"BTM: {snapshotSettings.bufferTimeMultiplier:F2}");
			GUILayout.Box($"RTT: {NetworkTime.rtt * 1000.0:000}");
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}

	private static void InitTimeInterpolation()
	{
		localTimeline = 0.0;
		localTimescale = 1.0;
		snapshots.Clear();
		driftEma = new ExponentialMovingAverage(NetworkServer.sendRate * snapshotSettings.driftEmaDuration);
		deliveryTimeEma = new ExponentialMovingAverage(NetworkServer.sendRate * snapshotSettings.deliveryTimeEmaDuration);
	}

	private static void OnTimeSnapshotMessage(TimeSnapshotMessage _)
	{
		OnTimeSnapshot(new TimeSnapshot(connection.remoteTimeStamp, NetworkTime.localTime));
	}

	public static void OnTimeSnapshot(TimeSnapshot snap)
	{
		if (snapshotSettings.dynamicAdjustment)
		{
			snapshotSettings.bufferTimeMultiplier = SnapshotInterpolation.DynamicAdjustment(NetworkServer.sendInterval, deliveryTimeEma.StandardDeviation, snapshotSettings.dynamicAdjustmentTolerance);
		}
		SnapshotInterpolation.InsertAndAdjust(snapshots, snap, ref localTimeline, ref localTimescale, NetworkServer.sendInterval, bufferTime, snapshotSettings.catchupSpeed, snapshotSettings.slowdownSpeed, ref driftEma, snapshotSettings.catchupNegativeThreshold, snapshotSettings.catchupPositiveThreshold, ref deliveryTimeEma);
	}

	private static void UpdateTimeInterpolation()
	{
		if (snapshots.Count > 0)
		{
			SnapshotInterpolation.StepTime(Time.unscaledDeltaTime, ref localTimeline, localTimescale);
			SnapshotInterpolation.StepInterpolation(snapshots, localTimeline, out var _, out var _, out var _);
		}
	}
}
