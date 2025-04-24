using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace kcp2k;

public class KcpPeer
{
	internal Kcp kcp;

	private readonly uint cookie;

	internal readonly byte[] receivedCookie = new byte[4];

	private readonly Action<ArraySegment<byte>> RawSend;

	private KcpState state;

	private readonly Action OnAuthenticated;

	private readonly Action<ArraySegment<byte>, KcpChannel> OnData;

	private readonly Action OnDisconnected;

	private readonly Action<ErrorCode, string> OnError;

	public const int DEFAULT_TIMEOUT = 10000;

	public int timeout;

	private uint lastReceiveTime;

	private readonly Stopwatch watch = new Stopwatch();

	private const int CHANNEL_HEADER_SIZE = 1;

	private const int COOKIE_HEADER_SIZE = 4;

	private const int METADATA_SIZE = 5;

	private readonly byte[] kcpMessageBuffer;

	private readonly byte[] kcpSendBuffer;

	private readonly byte[] rawSendBuffer;

	public const int PING_INTERVAL = 1000;

	private uint lastPingTime;

	internal const int QueueDisconnectThreshold = 10000;

	public readonly int unreliableMax;

	public readonly int reliableMax;

	public int SendQueueCount => kcp.snd_queue.Count;

	public int ReceiveQueueCount => kcp.rcv_queue.Count;

	public int SendBufferCount => kcp.snd_buf.Count;

	public int ReceiveBufferCount => kcp.rcv_buf.Count;

	public uint MaxSendRate => kcp.snd_wnd * kcp.mtu * 1000 / kcp.interval;

	public uint MaxReceiveRate => kcp.rcv_wnd * kcp.mtu * 1000 / kcp.interval;

	private static int ReliableMaxMessageSize_Unconstrained(int mtu, uint rcv_wnd)
	{
		return (mtu - 24 - 5) * (int)(rcv_wnd - 1) - 1;
	}

	public static int ReliableMaxMessageSize(int mtu, uint rcv_wnd)
	{
		return ReliableMaxMessageSize_Unconstrained(mtu, Math.Min(rcv_wnd, 255u));
	}

	public static int UnreliableMaxMessageSize(int mtu)
	{
		return mtu - 5;
	}

	public KcpPeer(Action<ArraySegment<byte>> output, Action OnAuthenticated, Action<ArraySegment<byte>, KcpChannel> OnData, Action OnDisconnected, Action<ErrorCode, string> OnError, KcpConfig config, uint cookie)
	{
		this.OnAuthenticated = OnAuthenticated;
		this.OnData = OnData;
		this.OnDisconnected = OnDisconnected;
		this.OnError = OnError;
		RawSend = output;
		kcp = new Kcp(0u, RawSendReliable);
		this.cookie = cookie;
		kcp.SetNoDelay(config.NoDelay ? 1u : 0u, config.Interval, config.FastResend, !config.CongestionWindow);
		kcp.SetWindowSize(config.SendWindowSize, config.ReceiveWindowSize);
		kcp.SetMtu((uint)(config.Mtu - 5));
		rawSendBuffer = new byte[config.Mtu];
		unreliableMax = UnreliableMaxMessageSize(config.Mtu);
		reliableMax = ReliableMaxMessageSize(config.Mtu, config.ReceiveWindowSize);
		kcp.dead_link = config.MaxRetransmits;
		kcpMessageBuffer = new byte[1 + reliableMax];
		kcpSendBuffer = new byte[1 + reliableMax];
		timeout = config.Timeout;
		watch.Start();
	}

	private void HandleTimeout(uint time)
	{
		if (time >= lastReceiveTime + timeout)
		{
			OnError(ErrorCode.Timeout, $"KcpPeer: Connection timed out after not receiving any message for {timeout}ms. Disconnecting.");
			Disconnect();
		}
	}

	private void HandleDeadLink()
	{
		if (kcp.state == -1)
		{
			OnError(ErrorCode.Timeout, $"KcpPeer: dead_link detected: a message was retransmitted {kcp.dead_link} times without ack. Disconnecting.");
			Disconnect();
		}
	}

	private void HandlePing(uint time)
	{
		if (time >= lastPingTime + 1000)
		{
			SendPing();
			lastPingTime = time;
		}
	}

	private void HandleChoked()
	{
		int num = kcp.rcv_queue.Count + kcp.snd_queue.Count + kcp.rcv_buf.Count + kcp.snd_buf.Count;
		if (num >= 10000)
		{
			OnError(ErrorCode.Congestion, "KcpPeer: disconnecting connection because it can't process data fast enough.\n" + $"Queue total {num}>{10000}. rcv_queue={kcp.rcv_queue.Count} snd_queue={kcp.snd_queue.Count} rcv_buf={kcp.rcv_buf.Count} snd_buf={kcp.snd_buf.Count}\n" + "* Try to Enable NoDelay, decrease INTERVAL, disable Congestion Window (= enable NOCWND!), increase SEND/RECV WINDOW or compress data.\n* Or perhaps the network is simply too slow on our end, or on the other end.");
			kcp.snd_queue.Clear();
			Disconnect();
		}
	}

