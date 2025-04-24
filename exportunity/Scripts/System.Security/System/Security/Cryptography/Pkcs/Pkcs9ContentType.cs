using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

public sealed class Pkcs9ContentType : Pkcs9AttributeObject
{
	private volatile Oid _lazyContentType;

	public Oid ContentType => _lazyContentType ?? (_lazyContentType = Decode(base.RawData));

	public Pkcs9ContentType()
		: base(Oid.FromOidValue("1.2.840.113549.1.9.3", OidGroup.ExtensionOrAttribute))
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_lazyContentType = null;
	}

	private static Oid Decode(byte[] rawData)
	{
		if (rawData == null)
		{
			return null;
		}
		return new Oid(PkcsPal.Instance.DecodeOid(rawData));
	}
}
