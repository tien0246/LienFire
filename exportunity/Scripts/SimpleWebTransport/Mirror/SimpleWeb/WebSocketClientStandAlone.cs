using System;
using System.Net.Sockets;
using System.Threading;

namespace Mirror.SimpleWeb;

public class WebSocketClientStandAlone : SimpleWebClient
{
	private readonly ClientSslHelper sslHelper;

	private readonly ClientHandshake handshake;

	private readonly TcpConfig tcpConfig;

	private Connection conn;

	internal WebSocketClientStandAlone(int maxMessageSize, int maxMessagesPerTick, TcpConfig tcpConfig)
		: base(maxMessageSize, maxMessagesPerTick)
	{
		sslHelper = new ClientSslHelper();
		handshake = new ClientHandshake();
		this.tcpConfig = tcpConfig;
	}

	public override void Connect(Uri serverAddress)
	{
		state = ClientState.Connecting;
		TcpClient client = new TcpClient();
		tcpConfig.ApplyTo(client);
		conn = new Connection(client, AfterConnectionDisposed);
		Thread thread = new Thread((ThreadStart)delegate
		{
			ConnectAndReceiveLoop(serverAddress);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void ConnectAndReceiveLoop(Uri serverAddress)
	{
		try
		{
			TcpClient client = conn.client;
			conn.receiveThread = Thread.CurrentThread;
			try
			{
				client.Connect(serverAddress.Host, serverAddress.Port);
			}
			catch (SocketException)
			{
				client.Dispose();
				throw;
			}
			if (!sslHelper.TryCreateStream(conn, serverAddress))
			{
				conn.Dispose();
				return;
			}
			if (!handshake.TryHandshake(conn, serverAddress))
			{
				conn.Dispose();
				return;
			}
			state = ClientState.Connected;
			receiveQueue.Enqueue(new Message(EventType.Connected));
			Thread thread = new Thread((ThreadStart)delegate
			{
				SendLoop.Loop(new SendLoop.Config(conn, 8 + maxMessageSize, setMask: true));
			});
			conn.sendThread = thread;
			thread.IsBackground = true;
			thread.Start();
			ReceiveLoop.Loop(new ReceiveLoop.Config(conn, maxMessageSize, expectMask: false, receiveQueue, bufferPool));
		}
		catch (ThreadInterruptedException)
		{
		}
		catch (ThreadAbortException)
		{
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
		finally
		{
			conn?.Dispose();
		}
	}

	private void AfterConnectionDisposed(Connection conn)
	{
		state = ClientState.NotConnected;
		receiveQueue.Enqueue(new Message(EventType.Disconnected));
	}

	public override void Disconnect()
	{
		state = ClientState.Disconnecting;
		if (conn == null)
		{
			state = ClientState.NotConnected;
		}
		else
		{
			conn?.Dispose();
		}
	}

	public override void Send(ArraySegment<byte> segment)
	{
		ArrayBuffer arrayBuffer = bufferPool.Take(segment.Count);
		arrayBuffer.CopyFrom(segment);
		conn.sendQueue.Enqueue(arrayBuffer);
		conn.sendPending.Set();
	}
}
