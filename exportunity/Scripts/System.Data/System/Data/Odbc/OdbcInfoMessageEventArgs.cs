using System.Text;
using Unity;

namespace System.Data.Odbc;

public sealed class OdbcInfoMessageEventArgs : EventArgs
{
	private OdbcErrorCollection _errors;

	public OdbcErrorCollection Errors => _errors;

	public string Message
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (OdbcError error in Errors)
			{
				if (0 < stringBuilder.Length)
				{
					stringBuilder.Append(Environment.NewLine);
				}
				stringBuilder.Append(error.Message);
			}
			return stringBuilder.ToString();
		}
	}

	internal OdbcInfoMessageEventArgs(OdbcErrorCollection errors)
	{
		_errors = errors;
	}

	public override string ToString()
	{
		return Message;
	}

	internal OdbcInfoMessageEventArgs()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
