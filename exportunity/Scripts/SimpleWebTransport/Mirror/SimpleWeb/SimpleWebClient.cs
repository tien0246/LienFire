using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Mirror.SimpleWeb;

public abstract class SimpleWebClient
{
	private readonly int maxMessagesPerTick;

	protected readonly int maxMessageSize;

	public readonly ConcurrentQueue<Message> receiveQueue = new ConcurrentQueue<Message>();

	protected readonly BufferPool bufferPool;

	protected ClientState state;

	public ClientState ConnectionState => state;

	public event Action onConnect;

	public event Action onDisconnect;

	public event Action<ArraySegment<byte>> onData;

	public event Action<Exception> onError;

	public static SimpleWebClient Create(int maxMessageSize, int maxMessagesPerTick, TcpConfig tcpConfig)
	{
		return new WebSocketClientStandAlone(maxMessageSize, maxMessagesPerTick, tcpConfig);
	}

	protected SimpleWebClient(int maxMessageSize, int maxMessagesPerTick)
	{
		this.maxMessageSize = maxMessageSize;
		this.maxMessagesPerTick = maxMessagesPerTick;
		bufferPool = new BufferPool(5, 20, maxMessageSize);
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
		while ((flag || behaviour.enabled) && num < maxMessagesPerTick && receiveQueue.TryDequeue(out result))
		{
			num++;
			switch (result.type)
			{
			case EventType.Connected:
				this.onConnect?.Invoke();
				break;
			case EventType.Data:
				this.onData?.Invoke(result.data.ToSegment());
				result.data.Release();
				break;
			case EventType.Disconnected:
				this.onDisconnect?.Invoke();
				break;
			case EventType.Error:
				this.onError?.Invoke(result.exception);
				break;
			}
		}
		if (receiveQueue.Count > 0)
		{
			Debug.LogWarning($"SimpleWebClient ProcessMessageQueue has {receiveQueue.Count} remaining.");
		}
	}

	public abstract void Connect(Uri serverAddress);

	public abstract void Disconnect();

	public abstract void Send(ArraySegment<byte> segment);
}
