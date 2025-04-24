using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Security.Cryptography;

public abstract class DataProtector
{
	private string m_applicationName;

	private string m_primaryPurpose;

	private IEnumerable<string> m_specificPurposes;

	private volatile byte[] m_hashedPurpose;

	protected string ApplicationName => m_applicationName;

	protected virtual bool PrependHashedPurposeToPlaintext => true;

	protected string PrimaryPurpose => m_primaryPurpose;

	protected IEnumerable<string> SpecificPurposes => m_specificPurposes;

	protected DataProtector(string applicationName, string primaryPurpose, string[] specificPurposes)
	{
		if (string.IsNullOrWhiteSpace(applicationName))
		{
			throw new ArgumentException("Invalid application name and/or purpose", "applicationName");
		}
		if (string.IsNullOrWhiteSpace(primaryPurpose))
		{
			throw new ArgumentException("Invalid application name and/or purpose", "primaryPurpose");
		}
		if (specificPurposes != null)
		{
			for (int i = 0; i < specificPurposes.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(specificPurposes[i]))
				{
					throw new ArgumentException("Invalid application name and/or purpose", "specificPurposes");
				}
			}
		}
		m_applicationName = applicationName;
		m_primaryPurpose = primaryPurpose;
		List<string> list = new List<string>();
		if (specificPurposes != null)
		{
			list.AddRange(specificPurposes);
		}
		m_specificPurposes = list;
	}

	protected virtual byte[] GetHashedPurpose()
	{
		if (m_hashedPurpose == null)
		{
			using HashAlgorithm hashAlgorithm = HashAlgorithm.Create("System.Security.Cryptography.Sha256Cng");
			using (BinaryWriter binaryWriter = new BinaryWriter(new CryptoStream(new MemoryStream(), hashAlgorithm, CryptoStreamMode.Write), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)))
			{
				binaryWriter.Write(ApplicationName);
				binaryWriter.Write(PrimaryPurpose);
				foreach (string specificPurpose in SpecificPurposes)
				{
					binaryWriter.Write(specificPurpose);
				}
			}
			m_hashedPurpose = hashAlgorithm.Hash;
		}
		return m_hashedPurpose;
	}

	public abstract bool IsReprotectRequired(byte[] encryptedData);

	public static DataProtector Create(string providerClass, string applicationName, string primaryPurpose, params string[] specificPurposes)
	{
		if (providerClass == null)
		{
			throw new ArgumentNullException("providerClass");
		}
		return (DataProtector)CryptoConfig.CreateFromName(providerClass, applicationName, primaryPurpose, specificPurposes);
	}

	public byte[] Protect(byte[] userData)
	{
		if (userData == null)
		{
			throw new ArgumentNullException("userData");
		}
		if (PrependHashedPurposeToPlaintext)
		{
			byte[] hashedPurpose = GetHashedPurpose();
			byte[] array = new byte[userData.Length + hashedPurpose.Length];
			Array.Copy(hashedPurpose, 0, array, 0, hashedPurpose.Length);
			Array.Copy(userData, 0, array, hashedPurpose.Length, userData.Length);
			userData = array;
		}
		return ProviderProtect(userData);
	}

	protected abstract byte[] ProviderProtect(byte[] userData);

	protected abstract byte[] ProviderUnprotect(byte[] encryptedData);

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public byte[] Unprotect(byte[] encryptedData)
	{
		if (encryptedData == null)
		{
			throw new ArgumentNullException("encryptedData");
		}
		if (PrependHashedPurposeToPlaintext)
		{
			byte[] array = ProviderUnprotect(encryptedData);
			byte[] hashedPurpose = GetHashedPurpose();
			bool flag = array.Length >= hashedPurpose.Length;
			for (int i = 0; i < hashedPurpose.Length; i++)
			{
				if (hashedPurpose[i] != array[i % array.Length])
				{
					flag = false;
				}
			}
			if (!flag)
			{
				throw new CryptographicException("Invalid data protection purpose");
			}
			byte[] array2 = new byte[array.Length - hashedPurpose.Length];
			Array.Copy(array, hashedPurpose.Length, array2, 0, array2.Length);
			return array2;
		}
		return ProviderUnprotect(encryptedData);
	}
}
