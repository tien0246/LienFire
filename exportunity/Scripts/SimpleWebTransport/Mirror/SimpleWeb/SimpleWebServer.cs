using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.SimpleWeb;

public class SimpleWebServer
{
	private readonly int maxMessagesPerTick;

	private readonly WebSocketServer server;

	private readonly BufferPool bufferPool;

	public bool Active { get; private set; }

	public event Action<int> onConnect;

	public event Action<int> onDisconnect;

	public event Action<int, ArraySegment<byte>> onData;

	public event Action<int, Exception> onError;

	public SimpleWebServer(int maxMessagesPerTick, TcpConfig tcpConfig, int maxMessageSize, int handshakeMaxSize, SslConfig sslConfig)
	{
		this.maxMessagesPerTick = maxMessagesPerTick;
		int largest = Math.Max(maxMessageSize, handshakeMaxSize);
		bufferPool = new BufferPool(5, 20, largest);
		server = new WebSocketServer(tcpConfig, maxMessageSize, handshakeMaxSize, sslConfig, bufferPool);
	}

	public void Start(ushort port)
	{
		server.Listen(port);
		Active = true;
	}

	public void Stop()
	{
		server.Stop();
		Active = false;
	}

	public void SendAll(List<int> connectionIds, ArraySegment<byte> source)
	{
		ArrayBuffer arrayBuffer = bufferPool.Take(source.Count);
		arrayBuffer.CopyFrom(source);
		arrayBuffer.SetReleasesRequired(connectionIds.Count);
		foreach (int connectionId in connectionIds)
		{
			server.Send(connectionId, arrayBuffer);
		}
	}

	public void SendOne(int connectionId, ArraySegment<byte> source)
	{
		ArrayBuffer arrayBuffer = bufferPool.Take(source.Count);
		arrayBuffer.CopyFrom(source);
		server.Send(connectionId, arrayBuffer);
	}

	public bool KickClient(int connectionId)
	{
		return server.CloseConnection(connectionId);
	}

	public string GetClientAddress(int connectionId)
	{
		return server.GetClientAddress(connectionId);
	}

	public void ProcessMessageQueue()
	{
		ProcessMessageQueue(null);
	}

	public void ProcessMessageQueue(MonoBehaviour behaviour)
	{
		int num = 0;
		bool flag = behaviour == null;
		Message result;
		while ((flag || behaviour.enabled) && num < maxMessagesPerTick && server.receiveQueue.TryDequeue(out result))
		{
			num++;
			switch (result.type)
			{
			case EventType.Connected:
				this.onConnect?.Invoke(result.connId);
				break;
			case EventType.Data:
				this.onData?.Invoke(result.connId, result.data.ToSegment());
				result.data.Release();
				break;
			case EventType.Disconnected:
				this.onDisconnect?.Invoke(result.connId);
				break;
			case EventType.Error:
				this.onError?.Invoke(result.connId, result.exception);
				break;
			}
		}
		if (server.receiveQueue.Count > 0)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"SimpleWebServer ProcessMessageQueue has {server.receiveQueue.Count} remaining.");
			Console.ResetColor();
		}
	}
}
