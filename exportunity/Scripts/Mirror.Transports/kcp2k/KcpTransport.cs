using System;
using System.Linq;
using System.Net;
using Mirror;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace kcp2k;

[HelpURL("https://mirror-networking.gitbook.io/docs/transports/kcp-transport")]
[DisallowMultipleComponent]
public class KcpTransport : Transport, PortTransport
{
	public const string Scheme = "kcp";

	[Header("Transport Configuration")]
	[FormerlySerializedAs("Port")]
	public ushort port = 7777;

	[Tooltip("DualMode listens to IPv6 and IPv4 simultaneously. Disable if the platform only supports IPv4.")]
	public bool DualMode = true;

	[Tooltip("NoDelay is recommended to reduce latency. This also scales better without buffers getting full.")]
	public bool NoDelay = true;

	[Tooltip("KCP internal update interval. 100ms is KCP default, but a lower interval is recommended to minimize latency and to scale to more networked entities.")]
	public uint Interval = 10u;

	[Tooltip("KCP timeout in milliseconds. Note that KCP sends a ping automatically.")]
	public int Timeout = 10000;

	[Tooltip("Socket receive buffer size. Large buffer helps support more connections. Increase operating system socket buffer size limits if needed.")]
	public int RecvBufferSize = 7361536;

	[Tooltip("Socket send buffer size. Large buffer helps support more connections. Increase operating system socket buffer size limits if needed.")]
	public int SendBufferSize = 7361536;

	[Header("Advanced")]
	[Tooltip("KCP fastresend parameter. Faster resend for the cost of higher bandwidth. 0 in normal mode, 2 in turbo mode.")]
	public int FastResend = 2;

	[Tooltip("KCP congestion window. Restricts window size to reduce congestion. Results in only 2-3 MTU messages per Flush even on loopback. Best to keept his disabled.")]
	private bool CongestionWindow;

	[Tooltip("KCP window size can be modified to support higher loads. This also increases max message size.")]
	public uint ReceiveWindowSize = 4096u;

	[Tooltip("KCP window size can be modified to support higher loads.")]
	public uint SendWindowSize = 4096u;

	[Tooltip("KCP will try to retransmit lost messages up to MaxRetransmit (aka dead_link) before disconnecting.")]
	public uint MaxRetransmit = 40u;

	[Tooltip("Enable to automatically set client & server send/recv buffers to OS limit. Avoids issues with too small buffers under heavy load, potentially dropping connections. Increase the OS limit if this is still too small.")]
	[FormerlySerializedAs("MaximizeSendReceiveBuffersToOSLimit")]
	public bool MaximizeSocketBuffers = true;

	[Header("Allowed Max Message Sizes\nBased on Receive Window Size")]
	[Tooltip("KCP reliable max message size shown for convenience. Can be changed via ReceiveWindowSize.")]
	[ReadOnly]
	public int ReliableMaxMessageSize;

	[Tooltip("KCP unreliable channel max message size for convenience. Not changeable.")]
	[ReadOnly]
	public int UnreliableMaxMessageSize;

	protected KcpConfig config;

	private const int MTU = 1200;

	protected KcpServer server;

	protected KcpClient client;

	[Header("Debug")]
	public bool debugLog;

	public bool statisticsGUI;

	public bool statisticsLog;

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

	public static int FromKcpChannel(KcpChannel channel)
	{
		if (channel != KcpChannel.Reliable)
		{
			return 1;
		}
		return 0;
	}

	public static KcpChannel ToKcpChannel(int channel)
	{
		if (channel != 0)
		{
			return KcpChannel.Unreliable;
		}
		return KcpChannel.Reliable;
	}

	public static TransportError ToTransportError(ErrorCode error)
	{
		return error switch
		{
			ErrorCode.DnsResolve => TransportError.DnsResolve, 
			ErrorCode.Timeout => TransportError.Timeout, 
			ErrorCode.Congestion => TransportError.Congestion, 
			ErrorCode.InvalidReceive => TransportError.InvalidReceive, 
			ErrorCode.InvalidSend => TransportError.InvalidSend, 
			ErrorCode.ConnectionClosed => TransportError.ConnectionClosed, 
			ErrorCode.Unexpected => TransportError.Unexpected, 
			_ => throw new InvalidCastException($"KCP: missing error translation for {error}"), 
		};
	}

