using System.Data.Common;
using System.Data.ProviderBase;
using System.Globalization;
using System.IO;
using System.Text;

namespace System.Data.Odbc;

internal class OdbcMetaDataFactory : DbMetaDataFactory
{
	private readonly struct SchemaFunctionName
	{
		internal readonly string _schemaName;

		internal readonly ODBC32.SQL_API _odbcFunction;

		internal SchemaFunctionName(string schemaName, ODBC32.SQL_API odbcFunction)
		{
			_schemaName = schemaName;
			_odbcFunction = odbcFunction;
		}
	}

	private const string _collectionName = "CollectionName";

	private const string _populationMechanism = "PopulationMechanism";

	private const string _prepareCollection = "PrepareCollection";

	private readonly SchemaFunctionName[] _schemaMapping;

	internal static readonly char[] KeywordSeparatorChar = new char[1] { ',' };

	internal OdbcMetaDataFactory(Stream XMLStream, string serverVersion, string serverVersionNormalized, OdbcConnection connection)
		: base(XMLStream, serverVersion, serverVersionNormalized)
	{
		_schemaMapping = new SchemaFunctionName[8]
		{
			new SchemaFunctionName(DbMetaDataCollectionNames.DataTypes, ODBC32.SQL_API.SQLGETTYPEINFO),
			new SchemaFunctionName(OdbcMetaDataCollectionNames.Columns, ODBC32.SQL_API.SQLCOLUMNS),
			new SchemaFunctionName(OdbcMetaDataCollectionNames.Indexes, ODBC32.SQL_API.SQLSTATISTICS),
			new SchemaFunctionName(OdbcMetaDataCollectionNames.Procedures, ODBC32.SQL_API.SQLPROCEDURES),
			new SchemaFunctionName(OdbcMetaDataCollectionNames.ProcedureColumns, ODBC32.SQL_API.SQLPROCEDURECOLUMNS),
			new SchemaFunctionName(OdbcMetaDataCollectionNames.ProcedureParameters, ODBC32.SQL_API.SQLPROCEDURECOLUMNS),
			new SchemaFunctionName(OdbcMetaDataCollectionNames.Tables, ODBC32.SQL_API.SQLTABLES),
			new SchemaFunctionName(OdbcMetaDataCollectionNames.Views, ODBC32.SQL_API.SQLTABLES)
		};
		DataTable dataTable = base.CollectionDataSet.Tables[DbMetaDataCollectionNames.MetaDataCollections];
		if (dataTable == null)
		{
			throw ADP.UnableToBuildCollection(DbMetaDataCollectionNames.MetaDataCollections);
		}
		dataTable = CloneAndFilterCollection(DbMetaDataCollectionNames.MetaDataCollections, null);
		DataTable dataTable2 = base.CollectionDataSet.Tables[DbMetaDataCollectionNames.Restrictions];
		if (dataTable2 != null)
		{
			dataTable2 = CloneAndFilterCollection(DbMetaDataCollectionNames.Restrictions, null);
		}
		DataColumn column = dataTable.Columns["PopulationMechanism"];
		DataColumn column2 = dataTable.Columns["CollectionName"];
		DataColumn column3 = null;
		if (dataTable2 != null)
		{
			column3 = dataTable2.Columns["CollectionName"];
		}
		foreach (DataRow row in dataTable.Rows)
		{
			if (!((string)row[column] == "PrepareCollection"))
			{
				continue;
			}
			int num = -1;
			for (int i = 0; i < _schemaMapping.Length; i++)
			{
				if (_schemaMapping[i]._schemaName == (string)row[column2])
				{
					num = i;
					break;
				}
			}
			if (num == -1 || connection.SQLGetFunctions(_schemaMapping[num]._odbcFunction))
			{
				continue;
			}
			if (dataTable2 != null)
			{
				foreach (DataRow row2 in dataTable2.Rows)
				{
					if ((string)row[column2] == (string)row2[column3])
					{
						row2.Delete();
					}
				}
				dataTable2.AcceptChanges();
			}
			row.Delete();
		}
		dataTable.AcceptChanges();
		base.CollectionDataSet.Tables.Remove(base.CollectionDataSet.Tables[DbMetaDataCollectionNames.MetaDataCollections]);
		base.CollectionDataSet.Tables.Add(dataTable);
		if (dataTable2 != null)
		{
			base.CollectionDataSet.Tables.Remove(base.CollectionDataSet.Tables[DbMetaDataCollectionNames.Restrictions]);
			base.CollectionDataSet.Tables.Add(dataTable2);
		}
	}

