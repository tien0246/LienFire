using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.Profiling;

namespace Mirror.SimpleWeb;

internal static class ReceiveLoop
{
	public struct Config
	{
		public readonly Connection conn;

		public readonly int maxMessageSize;

		public readonly bool expectMask;

		public readonly ConcurrentQueue<Message> queue;

		public readonly BufferPool bufferPool;

		public Config(Connection conn, int maxMessageSize, bool expectMask, ConcurrentQueue<Message> queue, BufferPool bufferPool)
		{
			this.conn = conn ?? throw new ArgumentNullException("conn");
			this.maxMessageSize = maxMessageSize;
			this.expectMask = expectMask;
			this.queue = queue ?? throw new ArgumentNullException("queue");
			this.bufferPool = bufferPool ?? throw new ArgumentNullException("bufferPool");
		}

		public void Deconstruct(out Connection conn, out int maxMessageSize, out bool expectMask, out ConcurrentQueue<Message> queue, out BufferPool bufferPool)
		{
			conn = this.conn;
			maxMessageSize = this.maxMessageSize;
			expectMask = this.expectMask;
			queue = this.queue;
			bufferPool = this.bufferPool;
		}
	}

	private struct Header
	{
		public int payloadLength;

		public int offset;

		public int opcode;

		public bool finished;
	}

