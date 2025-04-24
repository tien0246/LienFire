using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Mono.Security;
using Mono.Security.Authenticode;

namespace Mono;

internal abstract class X509PalImpl
{
	private static byte[] signedData = new byte[9] { 42, 134, 72, 134, 247, 13, 1, 7, 2 };

	public bool SupportsLegacyBasicConstraintsExtension => false;

	public abstract X509CertificateImpl Import(byte[] data);

	public abstract X509Certificate2Impl Import(byte[] data, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags);

	public abstract X509Certificate2Impl Import(X509Certificate cert);

	private static byte[] PEM(string type, byte[] data)
	{
		string text = Encoding.ASCII.GetString(data);
		string text2 = $"-----BEGIN {type}-----";
		string value = $"-----END {type}-----";
		int num = text.IndexOf(text2) + text2.Length;
		int num2 = text.IndexOf(value, num);
		return Convert.FromBase64String(text.Substring(num, num2 - num));
	}

	protected static byte[] ConvertData(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			return data;
		}
		if (data[0] != 48)
		{
			try
			{
				return PEM("CERTIFICATE", data);
			}
			catch
			{
			}
		}
		return data;
	}

	internal X509Certificate2Impl ImportFallback(byte[] data)
	{
		data = ConvertData(data);
		using SafePasswordHandle password = new SafePasswordHandle((string)null);
		return new X509Certificate2ImplMono(data, password, X509KeyStorageFlags.DefaultKeySet);
	}

	internal X509Certificate2Impl ImportFallback(byte[] data, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
	{
		return new X509Certificate2ImplMono(data, password, keyStorageFlags);
	}

	public X509ContentType GetCertContentType(byte[] rawData)
	{
		if (rawData == null || rawData.Length == 0)
		{
			throw new ArgumentException("rawData");
		}
		if (rawData[0] == 48)
		{
			try
			{
				ASN1 aSN = new ASN1(rawData);
				if (aSN.Count == 3 && aSN[0].Tag == 48 && aSN[1].Tag == 48 && aSN[2].Tag == 3)
				{
					return X509ContentType.Cert;
				}
				if (aSN.Count == 3 && aSN[0].Tag == 2 && aSN[1].Tag == 48 && aSN[2].Tag == 48)
				{
					return X509ContentType.Pfx;
				}
				if (aSN.Count > 0 && aSN[0].Tag == 6 && aSN[0].CompareValue(signedData))
				{
					return X509ContentType.Pkcs7;
				}
				return X509ContentType.Unknown;
			}
			catch (Exception)
			{
				return X509ContentType.Unknown;
			}
		}
		if (Encoding.ASCII.GetString(rawData).IndexOf("-----BEGIN CERTIFICATE-----") >= 0)
		{
			return X509ContentType.Cert;
		}
		try
		{
			new AuthenticodeDeformatter(rawData);
			return X509ContentType.Authenticode;
		}
		catch
		{
			return X509ContentType.Unknown;
		}
	}

	public X509ContentType GetCertContentType(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		if (fileName.Length == 0)
		{
			throw new ArgumentException("fileName");
		}
		byte[] rawData = File.ReadAllBytes(fileName);
		return GetCertContentType(rawData);
	}
}
