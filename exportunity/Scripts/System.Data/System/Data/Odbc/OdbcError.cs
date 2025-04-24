using Unity;

namespace System.Data.Odbc;

[Serializable]
public sealed class OdbcError
{
	internal string _message;

	internal string _state;

	internal int _nativeerror;

	internal string _source;

	public string Message
	{
		get
		{
			if (_message == null)
			{
				return string.Empty;
			}
			return _message;
		}
	}

	public string SQLState => _state;

	public int NativeError => _nativeerror;

	public string Source
	{
		get
		{
			if (_source == null)
			{
				return string.Empty;
			}
			return _source;
		}
	}

	internal OdbcError(string source, string message, string state, int nativeerror)
	{
		_source = source;
		_message = message;
		_state = state;
		_nativeerror = nativeerror;
	}

	internal void SetSource(string Source)
	{
		_source = Source;
	}

	public override string ToString()
	{
		return Message;
	}

	internal OdbcError()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
