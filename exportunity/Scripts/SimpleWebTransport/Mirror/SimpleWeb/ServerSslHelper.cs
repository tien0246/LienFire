using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Mirror.SimpleWeb;

internal class ServerSslHelper
{
	private readonly SslConfig config;

	private readonly X509Certificate2 certificate;

	public ServerSslHelper(SslConfig sslConfig)
	{
		Console.Clear();
		config = sslConfig;
		if (config.enabled)
		{
			certificate = new X509Certificate2(config.certPath, config.certPassword);
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("[SimpleWebTransport] SSL Certificate " + certificate.Subject + " loaded with expiration of " + certificate.GetExpirationDateString());
			Console.ResetColor();
		}
	}

	internal bool TryCreateStream(Connection conn)
	{
		NetworkStream stream = conn.client.GetStream();
		if (config.enabled)
		{
			try
			{
				conn.stream = CreateStream(stream);
				return true;
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("[SimpleWebTransport] Create SSLStream Failed: " + ex.Message);
				Console.ResetColor();
				return false;
			}
		}
		conn.stream = stream;
		return true;
	}

	private Stream CreateStream(NetworkStream stream)
	{
		SslStream sslStream = new SslStream(stream, leaveInnerStreamOpen: true, acceptClient);
		sslStream.AuthenticateAsServer(certificate, clientCertificateRequired: false, config.sslProtocols, checkCertificateRevocation: false);
		return sslStream;
	}

	private bool acceptClient(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		return true;
	}
}
