using System.Data.Common;

namespace System.Data.OleDb;

[System.MonoTODO("OleDb is not implemented.")]
public sealed class OleDbError
{
	public string Message
	{
		get
		{
			throw ADP.OleDb();
		}
	}

	public int NativeError
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

	public string SQLState
	{
		get
		{
			throw ADP.OleDb();
		}
	}

	internal OleDbError()
	{
	}

	public override string ToString()
	{
		throw ADP.OleDb();
	}
}
