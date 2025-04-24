using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mirror;

[HelpURL("https://mirror-networking.gitbook.io/docs/transports/latency-simulaton-transport")]
[DisallowMultipleComponent]
public class LatencySimulation : Transport
{
	public Transport wrap;

	[Header("Common")]
	[Tooltip("Jitter latency via perlin(Time * jitterSpeed) * jitter")]
	[FormerlySerializedAs("latencySpikeMultiplier")]
	[Range(0f, 1f)]
	public float jitter = 0.02f;

	[Tooltip("Jitter latency via perlin(Time * jitterSpeed) * jitter")]
	[FormerlySerializedAs("latencySpikeSpeedMultiplier")]
	public float jitterSpeed = 1f;

	[Header("Reliable Messages")]
	[Tooltip("Reliable latency in milliseconds (1000 = 1 second)")]
	[Range(0f, 10000f)]
	public float reliableLatency = 100f;

	[Header("Unreliable Messages")]
	[Tooltip("Packet loss in %\n2% recommended for long term play testing, upto 5% for short bursts.\nAnything higher, or for a prolonged amount of time, suggests user has a connection fault.")]
	[Range(0f, 100f)]
	public float unreliableLoss = 2f;

	[Tooltip("Unreliable latency in milliseconds (1000 = 1 second) \n100ms recommended for long term play testing, upto 500ms for short bursts.\nAnything higher, or for a prolonged amount of time, suggests user has a connection fault.")]
	[Range(0f, 10000f)]
	public float unreliableLatency = 100f;

	[Tooltip("Scramble % of unreliable messages, just like over the real network. Mirror unreliable is unordered.")]
	[Range(0f, 100f)]
	public float unreliableScramble = 2f;

	private List<QueuedMessage> reliableClientToServer = new List<QueuedMessage>();

	private List<QueuedMessage> reliableServerToClient = new List<QueuedMessage>();

	private List<QueuedMessage> unreliableClientToServer = new List<QueuedMessage>();

	private List<QueuedMessage> unreliableServerToClient = new List<QueuedMessage>();

	private System.Random random = new System.Random();

	public void Awake()
	{
		if (wrap == null)
		{
			throw new Exception("LatencySimulationTransport requires an underlying transport to wrap around.");
		}
	}

	private void OnEnable()
	{
		wrap.enabled = true;
	}

	private void OnDisable()
	{
		wrap.enabled = false;
	}

	protected virtual float Noise(float time)
	{
		return Mathf.PerlinNoise(time, time);
	}

	private float SimulateLatency(int channeldId)
	{
		float num = Noise((float)Time.unscaledTimeAsDouble * jitterSpeed) * jitter;
		return channeldId switch
		{
			0 => reliableLatency / 1000f + num, 
			1 => unreliableLatency / 1000f + num, 
			_ => 0f, 
		};
	}

	private void SimulateSend(int connectionId, ArraySegment<byte> segment, int channelId, float latency, List<QueuedMessage> reliableQueue, List<QueuedMessage> unreliableQueue)
	{
		byte[] array = new byte[segment.Count];
		Buffer.BlockCopy(segment.Array, segment.Offset, array, 0, segment.Count);
		QueuedMessage item = new QueuedMessage
		{
			connectionId = connectionId,
			bytes = array,
			time = Time.unscaledTimeAsDouble + (double)latency
		};
		switch (channelId)
		{
		case 0:
			reliableQueue.Add(item);
			break;
		case 1:
			if (!(random.NextDouble() < (double)(unreliableLoss / 100f)))
			{
				bool num = random.NextDouble() < (double)(unreliableScramble / 100f);
				int count = unreliableQueue.Count;
				int index = (num ? random.Next(0, count + 1) : count);
				unreliableQueue.Insert(index, item);
			}
			break;
		default:
			Debug.LogError(string.Format("{0} unexpected channelId: {1}", "LatencySimulation", channelId));
			break;
		}
	}

	public override bool Available()
	{
		return wrap.Available();
	}

	public override void ClientConnect(string address)
	{
		wrap.OnClientConnected = OnClientConnected;
		wrap.OnClientDataReceived = OnClientDataReceived;
		wrap.OnClientError = OnClientError;
		wrap.OnClientDisconnected = OnClientDisconnected;
		wrap.ClientConnect(address);
	}

