using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001;

[Serializable]
[ComVisible(true)]
public sealed class SoapNmtokens : ISoapXsd
{
	private string _value;

	public string Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	public static string XsdType => "NMTOKENS";

	public SoapNmtokens()
	{
	}

	public SoapNmtokens(string value)
	{
		_value = SoapHelper.Normalize(value);
	}

	public string GetXsdType()
	{
		return XsdType;
	}

	public static SoapNmtokens Parse(string value)
	{
		return new SoapNmtokens(value);
	}

	public override string ToString()
	{
		return _value;
	}
}
