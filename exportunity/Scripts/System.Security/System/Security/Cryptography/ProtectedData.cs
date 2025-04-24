using Mono.Security.Cryptography;

namespace System.Security.Cryptography;

public sealed class ProtectedData
{
	private enum DataProtectionImplementation
	{
		Unknown = 0,
		Win32CryptoProtect = 1,
		ManagedProtection = 2,
		Unsupported = int.MinValue
	}

	private static DataProtectionImplementation impl;

	private ProtectedData()
	{
	}

	public static byte[] Protect(byte[] userData, byte[] optionalEntropy, DataProtectionScope scope)
	{
		if (userData == null)
		{
			throw new ArgumentNullException("userData");
		}
		Check(scope);
		if (impl == DataProtectionImplementation.ManagedProtection)
		{
			try
			{
				return ManagedProtection.Protect(userData, optionalEntropy, scope);
			}
			catch (Exception inner)
			{
				throw new CryptographicException(Locale.GetText("Data protection failed."), inner);
			}
		}
		throw new PlatformNotSupportedException();
	}

	public static byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope)
	{
		if (encryptedData == null)
		{
			throw new ArgumentNullException("encryptedData");
		}
		Check(scope);
		if (impl == DataProtectionImplementation.ManagedProtection)
		{
			try
			{
				return ManagedProtection.Unprotect(encryptedData, optionalEntropy, scope);
			}
			catch (Exception inner)
			{
				throw new CryptographicException(Locale.GetText("Data unprotection failed."), inner);
			}
		}
		throw new PlatformNotSupportedException();
	}

	private static void Detect()
	{
		PlatformID platform = Environment.OSVersion.Platform;
		if (platform != PlatformID.Win32NT && platform == PlatformID.Unix)
		{
			impl = DataProtectionImplementation.ManagedProtection;
		}
		else
		{
			impl = DataProtectionImplementation.Unsupported;
		}
	}

	private static void Check(DataProtectionScope scope)
	{
		switch (impl)
		{
		case DataProtectionImplementation.Unknown:
			Detect();
			break;
		case DataProtectionImplementation.Unsupported:
			throw new PlatformNotSupportedException();
		}
	}
}