	public override void ClientConnect(Uri uri)
	{
		wrap.OnClientConnected = OnClientConnected;
		wrap.OnClientDataReceived = OnClientDataReceived;
		wrap.OnClientError = OnClientError;
		wrap.OnClientDisconnected = OnClientDisconnected;
		wrap.ClientConnect(uri);
	}

	public override bool ClientConnected()
	{
		return wrap.ClientConnected();
	}

	public override void ClientDisconnect()
	{
		wrap.ClientDisconnect();
		reliableClientToServer.Clear();
		unreliableClientToServer.Clear();
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId)
	{
		float latency = SimulateLatency(channelId);
		SimulateSend(0, segment, channelId, latency, reliableClientToServer, unreliableClientToServer);
	}

	public override Uri ServerUri()
	{
		return wrap.ServerUri();
	}

	public override bool ServerActive()
	{
		return wrap.ServerActive();
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		return wrap.ServerGetClientAddress(connectionId);
	}

	public override void ServerDisconnect(int connectionId)
	{
		wrap.ServerDisconnect(connectionId);
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		float latency = SimulateLatency(channelId);
		SimulateSend(connectionId, segment, channelId, latency, reliableServerToClient, unreliableServerToClient);
	}

	public override void ServerStart()
	{
		wrap.OnServerConnected = OnServerConnected;
		wrap.OnServerDataReceived = OnServerDataReceived;
		wrap.OnServerError = OnServerError;
		wrap.OnServerDisconnected = OnServerDisconnected;
		wrap.ServerStart();
	}

	public override void ServerStop()
	{
		wrap.ServerStop();
		reliableServerToClient.Clear();
		unreliableServerToClient.Clear();
	}

	public override void ClientEarlyUpdate()
	{
		wrap.ClientEarlyUpdate();
	}

	public override void ServerEarlyUpdate()
	{
		wrap.ServerEarlyUpdate();
	}

	public override void ClientLateUpdate()
	{
		for (int i = 0; i < reliableClientToServer.Count; i++)
		{
			QueuedMessage queuedMessage = reliableClientToServer[i];
			if (queuedMessage.time <= Time.unscaledTimeAsDouble)
			{
				wrap.ClientSend(new ArraySegment<byte>(queuedMessage.bytes));
				reliableClientToServer.RemoveAt(i);
				i--;
			}
		}
		for (int j = 0; j < unreliableClientToServer.Count; j++)
		{
			QueuedMessage queuedMessage2 = unreliableClientToServer[j];
			if (queuedMessage2.time <= Time.unscaledTimeAsDouble)
			{
				wrap.ClientSend(new ArraySegment<byte>(queuedMessage2.bytes));
				unreliableClientToServer.RemoveAt(j);
				j--;
			}
		}
		wrap.ClientLateUpdate();
	}

	public override void ServerLateUpdate()
	{
		for (int i = 0; i < reliableServerToClient.Count; i++)
		{
			QueuedMessage queuedMessage = reliableServerToClient[i];
			if (queuedMessage.time <= Time.unscaledTimeAsDouble)
			{
				wrap.ServerSend(queuedMessage.connectionId, new ArraySegment<byte>(queuedMessage.bytes));
				reliableServerToClient.RemoveAt(i);
				i--;
			}
		}
		for (int j = 0; j < unreliableServerToClient.Count; j++)
		{
			QueuedMessage queuedMessage2 = unreliableServerToClient[j];
			if (queuedMessage2.time <= Time.unscaledTimeAsDouble)
			{
				wrap.ServerSend(queuedMessage2.connectionId, new ArraySegment<byte>(queuedMessage2.bytes));
				unreliableServerToClient.RemoveAt(j);
				j--;
			}
		}
		wrap.ServerLateUpdate();
	}

	public override int GetBatchThreshold(int channelId)
	{
		return wrap.GetBatchThreshold(channelId);
	}

	public override int GetMaxPacketSize(int channelId = 0)
	{
		return wrap.GetMaxPacketSize(channelId);
	}

	public override void Shutdown()
	{
		wrap.Shutdown();
	}

	public override string ToString()
	{
		return string.Format("{0} {1}", "LatencySimulation", wrap);
	}
}
