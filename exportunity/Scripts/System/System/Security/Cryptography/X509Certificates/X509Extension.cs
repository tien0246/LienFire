using System.Text;

namespace System.Security.Cryptography.X509Certificates;

public class X509Extension : AsnEncodedData
{
	private bool _critical;

	public bool Critical
	{
		get
		{
			return _critical;
		}
		set
		{
			_critical = value;
		}
	}

	protected X509Extension()
	{
	}

	public X509Extension(AsnEncodedData encodedExtension, bool critical)
	{
		if (encodedExtension.Oid == null)
		{
			throw new ArgumentNullException("encodedExtension.Oid");
		}
		base.Oid = encodedExtension.Oid;
		base.RawData = encodedExtension.RawData;
		_critical = critical;
	}

	public X509Extension(Oid oid, byte[] rawData, bool critical)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		base.Oid = oid;
		base.RawData = rawData;
		_critical = critical;
	}

	public X509Extension(string oid, byte[] rawData, bool critical)
		: base(oid, rawData)
	{
		_critical = critical;
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		if (asnEncodedData == null)
		{
			throw new ArgumentNullException("encodedData");
		}
		if (!(asnEncodedData is X509Extension x509Extension))
		{
			throw new ArgumentException(global::Locale.GetText("Expected a X509Extension instance."));
		}
		base.CopyFrom(asnEncodedData);
		_critical = x509Extension.Critical;
	}

	internal string FormatUnkownData(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < data.Length; i++)
		{
			stringBuilder.Append(data[i].ToString("X2"));
		}
		return stringBuilder.ToString();
	}
}