	protected virtual void Awake()
	{
		if (debugLog)
		{
			Log.Info = Debug.Log;
		}
		else
		{
			Log.Info = delegate
			{
			};
		}
		Log.Warning = Debug.LogWarning;
		Log.Error = Debug.LogError;
		config = new KcpConfig(DualMode, RecvBufferSize, SendBufferSize, 1200, NoDelay, Interval, FastResend, CongestionWindow, SendWindowSize, ReceiveWindowSize, Timeout, MaxRetransmit);
		client = new KcpClient(delegate
		{
			OnClientConnected();
		}, delegate(ArraySegment<byte> message, KcpChannel channel)
		{
			OnClientDataReceived(message, FromKcpChannel(channel));
		}, delegate
		{
			OnClientDisconnected();
		}, delegate(ErrorCode error, string reason)
		{
			OnClientError(ToTransportError(error), reason);
		}, config);
		server = new KcpServer(delegate(int connectionId)
		{
			OnServerConnected(connectionId);
		}, delegate(int connectionId, ArraySegment<byte> message, KcpChannel channel)
		{
			OnServerDataReceived(connectionId, message, FromKcpChannel(channel));
		}, delegate(int connectionId)
		{
			OnServerDisconnected(connectionId);
		}, delegate(int connectionId, ErrorCode error, string reason)
		{
			OnServerError(connectionId, ToTransportError(error), reason);
		}, config);
		if (statisticsLog)
		{
			InvokeRepeating("OnLogStatistics", 1f, 1f);
		}
		Debug.Log("KcpTransport initialized!");
	}

	protected virtual void OnValidate()
	{
		ReliableMaxMessageSize = KcpPeer.ReliableMaxMessageSize(1200, ReceiveWindowSize);
		UnreliableMaxMessageSize = KcpPeer.UnreliableMaxMessageSize(1200);
	}

	public override bool Available()
	{
		return Application.platform != RuntimePlatform.WebGLPlayer;
	}

	public override bool ClientConnected()
	{
		return client.connected;
	}

	public override void ClientConnect(string address)
	{
		client.Connect(address, Port);
	}

	public override void ClientConnect(Uri uri)
	{
		if (uri.Scheme != "kcp")
		{
			throw new ArgumentException(string.Format("Invalid url {0}, use {1}://host:port instead", uri, "kcp"), "uri");
		}
		int num = (uri.IsDefaultPort ? Port : uri.Port);
		client.Connect(uri.Host, (ushort)num);
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId)
	{
		client.Send(segment, ToKcpChannel(channelId));
		OnClientDataSent?.Invoke(segment, channelId);
	}

	public override void ClientDisconnect()
	{
		client.Disconnect();
	}

	public override void ClientEarlyUpdate()
	{
		if (base.enabled)
		{
			client.TickIncoming();
		}
	}

	public override void ClientLateUpdate()
	{
		client.TickOutgoing();
	}

	public override Uri ServerUri()
	{
		return new UriBuilder
		{
			Scheme = "kcp",
			Host = Dns.GetHostName(),
			Port = Port
		}.Uri;
	}

	public override bool ServerActive()
	{
		return server.IsActive();
	}

	public override void ServerStart()
	{
		server.Start(Port);
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		server.Send(connectionId, segment, ToKcpChannel(channelId));
		OnServerDataSent?.Invoke(connectionId, segment, channelId);
	}

	public override void ServerDisconnect(int connectionId)
	{
		server.Disconnect(connectionId);
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		IPEndPoint clientEndPoint = server.GetClientEndPoint(connectionId);
		if (clientEndPoint == null)
		{
			return "";
		}
		return clientEndPoint.Address.ToString();
	}

	public override void ServerStop()
	{
		server.Stop();
	}

	public override void ServerEarlyUpdate()
	{
		if (base.enabled)
		{
			server.TickIncoming();
		}
	}

	public override void ServerLateUpdate()
	{
		server.TickOutgoing();
	}

	public override void Shutdown()
	{
	}

	public override int GetMaxPacketSize(int channelId = 0)
	{
		if (channelId == 1)
		{
			return KcpPeer.UnreliableMaxMessageSize(config.Mtu);
		}
		return KcpPeer.ReliableMaxMessageSize(config.Mtu, ReceiveWindowSize);
	}

	public override int GetBatchThreshold(int channelId)
	{
		return KcpPeer.UnreliableMaxMessageSize(config.Mtu);
	}

	public long GetAverageMaxSendRate()
	{
		if (server.connections.Count <= 0)
		{
			return 0L;
		}
		return server.connections.Values.Sum((KcpServerConnection conn) => conn.peer.MaxSendRate) / server.connections.Count;
	}

