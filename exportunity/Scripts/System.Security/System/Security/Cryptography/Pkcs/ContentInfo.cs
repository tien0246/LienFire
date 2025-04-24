using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

public sealed class ContentInfo
{
	public Oid ContentType { get; }

	public byte[] Content { get; }

	public ContentInfo(byte[] content)
		: this(Oid.FromOidValue("1.2.840.113549.1.7.1", OidGroup.ExtensionOrAttribute), content)
	{
	}

	public ContentInfo(Oid contentType, byte[] content)
	{
		if (contentType == null)
		{
			throw new ArgumentNullException("contentType");
		}
		if (content == null)
		{
			throw new ArgumentNullException("content");
		}
		ContentType = contentType;
		Content = content;
	}

	public static Oid GetContentType(byte[] encodedMessage)
	{
		if (encodedMessage == null)
		{
			throw new ArgumentNullException("encodedMessage");
		}
		return PkcsPal.Instance.GetEncodedMessageType(encodedMessage);
	}
}
