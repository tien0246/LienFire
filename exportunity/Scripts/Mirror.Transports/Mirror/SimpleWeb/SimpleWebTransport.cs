using System;
using System.Net;
using System.Security.Authentication;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mirror.SimpleWeb;

[DisallowMultipleComponent]
public class SimpleWebTransport : Transport, PortTransport
{
	public const string NormalScheme = "ws";

	public const string SecureScheme = "wss";

	[Tooltip("Port to use for server and client")]
	public ushort port = 7778;

	[Tooltip("Protect against allocation attacks by keeping the max message size small. Otherwise an attacker might send multiple fake packets with 2GB headers, causing the server to run out of memory after allocating multiple large packets.")]
	public int maxMessageSize = 16384;

	[Tooltip("Max size for http header send as handshake for websockets")]
	public int handshakeMaxSize = 3000;

	[Tooltip("disables nagle algorithm. lowers CPU% and latency but increases bandwidth")]
	public bool noDelay = true;

	[Tooltip("Send would stall forever if the network is cut off during a send, so we need a timeout (in milliseconds)")]
	public int sendTimeout = 5000;

	[Tooltip("How long without a message before disconnecting (in milliseconds)")]
	public int receiveTimeout = 20000;

	[Tooltip("Caps the number of messages the server will process per tick. Allows LateUpdate to finish to let the reset of unity continue in case more messages arrive before they are processed")]
	public int serverMaxMessagesPerTick = 10000;

	[Tooltip("Caps the number of messages the client will process per tick. Allows LateUpdate to finish to let the reset of unity continue in case more messages arrive before they are processed")]
	public int clientMaxMessagesPerTick = 1000;

	[Header("Server settings")]
	[Tooltip("Groups messages in queue before calling Stream.Send")]
	public bool batchSend = true;

	[Tooltip("Waits for 1ms before grouping and sending messages.\nThis gives time for mirror to finish adding message to queue so that less groups need to be made.\nIf WaitBeforeSend is true then BatchSend Will also be set to true")]
	public bool waitBeforeSend = true;

	[Header("Ssl Settings")]
	[Tooltip("Sets connect scheme to wss. Useful when client needs to connect using wss when TLS is outside of transport.\nNOTE: if sslEnabled is true clientUseWss is also true")]
	public bool clientUseWss;

	[Tooltip("Requires wss connections on server, only to be used with SSL cert.json, never with reverse proxy.\nNOTE: if sslEnabled is true clientUseWss is also true")]
	public bool sslEnabled;

	[Tooltip("Path to json file that contains path to cert and its password\nUse Json file so that cert password is not included in client builds\nSee Assets/Mirror/Transports/.cert.example.Json")]
	public string sslCertJson = "./cert.json";

	[Tooltip("Protocols that SSL certificate is created to support.")]
	public SslProtocols sslProtocols = SslProtocols.Tls12;

	[Header("Debug")]
	[Tooltip("Log functions uses ConditionalAttribute which will effect which log methods are allowed. DEBUG allows warn/error, SIMPLEWEB_LOG_ENABLED allows all")]
	[FormerlySerializedAs("logLevels")]
	[SerializeField]
	private Log.Levels _logLevels = Log.Levels.info;

	private SimpleWebClient client;

	private SimpleWebServer server;

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

	public Log.Levels LogLevels
	{
		get
		{
			return _logLevels;
		}
		set
		{
			_logLevels = value;
			Log.level = _logLevels;
		}
	}

	private TcpConfig TcpConfig => new TcpConfig(noDelay, sendTimeout, receiveTimeout);

	private void Awake()
	{
		Log.level = _logLevels;
	}

	private void OnValidate()
	{
		Log.level = _logLevels;
	}

	public override bool Available()
	{
		return true;
	}

	public override int GetMaxPacketSize(int channelId = 0)
	{
		return maxMessageSize;
	}