	private object BooleanFromODBC(object odbcSource)
	{
		if (odbcSource != DBNull.Value)
		{
			if (Convert.ToInt32(odbcSource, null) == 0)
			{
				return false;
			}
			return true;
		}
		return DBNull.Value;
	}

	private OdbcCommand GetCommand(OdbcConnection connection)
	{
		OdbcCommand odbcCommand = connection.CreateCommand();
		odbcCommand.Transaction = connection.LocalTransaction;
		return odbcCommand;
	}

	private DataTable DataTableFromDataReader(IDataReader reader, string tableName)
	{
		object[] values;
		DataTable dataTable = NewDataTableFromReader(reader, out values, tableName);
		while (reader.Read())
		{
			reader.GetValues(values);
			dataTable.Rows.Add(values);
		}
		return dataTable;
	}

	private void DataTableFromDataReaderDataTypes(DataTable dataTypesTable, OdbcDataReader dataReader, OdbcConnection connection)
	{
		DataTable dataTable = null;
		dataTable = dataReader.GetSchemaTable();
		if (dataTable == null)
		{
			throw ADP.OdbcNoTypesFromProvider();
		}
		object[] array = new object[dataTable.Rows.Count];
		DataColumn column = dataTypesTable.Columns[DbMetaDataColumnNames.TypeName];
		DataColumn column2 = dataTypesTable.Columns[DbMetaDataColumnNames.ProviderDbType];
		DataColumn column3 = dataTypesTable.Columns[DbMetaDataColumnNames.ColumnSize];
		DataColumn column4 = dataTypesTable.Columns[DbMetaDataColumnNames.CreateParameters];
		DataColumn column5 = dataTypesTable.Columns[DbMetaDataColumnNames.DataType];
		DataColumn column6 = dataTypesTable.Columns[DbMetaDataColumnNames.IsAutoIncrementable];
		DataColumn column7 = dataTypesTable.Columns[DbMetaDataColumnNames.IsCaseSensitive];
		DataColumn column8 = dataTypesTable.Columns[DbMetaDataColumnNames.IsFixedLength];
		DataColumn column9 = dataTypesTable.Columns[DbMetaDataColumnNames.IsFixedPrecisionScale];
		DataColumn column10 = dataTypesTable.Columns[DbMetaDataColumnNames.IsLong];
		DataColumn column11 = dataTypesTable.Columns[DbMetaDataColumnNames.IsNullable];
		DataColumn column12 = dataTypesTable.Columns[DbMetaDataColumnNames.IsSearchable];
		DataColumn column13 = dataTypesTable.Columns[DbMetaDataColumnNames.IsSearchableWithLike];
		DataColumn column14 = dataTypesTable.Columns[DbMetaDataColumnNames.IsUnsigned];
		DataColumn column15 = dataTypesTable.Columns[DbMetaDataColumnNames.MaximumScale];
		DataColumn column16 = dataTypesTable.Columns[DbMetaDataColumnNames.MinimumScale];
		DataColumn column17 = dataTypesTable.Columns[DbMetaDataColumnNames.LiteralPrefix];
		DataColumn column18 = dataTypesTable.Columns[DbMetaDataColumnNames.LiteralSuffix];
		DataColumn column19 = dataTypesTable.Columns[OdbcMetaDataColumnNames.SQLType];
		while (dataReader.Read())
		{
			dataReader.GetValues(array);
			DataRow dataRow = dataTypesTable.NewRow();
			dataRow[column] = array[0];
			dataRow[column19] = array[1];
			ODBC32.SQL_TYPE sQL_TYPE = (ODBC32.SQL_TYPE)(int)Convert.ChangeType(array[1], typeof(int), null);
			if (!connection.IsV3Driver)
			{
				switch (sQL_TYPE)
				{
				case (ODBC32.SQL_TYPE)9:
					sQL_TYPE = ODBC32.SQL_TYPE.TYPE_DATE;
					break;
				case (ODBC32.SQL_TYPE)10:
					sQL_TYPE = ODBC32.SQL_TYPE.TYPE_TIME;
					break;
				}
			}
			TypeMap typeMap;
			try
			{
				typeMap = TypeMap.FromSqlType(sQL_TYPE);
			}
			catch (ArgumentException)
			{
				typeMap = null;
			}
			if (typeMap != null)
			{
				dataRow[column2] = typeMap._odbcType;
				dataRow[column5] = typeMap._type.FullName;
				switch (sQL_TYPE)
				{
				case ODBC32.SQL_TYPE.SS_XML:
				case ODBC32.SQL_TYPE.WLONGVARCHAR:
				case ODBC32.SQL_TYPE.LONGVARBINARY:
				case ODBC32.SQL_TYPE.LONGVARCHAR:
					dataRow[column10] = true;
					dataRow[column8] = false;
					break;
				case ODBC32.SQL_TYPE.WVARCHAR:
				case ODBC32.SQL_TYPE.VARBINARY:
				case ODBC32.SQL_TYPE.VARCHAR:
					dataRow[column10] = false;
					dataRow[column8] = false;
					break;
				case ODBC32.SQL_TYPE.SS_TIME_EX:
				case ODBC32.SQL_TYPE.SS_UTCDATETIME:
				case ODBC32.SQL_TYPE.SS_VARIANT:
				case ODBC32.SQL_TYPE.GUID:
				case ODBC32.SQL_TYPE.WCHAR:
				case ODBC32.SQL_TYPE.BIT:
				case ODBC32.SQL_TYPE.TINYINT:
				case ODBC32.SQL_TYPE.BIGINT:
				case ODBC32.SQL_TYPE.BINARY:
				case ODBC32.SQL_TYPE.CHAR:
				case ODBC32.SQL_TYPE.NUMERIC:
				case ODBC32.SQL_TYPE.DECIMAL:
				case ODBC32.SQL_TYPE.INTEGER:
				case ODBC32.SQL_TYPE.SMALLINT:
				case ODBC32.SQL_TYPE.FLOAT:
				case ODBC32.SQL_TYPE.REAL:
				case ODBC32.SQL_TYPE.DOUBLE:
				case ODBC32.SQL_TYPE.TIMESTAMP:
				case ODBC32.SQL_TYPE.TYPE_DATE:
				case ODBC32.SQL_TYPE.TYPE_TIME:
				case ODBC32.SQL_TYPE.TYPE_TIMESTAMP:
					dataRow[column10] = false;
					dataRow[column8] = true;
					break;
				}
			}
			dataRow[column3] = array[2];
			dataRow[column4] = array[5];
			if (array[11] == DBNull.Value || Convert.ToInt16(array[11], null) == 0)
			{
				dataRow[column6] = false;
			}
			else
			{
				dataRow[column6] = true;
			}
			dataRow[column7] = BooleanFromODBC(array[7]);
			dataRow[column9] = BooleanFromODBC(array[10]);
			if (array[6] != DBNull.Value)
			{
				switch ((ODBC32.SQL_NULLABILITY)(ushort)Convert.ToInt16(array[6], null))
				{
				case ODBC32.SQL_NULLABILITY.NO_NULLS:
					dataRow[column11] = false;
					break;
				case ODBC32.SQL_NULLABILITY.NULLABLE:
					dataRow[column11] = true;
					break;
				case ODBC32.SQL_NULLABILITY.UNKNOWN:
					dataRow[column11] = DBNull.Value;
					break;
				}
			}
			if (DBNull.Value != array[8])
			{
				switch (Convert.ToInt16(array[8], null))
				{
				case 0:
					dataRow[column12] = false;
					dataRow[column13] = false;
					break;
				case 1:
					dataRow[column12] = false;
					dataRow[column13] = true;
					break;
				case 2:
					dataRow[column12] = true;
					dataRow[column13] = false;
					break;
				case 3:
					dataRow[column12] = true;
					dataRow[column13] = true;
					break;
				}
			}
			dataRow[column14] = BooleanFromODBC(array[9]);
			if (array[14] != DBNull.Value)
			{
				dataRow[column15] = array[14];
			}
			if (array[13] != DBNull.Value)
			{
				dataRow[column16] = array[13];
			}
			if (array[3] != DBNull.Value)
			{
				dataRow[column17] = array[3];
			}
			if (array[4] != DBNull.Value)
			{
				dataRow[column18] = array[4];
			}
			dataTypesTable.Rows.Add(dataRow);
		}
	}

