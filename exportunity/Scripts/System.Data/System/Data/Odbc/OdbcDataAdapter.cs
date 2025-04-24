using System.Data.Common;

namespace System.Data.Odbc;

public sealed class OdbcDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
{
	private static readonly object s_eventRowUpdated = new object();

	private static readonly object s_eventRowUpdating = new object();

	private OdbcCommand _deleteCommand;

	private OdbcCommand _insertCommand;

	private OdbcCommand _selectCommand;

	private OdbcCommand _updateCommand;

	public new OdbcCommand DeleteCommand
	{
		get
		{
			return _deleteCommand;
		}
		set
		{
			_deleteCommand = value;
		}
	}

	IDbCommand IDbDataAdapter.DeleteCommand
	{
		get
		{
			return _deleteCommand;
		}
		set
		{
			_deleteCommand = (OdbcCommand)value;
		}
	}

	public new OdbcCommand InsertCommand
	{
		get
		{
			return _insertCommand;
		}
		set
		{
			_insertCommand = value;
		}
	}

	IDbCommand IDbDataAdapter.InsertCommand
	{
		get
		{
			return _insertCommand;
		}
		set
		{
			_insertCommand = (OdbcCommand)value;
		}
	}

	public new OdbcCommand SelectCommand
	{
		get
		{
			return _selectCommand;
		}
		set
		{
			_selectCommand = value;
		}
	}

	IDbCommand IDbDataAdapter.SelectCommand
	{
		get
		{
			return _selectCommand;
		}
		set
		{
			_selectCommand = (OdbcCommand)value;
		}
	}

	public new OdbcCommand UpdateCommand
	{
		get
		{
			return _updateCommand;
		}
		set
		{
			_updateCommand = value;
		}
	}

	IDbCommand IDbDataAdapter.UpdateCommand
	{
		get
		{
			return _updateCommand;
		}
		set
		{
			_updateCommand = (OdbcCommand)value;
		}
	}

	public event OdbcRowUpdatedEventHandler RowUpdated
	{
		add
		{
			base.Events.AddHandler(s_eventRowUpdated, value);
		}
		remove
		{
			base.Events.RemoveHandler(s_eventRowUpdated, value);
		}
	}

	public event OdbcRowUpdatingEventHandler RowUpdating
	{
		add
		{
			OdbcRowUpdatingEventHandler odbcRowUpdatingEventHandler = (OdbcRowUpdatingEventHandler)base.Events[s_eventRowUpdating];
			if (odbcRowUpdatingEventHandler != null && value.Target is OdbcCommandBuilder)
			{
				OdbcRowUpdatingEventHandler odbcRowUpdatingEventHandler2 = (OdbcRowUpdatingEventHandler)ADP.FindBuilder(odbcRowUpdatingEventHandler);
				if (odbcRowUpdatingEventHandler2 != null)
				{
					base.Events.RemoveHandler(s_eventRowUpdating, odbcRowUpdatingEventHandler2);
				}
			}
			base.Events.AddHandler(s_eventRowUpdating, value);
		}
		remove
		{
			base.Events.RemoveHandler(s_eventRowUpdating, value);
		}
	}

	public OdbcDataAdapter()
	{
		GC.SuppressFinalize(this);
	}

	public OdbcDataAdapter(OdbcCommand selectCommand)
		: this()
	{
		SelectCommand = selectCommand;
	}

	public OdbcDataAdapter(string selectCommandText, OdbcConnection selectConnection)
		: this()
	{
		SelectCommand = new OdbcCommand(selectCommandText, selectConnection);
	}

	public OdbcDataAdapter(string selectCommandText, string selectConnectionString)
		: this()
	{
		OdbcConnection connection = new OdbcConnection(selectConnectionString);
		SelectCommand = new OdbcCommand(selectCommandText, connection);
	}

	private OdbcDataAdapter(OdbcDataAdapter from)
		: base(from)
	{
		GC.SuppressFinalize(this);
	}

	object ICloneable.Clone()
	{
		return new OdbcDataAdapter(this);
	}

	protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
	{
		return new OdbcRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
	}

	protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
	{
		return new OdbcRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
	}

	protected override void OnRowUpdated(RowUpdatedEventArgs value)
	{
		OdbcRowUpdatedEventHandler odbcRowUpdatedEventHandler = (OdbcRowUpdatedEventHandler)base.Events[s_eventRowUpdated];
		if (odbcRowUpdatedEventHandler != null && value is OdbcRowUpdatedEventArgs)
		{
			odbcRowUpdatedEventHandler(this, (OdbcRowUpdatedEventArgs)value);
		}
		base.OnRowUpdated(value);
	}

	protected override void OnRowUpdating(RowUpdatingEventArgs value)
	{
		OdbcRowUpdatingEventHandler odbcRowUpdatingEventHandler = (OdbcRowUpdatingEventHandler)base.Events[s_eventRowUpdating];
		if (odbcRowUpdatingEventHandler != null && value is OdbcRowUpdatingEventArgs)
		{
			odbcRowUpdatingEventHandler(this, (OdbcRowUpdatingEventArgs)value);
		}
		base.OnRowUpdating(value);
	}
}
