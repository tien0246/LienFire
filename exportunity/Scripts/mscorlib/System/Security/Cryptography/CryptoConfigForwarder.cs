namespace System.Security.Cryptography;

internal static class CryptoConfigForwarder
{
	internal static object CreateFromName(string name)
	{
		return CryptoConfig.CreateFromName(name);
	}

	internal static HashAlgorithm CreateDefaultHashAlgorithm()
	{
		return (HashAlgorithm)CreateFromName("System.Security.Cryptography.HashAlgorithm");
	}
}
