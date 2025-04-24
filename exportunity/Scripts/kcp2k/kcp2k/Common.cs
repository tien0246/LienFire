using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace kcp2k;

public static class Common
{
	private static readonly RNGCryptoServiceProvider cryptoRandom = new RNGCryptoServiceProvider();

	private static readonly byte[] cryptoRandomBuffer = new byte[4];

	public static bool ResolveHostname(string hostname, out IPAddress[] addresses)
	{
		try
		{
			addresses = Dns.GetHostAddresses(hostname);
			return addresses.Length >= 1;
		}
		catch (SocketException arg)
		{
			Log.Info($"Failed to resolve host: {hostname} reason: {arg}");
			addresses = null;
			return false;
		}
	}

	public static void ConfigureSocketBuffers(Socket socket, int recvBufferSize, int sendBufferSize)
	{
		int receiveBufferSize = socket.ReceiveBufferSize;
		int sendBufferSize2 = socket.SendBufferSize;
		try
		{
			socket.ReceiveBufferSize = recvBufferSize;
			socket.SendBufferSize = sendBufferSize;
		}
		catch (SocketException)
		{
			Log.Warning($"Kcp: failed to set Socket RecvBufSize = {recvBufferSize} SendBufSize = {sendBufferSize}");
		}
		Log.Info($"Kcp: RecvBuf = {receiveBufferSize}=>{socket.ReceiveBufferSize} ({socket.ReceiveBufferSize / receiveBufferSize}x) SendBuf = {sendBufferSize2}=>{socket.SendBufferSize} ({socket.SendBufferSize / sendBufferSize2}x)");
	}

	public static int ConnectionHash(EndPoint endPoint)
	{
		return endPoint.GetHashCode();
	}

	public static uint GenerateCookie()
	{
		cryptoRandom.GetBytes(cryptoRandomBuffer);
		return BitConverter.ToUInt32(cryptoRandomBuffer, 0);
	}
}
