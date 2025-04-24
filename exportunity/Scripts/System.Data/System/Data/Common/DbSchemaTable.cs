namespace System.Data.Common;

internal sealed class DbSchemaTable
{
	private enum ColumnEnum
	{
		ColumnName = 0,
		ColumnOrdinal = 1,
		ColumnSize = 2,
		BaseServerName = 3,
		BaseCatalogName = 4,
		BaseColumnName = 5,
		BaseSchemaName = 6,
		BaseTableName = 7,
		IsAutoIncrement = 8,
		IsUnique = 9,
		IsKey = 10,
		IsRowVersion = 11,
		DataType = 12,
		ProviderSpecificDataType = 13,
		AllowDBNull = 14,
		ProviderType = 15,
		IsExpression = 16,
		IsHidden = 17,
		IsLong = 18,
		IsReadOnly = 19,
		SchemaMappingUnsortedIndex = 20
	}

	private static readonly string[] s_DBCOLUMN_NAME = new string[21]
	{
		SchemaTableColumn.ColumnName,
		SchemaTableColumn.ColumnOrdinal,
		SchemaTableColumn.ColumnSize,
		SchemaTableOptionalColumn.BaseServerName,
		SchemaTableOptionalColumn.BaseCatalogName,
		SchemaTableColumn.BaseColumnName,
		SchemaTableColumn.BaseSchemaName,
		SchemaTableColumn.BaseTableName,
		SchemaTableOptionalColumn.IsAutoIncrement,
		SchemaTableColumn.IsUnique,
		SchemaTableColumn.IsKey,
		SchemaTableOptionalColumn.IsRowVersion,
		SchemaTableColumn.DataType,
		SchemaTableOptionalColumn.ProviderSpecificDataType,
		SchemaTableColumn.AllowDBNull,
		SchemaTableColumn.ProviderType,
		SchemaTableColumn.IsExpression,
		SchemaTableOptionalColumn.IsHidden,
		SchemaTableColumn.IsLong,
		SchemaTableOptionalColumn.IsReadOnly,
		"SchemaMapping Unsorted Index"
	};

	internal DataTable _dataTable;

	private DataColumnCollection _columns;

	private DataColumn[] _columnCache = new DataColumn[s_DBCOLUMN_NAME.Length];

	private bool _returnProviderSpecificTypes;

	internal DataColumn ColumnName => CachedDataColumn(ColumnEnum.ColumnName);

	internal DataColumn Size => CachedDataColumn(ColumnEnum.ColumnSize);

	internal DataColumn BaseServerName => CachedDataColumn(ColumnEnum.BaseServerName);

	internal DataColumn BaseColumnName => CachedDataColumn(ColumnEnum.BaseColumnName);

	internal DataColumn BaseTableName => CachedDataColumn(ColumnEnum.BaseTableName);

	internal DataColumn BaseCatalogName => CachedDataColumn(ColumnEnum.BaseCatalogName);

	internal DataColumn BaseSchemaName => CachedDataColumn(ColumnEnum.BaseSchemaName);

	internal DataColumn IsAutoIncrement => CachedDataColumn(ColumnEnum.IsAutoIncrement);

	internal DataColumn IsUnique => CachedDataColumn(ColumnEnum.IsUnique);

	internal DataColumn IsKey => CachedDataColumn(ColumnEnum.IsKey);

	internal DataColumn IsRowVersion => CachedDataColumn(ColumnEnum.IsRowVersion);

	internal DataColumn AllowDBNull => CachedDataColumn(ColumnEnum.AllowDBNull);

	internal DataColumn IsExpression => CachedDataColumn(ColumnEnum.IsExpression);

	internal DataColumn IsHidden => CachedDataColumn(ColumnEnum.IsHidden);

	internal DataColumn IsLong => CachedDataColumn(ColumnEnum.IsLong);

	internal DataColumn IsReadOnly => CachedDataColumn(ColumnEnum.IsReadOnly);

	internal DataColumn UnsortedIndex => CachedDataColumn(ColumnEnum.SchemaMappingUnsortedIndex);

	internal DataColumn DataType
	{
		get
		{
			if (_returnProviderSpecificTypes)
			{
				return CachedDataColumn(ColumnEnum.ProviderSpecificDataType, ColumnEnum.DataType);
			}
			return CachedDataColumn(ColumnEnum.DataType);
		}
	}

	internal DbSchemaTable(DataTable dataTable, bool returnProviderSpecificTypes)
	{
		_dataTable = dataTable;
		_columns = dataTable.Columns;
		_returnProviderSpecificTypes = returnProviderSpecificTypes;
	}

	private DataColumn CachedDataColumn(ColumnEnum column)
	{
		return CachedDataColumn(column, column);
	}

	private DataColumn CachedDataColumn(ColumnEnum column, ColumnEnum column2)
	{
		DataColumn dataColumn = _columnCache[(int)column];
		if (dataColumn == null)
		{
			int num = _columns.IndexOf(s_DBCOLUMN_NAME[(int)column]);
			if (-1 == num && column != column2)
			{
				num = _columns.IndexOf(s_DBCOLUMN_NAME[(int)column2]);
			}
			if (-1 != num)
			{
				dataColumn = _columns[num];
				_columnCache[(int)column] = dataColumn;
			}
		}
		return dataColumn;
	}
}
