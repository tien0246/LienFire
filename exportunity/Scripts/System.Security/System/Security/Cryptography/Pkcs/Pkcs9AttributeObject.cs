namespace System.Security.Cryptography.Pkcs;

public class Pkcs9AttributeObject : AsnEncodedData
{
	public new Oid Oid => base.Oid;

	public Pkcs9AttributeObject()
	{
	}

	public Pkcs9AttributeObject(string oid, byte[] encodedData)
		: this(new AsnEncodedData(oid, encodedData))
	{
	}

	public Pkcs9AttributeObject(Oid oid, byte[] encodedData)
		: this(new AsnEncodedData(oid, encodedData))
	{
	}

	public Pkcs9AttributeObject(AsnEncodedData asnEncodedData)
		: base(asnEncodedData)
	{
		if (asnEncodedData.Oid == null)
		{
			throw new ArgumentNullException("Oid");
		}
		if ((base.Oid.Value ?? throw new ArgumentNullException("oid.Value")).Length == 0)
		{
			throw new ArgumentException("String cannot be empty or null.", "oid.Value");
		}
	}

	internal Pkcs9AttributeObject(Oid oid)
	{
		base.Oid = oid;
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		if (asnEncodedData == null)
		{
			throw new ArgumentNullException("asnEncodedData");
		}
		if (!(asnEncodedData is Pkcs9AttributeObject))
		{
			throw new ArgumentException("The parameter should be a PKCS 9 attribute.");
		}
		base.CopyFrom(asnEncodedData);
	}
}
