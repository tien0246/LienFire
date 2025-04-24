using System;
using System.Net;
using System.Net.Sockets;

namespace kcp2k;

public class KcpClient
{
	public KcpPeer peer;

	protected Socket socket;

	public EndPoint remoteEndPoint;

	protected readonly KcpConfig config;

	protected readonly byte[] rawReceiveBuffer;

	protected readonly Action OnConnected;

	protected readonly Action<ArraySegment<byte>, KcpChannel> OnData;

	protected readonly Action OnDisconnected;

	protected readonly Action<ErrorCode, string> OnError;

	public bool connected;

	public KcpClient(Action OnConnected, Action<ArraySegment<byte>, KcpChannel> OnData, Action OnDisconnected, Action<ErrorCode, string> OnError, KcpConfig config)
	{
		this.OnConnected = OnConnected;
		this.OnData = OnData;
		this.OnDisconnected = OnDisconnected;
		this.OnError = OnError;
		this.config = config;
		rawReceiveBuffer = new byte[config.Mtu];
	}

	public void Connect(string address, ushort port)
	{
		if (connected)
		{
			Log.Warning("KcpClient: already connected!");
			return;
		}
		if (!Common.ResolveHostname(address, out var addresses))
		{
			OnError(ErrorCode.DnsResolve, "Failed to resolve host: " + address);
			OnDisconnected();
			return;
		}
		peer = new KcpPeer(RawSend, OnAuthenticatedWrap, OnData, OnDisconnectedWrap, OnError, config, 0u);
		Log.Info($"KcpClient: connect to {address}:{port}");
		remoteEndPoint = new IPEndPoint(addresses[0], port);
		socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
		socket.Blocking = false;
		Common.ConfigureSocketBuffers(socket, config.RecvBufferSize, config.SendBufferSize);
		socket.Connect(remoteEndPoint);
		peer.SendHandshake();
		void OnAuthenticatedWrap()
		{
			Log.Info("KcpClient: OnConnected");
			connected = true;
			OnConnected();
		}
		void OnDisconnectedWrap()
		{
			Log.Info("KcpClient: OnDisconnected");
			connected = false;
			peer = null;
			socket?.Close();
			socket = null;
			remoteEndPoint = null;
			OnDisconnected();
		}
	}

	protected virtual bool RawReceive(out ArraySegment<byte> segment)
	{
		segment = default(ArraySegment<byte>);
		if (socket == null)
		{
			return false;
		}
		try
		{
			return socket.ReceiveNonBlocking(rawReceiveBuffer, out segment);
		}
		catch (SocketException arg)
		{
			Log.Info($"KcpClient: looks like the other end has closed the connection. This is fine: {arg}");
			peer.Disconnect();
			return false;
		}
	}

	protected virtual void RawSend(ArraySegment<byte> data)
	{
		try
		{
			socket.SendNonBlocking(data);
		}
		catch (SocketException arg)
		{
			Log.Error($"KcpClient: Send failed: {arg}");
		}
	}

	public void Send(ArraySegment<byte> segment, KcpChannel channel)
	{
		if (!connected)
		{
			Log.Warning("KcpClient: can't send because not connected!");
		}
		else
		{
			peer.SendData(segment, channel);
		}
	}

	public void Disconnect()
	{
		if (connected)
		{
			peer?.Disconnect();
		}
	}

	public virtual void TickIncoming()
	{
		if (peer != null)
		{
			ArraySegment<byte> segment;
			while (RawReceive(out segment))
			{
				peer.RawInput(segment);
			}
		}
		peer?.TickIncoming();
	}

	public virtual void TickOutgoing()
	{
		peer?.TickOutgoing();
	}

	public virtual void Tick()
	{
		TickIncoming();
		TickOutgoing();
	}
}
