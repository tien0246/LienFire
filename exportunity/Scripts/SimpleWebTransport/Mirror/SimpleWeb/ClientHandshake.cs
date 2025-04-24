using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Mirror.SimpleWeb;

internal class ClientHandshake
{
	public bool TryHandshake(Connection conn, Uri uri)
	{
		try
		{
			Stream stream = conn.stream;
			byte[] array = new byte[16];
			using (RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				rNGCryptoServiceProvider.GetBytes(array);
			}
			string text = Convert.ToBase64String(array);
			string s = text + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			string text2 = Convert.ToBase64String(SHA1.Create().ComputeHash(bytes));
			string s2 = "GET " + uri.PathAndQuery + " HTTP/1.1\r\n" + $"Host: {uri.Host}:{uri.Port}\r\n" + "Upgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Key: " + text + "\r\nSec-WebSocket-Version: 13\r\n\r\n";
			byte[] bytes2 = Encoding.ASCII.GetBytes(s2);
			stream.Write(bytes2, 0, bytes2.Length);
			byte[] array2 = new byte[1000];
			int? num = ReadHelper.SafeReadTillMatch(stream, array2, 0, array2.Length, Constants.endOfHandshake);
			if (!num.HasValue)
			{
				return false;
			}
			string text3 = Encoding.ASCII.GetString(array2, 0, num.Value);
			string text4 = "Sec-WebSocket-Accept: ";
			int num2 = text3.IndexOf(text4, StringComparison.InvariantCultureIgnoreCase);
			if (num2 < 0)
			{
				return false;
			}
			num2 += text4.Length;
			int num3 = text3.IndexOf("\r\n", num2);
			if (text3.Substring(num2, num3 - num2) != text2)
			{
				return false;
			}
			return true;
		}
		catch (Exception e)
		{
			Log.Exception(e);
			return false;
		}
	}
}
