using Unity;

namespace System.Security.Cryptography.Pkcs;

public sealed class PublicKeyInfo
{
	public AlgorithmIdentifier Algorithm { get; }

	public byte[] KeyValue { get; }

	internal PublicKeyInfo(AlgorithmIdentifier algorithm, byte[] keyValue)
	{
		Algorithm = algorithm;
		KeyValue = keyValue;
	}

	internal PublicKeyInfo()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
