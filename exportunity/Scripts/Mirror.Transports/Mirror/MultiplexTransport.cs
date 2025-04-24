using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
public class MultiplexTransport : Transport
{
	public Transport[] transports;

	private Transport available;

	private readonly Dictionary<KeyValuePair<int, int>, int> originalToMultiplexedId = new Dictionary<KeyValuePair<int, int>, int>(100);

	private readonly Dictionary<int, KeyValuePair<int, int>> multiplexedToOriginalId = new Dictionary<int, KeyValuePair<int, int>>(100);

	private int nextMultiplexedId = 1;

	public int AddToLookup(int originalConnectionId, int transportIndex)
	{
		KeyValuePair<int, int> keyValuePair = new KeyValuePair<int, int>(originalConnectionId, transportIndex);
		int num = nextMultiplexedId++;
		originalToMultiplexedId[keyValuePair] = num;
		multiplexedToOriginalId[num] = keyValuePair;
		return num;
	}

	public void RemoveFromLookup(int originalConnectionId, int transportIndex)
	{
		KeyValuePair<int, int> key = new KeyValuePair<int, int>(originalConnectionId, transportIndex);
		int key2 = originalToMultiplexedId[key];
		originalToMultiplexedId.Remove(key);
		multiplexedToOriginalId.Remove(key2);
	}

	public void OriginalId(int multiplexId, out int originalConnectionId, out int transportIndex)
	{
		KeyValuePair<int, int> keyValuePair = multiplexedToOriginalId[multiplexId];
		originalConnectionId = keyValuePair.Key;
		transportIndex = keyValuePair.Value;
	}

	public int MultiplexId(int originalConnectionId, int transportIndex)
	{
		KeyValuePair<int, int> key = new KeyValuePair<int, int>(originalConnectionId, transportIndex);
		return originalToMultiplexedId[key];
	}

	public void Awake()
	{
		if (transports == null || transports.Length == 0)
		{
			Debug.LogError("[Multiplexer] Multiplex transport requires at least 1 underlying transport");
		}
	}

	public override void ClientEarlyUpdate()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ClientEarlyUpdate();
		}
	}

	public override void ServerEarlyUpdate()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ServerEarlyUpdate();
		}
	}

	public override void ClientLateUpdate()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ClientLateUpdate();
		}
	}

	public override void ServerLateUpdate()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ServerLateUpdate();
		}
	}

	private void OnEnable()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
	}

	private void OnDisable()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	public override bool Available()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Available())
			{
				return true;
			}
		}
		return false;
	}

	public override void ClientConnect(string address)
	{
		Transport[] array = transports;
		foreach (Transport transport in array)
		{
			if (transport.Available())
			{
				available = transport;
				transport.OnClientConnected = OnClientConnected;
				transport.OnClientDataReceived = OnClientDataReceived;
				transport.OnClientError = OnClientError;
				transport.OnClientDisconnected = OnClientDisconnected;
				transport.ClientConnect(address);
				return;
			}
		}
		throw new ArgumentException("[Multiplexer] No transport suitable for this platform");
	}

	public override void ClientConnect(Uri uri)
	{
		Transport[] array = transports;
		foreach (Transport transport in array)
		{
			if (transport.Available())
			{
				try
				{
					available = transport;
					transport.OnClientConnected = OnClientConnected;
					transport.OnClientDataReceived = OnClientDataReceived;
					transport.OnClientError = OnClientError;
					transport.OnClientDisconnected = OnClientDisconnected;
					transport.ClientConnect(uri);
					return;
				}
				catch (ArgumentException)
				{
				}
			}
		}
		throw new ArgumentException("[Multiplexer] No transport suitable for this platform");
	}

	public override bool ClientConnected()
	{
		if ((object)available != null)
		{
			return available.ClientConnected();
		}
		return false;
	}

	public override void ClientDisconnect()
	{
		if ((object)available != null)
		{
			available.ClientDisconnect();
		}
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId)
	{
		available.ClientSend(segment, channelId);
	}

	private void AddServerCallbacks()
	{
		for (int i = 0; i < transports.Length; i++)
		{
			int transportIndex = i;
			Transport obj = transports[i];
			obj.OnServerConnected = delegate(int originalConnectionId)
			{
				int obj2 = AddToLookup(originalConnectionId, transportIndex);
				OnServerConnected(obj2);
			};
			obj.OnServerDataReceived = delegate(int originalConnectionId, ArraySegment<byte> data, int channel)
			{
				int arg = MultiplexId(originalConnectionId, transportIndex);
				OnServerDataReceived(arg, data, channel);
			};
			obj.OnServerError = delegate(int originalConnectionId, TransportError error, string reason)
			{
				int arg = MultiplexId(originalConnectionId, transportIndex);
				OnServerError(arg, error, reason);
			};
			obj.OnServerDisconnected = delegate(int originalConnectionId)
			{
				int obj2 = MultiplexId(originalConnectionId, transportIndex);
				OnServerDisconnected(obj2);
				RemoveFromLookup(originalConnectionId, transportIndex);
			};
		}
	}

	public override Uri ServerUri()
	{
		return transports[0].ServerUri();
	}

	public override bool ServerActive()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].ServerActive())
			{
				return false;
			}
		}
		return true;
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		OriginalId(connectionId, out var originalConnectionId, out var transportIndex);
		return transports[transportIndex].ServerGetClientAddress(originalConnectionId);
	}

	public override void ServerDisconnect(int connectionId)
	{
		OriginalId(connectionId, out var originalConnectionId, out var transportIndex);
		transports[transportIndex].ServerDisconnect(originalConnectionId);
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		OriginalId(connectionId, out var originalConnectionId, out var transportIndex);
		transports[transportIndex].ServerSend(originalConnectionId, segment, channelId);
	}

	public override void ServerStart()
	{
		AddServerCallbacks();
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ServerStart();
		}
	}

	public override void ServerStop()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ServerStop();
		}
	}

	public override int GetMaxPacketSize(int channelId = 0)
	{
		int num = int.MaxValue;
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			num = Mathf.Min(array[i].GetMaxPacketSize(channelId), num);
		}
		return num;
	}

	public override void Shutdown()
	{
		Transport[] array = transports;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Shutdown();
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		Transport[] array = transports;
		foreach (Transport transport in array)
		{
			stringBuilder.AppendLine(transport.ToString());
		}
		return stringBuilder.ToString().Trim();
	}
}
