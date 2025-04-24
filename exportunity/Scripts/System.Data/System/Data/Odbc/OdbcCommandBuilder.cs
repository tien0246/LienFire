using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;

namespace System.Data.Odbc;

public sealed class OdbcCommandBuilder : DbCommandBuilder
{
	public new OdbcDataAdapter DataAdapter
	{
		get
		{
			return base.DataAdapter as OdbcDataAdapter;
		}
		set
		{
			base.DataAdapter = value;
		}
	}

	public OdbcCommandBuilder()
	{
		GC.SuppressFinalize(this);
	}

	public OdbcCommandBuilder(OdbcDataAdapter adapter)
		: this()
	{
		DataAdapter = adapter;
	}

	private void OdbcRowUpdatingHandler(object sender, OdbcRowUpdatingEventArgs ruevent)
	{
		RowUpdatingHandler(ruevent);
	}

	public new OdbcCommand GetInsertCommand()
	{
		return (OdbcCommand)base.GetInsertCommand();
	}

	public new OdbcCommand GetInsertCommand(bool useColumnsForParameterNames)
	{
		return (OdbcCommand)base.GetInsertCommand(useColumnsForParameterNames);
	}

	public new OdbcCommand GetUpdateCommand()
	{
		return (OdbcCommand)base.GetUpdateCommand();
	}

	public new OdbcCommand GetUpdateCommand(bool useColumnsForParameterNames)
	{
		return (OdbcCommand)base.GetUpdateCommand(useColumnsForParameterNames);
	}

	public new OdbcCommand GetDeleteCommand()
	{
		return (OdbcCommand)base.GetDeleteCommand();
	}

	public new OdbcCommand GetDeleteCommand(bool useColumnsForParameterNames)
	{
		return (OdbcCommand)base.GetDeleteCommand(useColumnsForParameterNames);
	}

	protected override string GetParameterName(int parameterOrdinal)
	{
		return "p" + parameterOrdinal.ToString(CultureInfo.InvariantCulture);
	}

	protected override string GetParameterName(string parameterName)
	{
		return parameterName;
	}

	protected override string GetParameterPlaceholder(int parameterOrdinal)
	{
		return "?";
	}

	protected override void ApplyParameterInfo(DbParameter parameter, DataRow datarow, StatementType statementType, bool whereClause)
	{
		OdbcParameter odbcParameter = (OdbcParameter)parameter;
		object obj = datarow[SchemaTableColumn.ProviderType];
		odbcParameter.OdbcType = (OdbcType)obj;
		object obj2 = datarow[SchemaTableColumn.NumericPrecision];
		if (DBNull.Value != obj2)
		{
			byte b = (byte)(short)obj2;
			odbcParameter.PrecisionInternal = (byte)((byte.MaxValue != b) ? b : 0);
		}
		obj2 = datarow[SchemaTableColumn.NumericScale];
		if (DBNull.Value != obj2)
		{
			byte b2 = (byte)(short)obj2;
			odbcParameter.ScaleInternal = (byte)((byte.MaxValue != b2) ? b2 : 0);
		}
	}

	public static void DeriveParameters(OdbcCommand command)
	{
		if (command == null)
		{
			throw ADP.ArgumentNull("command");
		}
		switch (command.CommandType)
		{
		case CommandType.Text:
			throw ADP.DeriveParametersNotSupported(command);
		case CommandType.TableDirect:
			throw ADP.DeriveParametersNotSupported(command);
		default:
			throw ADP.InvalidCommandType(command.CommandType);
		case CommandType.StoredProcedure:
		{
			if (string.IsNullOrEmpty(command.CommandText))
			{
				throw ADP.CommandTextRequired("DeriveParameters");
			}
			OdbcConnection connection = command.Connection;
			if (connection == null)
			{
				throw ADP.ConnectionRequired("DeriveParameters");
			}
			ConnectionState state = connection.State;
			if (ConnectionState.Open != state)
			{
				throw ADP.OpenConnectionRequired("DeriveParameters", state);
			}
			OdbcParameter[] array = DeriveParametersFromStoredProcedure(connection, command);
			OdbcParameterCollection parameters = command.Parameters;
			parameters.Clear();
			int num = array.Length;
			if (0 < num)
			{
				for (int i = 0; i < array.Length; i++)
				{
					parameters.Add(array[i]);
				}
			}
			break;
		}
		}
	}

