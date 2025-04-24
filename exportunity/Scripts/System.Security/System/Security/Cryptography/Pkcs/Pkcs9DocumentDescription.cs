using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

public sealed class Pkcs9DocumentDescription : Pkcs9AttributeObject
{
	private volatile string _lazyDocumentDescription;

	public string DocumentDescription => _lazyDocumentDescription ?? (_lazyDocumentDescription = Decode(base.RawData));

	public Pkcs9DocumentDescription()
		: base(new Oid("1.3.6.1.4.1.311.88.2.2"))
	{
	}

	public Pkcs9DocumentDescription(string documentDescription)
		: base("1.3.6.1.4.1.311.88.2.2", Encode(documentDescription))
	{
		_lazyDocumentDescription = documentDescription;
	}

	public Pkcs9DocumentDescription(byte[] encodedDocumentDescription)
		: base("1.3.6.1.4.1.311.88.2.2", encodedDocumentDescription)
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_lazyDocumentDescription = null;
	}

	private static string Decode(byte[] rawData)
	{
		if (rawData == null)
		{
			return null;
		}
		return PkcsPal.Instance.DecodeOctetString(rawData).OctetStringToUnicode();
	}

	private static byte[] Encode(string documentDescription)
	{
		if (documentDescription == null)
		{
			throw new ArgumentNullException("documentDescription");
		}
		byte[] octets = documentDescription.UnicodeToOctetString();
		return PkcsPal.Instance.EncodeOctetString(octets);
	}
}
