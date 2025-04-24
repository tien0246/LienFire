using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Mirror.SimpleWeb;

public class WebSocketServer
{
	public readonly ConcurrentQueue<Message> receiveQueue = new ConcurrentQueue<Message>();

	private readonly TcpConfig tcpConfig;

	private readonly int maxMessageSize;

	private TcpListener listener;

	private Thread acceptThread;

	private bool serverStopped;

	private readonly ServerHandshake handShake;

	private readonly ServerSslHelper sslHelper;

	private readonly BufferPool bufferPool;

	private readonly ConcurrentDictionary<int, Connection> connections = new ConcurrentDictionary<int, Connection>();

	private int _idCounter;

	public WebSocketServer(TcpConfig tcpConfig, int maxMessageSize, int handshakeMaxSize, SslConfig sslConfig, BufferPool bufferPool)
	{
		this.tcpConfig = tcpConfig;
		this.maxMessageSize = maxMessageSize;
		sslHelper = new ServerSslHelper(sslConfig);
		this.bufferPool = bufferPool;
		handShake = new ServerHandshake(this.bufferPool, handshakeMaxSize);
	}

	public void Listen(int port)
	{
		listener = TcpListener.Create(port);
		listener.Start();
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"[SimpleWebTransport] Server Started on {port}!");
		Console.ResetColor();
		acceptThread = new Thread(acceptLoop);
		acceptThread.IsBackground = true;
		acceptThread.Start();
	}

	public void Stop()
	{
		serverStopped = true;
		acceptThread?.Interrupt();
		listener?.Stop();
		acceptThread = null;
		Console.WriteLine("[SimpleWebTransport] Server stopped...closing all connections.");
		Connection[] array = connections.Values.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Dispose();
		}
		connections.Clear();
	}

	private void acceptLoop()
	{
		try
		{
			try
			{
				while (true)
				{
					TcpClient client = listener.AcceptTcpClient();
					tcpConfig.ApplyTo(client);
					Connection conn = new Connection(client, AfterConnectionDisposed);
					Console.WriteLine($"[SimpleWebTransport] A client connected {conn}", false);
					Thread thread = new Thread((ThreadStart)delegate
					{
						HandshakeAndReceiveLoop(conn);
					});
					conn.receiveThread = thread;
					thread.IsBackground = true;
					thread.Start();
				}
			}
			catch (SocketException)
			{
				Utils.CheckForInterupt();
				throw;
			}
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
	}

	private void HandshakeAndReceiveLoop(Connection conn)
	{
		try
		{
			if (!sslHelper.TryCreateStream(conn))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"[SimpleWebTransport] Failed to create SSL Stream {conn}");
				Console.ResetColor();
				conn.Dispose();
			}
			else if (handShake.TryHandshake(conn))
			{
				Console.WriteLine($"[SimpleWebTransport] Sent Handshake {conn}, false");
				if (serverStopped)
				{
					Console.WriteLine("[SimpleWebTransport] Server stops after successful handshake", false);
					return;
				}
				conn.connId = Interlocked.Increment(ref _idCounter);
				connections.TryAdd(conn.connId, conn);
				receiveQueue.Enqueue(new Message(conn.connId, EventType.Connected));
				Thread thread = new Thread((ThreadStart)delegate
				{
					SendLoop.Loop(new SendLoop.Config(conn, 4 + maxMessageSize, setMask: false));
				});
				conn.sendThread = thread;
				thread.IsBackground = true;
				thread.Name = $"SendThread {conn.connId}";
				thread.Start();
				ReceiveLoop.Loop(new ReceiveLoop.Config(conn, maxMessageSize, expectMask: true, receiveQueue, bufferPool));
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"[SimpleWebTransport] Handshake Failed {conn}");
				Console.ResetColor();
				conn.Dispose();
			}
		}
		catch (ThreadInterruptedException ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("[SimpleWebTransport] Handshake ThreadInterruptedException " + ex.Message);
			Console.ResetColor();
		}
		catch (ThreadAbortException ex2)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("[SimpleWebTransport] Handshake ThreadAbortException " + ex2.Message);
			Console.ResetColor();
		}
		catch (Exception ex3)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("[SimpleWebTransport] Handshake Exception " + ex3.Message);
			Console.ResetColor();
		}
		finally
		{
			conn.Dispose();
		}
	}

	private void AfterConnectionDisposed(Connection conn)
	{
		if (conn.connId != -1)
		{
			receiveQueue.Enqueue(new Message(conn.connId, EventType.Disconnected));
			connections.TryRemove(conn.connId, out var _);
		}
	}

	public void Send(int id, ArrayBuffer buffer)
	{
		if (connections.TryGetValue(id, out var value))
		{
			value.sendQueue.Enqueue(buffer);
			value.sendPending.Set();
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"[SimpleWebTransport] Cannot send message to {id} because connection was not found in dictionary. Maybe it disconnected.");
			Console.ResetColor();
		}
	}

	public bool CloseConnection(int id)
	{
		if (connections.TryGetValue(id, out var value))
		{
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.WriteLine($"[SimpleWebTransport] Kicking connection {id}");
			Console.ResetColor();
			value.Dispose();
			return true;
		}
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine($"[SimpleWebTransport] Failed to kick {id} because id not found.");
		Console.ResetColor();
		return false;
	}

	public string GetClientAddress(int id)
	{
		if (connections.TryGetValue(id, out var value))
		{
			return value.client.Client.RemoteEndPoint.ToString();
		}
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine($"[SimpleWebTransport] Cannot get address of connection {id} because connection was not found in dictionary.");
		Console.ResetColor();
		return null;
	}
}
