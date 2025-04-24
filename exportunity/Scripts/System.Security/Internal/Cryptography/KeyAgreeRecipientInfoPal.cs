using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;

namespace Internal.Cryptography;

internal abstract class KeyAgreeRecipientInfoPal : RecipientInfoPal
{
	public abstract DateTime Date { get; }

	public abstract SubjectIdentifierOrKey OriginatorIdentifierOrKey { get; }

	public abstract CryptographicAttributeObject OtherKeyAttribute { get; }

	internal KeyAgreeRecipientInfoPal()
	{
	}
}