	public override void Shutdown()
	{
		client?.Disconnect();
		client = null;
		server?.Stop();
		server = null;
	}

	private string GetClientScheme()
	{
		if (!sslEnabled && !clientUseWss)
		{
			return "ws";
		}
		return "wss";
	}

	public override bool ClientConnected()
	{
		if (client != null)
		{
			return client.ConnectionState != ClientState.NotConnected;
		}
		return false;
	}

	public override void ClientConnect(string hostname)
	{
		UriBuilder uriBuilder = new UriBuilder
		{
			Scheme = GetClientScheme(),
			Host = hostname,
			Port = port
		};
		ClientConnect(uriBuilder.Uri);
	}

	public override void ClientConnect(Uri uri)
	{
		if (ClientConnected())
		{
			Debug.LogError("[SimpleWebTransport] Already Connected");
			return;
		}
		client = SimpleWebClient.Create(maxMessageSize, clientMaxMessagesPerTick, TcpConfig);
		if (client != null)
		{
			client.onConnect += OnClientConnected.Invoke;
			client.onDisconnect += delegate
			{
				OnClientDisconnected();
				client = null;
			};
			client.onData += delegate(ArraySegment<byte> data)
			{
				OnClientDataReceived(data, 0);
			};
			client.onError += delegate(Exception e)
			{
				OnClientError(TransportError.Unexpected, e.ToString());
				ClientDisconnect();
			};
			client.Connect(uri);
		}
	}

	public override void ClientDisconnect()
	{
		client?.Disconnect();
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId)
	{
		if (!ClientConnected())
		{
			Debug.LogError("[SimpleWebTransport] Not Connected");
		}
		else if (segment.Count <= maxMessageSize && segment.Count != 0)
		{
			client.Send(segment);
			OnClientDataSent?.Invoke(segment, 0);
		}
	}

	public override void ClientEarlyUpdate()
	{
		client?.ProcessMessageQueue(this);
	}

	private string GetServerScheme()
	{
		if (!sslEnabled)
		{
			return "ws";
		}
		return "wss";
	}

	public override Uri ServerUri()
	{
		return new UriBuilder
		{
			Scheme = GetServerScheme(),
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
		if (ServerActive())
		{
			Debug.LogError("[SimpleWebTransport] Server Already Started");
		}
		SslConfig sslConfig = SslConfigLoader.Load(sslEnabled, sslCertJson, sslProtocols);
		server = new SimpleWebServer(serverMaxMessagesPerTick, TcpConfig, maxMessageSize, handshakeMaxSize, sslConfig);
		server.onConnect += OnServerConnected.Invoke;
		server.onDisconnect += OnServerDisconnected.Invoke;
		server.onData += delegate(int connId, ArraySegment<byte> data)
		{
			OnServerDataReceived(connId, data, 0);
		};
		server.onError += delegate(int connId, Exception exception)
		{
			OnServerError(connId, TransportError.Unexpected, exception.ToString());
		};
		SendLoopConfig.batchSend = batchSend || waitBeforeSend;
		SendLoopConfig.sleepBeforeSend = waitBeforeSend;
		server.Start(port);
	}

	public override void ServerStop()
	{
		if (!ServerActive())
		{
			Debug.LogError("[SimpleWebTransport] Server Not Active");
		}
		server.Stop();
		server = null;
	}

	public override void ServerDisconnect(int connectionId)
	{
		if (!ServerActive())
		{
			Debug.LogError("[SimpleWebTransport] Server Not Active");
		}
		server.KickClient(connectionId);
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		if (ServerActive() && segment.Count <= maxMessageSize && segment.Count != 0)
		{
			server.SendOne(connectionId, segment);
			OnServerDataSent?.Invoke(connectionId, segment, 0);
		}
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		return server.GetClientAddress(connectionId);
	}

	public override void ServerEarlyUpdate()
	{
		server?.ProcessMessageQueue(this);
	}
}
