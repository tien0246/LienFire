using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

public class LocalConnectionToServer : NetworkConnectionToServer
{
	internal LocalConnectionToClient connectionToClient;

	internal readonly Queue<NetworkWriterPooled> queue = new Queue<NetworkWriterPooled>();

	private bool connectedEventPending;

	private bool disconnectedEventPending;

	[Obsolete("Use LocalConnectionToClient.address instead.")]
	public string address => "localhost";

	internal void QueueConnectedEvent()
	{
		connectedEventPending = true;
	}

	internal void QueueDisconnectedEvent()
	{
		disconnectedEventPending = true;
	}

	internal override void Send(ArraySegment<byte> segment, int channelId = 0)
	{
		if (segment.Count == 0)
		{
			Debug.LogError("LocalConnection.SendBytes cannot send zero bytes");
			return;
		}
		Batcher batchForChannelId = GetBatchForChannelId(channelId);
		batchForChannelId.AddMessage(segment, NetworkTime.localTime);
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		if (batchForChannelId.GetBatch(networkWriterPooled))
		{
			NetworkServer.OnTransportData(connectionId, networkWriterPooled.ToArraySegment(), channelId);
		}
		else
		{
			Debug.LogError("Local connection failed to make batch. This should never happen.");
		}
	}

	internal override void Update()
	{
		base.Update();
		if (connectedEventPending)
		{
			connectedEventPending = false;
			NetworkClient.OnConnectedEvent?.Invoke();
		}
		while (queue.Count > 0)
		{
			NetworkWriterPooled networkWriterPooled = queue.Dequeue();
			ArraySegment<byte> message = networkWriterPooled.ToArraySegment();
			Batcher batchForChannelId = GetBatchForChannelId(0);
			batchForChannelId.AddMessage(message, NetworkTime.localTime);
			using (NetworkWriterPooled networkWriterPooled2 = NetworkWriterPool.Get())
			{
				if (batchForChannelId.GetBatch(networkWriterPooled2))
				{
					NetworkClient.OnTransportData(networkWriterPooled2.ToArraySegment(), 0);
				}
			}
			NetworkWriterPool.Return(networkWriterPooled);
		}
		if (disconnectedEventPending)
		{
			disconnectedEventPending = false;
			NetworkClient.OnDisconnectedEvent?.Invoke();
		}
	}

	internal void DisconnectInternal()
	{
		isReady = false;
		NetworkClient.ready = false;
	}

	public override void Disconnect()
	{
		connectionToClient.DisconnectInternal();
		DisconnectInternal();
		NetworkServer.RemoveLocalConnection();
		NetworkClient.OnTransportDisconnected();
	}

	internal override bool IsAlive(float timeout)
	{
		return true;
	}
}
