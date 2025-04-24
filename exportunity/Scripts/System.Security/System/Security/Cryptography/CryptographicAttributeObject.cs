namespace System.Security.Cryptography;

public sealed class CryptographicAttributeObject
{
	private readonly Oid _oid;

	public Oid Oid => new Oid(_oid);

	public AsnEncodedDataCollection Values { get; }

	public CryptographicAttributeObject(Oid oid)
		: this(oid, new AsnEncodedDataCollection())
	{
	}

	public CryptographicAttributeObject(Oid oid, AsnEncodedDataCollection values)
	{
		_oid = new Oid(oid);
		if (values == null)
		{
			Values = new AsnEncodedDataCollection();
			return;
		}
		AsnEncodedDataEnumerator enumerator = values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			AsnEncodedData current = enumerator.Current;
			if (!string.Equals(current.Oid.Value, oid.Value, StringComparison.Ordinal))
			{
				throw new InvalidOperationException(global::SR.Format("AsnEncodedData element in the collection has wrong Oid value: expected = '{0}', actual = '{1}'.", oid.Value, current.Oid.Value));
			}
		}
		Values = values;
	}
}
