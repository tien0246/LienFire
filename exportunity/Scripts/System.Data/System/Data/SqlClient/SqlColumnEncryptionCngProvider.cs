using Unity;

namespace System.Data.SqlClient;

public class SqlColumnEncryptionCngProvider : SqlColumnEncryptionKeyStoreProvider
{
	public const string ProviderName = "MSSQL_CNG_STORE";

	public SqlColumnEncryptionCngProvider()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}

	public override byte[] DecryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] encryptedColumnEncryptionKey)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
		return null;
	}

	public override byte[] EncryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] columnEncryptionKey)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
		return null;
	}

	public override byte[] SignColumnMasterKeyMetadata(string masterKeyPath, bool allowEnclaveComputations)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
		return null;
	}

	public override bool VerifyColumnMasterKeyMetadata(string masterKeyPath, bool allowEnclaveComputations, byte[] signature)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
		return default(bool);
	}
}
