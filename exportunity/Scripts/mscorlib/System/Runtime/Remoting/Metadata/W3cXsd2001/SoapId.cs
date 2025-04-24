using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001;

[Serializable]
[ComVisible(true)]
public sealed class SoapId : ISoapXsd
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

	public static string XsdType => "ID";

	public SoapId()
	{
	}

	public SoapId(string value)
	{
		_value = SoapHelper.Normalize(value);
	}

	public string GetXsdType()
	{
		return XsdType;
	}

	public static SoapId Parse(string value)
	{
		return new SoapId(value);
	}

	public override string ToString()
	{
		return _value;
	}
}
