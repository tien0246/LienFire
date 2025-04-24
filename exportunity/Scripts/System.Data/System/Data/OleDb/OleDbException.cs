using System.Data.Common;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Data.OleDb;

[System.MonoTODO("OleDb is not implemented.")]
public sealed class OleDbException : DbException
{
	public override int ErrorCode
	{
		get
		{
			throw ADP.OleDb();
		}
	}

	public OleDbErrorCollection Errors
	{
		get
		{
			throw ADP.OleDb();
		}
	}

	internal OleDbException()
	{
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo si, StreamingContext context)
	{
		throw ADP.OleDb();
	}
}