	private static OdbcParameter[] DeriveParametersFromStoredProcedure(OdbcConnection connection, OdbcCommand command)
	{
		List<OdbcParameter> list = new List<OdbcParameter>();
		CMDWrapper statementHandle = command.GetStatementHandle();
		OdbcStatementHandle statementHandle2 = statementHandle.StatementHandle;
		string text = connection.QuoteChar("DeriveParameters");
		string[] array = MultipartIdentifier.ParseMultipartIdentifier(command.CommandText, text, text, '.', 4, removequotes: true, "OdbcCommandBuilder.DeriveParameters failed because the OdbcCommand.CommandText property value is an invalid multipart name", ThrowOnEmptyMultipartName: false);
		if (array[3] == null)
		{
			array[3] = command.CommandText;
		}
		ODBC32.RetCode retCode = statementHandle2.ProcedureColumns(array[1], array[2], array[3], null);
		if (retCode != ODBC32.RetCode.SUCCESS)
		{
			connection.HandleError(statementHandle2, retCode);
		}
		using (OdbcDataReader odbcDataReader = new OdbcDataReader(command, statementHandle, CommandBehavior.Default))
		{
			odbcDataReader.FirstResult();
			_ = odbcDataReader.FieldCount;
			while (odbcDataReader.Read())
			{
				OdbcParameter odbcParameter = new OdbcParameter();
				odbcParameter.ParameterName = odbcDataReader.GetString(3);
				switch ((ODBC32.SQL_PARAM)odbcDataReader.GetInt16(4))
				{
				case ODBC32.SQL_PARAM.INPUT:
					odbcParameter.Direction = ParameterDirection.Input;
					break;
				case ODBC32.SQL_PARAM.OUTPUT:
					odbcParameter.Direction = ParameterDirection.Output;
					break;
				case ODBC32.SQL_PARAM.INPUT_OUTPUT:
					odbcParameter.Direction = ParameterDirection.InputOutput;
					break;
				case ODBC32.SQL_PARAM.RETURN_VALUE:
					odbcParameter.Direction = ParameterDirection.ReturnValue;
					break;
				}
				odbcParameter.OdbcType = TypeMap.FromSqlType((ODBC32.SQL_TYPE)odbcDataReader.GetInt16(5))._odbcType;
				odbcParameter.Size = odbcDataReader.GetInt32(7);
				OdbcType odbcType = odbcParameter.OdbcType;
				if ((uint)(odbcType - 6) <= 1u)
				{
					odbcParameter.ScaleInternal = (byte)odbcDataReader.GetInt16(9);
					odbcParameter.PrecisionInternal = (byte)odbcDataReader.GetInt16(10);
				}
				list.Add(odbcParameter);
			}
		}
		retCode = statementHandle2.CloseCursor();
		return list.ToArray();
	}

	public override string QuoteIdentifier(string unquotedIdentifier)
	{
		return QuoteIdentifier(unquotedIdentifier, null);
	}

	public string QuoteIdentifier(string unquotedIdentifier, OdbcConnection connection)
	{
		ADP.CheckArgumentNull(unquotedIdentifier, "unquotedIdentifier");
		string text = QuotePrefix;
		string quoteSuffix = QuoteSuffix;
		if (string.IsNullOrEmpty(text))
		{
			if (connection == null)
			{
				connection = DataAdapter?.SelectCommand?.Connection;
				if (connection == null)
				{
					throw ADP.QuotePrefixNotSet("QuoteIdentifier");
				}
			}
			text = connection.QuoteChar("QuoteIdentifier");
			quoteSuffix = text;
		}
		if (!string.IsNullOrEmpty(text) && text != " ")
		{
			return ADP.BuildQuotedString(text, quoteSuffix, unquotedIdentifier);
		}
		return unquotedIdentifier;
	}

	protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
	{
		if (adapter == base.DataAdapter)
		{
			((OdbcDataAdapter)adapter).RowUpdating -= OdbcRowUpdatingHandler;
		}
		else
		{
			((OdbcDataAdapter)adapter).RowUpdating += OdbcRowUpdatingHandler;
		}
	}

	public override string UnquoteIdentifier(string quotedIdentifier)
	{
		return UnquoteIdentifier(quotedIdentifier, null);
	}

	public string UnquoteIdentifier(string quotedIdentifier, OdbcConnection connection)
	{
		ADP.CheckArgumentNull(quotedIdentifier, "quotedIdentifier");
		string text = QuotePrefix;
		string quoteSuffix = QuoteSuffix;
		if (string.IsNullOrEmpty(text))
		{
			if (connection == null)
			{
				connection = DataAdapter?.SelectCommand?.Connection;
				if (connection == null)
				{
					throw ADP.QuotePrefixNotSet("UnquoteIdentifier");
				}
			}
			text = connection.QuoteChar("UnquoteIdentifier");
			quoteSuffix = text;
		}
		if (!string.IsNullOrEmpty(text) || text != " ")
		{
			ADP.RemoveStringQuotes(text, quoteSuffix, quotedIdentifier, out var unquotedString);
			return unquotedString;
		}
		return quotedIdentifier;
	}
}