	private bool ReceiveNextReliable(out KcpHeader header, out ArraySegment<byte> message)
	{
		message = default(ArraySegment<byte>);
		header = KcpHeader.Disconnect;
		int num = kcp.PeekSize();
		if (num <= 0)
		{
			return false;
		}
		if (num > kcpMessageBuffer.Length)
		{
			OnError(ErrorCode.InvalidReceive, $"KcpPeer: possible allocation attack for msgSize {num} > buffer {kcpMessageBuffer.Length}. Disconnecting the connection.");
			Disconnect();
			return false;
		}
		int num2 = kcp.Receive(kcpMessageBuffer, num);
		if (num2 < 0)
		{
			OnError(ErrorCode.InvalidReceive, $"KcpPeer: Receive failed with error={num2}. closing connection.");
			Disconnect();
			return false;
		}
		header = (KcpHeader)kcpMessageBuffer[0];
		message = new ArraySegment<byte>(kcpMessageBuffer, 1, num - 1);
		lastReceiveTime = (uint)watch.ElapsedMilliseconds;
		return true;
	}

	private void TickIncoming_Connected(uint time)
	{
		HandleTimeout(time);
		HandleDeadLink();
		HandlePing(time);
		HandleChoked();
		if (!ReceiveNextReliable(out var header, out var message))
		{
			return;
		}
		switch (header)
		{
		case KcpHeader.Handshake:
		{
			if (message.Count != 4)
			{
				OnError(ErrorCode.InvalidReceive, $"KcpPeer: received invalid handshake message with size {message.Count} != 4. Disconnecting the connection.");
				Disconnect();
				break;
			}
			Buffer.BlockCopy(message.Array, message.Offset, receivedCookie, 0, 4);
			uint num = BitConverter.ToUInt32(message.Array, message.Offset);
			Log.Info($"KcpPeer: received handshake with cookie={num}");
			state = KcpState.Authenticated;
			OnAuthenticated?.Invoke();
			break;
		}
		case KcpHeader.Data:
		case KcpHeader.Disconnect:
			OnError(ErrorCode.InvalidReceive, $"KcpPeer: received invalid header {header} while Connected. Disconnecting the connection.");
			Disconnect();
			break;
		case KcpHeader.Ping:
			break;
		}
	}

	private void TickIncoming_Authenticated(uint time)
	{
		HandleTimeout(time);
		HandleDeadLink();
		HandlePing(time);
		HandleChoked();
		KcpHeader header;
		ArraySegment<byte> message;
		while (ReceiveNextReliable(out header, out message))
		{
			switch (header)
			{
			case KcpHeader.Handshake:
				Log.Warning($"KcpPeer: received invalid header {header} while Authenticated. Disconnecting the connection.");
				Disconnect();
				break;
			case KcpHeader.Data:
				if (message.Count > 0)
				{
					OnData?.Invoke(message, KcpChannel.Reliable);
					break;
				}
				OnError(ErrorCode.InvalidReceive, "KcpPeer: received empty Data message while Authenticated. Disconnecting the connection.");
				Disconnect();
				break;
			case KcpHeader.Disconnect:
				Log.Info("KcpPeer: received disconnect message");
				Disconnect();
				break;
			}
		}
	}

	public void TickIncoming()
	{
		uint time = (uint)watch.ElapsedMilliseconds;
		try
		{
			switch (state)
			{
			case KcpState.Connected:
				TickIncoming_Connected(time);
				break;
			case KcpState.Authenticated:
				TickIncoming_Authenticated(time);
				break;
			case KcpState.Disconnected:
				break;
			}
		}
		catch (SocketException arg)
		{
			OnError(ErrorCode.ConnectionClosed, $"KcpPeer: Disconnecting because {arg}. This is fine.");
			Disconnect();
		}
		catch (ObjectDisposedException arg2)
		{
			OnError(ErrorCode.ConnectionClosed, $"KcpPeer: Disconnecting because {arg2}. This is fine.");
			Disconnect();
		}
		catch (Exception arg3)
		{
			OnError(ErrorCode.Unexpected, $"KcpPeer: unexpected Exception: {arg3}");
			Disconnect();
		}
	}

	public void TickOutgoing()
	{
		uint currentTimeMilliSeconds = (uint)watch.ElapsedMilliseconds;
		try
		{
			switch (state)
			{
			case KcpState.Connected:
			case KcpState.Authenticated:
				kcp.Update(currentTimeMilliSeconds);
				break;
			}
		}
		catch (SocketException arg)
		{
			OnError(ErrorCode.ConnectionClosed, $"KcpPeer: Disconnecting because {arg}. This is fine.");
			Disconnect();
		}
		catch (ObjectDisposedException arg2)
		{
			OnError(ErrorCode.ConnectionClosed, $"KcpPeer: Disconnecting because {arg2}. This is fine.");
			Disconnect();
		}
		catch (Exception arg3)
		{
			OnError(ErrorCode.Unexpected, $"KcpPeer: unexpected exception: {arg3}");
			Disconnect();
		}
	}

