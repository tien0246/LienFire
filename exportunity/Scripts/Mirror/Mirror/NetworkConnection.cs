using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public abstract class NetworkConnection
{
	public const int LocalConnectionId = 0;

	public readonly int connectionId;

	public bool isAuthenticated;

	public object authenticationData;

	public bool isReady;

	public float lastMessageTime;

	public readonly HashSet<NetworkIdentity> owned = new HashSet<NetworkIdentity>();

	protected Dictionary<int, Batcher> batches = new Dictionary<int, Batcher>();

	public NetworkIdentity identity { get; internal set; }

	public double remoteTimeStamp { get; internal set; }

	internal NetworkConnection()
	{
		lastMessageTime = Time.time;
	}

	internal NetworkConnection(int networkConnectionId)
		: this()
	{
		connectionId = networkConnectionId;
	}

	protected Batcher GetBatchForChannelId(int channelId)
	{
		if (!batches.TryGetValue(channelId, out var value))
		{
			value = new Batcher(Transport.active.GetBatchThreshold(channelId));
			batches[channelId] = value;
		}
		return value;
	}

	protected static bool ValidatePacketSize(ArraySegment<byte> segment, int channelId)
	{
		int maxPacketSize = Transport.active.GetMaxPacketSize(channelId);
		if (segment.Count > maxPacketSize)
		{
			Debug.LogError($"NetworkConnection.ValidatePacketSize: cannot send packet larger than {maxPacketSize} bytes, was {segment.Count} bytes");
			return false;
		}
		if (segment.Count == 0)
		{
			Debug.LogError("NetworkConnection.ValidatePacketSize: cannot send zero bytes");
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Send<T>(T message, int channelId = 0) where T : struct, NetworkMessage
	{
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		NetworkMessages.Pack(message, networkWriterPooled);
		NetworkDiagnostics.OnSend(message, channelId, networkWriterPooled.Position, 1);
		Send(networkWriterPooled.ToArraySegment(), channelId);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal virtual void Send(ArraySegment<byte> segment, int channelId = 0)
	{
		GetBatchForChannelId(channelId).AddMessage(segment, NetworkTime.localTime);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void SendToTransport(ArraySegment<byte> segment, int channelId = 0);

	internal virtual void Update()
	{
		foreach (KeyValuePair<int, Batcher> batch in batches)
		{
			using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
			while (batch.Value.GetBatch(networkWriterPooled))
			{
				ArraySegment<byte> segment = networkWriterPooled.ToArraySegment();
				if (ValidatePacketSize(segment, batch.Key))
				{
					SendToTransport(segment, batch.Key);
					networkWriterPooled.Position = 0;
				}
			}
		}
	}

	internal virtual bool IsAlive(float timeout)
	{
		return Time.time - lastMessageTime < timeout;
	}

	public abstract void Disconnect();

	public override string ToString()
	{
		return $"connection({connectionId})";
	}
}
