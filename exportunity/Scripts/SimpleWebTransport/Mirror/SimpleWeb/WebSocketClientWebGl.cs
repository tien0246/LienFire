using System;
using System.Collections.Generic;
using AOT;

namespace Mirror.SimpleWeb;

public class WebSocketClientWebGl : SimpleWebClient
{
	private static readonly Dictionary<int, WebSocketClientWebGl> instances = new Dictionary<int, WebSocketClientWebGl>();

	private int index;

	private Queue<byte[]> ConnectingSendQueue;

	internal WebSocketClientWebGl(int maxMessageSize, int maxMessagesPerTick)
		: base(maxMessageSize, maxMessagesPerTick)
	{
		throw new NotSupportedException();
	}

	public bool CheckJsConnected()
	{
		return SimpleWebJSLib.IsConnected(index);
	}

	public override void Connect(Uri serverAddress)
	{
		index = SimpleWebJSLib.Connect(serverAddress.ToString(), OpenCallback, CloseCallBack, MessageCallback, ErrorCallback);
		instances.Add(index, this);
		state = ClientState.Connecting;
	}

	public override void Disconnect()
	{
		state = ClientState.Disconnecting;
		SimpleWebJSLib.Disconnect(index);
	}

	public override void Send(ArraySegment<byte> segment)
	{
		if (segment.Count > maxMessageSize)
		{
			return;
		}
		if (state == ClientState.Connected)
		{
			SimpleWebJSLib.Send(index, segment.Array, segment.Offset, segment.Count);
			return;
		}
		if (ConnectingSendQueue == null)
		{
			ConnectingSendQueue = new Queue<byte[]>();
		}
		ConnectingSendQueue.Enqueue(segment.ToArray());
	}

	private void onOpen()
	{
		receiveQueue.Enqueue(new Message(EventType.Connected));
		state = ClientState.Connected;
		if (ConnectingSendQueue != null)
		{
			while (ConnectingSendQueue.Count > 0)
			{
				byte[] array = ConnectingSendQueue.Dequeue();
				SimpleWebJSLib.Send(index, array, 0, array.Length);
			}
			ConnectingSendQueue = null;
		}
	}

	private void onClose()
	{
		receiveQueue.Enqueue(new Message(EventType.Disconnected));
		state = ClientState.NotConnected;
		instances.Remove(index);
	}

	private void onMessage(IntPtr bufferPtr, int count)
	{
		try
		{
			ArrayBuffer arrayBuffer = bufferPool.Take(count);
			arrayBuffer.CopyFrom(bufferPtr, count);
			receiveQueue.Enqueue(new Message(arrayBuffer));
		}
		catch (Exception exception)
		{
			receiveQueue.Enqueue(new Message(exception));
		}
	}

	private void onErr()
	{
		receiveQueue.Enqueue(new Message(new Exception("Javascript Websocket error")));
		Disconnect();
	}

	[MonoPInvokeCallback(typeof(Action<int>))]
	private static void OpenCallback(int index)
	{
		instances[index].onOpen();
	}

	[MonoPInvokeCallback(typeof(Action<int>))]
	private static void CloseCallBack(int index)
	{
		instances[index].onClose();
	}

	[MonoPInvokeCallback(typeof(Action<int, IntPtr, int>))]
	private static void MessageCallback(int index, IntPtr bufferPtr, int count)
	{
		instances[index].onMessage(bufferPtr, count);
	}

	[MonoPInvokeCallback(typeof(Action<int>))]
	private static void ErrorCallback(int index)
	{
		instances[index].onErr();
	}
}