	private DataTable DataTableFromDataReaderIndex(IDataReader reader, string tableName, string restrictionIndexName)
	{
		object[] values;
		DataTable dataTable = NewDataTableFromReader(reader, out values, tableName);
		int num = 6;
		int num2 = 5;
		while (reader.Read())
		{
			reader.GetValues(values);
			if (IncludeIndexRow(values[num2], restrictionIndexName, Convert.ToInt16(values[num], null)))
			{
				dataTable.Rows.Add(values);
			}
		}
		return dataTable;
	}

	private DataTable DataTableFromDataReaderProcedureColumns(IDataReader reader, string tableName, bool isColumn)
	{
		object[] values;
		DataTable dataTable = NewDataTableFromReader(reader, out values, tableName);
		int num = 4;
		while (reader.Read())
		{
			reader.GetValues(values);
			if (values[num].GetType() == typeof(short) && (((short)values[num] == 3 && isColumn) || ((short)values[num] != 3 && !isColumn)))
			{
				dataTable.Rows.Add(values);
			}
		}
		return dataTable;
	}

	private DataTable DataTableFromDataReaderProcedures(IDataReader reader, string tableName, short procedureType)
	{
		object[] values;
		DataTable dataTable = NewDataTableFromReader(reader, out values, tableName);
		int num = 7;
		while (reader.Read())
		{
			reader.GetValues(values);
			if (values[num].GetType() == typeof(short) && (short)values[num] == procedureType)
			{
				dataTable.Rows.Add(values);
			}
		}
		return dataTable;
	}

