using System.Data.Common;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Unity;

namespace System.Data.Odbc;

[Serializable]
public sealed class OdbcException : DbException
{
	private OdbcErrorCollection _odbcErrors;

	public OdbcErrorCollection Errors => _odbcErrors;

	public override string Source
	{
		get
		{
			if (0 < Errors.Count)
			{
				string source = Errors[0].Source;
				if (!string.IsNullOrEmpty(source))
				{
					return source;
				}
				return "";
			}
			return "";
		}
	}

	internal static OdbcException CreateException(OdbcErrorCollection errors, ODBC32.RetCode retcode)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (OdbcError error in errors)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(Environment.NewLine);
			}
			stringBuilder.Append(global::SR.GetString("{0} [{1}] {2}", ODBC32.RetcodeToString(retcode), error.SQLState, error.Message));
		}
		return new OdbcException(stringBuilder.ToString(), errors);
	}

	internal OdbcException(string message, OdbcErrorCollection errors)
	{
		_odbcErrors = new OdbcErrorCollection();
		base._002Ector(message);
		_odbcErrors = errors;
		base.HResult = -2146232009;
	}

	private OdbcException(SerializationInfo si, StreamingContext sc)
	{
		_odbcErrors = new OdbcErrorCollection();
		base._002Ector(si, sc);
		_odbcErrors = (OdbcErrorCollection)si.GetValue("odbcErrors", typeof(OdbcErrorCollection));
		base.HResult = -2146232009;
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo si, StreamingContext context)
	{
		base.GetObjectData(si, context);
		si.AddValue("odbcRetcode", ODBC32.RETCODE.SUCCESS, typeof(ODBC32.RETCODE));
		si.AddValue("odbcErrors", _odbcErrors, typeof(OdbcErrorCollection));
	}

	internal OdbcException()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
