using System.IO;
using System.Security.Authentication;
using UnityEngine;

namespace Mirror.SimpleWeb;

public class SslConfigLoader
{
	internal struct Cert
	{
		public string path;

		public string password;
	}

	public static SslConfig Load(bool sslEnabled, string sslCertJson, SslProtocols sslProtocols)
	{
		if (!sslEnabled)
		{
			return default(SslConfig);
		}
		Cert cert = LoadCertJson(sslCertJson);
		return new SslConfig(sslEnabled, cert.path, cert.password, sslProtocols);
	}

	internal static Cert LoadCertJson(string certJsonPath)
	{
		Cert result = JsonUtility.FromJson<Cert>(File.ReadAllText(certJsonPath));
		if (string.IsNullOrWhiteSpace(result.path))
		{
			throw new InvalidDataException("Cert Json didn't not contain \"path\"");
		}
		if (string.IsNullOrEmpty(result.password))
		{
			result.password = string.Empty;
		}
		return result;
	}
}