	private void FillOutRestrictions(int restrictionsCount, string[] restrictions, object[] allRestrictions, string collectionName)
	{
		int i = 0;
		if (restrictions != null)
		{
			if (restrictions.Length > restrictionsCount)
			{
				throw ADP.TooManyRestrictions(collectionName);
			}
			for (i = 0; i < restrictions.Length; i++)
			{
				if (restrictions[i] != null)
				{
					allRestrictions[i] = restrictions[i];
				}
			}
		}
		for (; i < restrictionsCount; i++)
		{
			allRestrictions[i] = null;
		}
	}

	private DataTable GetColumnsCollection(string[] restrictions, OdbcConnection connection)
	{
		OdbcCommand odbcCommand = null;
		OdbcDataReader odbcDataReader = null;
		DataTable dataTable = null;
		try
		{
			odbcCommand = GetCommand(connection);
			string[] array = new string[4];
			object[] allRestrictions = array;
			FillOutRestrictions(4, restrictions, allRestrictions, OdbcMetaDataCollectionNames.Columns);
			OdbcCommand odbcCommand2 = odbcCommand;
			allRestrictions = array;
			odbcDataReader = odbcCommand2.ExecuteReaderFromSQLMethod(allRestrictions, ODBC32.SQL_API.SQLCOLUMNS);
			return DataTableFromDataReader(odbcDataReader, OdbcMetaDataCollectionNames.Columns);
		}
		finally
		{
			odbcDataReader?.Dispose();
			odbcCommand?.Dispose();
		}
	}

