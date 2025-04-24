using System.Data.Common;

namespace System.Data.Odbc;

public sealed class OdbcRowUpdatedEventArgs : RowUpdatedEventArgs
{
	public new OdbcCommand Command => (OdbcCommand)base.Command;

	public OdbcRowUpdatedEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		: base(row, command, statementType, tableMapping)
	{
	}
}
