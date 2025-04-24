using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public class NetworkConnectionToClient : NetworkConnection
{
	private readonly NetworkWriter reliableRpcs = new NetworkWriter();

	private readonly NetworkWriter unreliableRpcs = new NetworkWriter();

	public readonly HashSet<NetworkIdentity> observing = new HashSet<NetworkIdentity>();

	public Unbatcher unbatcher = new Unbatcher();

	private ExponentialMovingAverage driftEma;

	private ExponentialMovingAverage deliveryTimeEma;

	public double remoteTimeline;

	public double remoteTimescale;

	private double bufferTimeMultiplier = 2.0;

	private readonly SortedList<double, TimeSnapshot> snapshots = new SortedList<double, TimeSnapshot>();

	public int snapshotBufferSizeLimit = 64;

	public virtual string address => Transport.active.ServerGetClientAddress(connectionId);

	[Obsolete(".clientOwnedObjects was renamed to .owned :)")]
	public HashSet<NetworkIdentity> clientOwnedObjects => owned;

	private double bufferTime => (double)NetworkServer.sendInterval * bufferTimeMultiplier;

	public NetworkConnectionToClient(int networkConnectionId)
		: base(networkConnectionId)
	{
		driftEma = new ExponentialMovingAverage(NetworkServer.sendRate * NetworkClient.snapshotSettings.driftEmaDuration);
		deliveryTimeEma = new ExponentialMovingAverage(NetworkServer.sendRate * NetworkClient.snapshotSettings.deliveryTimeEmaDuration);
		snapshotBufferSizeLimit = Mathf.Max((int)NetworkClient.snapshotSettings.bufferTimeMultiplier, snapshotBufferSizeLimit);
	}

	public void OnTimeSnapshot(TimeSnapshot snapshot)
	{
		if (snapshots.Count < snapshotBufferSizeLimit)
		{
			if (NetworkClient.snapshotSettings.dynamicAdjustment)
			{
				bufferTimeMultiplier = SnapshotInterpolation.DynamicAdjustment(NetworkServer.sendInterval, deliveryTimeEma.StandardDeviation, NetworkClient.snapshotSettings.dynamicAdjustmentTolerance);
			}
			SnapshotInterpolation.InsertAndAdjust(snapshots, snapshot, ref remoteTimeline, ref remoteTimescale, NetworkServer.sendInterval, bufferTime, NetworkClient.snapshotSettings.catchupSpeed, NetworkClient.snapshotSettings.slowdownSpeed, ref driftEma, NetworkClient.snapshotSettings.catchupNegativeThreshold, NetworkClient.snapshotSettings.catchupPositiveThreshold, ref deliveryTimeEma);
		}
	}

	public void UpdateTimeInterpolation()
	{
		if (snapshots.Count > 0)
		{
			SnapshotInterpolation.StepTime(Time.unscaledDeltaTime, ref remoteTimeline, remoteTimescale);
			SnapshotInterpolation.StepInterpolation(snapshots, remoteTimeline, out var _, out var _, out var _);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override void SendToTransport(ArraySegment<byte> segment, int channelId = 0)
	{
		Transport.active.ServerSend(connectionId, segment, channelId);
	}

	private void FlushRpcs(NetworkWriter buffer, int channelId)
	{
		if (buffer.Position > 0)
		{
			Send(new RpcBufferMessage
			{
				payload = buffer
			}, channelId);
			buffer.Position = 0;
		}
	}

	private void BufferRpc(RpcMessage message, NetworkWriter buffer, int channelId, int maxMessageSize)
	{
		int num = maxMessageSize - 2 - 4 - 8;
		int position = buffer.Position;
		buffer.Write(message);
		if (buffer.Position - position > num)
		{
			Debug.LogWarning($"NetworkConnectionToClient: discarded RpcMesage for netId={message.netId} componentIndex={message.componentIndex} functionHash={message.functionHash} because it's larger than the rpc buffer limit of {num} bytes for the channel: {channelId}");
		}
		else if (buffer.Position > num)
		{
			buffer.Position = position;
			FlushRpcs(buffer, channelId);
			buffer.Write(message);
		}
	}

	internal void BufferRpc(RpcMessage message, int channelId)
	{
		int maxPacketSize = Transport.active.GetMaxPacketSize(channelId);
		switch (channelId)
		{
		case 0:
			BufferRpc(message, reliableRpcs, 0, maxPacketSize);
			break;
		case 1:
			BufferRpc(message, unreliableRpcs, 1, maxPacketSize);
			break;
		}
	}

	internal override void Update()
	{
		FlushRpcs(reliableRpcs, 0);
		FlushRpcs(unreliableRpcs, 1);
		base.Update();
	}

	public override void Disconnect()
	{
		isReady = false;
		reliableRpcs.Position = 0;
		unreliableRpcs.Position = 0;
		Transport.active.ServerDisconnect(connectionId);
	}

	internal void AddToObserving(NetworkIdentity netIdentity)
	{
		observing.Add(netIdentity);
		NetworkServer.ShowForConnection(netIdentity, this);
	}

	internal void RemoveFromObserving(NetworkIdentity netIdentity, bool isDestroyed)
	{
		observing.Remove(netIdentity);
		if (!isDestroyed)
		{
			NetworkServer.HideForConnection(netIdentity, this);
		}
	}

	internal void RemoveFromObservingsObservers()
	{
		foreach (NetworkIdentity item in observing)
		{
			item.RemoveObserver(this);
		}
		observing.Clear();
	}

	internal void AddOwnedObject(NetworkIdentity obj)
	{
		owned.Add(obj);
	}

	internal void RemoveOwnedObject(NetworkIdentity obj)
	{
		owned.Remove(obj);
	}

	internal void DestroyOwnedObjects()
	{
		foreach (NetworkIdentity item in new HashSet<NetworkIdentity>(owned))
		{
			if (item != null)
			{
				NetworkServer.Destroy(item.gameObject);
			}
		}
		owned.Clear();
	}
}
