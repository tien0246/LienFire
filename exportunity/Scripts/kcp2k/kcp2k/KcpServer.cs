using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace kcp2k;

public class KcpServer
{
	protected readonly Action<int> OnConnected;

	protected readonly Action<int, ArraySegment<byte>, KcpChannel> OnData;

	protected readonly Action<int> OnDisconnected;

	protected readonly Action<int, ErrorCode, string> OnError;

	protected readonly KcpConfig config;

	protected Socket socket;

	private EndPoint newClientEP;

	protected readonly byte[] rawReceiveBuffer;

	public Dictionary<int, KcpServerConnection> connections = new Dictionary<int, KcpServerConnection>();

	private readonly HashSet<int> connectionsToRemove = new HashSet<int>();

	public KcpServer(Action<int> OnConnected, Action<int, ArraySegment<byte>, KcpChannel> OnData, Action<int> OnDisconnected, Action<int, ErrorCode, string> OnError, KcpConfig config)
	{
		this.OnConnected = OnConnected;
		this.OnData = OnData;
		this.OnDisconnected = OnDisconnected;
		this.OnError = OnError;
		this.config = config;
		rawReceiveBuffer = new byte[config.Mtu];
		newClientEP = (config.DualMode ? new IPEndPoint(IPAddress.IPv6Any, 0) : new IPEndPoint(IPAddress.Any, 0));
	}

	public virtual bool IsActive()
	{
		return socket != null;
	}

	private static Socket CreateServerSocket(bool DualMode, ushort port)
	{
		if (DualMode)
		{
			Socket socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
			try
			{
				socket.DualMode = true;
			}
			catch (NotSupportedException arg)
			{
				Log.Warning($"Failed to set Dual Mode, continuing with IPv6 without Dual Mode. Error: {arg}");
			}
			socket.Bind(new IPEndPoint(IPAddress.IPv6Any, port));
			return socket;
		}
		Socket obj = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		obj.Bind(new IPEndPoint(IPAddress.Any, port));
		return obj;
	}

	public virtual void Start(ushort port)
	{
		if (socket != null)
		{
			Log.Warning("KcpServer: already started!");
			return;
		}
		socket = CreateServerSocket(config.DualMode, port);
		socket.Blocking = false;
		Common.ConfigureSocketBuffers(socket, config.RecvBufferSize, config.SendBufferSize);
	}

	public void Send(int connectionId, ArraySegment<byte> segment, KcpChannel channel)
	{
		if (connections.TryGetValue(connectionId, out var value))
		{
			value.peer.SendData(segment, channel);
		}
	}

	public void Disconnect(int connectionId)
	{
		if (connections.TryGetValue(connectionId, out var value))
		{
			value.peer.Disconnect();
		}
	}

	public IPEndPoint GetClientEndPoint(int connectionId)
	{
		if (connections.TryGetValue(connectionId, out var value))
		{
			return value.remoteEndPoint as IPEndPoint;
		}
		return null;
	}

	protected virtual bool RawReceiveFrom(out ArraySegment<byte> segment, out int connectionId)
	{
		segment = default(ArraySegment<byte>);
		connectionId = 0;
		if (socket == null)
		{
			return false;
		}
		try
		{
			if (socket.ReceiveFromNonBlocking(rawReceiveBuffer, out segment, ref newClientEP))
			{
				connectionId = Common.ConnectionHash(newClientEP);
				return true;
			}
		}
		catch (SocketException arg)
		{
			Log.Info($"KcpServer: ReceiveFrom failed: {arg}");
		}
		return false;
	}

	protected virtual void RawSend(int connectionId, ArraySegment<byte> data)
	{
		if (!connections.TryGetValue(connectionId, out var value))
		{
			Debug.LogWarning($"KcpServer: RawSend invalid connectionId={connectionId}");
			return;
		}
		try
		{
			socket.SendToNonBlocking(data, value.remoteEndPoint);
		}
		catch (SocketException arg)
		{
			Log.Error($"KcpServer: SendTo failed: {arg}");
		}
	}

	protected virtual KcpServerConnection CreateConnection(int connectionId)
	{
		Action<ArraySegment<byte>> output = delegate(ArraySegment<byte> data)
		{
			RawSend(connectionId, data);
		};
		KcpServerConnection connection = new KcpServerConnection(newClientEP);
		KcpPeer peer = new KcpPeer(cookie: Common.GenerateCookie(), output: output, OnAuthenticated: OnAuthenticatedWrap, OnData: OnDataWrap, OnDisconnected: OnDisconnectedWrap, OnError: OnErrorWrap, config: config);
		connection.peer = peer;
		return connection;
		void OnAuthenticatedWrap()
		{
			connection.peer.SendHandshake();
			connections.Add(connectionId, connection);
			Log.Info($"KcpServer: added connection({connectionId})");
			Log.Info($"KcpServer: OnConnected({connectionId})");
			OnConnected(connectionId);
		}
		void OnDataWrap(ArraySegment<byte> message, KcpChannel channel)
		{
			OnData(connectionId, message, channel);
		}
		void OnDisconnectedWrap()
		{
			connectionsToRemove.Add(connectionId);
			Log.Info($"KcpServer: OnDisconnected({connectionId})");
			OnDisconnected(connectionId);
		}
		void OnErrorWrap(ErrorCode error, string reason)
		{
			OnError(connectionId, error, reason);
		}
	}

	private void ProcessMessage(ArraySegment<byte> segment, int connectionId)
	{
		if (!connections.TryGetValue(connectionId, out var value))
		{
			value = CreateConnection(connectionId);
			value.peer.RawInput(segment);
			value.peer.TickIncoming();
		}
		else
		{
			value.peer.RawInput(segment);
		}
	}

	public virtual void TickIncoming()
	{
		ArraySegment<byte> segment;
		int connectionId;
		while (RawReceiveFrom(out segment, out connectionId))
		{
			ProcessMessage(segment, connectionId);
		}
		foreach (KcpServerConnection value in connections.Values)
		{
			value.peer.TickIncoming();
		}
		foreach (int item in connectionsToRemove)
		{
			connections.Remove(item);
		}
		connectionsToRemove.Clear();
	}

	public virtual void TickOutgoing()
	{
		foreach (KcpServerConnection value in connections.Values)
		{
			value.peer.TickOutgoing();
		}
	}

	public virtual void Tick()
	{
		TickIncoming();
		TickOutgoing();
	}

	public virtual void Stop()
	{
		socket?.Close();
		socket = null;
	}
}