	public long GetAverageMaxReceiveRate()
	{
		if (server.connections.Count <= 0)
		{
			return 0L;
		}
		return server.connections.Values.Sum((KcpServerConnection conn) => conn.peer.MaxReceiveRate) / server.connections.Count;
	}

	private long GetTotalSendQueue()
	{
		return server.connections.Values.Sum((KcpServerConnection conn) => conn.peer.SendQueueCount);
	}

	private long GetTotalReceiveQueue()
	{
		return server.connections.Values.Sum((KcpServerConnection conn) => conn.peer.ReceiveQueueCount);
	}

	private long GetTotalSendBuffer()
	{
		return server.connections.Values.Sum((KcpServerConnection conn) => conn.peer.SendBufferCount);
	}

	private long GetTotalReceiveBuffer()
	{
		return server.connections.Values.Sum((KcpServerConnection conn) => conn.peer.ReceiveBufferCount);
	}

	public static string PrettyBytes(long bytes)
	{
		if (bytes < 1024)
		{
			return $"{bytes} B";
		}
		if (bytes < 1048576)
		{
			return $"{(float)bytes / 1024f:F2} KB";
		}
		if (bytes < 1073741824)
		{
			return $"{(float)bytes / 1048576f:F2} MB";
		}
		return $"{(float)bytes / 1.0737418E+09f:F2} GB";
	}

	protected virtual void OnGUIStatistics()
	{
		GUILayout.BeginArea(new Rect(5f, 110f, 300f, 300f));
		if (ServerActive())
		{
			GUILayout.BeginVertical("Box");
			GUILayout.Label("SERVER");
			GUILayout.Label($"  connections: {server.connections.Count}");
			GUILayout.Label("  MaxSendRate (avg): " + PrettyBytes(GetAverageMaxSendRate()) + "/s");
			GUILayout.Label("  MaxRecvRate (avg): " + PrettyBytes(GetAverageMaxReceiveRate()) + "/s");
			GUILayout.Label($"  SendQueue: {GetTotalSendQueue()}");
			GUILayout.Label($"  ReceiveQueue: {GetTotalReceiveQueue()}");
			GUILayout.Label($"  SendBuffer: {GetTotalSendBuffer()}");
			GUILayout.Label($"  ReceiveBuffer: {GetTotalReceiveBuffer()}");
			GUILayout.EndVertical();
		}
		if (ClientConnected())
		{
			GUILayout.BeginVertical("Box");
			GUILayout.Label("CLIENT");
			GUILayout.Label("  MaxSendRate: " + PrettyBytes(client.peer.MaxSendRate) + "/s");
			GUILayout.Label("  MaxRecvRate: " + PrettyBytes(client.peer.MaxReceiveRate) + "/s");
			GUILayout.Label($"  SendQueue: {client.peer.SendQueueCount}");
			GUILayout.Label($"  ReceiveQueue: {client.peer.ReceiveQueueCount}");
			GUILayout.Label($"  SendBuffer: {client.peer.SendBufferCount}");
			GUILayout.Label($"  ReceiveBuffer: {client.peer.ReceiveBufferCount}");
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
	}

	protected virtual void OnLogStatistics()
	{
		if (ServerActive())
		{
			Debug.Log(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("kcp SERVER @ time: " + NetworkTime.localTime + "\n", $"  connections: {server.connections.Count}\n"), "  MaxSendRate (avg): ", PrettyBytes(GetAverageMaxSendRate()), "/s\n"), "  MaxRecvRate (avg): ", PrettyBytes(GetAverageMaxReceiveRate()), "/s\n"), $"  SendQueue: {GetTotalSendQueue()}\n"), $"  ReceiveQueue: {GetTotalReceiveQueue()}\n"), $"  SendBuffer: {GetTotalSendBuffer()}\n"), $"  ReceiveBuffer: {GetTotalReceiveBuffer()}\n\n"));
		}
		if (ClientConnected())
		{
			Debug.Log(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("kcp CLIENT @ time: " + NetworkTime.localTime + "\n", "  MaxSendRate: ", PrettyBytes(client.peer.MaxSendRate), "/s\n"), "  MaxRecvRate: ", PrettyBytes(client.peer.MaxReceiveRate), "/s\n"), $"  SendQueue: {client.peer.SendQueueCount}\n"), $"  ReceiveQueue: {client.peer.ReceiveQueueCount}\n"), $"  SendBuffer: {client.peer.SendBufferCount}\n"), $"  ReceiveBuffer: {client.peer.ReceiveBufferCount}\n\n"));
		}
	}

	public override string ToString()
	{
		return "KCP";
	}
}
