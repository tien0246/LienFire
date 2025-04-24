using System;
using System.Net;
using System.Net.Sockets;
using Telepathy;
using UnityEngine;

namespace Mirror;

[HelpURL("https://github.com/vis2k/Telepathy/blob/master/README.md")]
[DisallowMultipleComponent]
public class TelepathyTransport : Transport, PortTransport
{
	public const string Scheme = "tcp4";

	public ushort port = 7777;

	[Header("Common")]
	[Tooltip("Nagle Algorithm can be disabled by enabling NoDelay")]
	public bool NoDelay = true;

	[Tooltip("Send timeout in milliseconds.")]
	public int SendTimeout = 5000;

	[Tooltip("Receive timeout in milliseconds. High by default so users don't time out during scene changes.")]
	public int ReceiveTimeout = 30000;

	[Header("Server")]
	[Tooltip("Protect against allocation attacks by keeping the max message size small. Otherwise an attacker might send multiple fake packets with 2GB headers, causing the server to run out of memory after allocating multiple large packets.")]
	public int serverMaxMessageSize = 16384;

	[Tooltip("Server processes a limit amount of messages per tick to avoid a deadlock where it might end up processing forever if messages come in faster than we can process them.")]
	public int serverMaxReceivesPerTick = 10000;

	[Tooltip("Server send queue limit per connection for pending messages. Telepathy will disconnect a connection's queues reach that limit for load balancing. Better to kick one slow client than slowing down the whole server.")]
	public int serverSendQueueLimitPerConnection = 10000;

	[Tooltip("Server receive queue limit per connection for pending messages. Telepathy will disconnect a connection's queues reach that limit for load balancing. Better to kick one slow client than slowing down the whole server.")]
	public int serverReceiveQueueLimitPerConnection = 10000;

	[Header("Client")]
	[Tooltip("Protect against allocation attacks by keeping the max message size small. Otherwise an attacker host might send multiple fake packets with 2GB headers, causing the connected clients to run out of memory after allocating multiple large packets.")]
	public int clientMaxMessageSize = 16384;

	[Tooltip("Client processes a limit amount of messages per tick to avoid a deadlock where it might end up processing forever if messages come in faster than we can process them.")]
	public int clientMaxReceivesPerTick = 1000;

	[Tooltip("Client send queue limit for pending messages. Telepathy will disconnect if the connection's queues reach that limit in order to avoid ever growing latencies.")]
	public int clientSendQueueLimit = 10000;

	[Tooltip("Client receive queue limit for pending messages. Telepathy will disconnect if the connection's queues reach that limit in order to avoid ever growing latencies.")]
	public int clientReceiveQueueLimit = 10000;

	private Client client;

	private Server server;

	private Func<bool> enabledCheck;

	public ushort Port
	{
		get
		{
			return port;
		}
		set
		{
			port = value;
		}
	}

	private void Awake()
	{
		Log.Info = Debug.Log;
		Log.Warning = Debug.LogWarning;
		Log.Error = Debug.LogError;
		enabledCheck = () => base.enabled;
		Debug.Log("TelepathyTransport initialized!");
	}

	public override bool Available()
	{
		return Application.platform != RuntimePlatform.WebGLPlayer;
	}

	private void CreateClient()
	{
		client = new Client(clientMaxMessageSize);
		client.OnConnected = delegate
		{
			OnClientConnected();
		};
		client.OnData = delegate(ArraySegment<byte> segment)
		{
			OnClientDataReceived(segment, 0);
		};
		client.OnDisconnected = delegate
		{
			OnClientDisconnected?.Invoke();
		};
		client.NoDelay = NoDelay;
		client.SendTimeout = SendTimeout;
		client.ReceiveTimeout = ReceiveTimeout;
		client.SendQueueLimit = clientSendQueueLimit;
		client.ReceiveQueueLimit = clientReceiveQueueLimit;
	}

	public override bool ClientConnected()
	{
		if (client != null)
		{
			return client.Connected;
		}
		return false;
	}

	public override void ClientConnect(string address)
	{
		CreateClient();
		client.Connect(address, port);
	}

	public override void ClientConnect(Uri uri)
	{
		CreateClient();
		if (uri.Scheme != "tcp4")
		{
			throw new ArgumentException(string.Format("Invalid url {0}, use {1}://host:port instead", uri, "tcp4"), "uri");
		}
		int num = (uri.IsDefaultPort ? port : uri.Port);
		client.Connect(uri.Host, num);
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId)
	{
		client?.Send(segment);
		OnClientDataSent?.Invoke(segment, 0);
	}

	public override void ClientDisconnect()
	{
		client?.Disconnect();
		client = null;
		OnClientDisconnected?.Invoke();
	}

	public override void ClientEarlyUpdate()
	{
		if (base.enabled)
		{
			client?.Tick(clientMaxReceivesPerTick, enabledCheck);
		}
	}

	public override Uri ServerUri()
	{
		return new UriBuilder
		{
			Scheme = "tcp4",
			Host = Dns.GetHostName(),
			Port = port
		}.Uri;
	}

	public override bool ServerActive()
	{
		if (server != null)
		{
			return server.Active;
		}
		return false;
	}

	public override void ServerStart()
	{
		server = new Server(serverMaxMessageSize);
		server.OnConnected = delegate(int connectionId)
		{
			OnServerConnected(connectionId);
		};
		server.OnData = delegate(int connectionId, ArraySegment<byte> segment)
		{
			OnServerDataReceived(connectionId, segment, 0);
		};
		server.OnDisconnected = delegate(int connectionId)
		{
			OnServerDisconnected(connectionId);
		};
		server.NoDelay = NoDelay;
		server.SendTimeout = SendTimeout;
		server.ReceiveTimeout = ReceiveTimeout;
		server.SendQueueLimit = serverSendQueueLimitPerConnection;
		server.ReceiveQueueLimit = serverReceiveQueueLimitPerConnection;
		server.Start(port);
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		server?.Send(connectionId, segment);
		OnServerDataSent?.Invoke(connectionId, segment, 0);
	}

	public override void ServerDisconnect(int connectionId)
	{
		server?.Disconnect(connectionId);
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		try
		{
			return server?.GetClientAddress(connectionId);
		}
		catch (SocketException)
		{
			return "unknown";
		}
	}

	public override void ServerStop()
	{
		server?.Stop();
		server = null;
	}

	public override void ServerEarlyUpdate()
	{
		if (base.enabled)
		{
			server?.Tick(serverMaxReceivesPerTick, enabledCheck);
		}
	}

	public override void Shutdown()
	{
		Debug.Log("TelepathyTransport Shutdown()");
		client?.Disconnect();
		client = null;
		server?.Stop();
		server = null;
	}

	public override int GetMaxPacketSize(int channelId)
	{
		return serverMaxMessageSize;
	}

	public override string ToString()
	{
		if (server != null && server.Active && server.listener != null)
		{
			return $"Telepathy Server port: {port}";
		}
		if (client != null && (client.Connecting || client.Connected))
		{
			return $"Telepathy Client port: {port}";
		}
		return "Telepathy (inactive/disconnected)";
	}
}
