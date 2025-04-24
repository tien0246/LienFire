using System.Data.Common;

namespace System.Data.OleDb;

[System.MonoTODO("OleDb is not implemented.")]
public sealed class OleDbInfoMessageEventArgs : EventArgs
{
	public int ErrorCode
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

	public string Message
	{
		get
		{
			throw ADP.OleDb();
		}
	}

	public string Source
	{
		get
		{
			throw ADP.OleDb();
		}
	}

	internal OleDbInfoMessageEventArgs()
	{
		throw ADP.OleDb();
	}

	public override string ToString()
	{
		throw ADP.OleDb();
	}
}