	public static void Loop(Config config)
	{
		Config config2 = config;
		config2.Deconstruct(out var conn, out var maxMessageSize, out var expectMask, out var queue, out var _);
		Connection connection = conn;
		int num = maxMessageSize;
		bool flag = expectMask;
		ConcurrentQueue<Message> concurrentQueue = queue;
		byte[] buffer = new byte[4 + (flag ? 4 : 0) + num];
		try
		{
			try
			{
				TcpClient client = connection.client;
				while (client.Connected)
				{
					ReadOneMessage(config, buffer);
				}
			}
			catch (Exception)
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
		catch (ObjectDisposedException)
		{
		}
		catch (ReadHelperException)
		{
		}
		catch (SocketException exception)
		{
			concurrentQueue.Enqueue(new Message(connection.connId, exception));
		}
		catch (IOException exception2)
		{
			concurrentQueue.Enqueue(new Message(connection.connId, exception2));
		}
		catch (InvalidDataException exception3)
		{
			concurrentQueue.Enqueue(new Message(connection.connId, exception3));
		}
		catch (Exception ex6)
		{
			Log.Exception(ex6);
			concurrentQueue.Enqueue(new Message(connection.connId, ex6));
		}
		finally
		{
			Profiler.EndThreadProfiling();
			connection.Dispose();
		}
	}

	private static void ReadOneMessage(Config config, byte[] buffer)
	{
		Config config2 = config;
		config2.Deconstruct(out var conn, out var maxMessageSize, out var expectMask, out var queue, out var bufferPool);
		Connection connection = conn;
		int maxLength = maxMessageSize;
		bool expectMask2 = expectMask;
		ConcurrentQueue<Message> concurrentQueue = queue;
		BufferPool bufferPool2 = bufferPool;
		Stream stream = connection.stream;
		Header header = ReadHeader(config, buffer);
		int offset = header.offset;
		header.offset = ReadHelper.Read(stream, buffer, header.offset, header.payloadLength);
		if (header.finished)
		{
			switch (header.opcode)
			{
			case 2:
				HandleArrayMessage(config, buffer, offset, header.payloadLength);
				break;
			case 8:
				HandleCloseMessage(config, buffer, offset, header.payloadLength);
				break;
			}
			return;
		}
		Queue<ArrayBuffer> queue2 = new Queue<ArrayBuffer>();
		queue2.Enqueue(CopyMessageToBuffer(bufferPool2, expectMask2, buffer, offset, header.payloadLength));
		int num = header.payloadLength;
		while (!header.finished)
		{
			header = ReadHeader(config, buffer, opCodeContinuation: true);
			offset = header.offset;
			header.offset = ReadHelper.Read(stream, buffer, header.offset, header.payloadLength);
			queue2.Enqueue(CopyMessageToBuffer(bufferPool2, expectMask2, buffer, offset, header.payloadLength));
			num += header.payloadLength;
			MessageProcessor.ThrowIfMsgLengthTooLong(num, maxLength);
		}
		ArrayBuffer arrayBuffer = bufferPool2.Take(num);
		arrayBuffer.count = 0;
		while (queue2.Count > 0)
		{
			ArrayBuffer arrayBuffer2 = queue2.Dequeue();
			arrayBuffer2.CopyTo(arrayBuffer.array, arrayBuffer.count);
			arrayBuffer.count += arrayBuffer2.count;
			arrayBuffer2.Release();
		}
		concurrentQueue.Enqueue(new Message(connection.connId, arrayBuffer));
	}

	private static Header ReadHeader(Config config, byte[] buffer, bool opCodeContinuation = false)
	{
		Config config2 = config;
		config2.Deconstruct(out var conn, out var maxMessageSize, out var expectMask, out var _, out var _);
		Connection connection = conn;
		int maxLength = maxMessageSize;
		bool flag = expectMask;
		Stream stream = connection.stream;
		Header result = default(Header);
		result.offset = ReadHelper.Read(stream, buffer, result.offset, 2);
		if (MessageProcessor.NeedToReadShortLength(buffer))
		{
			result.offset = ReadHelper.Read(stream, buffer, result.offset, 2);
		}
		if (MessageProcessor.NeedToReadLongLength(buffer))
		{
			result.offset = ReadHelper.Read(stream, buffer, result.offset, 8);
		}
		MessageProcessor.ValidateHeader(buffer, maxLength, flag, opCodeContinuation);
		if (flag)
		{
			result.offset = ReadHelper.Read(stream, buffer, result.offset, 4);
		}
		result.opcode = MessageProcessor.GetOpcode(buffer);
		result.payloadLength = MessageProcessor.GetPayloadLength(buffer);
		result.finished = MessageProcessor.Finished(buffer);
		return result;
	}

	private static void HandleArrayMessage(Config config, byte[] buffer, int msgOffset, int payloadLength)
	{
		Config config2 = config;
		var (connection2, _, expectMask, concurrentQueue2, bufferPool2) = (Config)(ref config2);
		concurrentQueue2.Enqueue(new Message(data: CopyMessageToBuffer(bufferPool2, expectMask, buffer, msgOffset, payloadLength), connId: connection2.connId));
	}

	private static ArrayBuffer CopyMessageToBuffer(BufferPool bufferPool, bool expectMask, byte[] buffer, int msgOffset, int payloadLength)
	{
		ArrayBuffer arrayBuffer = bufferPool.Take(payloadLength);
		if (expectMask)
		{
			int maskOffset = msgOffset - 4;
			MessageProcessor.ToggleMask(buffer, msgOffset, arrayBuffer, payloadLength, buffer, maskOffset);
		}
		else
		{
			arrayBuffer.CopyFrom(buffer, msgOffset, payloadLength);
		}
		return arrayBuffer;
	}

	private static void HandleCloseMessage(Config config, byte[] buffer, int msgOffset, int payloadLength)
	{
		Config config2 = config;
		var (connection2, _, flag2, _, _) = (Config)(ref config2);
		if (flag2)
		{
			int maskOffset = msgOffset - 4;
			MessageProcessor.ToggleMask(buffer, msgOffset, payloadLength, buffer, maskOffset);
		}
		connection2.Dispose();
	}

	private static string GetCloseMessage(byte[] buffer, int msgOffset, int payloadLength)
	{
		return Encoding.UTF8.GetString(buffer, msgOffset + 2, payloadLength - 2);
	}

	private static int GetCloseCode(byte[] buffer, int msgOffset)
	{
		return (buffer[msgOffset] << 8) | buffer[msgOffset + 1];
	}
}
