using System;
using System.Data.Odbc;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;

internal static class Interop
{
	internal static class Odbc
	{
		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLAllocHandle(ODBC32.SQL_HANDLE HandleType, IntPtr InputHandle, out IntPtr OutputHandle);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLAllocHandle(ODBC32.SQL_HANDLE HandleType, OdbcHandle InputHandle, out IntPtr OutputHandle);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLBindCol(OdbcStatementHandle StatementHandle, ushort ColumnNumber, ODBC32.SQL_C TargetType, HandleRef TargetValue, IntPtr BufferLength, IntPtr StrLen_or_Ind);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLBindCol(OdbcStatementHandle StatementHandle, ushort ColumnNumber, ODBC32.SQL_C TargetType, IntPtr TargetValue, IntPtr BufferLength, IntPtr StrLen_or_Ind);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLBindParameter(OdbcStatementHandle StatementHandle, ushort ParameterNumber, short ParamDirection, ODBC32.SQL_C SQLCType, short SQLType, IntPtr cbColDef, IntPtr ibScale, HandleRef rgbValue, IntPtr BufferLength, HandleRef StrLen_or_Ind);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLCancel(OdbcStatementHandle StatementHandle);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLCloseCursor(OdbcStatementHandle StatementHandle);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLColAttributeW(OdbcStatementHandle StatementHandle, short ColumnNumber, short FieldIdentifier, CNativeBuffer CharacterAttribute, short BufferLength, out short StringLength, out IntPtr NumericAttribute);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLColumnsW(OdbcStatementHandle StatementHandle, string CatalogName, short NameLen1, string SchemaName, short NameLen2, string TableName, short NameLen3, string ColumnName, short NameLen4);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLDisconnect(IntPtr ConnectionHandle);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLDriverConnectW(OdbcConnectionHandle hdbc, IntPtr hwnd, string connectionstring, short cbConnectionstring, IntPtr connectionstringout, short cbConnectionstringoutMax, out short cbConnectionstringout, short fDriverCompletion);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLEndTran(ODBC32.SQL_HANDLE HandleType, IntPtr Handle, short CompletionType);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLExecDirectW(OdbcStatementHandle StatementHandle, string StatementText, int TextLength);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLExecute(OdbcStatementHandle StatementHandle);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLFetch(OdbcStatementHandle StatementHandle);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLFreeHandle(ODBC32.SQL_HANDLE HandleType, IntPtr StatementHandle);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLFreeStmt(OdbcStatementHandle StatementHandle, ODBC32.STMT Option);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLGetConnectAttrW(OdbcConnectionHandle ConnectionHandle, ODBC32.SQL_ATTR Attribute, byte[] Value, int BufferLength, out int StringLength);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLGetData(OdbcStatementHandle StatementHandle, ushort ColumnNumber, ODBC32.SQL_C TargetType, CNativeBuffer TargetValue, IntPtr BufferLength, out IntPtr StrLen_or_Ind);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLGetDescFieldW(OdbcDescriptorHandle StatementHandle, short RecNumber, ODBC32.SQL_DESC FieldIdentifier, CNativeBuffer ValuePointer, int BufferLength, out int StringLength);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLGetDiagRecW(ODBC32.SQL_HANDLE HandleType, OdbcHandle Handle, short RecNumber, StringBuilder rchState, out int NativeError, StringBuilder MessageText, short BufferLength, out short TextLength);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLGetDiagFieldW(ODBC32.SQL_HANDLE HandleType, OdbcHandle Handle, short RecNumber, short DiagIdentifier, StringBuilder rchState, short BufferLength, out short StringLength);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLGetFunctions(OdbcConnectionHandle hdbc, ODBC32.SQL_API fFunction, out short pfExists);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLGetInfoW(OdbcConnectionHandle hdbc, ODBC32.SQL_INFO fInfoType, byte[] rgbInfoValue, short cbInfoValueMax, out short pcbInfoValue);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLGetInfoW(OdbcConnectionHandle hdbc, ODBC32.SQL_INFO fInfoType, byte[] rgbInfoValue, short cbInfoValueMax, IntPtr pcbInfoValue);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLGetStmtAttrW(OdbcStatementHandle StatementHandle, ODBC32.SQL_ATTR Attribute, out IntPtr Value, int BufferLength, out int StringLength);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLGetTypeInfo(OdbcStatementHandle StatementHandle, short fSqlType);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLMoreResults(OdbcStatementHandle StatementHandle);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLNumResultCols(OdbcStatementHandle StatementHandle, out short ColumnCount);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLPrepareW(OdbcStatementHandle StatementHandle, string StatementText, int TextLength);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLPrimaryKeysW(OdbcStatementHandle StatementHandle, string CatalogName, short NameLen1, string SchemaName, short NameLen2, string TableName, short NameLen3);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLProcedureColumnsW(OdbcStatementHandle StatementHandle, string CatalogName, short NameLen1, string SchemaName, short NameLen2, string ProcName, short NameLen3, string ColumnName, short NameLen4);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLProceduresW(OdbcStatementHandle StatementHandle, string CatalogName, short NameLen1, string SchemaName, short NameLen2, string ProcName, short NameLen3);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLRowCount(OdbcStatementHandle StatementHandle, out IntPtr RowCount);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLSetConnectAttrW(OdbcConnectionHandle ConnectionHandle, ODBC32.SQL_ATTR Attribute, IDtcTransaction Value, int StringLength);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLSetConnectAttrW(OdbcConnectionHandle ConnectionHandle, ODBC32.SQL_ATTR Attribute, string Value, int StringLength);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLSetConnectAttrW(OdbcConnectionHandle ConnectionHandle, ODBC32.SQL_ATTR Attribute, IntPtr Value, int StringLength);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLSetConnectAttrW(IntPtr ConnectionHandle, ODBC32.SQL_ATTR Attribute, IntPtr Value, int StringLength);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLSetDescFieldW(OdbcDescriptorHandle StatementHandle, short ColumnNumber, ODBC32.SQL_DESC FieldIdentifier, HandleRef CharacterAttribute, int BufferLength);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLSetDescFieldW(OdbcDescriptorHandle StatementHandle, short ColumnNumber, ODBC32.SQL_DESC FieldIdentifier, IntPtr CharacterAttribute, int BufferLength);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLSetEnvAttr(OdbcEnvironmentHandle EnvironmentHandle, ODBC32.SQL_ATTR Attribute, IntPtr Value, ODBC32.SQL_IS StringLength);

		[DllImport("libodbc.so.2")]
		internal static extern ODBC32.RetCode SQLSetStmtAttrW(OdbcStatementHandle StatementHandle, int Attribute, IntPtr Value, int StringLength);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLSpecialColumnsW(OdbcStatementHandle StatementHandle, ODBC32.SQL_SPECIALCOLS IdentifierType, string CatalogName, short NameLen1, string SchemaName, short NameLen2, string TableName, short NameLen3, ODBC32.SQL_SCOPE Scope, ODBC32.SQL_NULLABILITY Nullable);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLStatisticsW(OdbcStatementHandle StatementHandle, string CatalogName, short NameLen1, string SchemaName, short NameLen2, string TableName, short NameLen3, short Unique, short Reserved);

		[DllImport("libodbc.so.2", CharSet = CharSet.Unicode)]
		internal static extern ODBC32.RetCode SQLTablesW(OdbcStatementHandle StatementHandle, string CatalogName, short NameLen1, string SchemaName, short NameLen2, string TableName, short NameLen3, string TableType, short NameLen4);
	}

	internal static class Libraries
	{
		internal const string Odbc32 = "libodbc.so.2";

		internal const string NetSecurityNative = "System.Net.Security.Native";
	}
}