	private void OnRawInputReliable(ArraySegment<byte> message)
	{
		int num = kcp.Input(message.Array, message.Offset, message.Count);
		if (num != 0)
		{
			Log.Warning($"KcpPeer: Input failed with error={num} for buffer with length={message.Count - 1}");
		}
	}

	private void OnRawInputUnreliable(ArraySegment<byte> message)
	{
		if (state == KcpState.Authenticated)
		{
			OnData?.Invoke(message, KcpChannel.Unreliable);
			lastReceiveTime = (uint)watch.ElapsedMilliseconds;
		}
		else
		{
			Log.Warning("KcpPeer: received unreliable message while not authenticated.");
		}
	}

	public void RawInput(ArraySegment<byte> segment)
	{
		if (segment.Count <= 0)
		{
			return;
		}
		byte b = segment.Array[segment.Offset];
		uint num = BitConverter.ToUInt32(segment.Array, segment.Offset + 1);
		if (state == KcpState.Authenticated && num != cookie)
		{
			Log.Warning($"KcpPeer: dropped message with invalid cookie: {num} expected: {cookie}.");
			return;
		}
		ArraySegment<byte> message = new ArraySegment<byte>(segment.Array, segment.Offset + 1 + 4, segment.Count - 1 - 4);
		switch (b)
		{
		case 1:
			OnRawInputReliable(message);
			break;
		case 2:
			OnRawInputUnreliable(message);
			break;
		default:
			Log.Warning($"KcpPeer: invalid channel header: {b}, likely internet noise");
			break;
		}
	}

	private void RawSendReliable(byte[] data, int length)
	{
		rawSendBuffer[0] = 1;
		Buffer.BlockCopy(receivedCookie, 0, rawSendBuffer, 1, 4);
		Buffer.BlockCopy(data, 0, rawSendBuffer, 5, length);
		ArraySegment<byte> obj = new ArraySegment<byte>(rawSendBuffer, 0, length + 1 + 4);
		RawSend(obj);
	}

	private void SendReliable(KcpHeader header, ArraySegment<byte> content)
	{
		if (1 + content.Count > kcpSendBuffer.Length)
		{
			OnError(ErrorCode.InvalidSend, $"KcpPeer: Failed to send reliable message of size {content.Count} because it's larger than ReliableMaxMessageSize={reliableMax}");
			return;
		}
		kcpSendBuffer[0] = (byte)header;
		if (content.Count > 0)
		{
			Buffer.BlockCopy(content.Array, content.Offset, kcpSendBuffer, 1, content.Count);
		}
		int num = kcp.Send(kcpSendBuffer, 0, 1 + content.Count);
		if (num < 0)
		{
			OnError(ErrorCode.InvalidSend, $"KcpPeer: Send failed with error={num} for content with length={content.Count}");
		}
	}

	private void SendUnreliable(ArraySegment<byte> message)
	{
		if (message.Count > unreliableMax)
		{
			Log.Error($"KcpPeer: Failed to send unreliable message of size {message.Count} because it's larger than UnreliableMaxMessageSize={unreliableMax}");
			return;
		}
		rawSendBuffer[0] = 2;
		Buffer.BlockCopy(receivedCookie, 0, rawSendBuffer, 1, 4);
		Buffer.BlockCopy(message.Array, message.Offset, rawSendBuffer, 5, message.Count);
		ArraySegment<byte> obj = new ArraySegment<byte>(rawSendBuffer, 0, message.Count + 1 + 4);
		RawSend(obj);
	}

	public void SendHandshake()
	{
		byte[] bytes = BitConverter.GetBytes(cookie);
		Log.Info($"KcpPeer: sending Handshake to other end with cookie={cookie}!");
		SendReliable(KcpHeader.Handshake, new ArraySegment<byte>(bytes));
	}

	public void SendData(ArraySegment<byte> data, KcpChannel channel)
	{
		if (data.Count == 0)
		{
			OnError(ErrorCode.InvalidSend, "KcpPeer: tried sending empty message. This should never happen. Disconnecting.");
			Disconnect();
			return;
		}
		switch (channel)
		{
		case KcpChannel.Reliable:
			SendReliable(KcpHeader.Data, data);
			break;
		case KcpChannel.Unreliable:
			SendUnreliable(data);
			break;
		}
	}

	private void SendPing()
	{
		SendReliable(KcpHeader.Ping, default(ArraySegment<byte>));
	}

	private void SendDisconnect()
	{
		SendReliable(KcpHeader.Disconnect, default(ArraySegment<byte>));
	}

	public void Disconnect()
	{
		if (state != KcpState.Disconnected)
		{
			try
			{
				SendDisconnect();
				kcp.Flush();
			}
			catch (SocketException)
			{
			}
			catch (ObjectDisposedException)
			{
			}
			Log.Info("KcpPeer: Disconnected.");
			state = KcpState.Disconnected;
			OnDisconnected?.Invoke();
		}
	}
}
