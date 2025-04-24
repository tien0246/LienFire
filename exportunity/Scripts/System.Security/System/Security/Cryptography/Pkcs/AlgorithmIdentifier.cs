namespace System.Security.Cryptography.Pkcs;

public sealed class AlgorithmIdentifier
{
	public Oid Oid { get; set; }

	public int KeyLength { get; set; }

	public byte[] Parameters { get; set; } = Array.Empty<byte>();

	public AlgorithmIdentifier()
		: this(Oid.FromOidValue("1.2.840.113549.3.7", OidGroup.EncryptionAlgorithm), 0)
	{
	}

	public AlgorithmIdentifier(Oid oid)
		: this(oid, 0)
	{
	}

	public AlgorithmIdentifier(Oid oid, int keyLength)
	{
		Oid = oid;
		KeyLength = keyLength;
	}
}
