using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.Profiling;

namespace Mirror.SimpleWeb;

internal static class SendLoop
{
	public struct Config
	{
		public readonly Connection conn;

		public readonly int bufferSize;

		public readonly bool setMask;

		public Config(Connection conn, int bufferSize, bool setMask)
		{
			this.conn = conn ?? throw new ArgumentNullException("conn");
			this.bufferSize = bufferSize;
			this.setMask = setMask;
		}

		public void Deconstruct(out Connection conn, out int bufferSize, out bool setMask)
		{
			conn = this.conn;
			bufferSize = this.bufferSize;
			setMask = this.setMask;
		}
	}

	public static void Loop(Config config)
	{
		Config config2 = config;
		config2.Deconstruct(out var conn, out var bufferSize, out var setMask);
		Connection connection = conn;
		int num = bufferSize;
		bool flag = setMask;
		byte[] buffer = new byte[num];
		MaskHelper maskHelper = (flag ? new MaskHelper() : null);
		try
		{
			TcpClient client = connection.client;
			Stream stream = connection.stream;
			if (client == null)
			{
				return;
			}
			while (client.Connected)
			{
				connection.sendPending.Wait();
				if (SendLoopConfig.sleepBeforeSend)
				{
					Thread.Sleep(1);
				}
				connection.sendPending.Reset();
				if (SendLoopConfig.batchSend)
				{
					int num2 = 0;
					ArrayBuffer result;
					while (connection.sendQueue.TryDequeue(out result))
					{
						if (!client.Connected)
						{
							result.Release();
							return;
						}
						int num3 = result.count + 4 + 4;
						if (num2 + num3 > num)
						{
							stream.Write(buffer, 0, num2);
							num2 = 0;
						}
						num2 = SendMessage(buffer, num2, result, flag, maskHelper);
						result.Release();
					}
					stream.Write(buffer, 0, num2);
					continue;
				}
				ArrayBuffer result2;
				while (connection.sendQueue.TryDequeue(out result2))
				{
					if (!client.Connected)
					{
						result2.Release();
						return;
					}
					int count = SendMessage(buffer, 0, result2, flag, maskHelper);
					stream.Write(buffer, 0, count);
					result2.Release();
				}
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
		finally
		{
			Profiler.EndThreadProfiling();
			connection.Dispose();
			maskHelper?.Dispose();
		}
	}

	private static int SendMessage(byte[] buffer, int startOffset, ArrayBuffer msg, bool setMask, MaskHelper maskHelper)
	{
		int count = msg.count;
		int num = WriteHeader(buffer, startOffset, count, setMask);
		if (setMask)
		{
			num = maskHelper.WriteMask(buffer, num);
		}
		msg.CopyTo(buffer, num);
		num += count;
		if (setMask)
		{
			int num2 = num - count;
			MessageProcessor.ToggleMask(buffer, num2, count, buffer, num2 - 4);
		}
		return num;
	}

	public static int WriteHeader(byte[] buffer, int startOffset, int msgLength, bool setMask)
	{
		int num = 0;
		buffer[startOffset] = 130;
		num++;
		if (msgLength <= 125)
		{
			buffer[startOffset + 1] = (byte)msgLength;
			num++;
		}
		else if (msgLength <= 65535)
		{
			buffer[startOffset + 1] = 126;
			buffer[startOffset + 2] = (byte)(msgLength >> 8);
			buffer[startOffset + 3] = (byte)msgLength;
			num += 3;
		}
		else
		{
			buffer[startOffset + 1] = 127;
			buffer[startOffset + 2] = 0;
			buffer[startOffset + 3] = 0;
			buffer[startOffset + 4] = 0;
			buffer[startOffset + 5] = 0;
			buffer[startOffset + 6] = (byte)(msgLength >> 24);
			buffer[startOffset + 7] = (byte)(msgLength >> 16);
			buffer[startOffset + 8] = (byte)(msgLength >> 8);
			buffer[startOffset + 9] = (byte)msgLength;
			num += 9;
		}
		if (setMask)
		{
			buffer[startOffset + 1] |= 128;
		}
		return num + startOffset;
	}
}
