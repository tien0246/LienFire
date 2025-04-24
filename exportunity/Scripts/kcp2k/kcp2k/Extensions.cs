using System;
using System.Net;
using System.Net.Sockets;

namespace kcp2k;

public static class Extensions
{
	public static bool SendToNonBlocking(this Socket socket, ArraySegment<byte> data, EndPoint remoteEP)
	{
		try
		{
			if (!socket.Poll(0, SelectMode.SelectWrite))
			{
				return false;
			}
			socket.SendTo(data.Array, data.Offset, data.Count, SocketFlags.None, remoteEP);
			return true;
		}
		catch (SocketException ex)
		{
			if (ex.SocketErrorCode == SocketError.WouldBlock)
			{
				return false;
			}
			throw;
		}
	}

	public static bool SendNonBlocking(this Socket socket, ArraySegment<byte> data)
	{
		try
		{
			if (!socket.Poll(0, SelectMode.SelectWrite))
			{
				return false;
			}
			socket.Send(data.Array, data.Offset, data.Count, SocketFlags.None);
			return true;
		}
		catch (SocketException ex)
		{
			if (ex.SocketErrorCode == SocketError.WouldBlock)
			{
				return false;
			}
			throw;
		}
	}

	public static bool ReceiveFromNonBlocking(this Socket socket, byte[] recvBuffer, out ArraySegment<byte> data, ref EndPoint remoteEP)
	{
		data = default(ArraySegment<byte>);
		try
		{
			if (!socket.Poll(0, SelectMode.SelectRead))
			{
				return false;
			}
			int count = socket.ReceiveFrom(recvBuffer, 0, recvBuffer.Length, SocketFlags.None, ref remoteEP);
			data = new ArraySegment<byte>(recvBuffer, 0, count);
			return true;
		}
		catch (SocketException ex)
		{
			if (ex.SocketErrorCode == SocketError.WouldBlock)
			{
				return false;
			}
			throw;
		}
	}

	public static bool ReceiveNonBlocking(this Socket socket, byte[] recvBuffer, out ArraySegment<byte> data)
	{
		data = default(ArraySegment<byte>);
		try
		{
			if (!socket.Poll(0, SelectMode.SelectRead))
			{
				return false;
			}
			int count = socket.Receive(recvBuffer, 0, recvBuffer.Length, SocketFlags.None);
			data = new ArraySegment<byte>(recvBuffer, 0, count);
			return true;
		}
		catch (SocketException ex)
		{
			if (ex.SocketErrorCode == SocketError.WouldBlock)
			{
				return false;
			}
			throw;
		}
	}
}