	private DataTable GetDataSourceInformationCollection(string[] restrictions, OdbcConnection connection)
	{
		if (!ADP.IsEmptyArray(restrictions))
		{
			throw ADP.TooManyRestrictions(DbMetaDataCollectionNames.DataSourceInformation);
		}
		DataTable dataTable = base.CollectionDataSet.Tables[DbMetaDataCollectionNames.DataSourceInformation];
		if (dataTable == null)
		{
			throw ADP.UnableToBuildCollection(DbMetaDataCollectionNames.DataSourceInformation);
		}
		dataTable = CloneAndFilterCollection(DbMetaDataCollectionNames.DataSourceInformation, null);
		if (dataTable.Rows.Count != 1)
		{
			throw ADP.IncorrectNumberOfDataSourceInformationRows();
		}
		DataRow dataRow = dataTable.Rows[0];
		string infoStringUnhandled = connection.GetInfoStringUnhandled(ODBC32.SQL_INFO.CATALOG_NAME_SEPARATOR);
		if (!string.IsNullOrEmpty(infoStringUnhandled))
		{
			StringBuilder stringBuilder = new StringBuilder();
			ADP.EscapeSpecialCharacters(infoStringUnhandled, stringBuilder);
			dataRow[DbMetaDataColumnNames.CompositeIdentifierSeparatorPattern] = stringBuilder.ToString();
		}
		infoStringUnhandled = connection.GetInfoStringUnhandled(ODBC32.SQL_INFO.DBMS_NAME);
		if (infoStringUnhandled != null)
		{
			dataRow[DbMetaDataColumnNames.DataSourceProductName] = infoStringUnhandled;
		}
		dataRow[DbMetaDataColumnNames.DataSourceProductVersion] = base.ServerVersion;
		dataRow[DbMetaDataColumnNames.DataSourceProductVersionNormalized] = base.ServerVersionNormalized;
		dataRow[DbMetaDataColumnNames.ParameterMarkerFormat] = "?";
		dataRow[DbMetaDataColumnNames.ParameterMarkerPattern] = "\\?";
		dataRow[DbMetaDataColumnNames.ParameterNameMaxLength] = 0;
		ODBC32.RetCode retCode = ((!connection.IsV3Driver) ? connection.GetInfoInt32Unhandled(ODBC32.SQL_INFO.SQL_OJ_CAPABILITIES_20, out var resultValue) : connection.GetInfoInt32Unhandled(ODBC32.SQL_INFO.SQL_OJ_CAPABILITIES_30, out resultValue));
		if (retCode == ODBC32.RetCode.SUCCESS || retCode == ODBC32.RetCode.SUCCESS_WITH_INFO)
		{
			SupportedJoinOperators supportedJoinOperators = SupportedJoinOperators.None;
			if ((resultValue & 1) != 0)
			{
				supportedJoinOperators |= SupportedJoinOperators.LeftOuter;
			}
			if ((resultValue & 2) != 0)
			{
				supportedJoinOperators |= SupportedJoinOperators.RightOuter;
			}
			if ((resultValue & 4) != 0)
			{
				supportedJoinOperators |= SupportedJoinOperators.FullOuter;
			}
			if ((resultValue & 0x20) != 0)
			{
				supportedJoinOperators |= SupportedJoinOperators.Inner;
			}
			dataRow[DbMetaDataColumnNames.SupportedJoinOperators] = supportedJoinOperators;
		}
		retCode = connection.GetInfoInt16Unhandled(ODBC32.SQL_INFO.GROUP_BY, out var resultValue2);
		GroupByBehavior groupByBehavior = GroupByBehavior.Unknown;
		if (retCode == ODBC32.RetCode.SUCCESS || retCode == ODBC32.RetCode.SUCCESS_WITH_INFO)
		{
			switch (resultValue2)
			{
			case 0:
				groupByBehavior = GroupByBehavior.NotSupported;
				break;
			case 1:
				groupByBehavior = GroupByBehavior.ExactMatch;
				break;
			case 2:
				groupByBehavior = GroupByBehavior.MustContainAll;
				break;
			case 3:
				groupByBehavior = GroupByBehavior.Unrelated;
				break;
			}
		}
		dataRow[DbMetaDataColumnNames.GroupByBehavior] = groupByBehavior;
		retCode = connection.GetInfoInt16Unhandled(ODBC32.SQL_INFO.IDENTIFIER_CASE, out resultValue2);
		IdentifierCase identifierCase = IdentifierCase.Unknown;
		if (retCode == ODBC32.RetCode.SUCCESS || retCode == ODBC32.RetCode.SUCCESS_WITH_INFO)
		{
			switch (resultValue2)
			{
			case 3:
				identifierCase = IdentifierCase.Sensitive;
				break;
			case 1:
			case 2:
			case 4:
				identifierCase = IdentifierCase.Insensitive;
				break;
			}
		}
		dataRow[DbMetaDataColumnNames.IdentifierCase] = identifierCase;
		switch (connection.GetInfoStringUnhandled(ODBC32.SQL_INFO.ORDER_BY_COLUMNS_IN_SELECT))
		{
		case "Y":
			dataRow[DbMetaDataColumnNames.OrderByColumnsInSelect] = true;
			break;
		case "N":
			dataRow[DbMetaDataColumnNames.OrderByColumnsInSelect] = false;
			break;
		}
		infoStringUnhandled = connection.QuoteChar("GetSchema");
		if (infoStringUnhandled != null && infoStringUnhandled != " " && infoStringUnhandled.Length == 1)
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			ADP.EscapeSpecialCharacters(infoStringUnhandled, stringBuilder2);
			string value = stringBuilder2.ToString();
			stringBuilder2.Length = 0;
			ADP.EscapeSpecialCharacters(infoStringUnhandled, stringBuilder2);
			stringBuilder2.Append("(([^");
			stringBuilder2.Append(value);
			stringBuilder2.Append("]|");
			stringBuilder2.Append(value);
			stringBuilder2.Append(value);
			stringBuilder2.Append(")*)");
			stringBuilder2.Append(value);
			dataRow[DbMetaDataColumnNames.QuotedIdentifierPattern] = stringBuilder2.ToString();
		}
		retCode = connection.GetInfoInt16Unhandled(ODBC32.SQL_INFO.QUOTED_IDENTIFIER_CASE, out resultValue2);
		IdentifierCase identifierCase2 = IdentifierCase.Unknown;
		if (retCode == ODBC32.RetCode.SUCCESS || retCode == ODBC32.RetCode.SUCCESS_WITH_INFO)
		{
			switch (resultValue2)
			{
			case 3:
				identifierCase2 = IdentifierCase.Sensitive;
				break;
			case 1:
			case 2:
			case 4:
				identifierCase2 = IdentifierCase.Insensitive;
				break;
			}
		}
		dataRow[DbMetaDataColumnNames.QuotedIdentifierCase] = identifierCase2;
		dataTable.AcceptChanges();
		return dataTable;
	}

	private DataTable GetDataTypesCollection(string[] restrictions, OdbcConnection connection)
	{
		if (!ADP.IsEmptyArray(restrictions))
		{
			throw ADP.TooManyRestrictions(DbMetaDataCollectionNames.DataTypes);
		}
		DataTable dataTable = base.CollectionDataSet.Tables[DbMetaDataCollectionNames.DataTypes];
		if (dataTable == null)
		{
			throw ADP.UnableToBuildCollection(DbMetaDataCollectionNames.DataTypes);
		}
		dataTable = CloneAndFilterCollection(DbMetaDataCollectionNames.DataTypes, null);
		OdbcCommand odbcCommand = null;
		OdbcDataReader odbcDataReader = null;
		object[] methodArguments = new object[1] { (short)0 };
		try
		{
			odbcCommand = GetCommand(connection);
			odbcDataReader = odbcCommand.ExecuteReaderFromSQLMethod(methodArguments, ODBC32.SQL_API.SQLGETTYPEINFO);
			DataTableFromDataReaderDataTypes(dataTable, odbcDataReader, connection);
		}
		finally
		{
			odbcDataReader?.Dispose();
			odbcCommand?.Dispose();
		}
		dataTable.AcceptChanges();
		return dataTable;
	}

	private DataTable GetIndexCollection(string[] restrictions, OdbcConnection connection)
	{
		OdbcCommand odbcCommand = null;
		OdbcDataReader odbcDataReader = null;
		DataTable dataTable = null;
		try
		{
			odbcCommand = GetCommand(connection);
			object[] array = new object[5];
			FillOutRestrictions(4, restrictions, array, OdbcMetaDataCollectionNames.Indexes);
			if (array[2] == null)
			{
				throw ODBC.GetSchemaRestrictionRequired();
			}
			array[3] = (short)1;
			array[4] = (short)1;
			odbcDataReader = odbcCommand.ExecuteReaderFromSQLMethod(array, ODBC32.SQL_API.SQLSTATISTICS);
			string restrictionIndexName = null;
			if (restrictions != null && restrictions.Length >= 4)
			{
				restrictionIndexName = restrictions[3];
			}
			return DataTableFromDataReaderIndex(odbcDataReader, OdbcMetaDataCollectionNames.Indexes, restrictionIndexName);
		}
		finally
		{
			odbcDataReader?.Dispose();
			odbcCommand?.Dispose();
		}
	}

	private DataTable GetProcedureColumnsCollection(string[] restrictions, OdbcConnection connection, bool isColumns)
	{
		OdbcCommand odbcCommand = null;
		OdbcDataReader odbcDataReader = null;
		DataTable dataTable = null;
		try
		{
			odbcCommand = GetCommand(connection);
			string[] array = new string[4];
			object[] allRestrictions = array;
			FillOutRestrictions(4, restrictions, allRestrictions, OdbcMetaDataCollectionNames.Columns);
			OdbcCommand odbcCommand2 = odbcCommand;
			allRestrictions = array;
			odbcDataReader = odbcCommand2.ExecuteReaderFromSQLMethod(allRestrictions, ODBC32.SQL_API.SQLPROCEDURECOLUMNS);
			string tableName = ((!isColumns) ? OdbcMetaDataCollectionNames.ProcedureParameters : OdbcMetaDataCollectionNames.ProcedureColumns);
			return DataTableFromDataReaderProcedureColumns(odbcDataReader, tableName, isColumns);
		}
		finally
		{
			odbcDataReader?.Dispose();
			odbcCommand?.Dispose();
		}
	}

	private DataTable GetProceduresCollection(string[] restrictions, OdbcConnection connection)
	{
		OdbcCommand odbcCommand = null;
		OdbcDataReader odbcDataReader = null;
		DataTable result = null;
		try
		{
			odbcCommand = GetCommand(connection);
			string[] array = new string[4];
			object[] allRestrictions = array;
			FillOutRestrictions(4, restrictions, allRestrictions, OdbcMetaDataCollectionNames.Procedures);
			OdbcCommand odbcCommand2 = odbcCommand;
			allRestrictions = array;
			odbcDataReader = odbcCommand2.ExecuteReaderFromSQLMethod(allRestrictions, ODBC32.SQL_API.SQLPROCEDURES);
			if (array[3] == null)
			{
				result = DataTableFromDataReader(odbcDataReader, OdbcMetaDataCollectionNames.Procedures);
			}
			else
			{
				short procedureType;
				if (restrictions[3] == "SQL_PT_UNKNOWN" || restrictions[3] == "0")
				{
					procedureType = 0;
				}
				else if (restrictions[3] == "SQL_PT_PROCEDURE" || restrictions[3] == "1")
				{
					procedureType = 1;
				}
				else
				{
					if (!(restrictions[3] == "SQL_PT_FUNCTION") && !(restrictions[3] == "2"))
					{
						throw ADP.InvalidRestrictionValue(OdbcMetaDataCollectionNames.Procedures, "PROCEDURE_TYPE", restrictions[3]);
					}
					procedureType = 2;
				}
				result = DataTableFromDataReaderProcedures(odbcDataReader, OdbcMetaDataCollectionNames.Procedures, procedureType);
			}
		}
		finally
		{
			odbcDataReader?.Dispose();
			odbcCommand?.Dispose();
		}
		return result;
	}

	private DataTable GetReservedWordsCollection(string[] restrictions, OdbcConnection connection)
	{
		if (!ADP.IsEmptyArray(restrictions))
		{
			throw ADP.TooManyRestrictions(DbMetaDataCollectionNames.ReservedWords);
		}
		DataTable dataTable = base.CollectionDataSet.Tables[DbMetaDataCollectionNames.ReservedWords];
		if (dataTable == null)
		{
			throw ADP.UnableToBuildCollection(DbMetaDataCollectionNames.ReservedWords);
		}
		dataTable = CloneAndFilterCollection(DbMetaDataCollectionNames.ReservedWords, null);
		DataColumn dataColumn = dataTable.Columns[DbMetaDataColumnNames.ReservedWord];
		if (dataColumn == null)
		{
			throw ADP.UnableToBuildCollection(DbMetaDataCollectionNames.ReservedWords);
		}
		string infoStringUnhandled = connection.GetInfoStringUnhandled(ODBC32.SQL_INFO.KEYWORDS);
		if (infoStringUnhandled != null)
		{
			string[] array = infoStringUnhandled.Split(KeywordSeparatorChar);
			for (int i = 0; i < array.Length; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[dataColumn] = array[i];
				dataTable.Rows.Add(dataRow);
				dataRow.AcceptChanges();
			}
		}
		return dataTable;
	}

	private DataTable GetTablesCollection(string[] restrictions, OdbcConnection connection, bool isTables)
	{
		OdbcCommand odbcCommand = null;
		OdbcDataReader odbcDataReader = null;
		DataTable dataTable = null;
		try
		{
			odbcCommand = GetCommand(connection);
			string[] array = new string[4];
			string text;
			string text2;
			if (isTables)
			{
				text = "TABLE,SYSTEM TABLE";
				text2 = OdbcMetaDataCollectionNames.Tables;
			}
			else
			{
				text = "VIEW";
				text2 = OdbcMetaDataCollectionNames.Views;
			}
			object[] allRestrictions = array;
			FillOutRestrictions(3, restrictions, allRestrictions, text2);
			array[3] = text;
			OdbcCommand odbcCommand2 = odbcCommand;
			allRestrictions = array;
			odbcDataReader = odbcCommand2.ExecuteReaderFromSQLMethod(allRestrictions, ODBC32.SQL_API.SQLTABLES);
			return DataTableFromDataReader(odbcDataReader, text2);
		}
		finally
		{
			odbcDataReader?.Dispose();
			odbcCommand?.Dispose();
		}
	}

	private bool IncludeIndexRow(object rowIndexName, string restrictionIndexName, short rowIndexType)
	{
		if (rowIndexType == 0)
		{
			return false;
		}
		if (restrictionIndexName != null && restrictionIndexName != (string)rowIndexName)
		{
			return false;
		}
		return true;
	}

	private DataTable NewDataTableFromReader(IDataReader reader, out object[] values, string tableName)
	{
		DataTable dataTable = new DataTable(tableName);
		dataTable.Locale = CultureInfo.InvariantCulture;
		foreach (DataRow row in reader.GetSchemaTable().Rows)
		{
			dataTable.Columns.Add(row["ColumnName"] as string, (Type)row["DataType"]);
		}
		values = new object[dataTable.Columns.Count];
		return dataTable;
	}

	protected override DataTable PrepareCollection(string collectionName, string[] restrictions, DbConnection connection)
	{
		DataTable dataTable = null;
		OdbcConnection connection2 = (OdbcConnection)connection;
		if (collectionName == OdbcMetaDataCollectionNames.Tables)
		{
			dataTable = GetTablesCollection(restrictions, connection2, isTables: true);
		}
		else if (collectionName == OdbcMetaDataCollectionNames.Views)
		{
			dataTable = GetTablesCollection(restrictions, connection2, isTables: false);
		}
		else if (collectionName == OdbcMetaDataCollectionNames.Columns)
		{
			dataTable = GetColumnsCollection(restrictions, connection2);
		}
		else if (collectionName == OdbcMetaDataCollectionNames.Procedures)
		{
			dataTable = GetProceduresCollection(restrictions, connection2);
		}
		else if (collectionName == OdbcMetaDataCollectionNames.ProcedureColumns)
		{
			dataTable = GetProcedureColumnsCollection(restrictions, connection2, isColumns: true);
		}
		else if (collectionName == OdbcMetaDataCollectionNames.ProcedureParameters)
		{
			dataTable = GetProcedureColumnsCollection(restrictions, connection2, isColumns: false);
		}
		else if (collectionName == OdbcMetaDataCollectionNames.Indexes)
		{
			dataTable = GetIndexCollection(restrictions, connection2);
		}
		else if (collectionName == DbMetaDataCollectionNames.DataTypes)
		{
			dataTable = GetDataTypesCollection(restrictions, connection2);
		}
		else if (collectionName == DbMetaDataCollectionNames.DataSourceInformation)
		{
			dataTable = GetDataSourceInformationCollection(restrictions, connection2);
		}
		else if (collectionName == DbMetaDataCollectionNames.ReservedWords)
		{
			dataTable = GetReservedWordsCollection(restrictions, connection2);
		}
		if (dataTable == null)
		{
			throw ADP.UnableToBuildCollection(collectionName);
		}
		return dataTable;
	}
}
