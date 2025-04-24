namespace System.Security.Cryptography.Xml;

public struct X509IssuerSerial
{
	public string IssuerName { get; set; }

	public string SerialNumber { get; set; }

	internal X509IssuerSerial(string issuerName, string serialNumber)
	{
		this = default(X509IssuerSerial);
		IssuerName = issuerName;
		SerialNumber = serialNumber;
	}
}
