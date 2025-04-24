namespace System.Data.Odbc;

internal sealed class DbCache
{
	private bool[] _isBadValue;

	private DbSchemaInfo[] _schema;

	private object[] _values;

	private OdbcDataReader _record;

	internal int _count;

	internal bool _randomaccess = true;

	internal object this[int i]
	{
		get
		{
			if (_isBadValue[i])
			{
				OverflowException ex = (OverflowException)Values[i];
				throw new OverflowException(ex.Message, ex);
			}
			return Values[i];
		}
		set
		{
			Values[i] = value;
			_isBadValue[i] = false;
		}
	}

	internal int Count => _count;

	internal object[] Values => _values;

	internal DbCache(OdbcDataReader record, int count)
	{
		_count = count;
		_record = record;
		_randomaccess = !record.IsBehavior(CommandBehavior.SequentialAccess);
		_values = new object[count];
		_isBadValue = new bool[count];
	}

	internal void InvalidateValue(int i)
	{
		_isBadValue[i] = true;
	}

	internal object AccessIndex(int i)
	{
		object[] values = Values;
		if (_randomaccess)
		{
			for (int j = 0; j < i; j++)
			{
				if (values[j] == null)
				{
					values[j] = _record.GetValue(j);
				}
			}
		}
		return values[i];
	}

	internal DbSchemaInfo GetSchema(int i)
	{
		if (_schema == null)
		{
			_schema = new DbSchemaInfo[Count];
		}
		if (_schema[i] == null)
		{
			_schema[i] = new DbSchemaInfo();
		}
		return _schema[i];
	}

	internal void FlushValues()
	{
		int num = _values.Length;
		for (int i = 0; i < num; i++)
		{
			_values[i] = null;
		}
	}
}
