using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

public sealed class Pkcs9DocumentName : Pkcs9AttributeObject
{
	private volatile string _lazyDocumentName;

	public string DocumentName => _lazyDocumentName ?? (_lazyDocumentName = Decode(base.RawData));

	public Pkcs9DocumentName()
		: base(new Oid("1.3.6.1.4.1.311.88.2.1"))
	{
	}

	public Pkcs9DocumentName(string documentName)
		: base("1.3.6.1.4.1.311.88.2.1", Encode(documentName))
	{
		_lazyDocumentName = documentName;
	}

	public Pkcs9DocumentName(byte[] encodedDocumentName)
		: base("1.3.6.1.4.1.311.88.2.1", encodedDocumentName)
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_lazyDocumentName = null;
	}

	private static string Decode(byte[] rawData)
	{
		if (rawData == null)
		{
			return null;
		}
		return PkcsPal.Instance.DecodeOctetString(rawData).OctetStringToUnicode();
	}

	private static byte[] Encode(string documentName)
	{
		if (documentName == null)
		{
			throw new ArgumentNullException("documentName");
		}
		byte[] octets = documentName.UnicodeToOctetString();
		return PkcsPal.Instance.EncodeOctetString(octets);
	}
}
