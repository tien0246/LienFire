using System.Threading;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

public sealed class Pkcs9SigningTime : Pkcs9AttributeObject
{
	private DateTime? _lazySigningTime;

	public DateTime SigningTime
	{
		get
		{
			if (!_lazySigningTime.HasValue)
			{
				_lazySigningTime = Decode(base.RawData);
				Interlocked.MemoryBarrier();
			}
			return _lazySigningTime.Value;
		}
	}

	public Pkcs9SigningTime()
		: this(DateTime.Now)
	{
	}

	public Pkcs9SigningTime(DateTime signingTime)
		: base("1.2.840.113549.1.9.5", Encode(signingTime))
	{
		_lazySigningTime = signingTime;
	}

	public Pkcs9SigningTime(byte[] encodedSigningTime)
		: base("1.2.840.113549.1.9.5", encodedSigningTime)
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_lazySigningTime = null;
	}

	private static DateTime Decode(byte[] rawData)
	{
		if (rawData == null)
		{
			return default(DateTime);
		}
		return PkcsPal.Instance.DecodeUtcTime(rawData);
	}

	private static byte[] Encode(DateTime signingTime)
	{
		return PkcsPal.Instance.EncodeUtcTime(signingTime);
	}
}
