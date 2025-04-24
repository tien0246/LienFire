using System.Runtime.InteropServices;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
public class CodeConnectAccess
{
	public static readonly string AnyScheme = "*";

	public static readonly int DefaultPort = -3;

	public static readonly int OriginPort = -4;

	public static readonly string OriginScheme = "$origin";

	private string _scheme;

	private int _port;

	public int Port => _port;

	public string Scheme => _scheme;

	[MonoTODO("(2.0) validations incomplete")]
	public CodeConnectAccess(string allowScheme, int allowPort)
	{
		if (allowScheme == null || allowScheme.Length == 0)
		{
			throw new ArgumentOutOfRangeException("allowScheme");
		}
		if (allowPort < 0 || allowPort > 65535)
		{
			throw new ArgumentOutOfRangeException("allowPort");
		}
		_scheme = allowScheme;
		_port = allowPort;
	}

	public override bool Equals(object o)
	{
		if (!(o is CodeConnectAccess codeConnectAccess))
		{
			return false;
		}
		if (_scheme == codeConnectAccess._scheme)
		{
			return _port == codeConnectAccess._port;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _scheme.GetHashCode() ^ _port;
	}

	public static CodeConnectAccess CreateAnySchemeAccess(int allowPort)
	{
		return new CodeConnectAccess(AnyScheme, allowPort);
	}

	public static CodeConnectAccess CreateOriginSchemeAccess(int allowPort)
	{
		return new CodeConnectAccess(OriginScheme, allowPort);
	}
}
