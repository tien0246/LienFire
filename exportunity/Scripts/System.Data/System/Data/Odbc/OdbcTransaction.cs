using System.Data.Common;
using Unity;

namespace System.Data.Odbc;

public sealed class OdbcTransaction : DbTransaction
{
	private OdbcConnection _connection;

	private IsolationLevel _isolevel;

	private OdbcConnectionHandle _handle;

	public new OdbcConnection Connection => _connection;

	protected override DbConnection DbConnection => Connection;

	public override IsolationLevel IsolationLevel
	{
		get
		{
			OdbcConnection connection = _connection;
			if (connection == null)
			{
				throw ADP.TransactionZombied(this);
			}
			if (IsolationLevel.Unspecified == _isolevel)
			{
				int connectAttr = connection.GetConnectAttr(ODBC32.SQL_ATTR.TXN_ISOLATION, ODBC32.HANDLER.THROW);
				switch ((ODBC32.SQL_TRANSACTION)connectAttr)
				{
				case ODBC32.SQL_TRANSACTION.READ_UNCOMMITTED:
					_isolevel = IsolationLevel.ReadUncommitted;
					break;
				case ODBC32.SQL_TRANSACTION.READ_COMMITTED:
					_isolevel = IsolationLevel.ReadCommitted;
					break;
				case ODBC32.SQL_TRANSACTION.REPEATABLE_READ:
					_isolevel = IsolationLevel.RepeatableRead;
					break;
				case ODBC32.SQL_TRANSACTION.SERIALIZABLE:
					_isolevel = IsolationLevel.Serializable;
					break;
				case ODBC32.SQL_TRANSACTION.SNAPSHOT:
					_isolevel = IsolationLevel.Snapshot;
					break;
				default:
					throw ODBC.NoMappingForSqlTransactionLevel(connectAttr);
				}
			}
			return _isolevel;
		}
	}

	internal OdbcTransaction(OdbcConnection connection, IsolationLevel isolevel, OdbcConnectionHandle handle)
	{
		_isolevel = IsolationLevel.Unspecified;
		base._002Ector();
		_connection = connection;
		_isolevel = isolevel;
		_handle = handle;
	}

	public override void Commit()
	{
		OdbcConnection connection = _connection;
		if (connection == null)
		{
			throw ADP.TransactionZombied(this);
		}
		connection.CheckState("CommitTransaction");
		if (_handle == null)
		{
			throw ODBC.NotInTransaction();
		}
		ODBC32.RetCode retCode = _handle.CompleteTransaction(0);
		if (retCode == ODBC32.RetCode.ERROR)
		{
			connection.HandleError(_handle, retCode);
		}
		connection.LocalTransaction = null;
		_connection = null;
		_handle = null;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			OdbcConnectionHandle handle = _handle;
			_handle = null;
			if (handle != null)
			{
				try
				{
					ODBC32.RetCode retCode = handle.CompleteTransaction(1);
					if (retCode == ODBC32.RetCode.ERROR && _connection != null)
					{
						ADP.TraceExceptionWithoutRethrow(_connection.HandleErrorNoThrow(handle, retCode));
					}
				}
				catch (Exception e)
				{
					if (!ADP.IsCatchableExceptionType(e))
					{
						throw;
					}
				}
			}
			if (_connection != null && _connection.IsOpen)
			{
				_connection.LocalTransaction = null;
			}
			_connection = null;
			_isolevel = IsolationLevel.Unspecified;
		}
		base.Dispose(disposing);
	}

	public override void Rollback()
	{
		OdbcConnection connection = _connection;
		if (connection == null)
		{
			throw ADP.TransactionZombied(this);
		}
		connection.CheckState("RollbackTransaction");
		if (_handle == null)
		{
			throw ODBC.NotInTransaction();
		}
		ODBC32.RetCode retCode = _handle.CompleteTransaction(1);
		if (retCode == ODBC32.RetCode.ERROR)
		{
			connection.HandleError(_handle, retCode);
		}
		connection.LocalTransaction = null;
		_connection = null;
		_handle = null;
	}

	internal OdbcTransaction()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
