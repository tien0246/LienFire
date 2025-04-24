using System.ComponentModel;
using System.Data.Common;
using System.Data.ProviderBase;
using System.EnterpriseServices;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Transactions;
using Unity;

namespace System.Data.Odbc;

public sealed class OdbcConnection : DbConnection, ICloneable
{
	private int _connectionTimeout = 15;

	private OdbcInfoMessageEventHandler _infoMessageEventHandler;

	private WeakReference _weakTransaction;

	private OdbcConnectionHandle _connectionHandle;

	private ConnectionState _extraState;

	private static readonly DbConnectionFactory s_connectionFactory = OdbcConnectionFactory.SingletonInstance;

	private DbConnectionOptions _userConnectionOptions;

	private DbConnectionPoolGroup _poolGroup;

	private DbConnectionInternal _innerConnection;

	private int _closeCount;

	internal OdbcConnectionHandle ConnectionHandle
	{
		get
		{
			return _connectionHandle;
		}
		set
		{
			_connectionHandle = value;
		}
	}

	public override string ConnectionString
	{
		get
		{
			return ConnectionString_Get();
		}
		set
		{
			ConnectionString_Set(value);
		}
	}

	[DefaultValue(15)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new int ConnectionTimeout
	{
		get
		{
			return _connectionTimeout;
		}
		set
		{
			if (value < 0)
			{
				throw ODBC.NegativeArgument();
			}
			if (IsOpen)
			{
				throw ODBC.CantSetPropertyOnOpenConnection();
			}
			_connectionTimeout = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override string Database
	{
		get
		{
			if (IsOpen && !ProviderInfo.NoCurrentCatalog)
			{
				return GetConnectAttrString(ODBC32.SQL_ATTR.CURRENT_CATALOG);
			}
			return string.Empty;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string DataSource
	{
		get
		{
			if (IsOpen)
			{
				return GetInfoStringUnhandled(ODBC32.SQL_INFO.SERVER_NAME, handleError: true);
			}
			return string.Empty;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string ServerVersion => InnerConnection.ServerVersion;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override ConnectionState State => InnerConnection.State;

	internal OdbcConnectionPoolGroupProviderInfo ProviderInfo => (OdbcConnectionPoolGroupProviderInfo)PoolGroup.ProviderInfo;

	internal ConnectionState InternalState => State | _extraState;

	internal bool IsOpen => InnerConnection is OdbcConnectionOpen;

	internal OdbcTransaction LocalTransaction
	{
		get
		{
			OdbcTransaction result = null;
			if (_weakTransaction != null)
			{
				result = (OdbcTransaction)_weakTransaction.Target;
			}
			return result;
		}
		set
		{
			_weakTransaction = null;
			if (value != null)
			{
				_weakTransaction = new WeakReference(value);
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string Driver
	{
		get
		{
			if (IsOpen)
			{
				if (ProviderInfo.DriverName == null)
				{
					ProviderInfo.DriverName = GetInfoStringUnhandled(ODBC32.SQL_INFO.DRIVER_NAME);
				}
				return ProviderInfo.DriverName;
			}
			return ADP.StrEmpty;
		}
	}

	internal bool IsV3Driver
	{
		get
		{
			if (ProviderInfo.DriverVersion == null)
			{
				ProviderInfo.DriverVersion = GetInfoStringUnhandled(ODBC32.SQL_INFO.DRIVER_ODBC_VER);
				if (ProviderInfo.DriverVersion != null && ProviderInfo.DriverVersion.Length >= 2)
				{
					try
					{
						ProviderInfo.IsV3Driver = int.Parse(ProviderInfo.DriverVersion.Substring(0, 2), CultureInfo.InvariantCulture) >= 3;
					}
					catch (FormatException e)
					{
						ProviderInfo.IsV3Driver = false;
						ADP.TraceExceptionWithoutRethrow(e);
					}
				}
				else
				{
					ProviderInfo.DriverVersion = "";
				}
			}
			return ProviderInfo.IsV3Driver;
		}
	}

	internal int CloseCount => _closeCount;

	internal DbConnectionFactory ConnectionFactory => s_connectionFactory;

	internal DbConnectionOptions ConnectionOptions => PoolGroup?.ConnectionOptions;

	internal DbConnectionInternal InnerConnection => _innerConnection;

	internal DbConnectionPoolGroup PoolGroup
	{
		get
		{
			return _poolGroup;
		}
		set
		{
			_poolGroup = value;
		}
	}

	internal DbConnectionOptions UserConnectionOptions => _userConnectionOptions;

	public event OdbcInfoMessageEventHandler InfoMessage
	{
		add
		{
			_infoMessageEventHandler = (OdbcInfoMessageEventHandler)Delegate.Combine(_infoMessageEventHandler, value);
		}
		remove
		{
			_infoMessageEventHandler = (OdbcInfoMessageEventHandler)Delegate.Remove(_infoMessageEventHandler, value);
		}
	}

	public OdbcConnection(string connectionString)
		: this()
	{
		ConnectionString = connectionString;
	}

	private OdbcConnection(OdbcConnection connection)
		: this()
	{
		CopyFrom(connection);
		_connectionTimeout = connection._connectionTimeout;
	}

	internal char EscapeChar(string method)
	{
		CheckState(method);
		if (!ProviderInfo.HasEscapeChar)
		{
			string infoStringUnhandled = GetInfoStringUnhandled(ODBC32.SQL_INFO.SEARCH_PATTERN_ESCAPE);
			ProviderInfo.EscapeChar = ((infoStringUnhandled.Length == 1) ? infoStringUnhandled[0] : QuoteChar(method)[0]);
		}
		return ProviderInfo.EscapeChar;
	}

	internal string QuoteChar(string method)
	{
		CheckState(method);
		if (!ProviderInfo.HasQuoteChar)
		{
			string infoStringUnhandled = GetInfoStringUnhandled(ODBC32.SQL_INFO.IDENTIFIER_QUOTE_CHAR);
			ProviderInfo.QuoteChar = ((1 == infoStringUnhandled.Length) ? infoStringUnhandled : "\0");
		}
		return ProviderInfo.QuoteChar;
	}

	public new OdbcTransaction BeginTransaction()
	{
		return BeginTransaction(IsolationLevel.Unspecified);
	}

	public new OdbcTransaction BeginTransaction(IsolationLevel isolevel)
	{
		return (OdbcTransaction)InnerConnection.BeginTransaction(isolevel);
	}

	private void RollbackDeadTransaction()
	{
		WeakReference weakTransaction = _weakTransaction;
		if (weakTransaction != null && !weakTransaction.IsAlive)
		{
			_weakTransaction = null;
			ConnectionHandle.CompleteTransaction(1);
		}
	}

	public override void ChangeDatabase(string value)
	{
		InnerConnection.ChangeDatabase(value);
	}

	internal void CheckState(string method)
	{
		ConnectionState internalState = InternalState;
		if (ConnectionState.Open != internalState)
		{
			throw ADP.OpenConnectionRequired(method, internalState);
		}
	}

	object ICloneable.Clone()
	{
		return new OdbcConnection(this);
	}

	internal bool ConnectionIsAlive(Exception innerException)
	{
		if (IsOpen)
		{
			if (!ProviderInfo.NoConnectionDead)
			{
				int connectAttr = GetConnectAttr(ODBC32.SQL_ATTR.CONNECTION_DEAD, ODBC32.HANDLER.IGNORE);
				if (1 == connectAttr)
				{
					Close();
					throw ADP.ConnectionIsDisabled(innerException);
				}
			}
			return true;
		}
		return false;
	}

	public new OdbcCommand CreateCommand()
	{
		return new OdbcCommand(string.Empty, this);
	}

	internal OdbcStatementHandle CreateStatementHandle()
	{
		return new OdbcStatementHandle(ConnectionHandle);
	}

	public override void Close()
	{
		InnerConnection.CloseConnection(this, ConnectionFactory);
		OdbcConnectionHandle connectionHandle = _connectionHandle;
		if (connectionHandle == null)
		{
			return;
		}
		_connectionHandle = null;
		WeakReference weakTransaction = _weakTransaction;
		if (weakTransaction != null)
		{
			_weakTransaction = null;
			IDisposable disposable = weakTransaction.Target as OdbcTransaction;
			if (disposable != null && weakTransaction.IsAlive)
			{
				disposable.Dispose();
			}
		}
		connectionHandle.Dispose();
	}

	private void DisposeMe(bool disposing)
	{
	}

	internal string GetConnectAttrString(ODBC32.SQL_ATTR attribute)
	{
		string result = "";
		int cbActual = 0;
		byte[] array = new byte[100];
		OdbcConnectionHandle connectionHandle = ConnectionHandle;
		if (connectionHandle != null)
		{
			ODBC32.RetCode connectionAttribute = connectionHandle.GetConnectionAttribute(attribute, array, out cbActual);
			if (array.Length + 2 <= cbActual)
			{
				array = new byte[cbActual + 2];
				connectionAttribute = connectionHandle.GetConnectionAttribute(attribute, array, out cbActual);
			}
			if (connectionAttribute == ODBC32.RetCode.SUCCESS || ODBC32.RetCode.SUCCESS_WITH_INFO == connectionAttribute)
			{
				result = (BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode).GetString(array, 0, Math.Min(cbActual, array.Length));
			}
			else if (connectionAttribute == ODBC32.RetCode.ERROR)
			{
				string diagSqlState = GetDiagSqlState();
				if ("HYC00" == diagSqlState || "HY092" == diagSqlState || "IM001" == diagSqlState)
				{
					FlagUnsupportedConnectAttr(attribute);
				}
			}
		}
		return result;
	}

	internal int GetConnectAttr(ODBC32.SQL_ATTR attribute, ODBC32.HANDLER handler)
	{
		int result = -1;
		int cbActual = 0;
		byte[] array = new byte[4];
		OdbcConnectionHandle connectionHandle = ConnectionHandle;
		if (connectionHandle != null)
		{
			ODBC32.RetCode connectionAttribute = connectionHandle.GetConnectionAttribute(attribute, array, out cbActual);
			if (connectionAttribute == ODBC32.RetCode.SUCCESS || ODBC32.RetCode.SUCCESS_WITH_INFO == connectionAttribute)
			{
				result = BitConverter.ToInt32(array, 0);
			}
			else
			{
				if (connectionAttribute == ODBC32.RetCode.ERROR)
				{
					string diagSqlState = GetDiagSqlState();
					if ("HYC00" == diagSqlState || "HY092" == diagSqlState || "IM001" == diagSqlState)
					{
						FlagUnsupportedConnectAttr(attribute);
					}
				}
				if (handler == ODBC32.HANDLER.THROW)
				{
					HandleError(connectionHandle, connectionAttribute);
				}
			}
		}
		return result;
	}

	private string GetDiagSqlState()
	{
		ConnectionHandle.GetDiagnosticField(out var sqlState);
		return sqlState;
	}

	internal ODBC32.RetCode GetInfoInt16Unhandled(ODBC32.SQL_INFO info, out short resultValue)
	{
		byte[] array = new byte[2];
		ODBC32.RetCode info2 = ConnectionHandle.GetInfo1(info, array);
		resultValue = BitConverter.ToInt16(array, 0);
		return info2;
	}

	internal ODBC32.RetCode GetInfoInt32Unhandled(ODBC32.SQL_INFO info, out int resultValue)
	{
		byte[] array = new byte[4];
		ODBC32.RetCode info2 = ConnectionHandle.GetInfo1(info, array);
		resultValue = BitConverter.ToInt32(array, 0);
		return info2;
	}

	private int GetInfoInt32Unhandled(ODBC32.SQL_INFO infotype)
	{
		byte[] array = new byte[4];
		ConnectionHandle.GetInfo1(infotype, array);
		return BitConverter.ToInt32(array, 0);
	}

	internal string GetInfoStringUnhandled(ODBC32.SQL_INFO info)
	{
		return GetInfoStringUnhandled(info, handleError: false);
	}

	private string GetInfoStringUnhandled(ODBC32.SQL_INFO info, bool handleError)
	{
		string result = null;
		short cbActual = 0;
		byte[] array = new byte[100];
		OdbcConnectionHandle connectionHandle = ConnectionHandle;
		if (connectionHandle != null)
		{
			ODBC32.RetCode info2 = connectionHandle.GetInfo2(info, array, out cbActual);
			if (array.Length < cbActual - 2)
			{
				array = new byte[cbActual + 2];
				info2 = connectionHandle.GetInfo2(info, array, out cbActual);
			}
			if (info2 == ODBC32.RetCode.SUCCESS || info2 == ODBC32.RetCode.SUCCESS_WITH_INFO)
			{
				result = (BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode).GetString(array, 0, Math.Min(cbActual, array.Length));
			}
			else if (handleError)
			{
				HandleError(ConnectionHandle, info2);
			}
		}
		else if (handleError)
		{
			result = "";
		}
		return result;
	}

	internal Exception HandleErrorNoThrow(OdbcHandle hrHandle, ODBC32.RetCode retcode)
	{
		switch (retcode)
		{
		case ODBC32.RetCode.SUCCESS_WITH_INFO:
			if (_infoMessageEventHandler != null)
			{
				OdbcErrorCollection diagErrors = ODBC32.GetDiagErrors(null, hrHandle, retcode);
				diagErrors.SetSource(Driver);
				OnInfoMessage(new OdbcInfoMessageEventArgs(diagErrors));
			}
			break;
		default:
		{
			OdbcException ex = OdbcException.CreateException(ODBC32.GetDiagErrors(null, hrHandle, retcode), retcode);
			ex?.Errors.SetSource(Driver);
			ConnectionIsAlive(ex);
			return ex;
		}
		case ODBC32.RetCode.SUCCESS:
			break;
		}
		return null;
	}

	internal void HandleError(OdbcHandle hrHandle, ODBC32.RetCode retcode)
	{
		Exception ex = HandleErrorNoThrow(hrHandle, retcode);
		if ((uint)retcode > 1u)
		{
			throw ex;
		}
	}

	public override void Open()
	{
		try
		{
			InnerConnection.OpenConnection(this, ConnectionFactory);
		}
		catch (DllNotFoundException ex) when (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			throw new DllNotFoundException("Dependency unixODBC with minimum version 2.3.1 is required." + Environment.NewLine + ex.Message);
		}
		if (ADP.NeedManualEnlistment())
		{
			EnlistTransaction(Transaction.Current);
		}
	}

	private void OnInfoMessage(OdbcInfoMessageEventArgs args)
	{
		if (_infoMessageEventHandler == null)
		{
			return;
		}
		try
		{
			_infoMessageEventHandler(this, args);
		}
		catch (Exception e)
		{
			if (!ADP.IsCatchableOrSecurityExceptionType(e))
			{
				throw;
			}
			ADP.TraceExceptionWithoutRethrow(e);
		}
	}

	public static void ReleaseObjectPool()
	{
		OdbcEnvironment.ReleaseObjectPool();
	}

	internal OdbcTransaction SetStateExecuting(string method, OdbcTransaction transaction)
	{
		if (_weakTransaction != null)
		{
			OdbcTransaction odbcTransaction = _weakTransaction.Target as OdbcTransaction;
			if (transaction != odbcTransaction)
			{
				if (transaction == null)
				{
					throw ADP.TransactionRequired(method);
				}
				if (this != transaction.Connection)
				{
					throw ADP.TransactionConnectionMismatch();
				}
				transaction = null;
			}
		}
		else if (transaction != null)
		{
			if (transaction.Connection != null)
			{
				throw ADP.TransactionConnectionMismatch();
			}
			transaction = null;
		}
		ConnectionState internalState = InternalState;
		if (ConnectionState.Open != internalState)
		{
			NotifyWeakReference(1);
			internalState = InternalState;
			if (ConnectionState.Open != internalState)
			{
				if ((ConnectionState.Fetching & internalState) != ConnectionState.Closed)
				{
					throw ADP.OpenReaderExists();
				}
				throw ADP.OpenConnectionRequired(method, internalState);
			}
		}
		return transaction;
	}

	internal void SetSupportedType(ODBC32.SQL_TYPE sqltype)
	{
		ODBC32.SQL_CVT sQL_CVT;
		switch (sqltype)
		{
		case ODBC32.SQL_TYPE.NUMERIC:
			sQL_CVT = ODBC32.SQL_CVT.NUMERIC;
			break;
		case ODBC32.SQL_TYPE.WCHAR:
			sQL_CVT = ODBC32.SQL_CVT.WCHAR;
			break;
		case ODBC32.SQL_TYPE.WVARCHAR:
			sQL_CVT = ODBC32.SQL_CVT.WVARCHAR;
			break;
		case ODBC32.SQL_TYPE.WLONGVARCHAR:
			sQL_CVT = ODBC32.SQL_CVT.WLONGVARCHAR;
			break;
		default:
			return;
		}
		ProviderInfo.TestedSQLTypes |= (int)sQL_CVT;
		ProviderInfo.SupportedSQLTypes |= (int)sQL_CVT;
	}

	internal void FlagRestrictedSqlBindType(ODBC32.SQL_TYPE sqltype)
	{
		ODBC32.SQL_CVT sQL_CVT;
		switch (sqltype)
		{
		default:
			return;
		case ODBC32.SQL_TYPE.NUMERIC:
			sQL_CVT = ODBC32.SQL_CVT.NUMERIC;
			break;
		case ODBC32.SQL_TYPE.DECIMAL:
			sQL_CVT = ODBC32.SQL_CVT.DECIMAL;
			break;
		}
		ProviderInfo.RestrictedSQLBindTypes |= (int)sQL_CVT;
	}

	internal void FlagUnsupportedConnectAttr(ODBC32.SQL_ATTR Attribute)
	{
		switch (Attribute)
		{
		case ODBC32.SQL_ATTR.CURRENT_CATALOG:
			ProviderInfo.NoCurrentCatalog = true;
			break;
		case ODBC32.SQL_ATTR.CONNECTION_DEAD:
			ProviderInfo.NoConnectionDead = true;
			break;
		}
	}

	internal void FlagUnsupportedStmtAttr(ODBC32.SQL_ATTR Attribute)
	{
		switch (Attribute)
		{
		case ODBC32.SQL_ATTR.QUERY_TIMEOUT:
			ProviderInfo.NoQueryTimeout = true;
			break;
		case (ODBC32.SQL_ATTR)1228:
			ProviderInfo.NoSqlSoptSSNoBrowseTable = true;
			break;
		case ODBC32.SQL_ATTR.SQL_COPT_SS_TXN_ISOLATION:
			ProviderInfo.NoSqlSoptSSHiddenColumns = true;
			break;
		}
	}

	internal void FlagUnsupportedColAttr(ODBC32.SQL_DESC v3FieldId, ODBC32.SQL_COLUMN v2FieldId)
	{
		if (IsV3Driver && v3FieldId == (ODBC32.SQL_DESC)1212)
		{
			ProviderInfo.NoSqlCASSColumnKey = true;
		}
	}

	internal bool SQLGetFunctions(ODBC32.SQL_API odbcFunction)
	{
		OdbcConnectionHandle connectionHandle = ConnectionHandle;
		if (connectionHandle != null)
		{
			short fExists;
			ODBC32.RetCode functions = connectionHandle.GetFunctions(odbcFunction, out fExists);
			if (functions != ODBC32.RetCode.SUCCESS)
			{
				HandleError(connectionHandle, functions);
			}
			if (fExists == 0)
			{
				return false;
			}
			return true;
		}
		throw ODBC.ConnectionClosed();
	}

	internal bool TestTypeSupport(ODBC32.SQL_TYPE sqltype)
	{
		ODBC32.SQL_CONVERT infotype;
		ODBC32.SQL_CVT sQL_CVT;
		switch (sqltype)
		{
		case ODBC32.SQL_TYPE.NUMERIC:
			infotype = ODBC32.SQL_CONVERT.NUMERIC;
			sQL_CVT = ODBC32.SQL_CVT.NUMERIC;
			break;
		case ODBC32.SQL_TYPE.WCHAR:
			infotype = ODBC32.SQL_CONVERT.CHAR;
			sQL_CVT = ODBC32.SQL_CVT.WCHAR;
			break;
		case ODBC32.SQL_TYPE.WVARCHAR:
			infotype = ODBC32.SQL_CONVERT.VARCHAR;
			sQL_CVT = ODBC32.SQL_CVT.WVARCHAR;
			break;
		case ODBC32.SQL_TYPE.WLONGVARCHAR:
			infotype = ODBC32.SQL_CONVERT.LONGVARCHAR;
			sQL_CVT = ODBC32.SQL_CVT.WLONGVARCHAR;
			break;
		default:
			return false;
		}
		if (((uint)ProviderInfo.TestedSQLTypes & (uint)sQL_CVT) == 0)
		{
			int infoInt32Unhandled = GetInfoInt32Unhandled((ODBC32.SQL_INFO)infotype);
			infoInt32Unhandled &= (int)sQL_CVT;
			ProviderInfo.TestedSQLTypes |= (int)sQL_CVT;
			ProviderInfo.SupportedSQLTypes |= infoInt32Unhandled;
		}
		return ((uint)ProviderInfo.SupportedSQLTypes & (uint)sQL_CVT) != 0;
	}

	internal bool TestRestrictedSqlBindType(ODBC32.SQL_TYPE sqltype)
	{
		ODBC32.SQL_CVT sQL_CVT;
		switch (sqltype)
		{
		case ODBC32.SQL_TYPE.NUMERIC:
			sQL_CVT = ODBC32.SQL_CVT.NUMERIC;
			break;
		case ODBC32.SQL_TYPE.DECIMAL:
			sQL_CVT = ODBC32.SQL_CVT.DECIMAL;
			break;
		default:
			return false;
		}
		return ((uint)ProviderInfo.RestrictedSQLBindTypes & (uint)sQL_CVT) != 0;
	}

	protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
	{
		DbTransaction result = InnerConnection.BeginTransaction(isolationLevel);
		GC.KeepAlive(this);
		return result;
	}

	internal OdbcTransaction Open_BeginTransaction(IsolationLevel isolevel)
	{
		CheckState("BeginTransaction");
		RollbackDeadTransaction();
		if (_weakTransaction != null && _weakTransaction.IsAlive)
		{
			throw ADP.ParallelTransactionsNotSupported(this);
		}
		switch (isolevel)
		{
		case IsolationLevel.Chaos:
			throw ODBC.NotSupportedIsolationLevel(isolevel);
		default:
			throw ADP.InvalidIsolationLevel(isolevel);
		case IsolationLevel.Unspecified:
		case IsolationLevel.ReadUncommitted:
		case IsolationLevel.ReadCommitted:
		case IsolationLevel.RepeatableRead:
		case IsolationLevel.Serializable:
		case IsolationLevel.Snapshot:
		{
			OdbcConnectionHandle connectionHandle = ConnectionHandle;
			ODBC32.RetCode retCode = connectionHandle.BeginTransaction(ref isolevel);
			if (retCode == ODBC32.RetCode.ERROR)
			{
				HandleError(connectionHandle, retCode);
			}
			OdbcTransaction odbcTransaction = new OdbcTransaction(this, isolevel, connectionHandle);
			_weakTransaction = new WeakReference(odbcTransaction);
			return odbcTransaction;
		}
		}
	}

	internal void Open_ChangeDatabase(string value)
	{
		CheckState("ChangeDatabase");
		if (value == null || value.Trim().Length == 0)
		{
			throw ADP.EmptyDatabaseName();
		}
		if (1024 < value.Length * 2 + 2)
		{
			throw ADP.DatabaseNameTooLong();
		}
		RollbackDeadTransaction();
		OdbcConnectionHandle connectionHandle = ConnectionHandle;
		ODBC32.RetCode retCode = connectionHandle.SetConnectionAttribute3(ODBC32.SQL_ATTR.CURRENT_CATALOG, value, checked(value.Length * 2));
		if (retCode != ODBC32.RetCode.SUCCESS)
		{
			HandleError(connectionHandle, retCode);
		}
	}

	internal string Open_GetServerVersion()
	{
		return GetInfoStringUnhandled(ODBC32.SQL_INFO.DBMS_VER, handleError: true);
	}

	public OdbcConnection()
	{
		GC.SuppressFinalize(this);
		_innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
	}

	private void CopyFrom(OdbcConnection connection)
	{
		ADP.CheckArgumentNull(connection, "connection");
		_userConnectionOptions = connection.UserConnectionOptions;
		_poolGroup = connection.PoolGroup;
		if (DbConnectionClosedNeverOpened.SingletonInstance == connection._innerConnection)
		{
			_innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
		}
		else
		{
			_innerConnection = DbConnectionClosedPreviouslyOpened.SingletonInstance;
		}
	}

	private string ConnectionString_Get()
	{
		bool shouldHidePassword = InnerConnection.ShouldHidePassword;
		DbConnectionOptions userConnectionOptions = UserConnectionOptions;
		if (userConnectionOptions == null)
		{
			return "";
		}
		return userConnectionOptions.UsersConnectionString(shouldHidePassword);
	}

	private void ConnectionString_Set(string value)
	{
		DbConnectionPoolKey key = new DbConnectionPoolKey(value);
		ConnectionString_Set(key);
	}

	private void ConnectionString_Set(DbConnectionPoolKey key)
	{
		DbConnectionOptions userConnectionOptions = null;
		DbConnectionPoolGroup connectionPoolGroup = ConnectionFactory.GetConnectionPoolGroup(key, null, ref userConnectionOptions);
		DbConnectionInternal innerConnection = InnerConnection;
		bool flag = innerConnection.AllowSetConnectionString;
		if (flag)
		{
			flag = SetInnerConnectionFrom(DbConnectionClosedBusy.SingletonInstance, innerConnection);
			if (flag)
			{
				_userConnectionOptions = userConnectionOptions;
				_poolGroup = connectionPoolGroup;
				_innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
			}
		}
		if (!flag)
		{
			throw ADP.OpenConnectionPropertySet("ConnectionString", innerConnection.State);
		}
	}

	internal void Abort(Exception e)
	{
		DbConnectionInternal innerConnection = _innerConnection;
		if (ConnectionState.Open == innerConnection.State)
		{
			Interlocked.CompareExchange(ref _innerConnection, DbConnectionClosedPreviouslyOpened.SingletonInstance, innerConnection);
			innerConnection.DoomThisConnection();
		}
	}

	internal void AddWeakReference(object value, int tag)
	{
		InnerConnection.AddWeakReference(value, tag);
	}

	protected override DbCommand CreateDbCommand()
	{
		DbCommand dbCommand = ConnectionFactory.ProviderFactory.CreateCommand();
		dbCommand.Connection = this;
		return dbCommand;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_userConnectionOptions = null;
			_poolGroup = null;
			Close();
		}
		DisposeMe(disposing);
		base.Dispose(disposing);
	}

	public override DataTable GetSchema()
	{
		return GetSchema(DbMetaDataCollectionNames.MetaDataCollections, null);
	}

	public override DataTable GetSchema(string collectionName)
	{
		return GetSchema(collectionName, null);
	}

	public override DataTable GetSchema(string collectionName, string[] restrictionValues)
	{
		return InnerConnection.GetSchema(ConnectionFactory, PoolGroup, this, collectionName, restrictionValues);
	}

	internal void NotifyWeakReference(int message)
	{
		InnerConnection.NotifyWeakReference(message);
	}

	internal void PermissionDemand()
	{
		DbConnectionOptions dbConnectionOptions = PoolGroup?.ConnectionOptions;
		if (dbConnectionOptions == null || dbConnectionOptions.IsEmpty)
		{
			throw ADP.NoConnectionString();
		}
		_ = UserConnectionOptions;
	}

	internal void RemoveWeakReference(object value)
	{
		InnerConnection.RemoveWeakReference(value);
	}

	internal void SetInnerConnectionEvent(DbConnectionInternal to)
	{
		ConnectionState connectionState = _innerConnection.State & ConnectionState.Open;
		ConnectionState connectionState2 = to.State & ConnectionState.Open;
		if (connectionState != connectionState2 && connectionState2 == ConnectionState.Closed)
		{
			_closeCount++;
		}
		_innerConnection = to;
		if (connectionState == ConnectionState.Closed && ConnectionState.Open == connectionState2)
		{
			OnStateChange(DbConnectionInternal.StateChangeOpen);
		}
		else if (ConnectionState.Open == connectionState && connectionState2 == ConnectionState.Closed)
		{
			OnStateChange(DbConnectionInternal.StateChangeClosed);
		}
		else if (connectionState != connectionState2)
		{
			OnStateChange(new StateChangeEventArgs(connectionState, connectionState2));
		}
	}

	internal bool SetInnerConnectionFrom(DbConnectionInternal to, DbConnectionInternal from)
	{
		return from == Interlocked.CompareExchange(ref _innerConnection, to, from);
	}

	internal void SetInnerConnectionTo(DbConnectionInternal to)
	{
		_innerConnection = to;
	}

	public void EnlistDistributedTransaction(ITransaction transaction)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
