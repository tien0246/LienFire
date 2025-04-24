using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs;

public sealed class CmsRecipient
{
	public SubjectIdentifierType RecipientIdentifierType { get; }

	public X509Certificate2 Certificate { get; }

	public CmsRecipient(X509Certificate2 certificate)
		: this(SubjectIdentifierType.IssuerAndSerialNumber, certificate)
	{
	}

	public CmsRecipient(SubjectIdentifierType recipientIdentifierType, X509Certificate2 certificate)
	{
		if (certificate == null)
		{
			throw new ArgumentNullException("certificate");
		}
		switch (recipientIdentifierType)
		{
		case SubjectIdentifierType.Unknown:
			recipientIdentifierType = SubjectIdentifierType.IssuerAndSerialNumber;
			break;
		default:
			throw new CryptographicException(global::SR.Format("The subject identifier type {0} is not valid.", recipientIdentifierType));
		case SubjectIdentifierType.IssuerAndSerialNumber:
		case SubjectIdentifierType.SubjectKeyIdentifier:
			break;
		}
		RecipientIdentifierType = recipientIdentifierType;
		Certificate = certificate;
	}
}
