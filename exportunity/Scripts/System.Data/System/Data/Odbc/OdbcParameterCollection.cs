using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;

namespace System.Data.Odbc;

public sealed class OdbcParameterCollection : DbParameterCollection
{
	private bool _rebindCollection;

	private static Type s_itemType = typeof(OdbcParameter);

	private List<OdbcParameter> _items;

	internal bool RebindCollection
	{
		get
		{
			return _rebindCollection;
		}
		set
		{
			_rebindCollection = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public new OdbcParameter this[int index]
	{
		get
		{
			return (OdbcParameter)GetParameter(index);
		}
		set
		{
			SetParameter(index, value);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new OdbcParameter this[string parameterName]
	{
		get
		{
			return (OdbcParameter)GetParameter(parameterName);
		}
		set
		{
			SetParameter(parameterName, value);
		}
	}

	public override int Count
	{
		get
		{
			if (_items == null)
			{
				return 0;
			}
			return _items.Count;
		}
	}

	private List<OdbcParameter> InnerList
	{
		get
		{
			List<OdbcParameter> list = _items;
			if (list == null)
			{
				list = (_items = new List<OdbcParameter>());
			}
			return list;
		}
	}

	public override bool IsFixedSize => ((IList)InnerList).IsFixedSize;

	public override bool IsReadOnly => ((IList)InnerList).IsReadOnly;

	public override bool IsSynchronized => ((ICollection)InnerList).IsSynchronized;

	public override object SyncRoot => ((ICollection)InnerList).SyncRoot;

	internal OdbcParameterCollection()
	{
	}

	public OdbcParameter Add(OdbcParameter value)
	{
		Add((object)value);
		return value;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Add(String parameterName, Object value) has been deprecated.  Use AddWithValue(String parameterName, Object value).  http://go.microsoft.com/fwlink/?linkid=14202", false)]
	public OdbcParameter Add(string parameterName, object value)
	{
		return Add(new OdbcParameter(parameterName, value));
	}

	public OdbcParameter AddWithValue(string parameterName, object value)
	{
		return Add(new OdbcParameter(parameterName, value));
	}

	public OdbcParameter Add(string parameterName, OdbcType odbcType)
	{
		return Add(new OdbcParameter(parameterName, odbcType));
	}

	public OdbcParameter Add(string parameterName, OdbcType odbcType, int size)
	{
		return Add(new OdbcParameter(parameterName, odbcType, size));
	}

	public OdbcParameter Add(string parameterName, OdbcType odbcType, int size, string sourceColumn)
	{
		return Add(new OdbcParameter(parameterName, odbcType, size, sourceColumn));
	}

	public void AddRange(OdbcParameter[] values)
	{
		AddRange((Array)values);
	}

	internal void Bind(OdbcCommand command, CMDWrapper cmdWrapper, CNativeBuffer parameterBuffer)
	{
		for (int i = 0; i < Count; i++)
		{
			this[i].Bind(cmdWrapper.StatementHandle, command, checked((short)(i + 1)), parameterBuffer, allowReentrance: true);
		}
		_rebindCollection = false;
	}

	internal int CalcParameterBufferSize(OdbcCommand command)
	{
		int parameterBufferSize = 0;
		for (int i = 0; i < Count; i++)
		{
			if (_rebindCollection)
			{
				this[i].HasChanged = true;
			}
			this[i].PrepareForBind(command, (short)(i + 1), ref parameterBufferSize);
			parameterBufferSize = (parameterBufferSize + (IntPtr.Size - 1)) & ~(IntPtr.Size - 1);
		}
		return parameterBufferSize;
	}

	internal void ClearBindings()
	{
		for (int i = 0; i < Count; i++)
		{
			this[i].ClearBinding();
		}
	}

	public override bool Contains(string value)
	{
		return -1 != IndexOf(value);
	}

	public bool Contains(OdbcParameter value)
	{
		return -1 != IndexOf(value);
	}

	public void CopyTo(OdbcParameter[] array, int index)
	{
		CopyTo((Array)array, index);
	}

	private void OnChange()
	{
		_rebindCollection = true;
	}

	internal void GetOutputValues(CMDWrapper cmdWrapper)
	{
		if (!_rebindCollection)
		{
			CNativeBuffer nativeParameterBuffer = cmdWrapper._nativeParameterBuffer;
			for (int i = 0; i < Count; i++)
			{
				this[i].GetOutputValue(nativeParameterBuffer);
			}
		}
	}

	public int IndexOf(OdbcParameter value)
	{
		return IndexOf((object)value);
	}

	public void Insert(int index, OdbcParameter value)
	{
		Insert(index, (object)value);
	}

	public void Remove(OdbcParameter value)
	{
		Remove((object)value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override int Add(object value)
	{
		OnChange();
		ValidateType(value);
		Validate(-1, value);
		InnerList.Add((OdbcParameter)value);
		return Count - 1;
	}

	public override void AddRange(Array values)
	{
		OnChange();
		if (values == null)
		{
			throw ADP.ArgumentNull("values");
		}
		foreach (object value in values)
		{
			ValidateType(value);
		}
		foreach (OdbcParameter value2 in values)
		{
			Validate(-1, value2);
			InnerList.Add(value2);
		}
	}

	private int CheckName(string parameterName)
	{
		int num = IndexOf(parameterName);
		if (num < 0)
		{
			throw ADP.ParametersSourceIndex(parameterName, this, s_itemType);
		}
		return num;
	}

	public override void Clear()
	{
		OnChange();
		List<OdbcParameter> innerList = InnerList;
		if (innerList == null)
		{
			return;
		}
		foreach (OdbcParameter item in innerList)
		{
			item.ResetParent();
		}
		innerList.Clear();
	}

	public override bool Contains(object value)
	{
		return -1 != IndexOf(value);
	}

	public override void CopyTo(Array array, int index)
	{
		((ICollection)InnerList).CopyTo(array, index);
	}

	public override IEnumerator GetEnumerator()
	{
		return ((IEnumerable)InnerList).GetEnumerator();
	}

	protected override DbParameter GetParameter(int index)
	{
		RangeCheck(index);
		return InnerList[index];
	}

	protected override DbParameter GetParameter(string parameterName)
	{
		int num = IndexOf(parameterName);
		if (num < 0)
		{
			throw ADP.ParametersSourceIndex(parameterName, this, s_itemType);
		}
		return InnerList[num];
	}

	private static int IndexOf(IEnumerable items, string parameterName)
	{
		if (items != null)
		{
			int num = 0;
			foreach (OdbcParameter item in items)
			{
				if (parameterName == item.ParameterName)
				{
					return num;
				}
				num++;
			}
			num = 0;
			foreach (OdbcParameter item2 in items)
			{
				if (ADP.DstCompare(parameterName, item2.ParameterName) == 0)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	public override int IndexOf(string parameterName)
	{
		return IndexOf(InnerList, parameterName);
	}

	public override int IndexOf(object value)
	{
		if (value != null)
		{
			ValidateType(value);
			List<OdbcParameter> innerList = InnerList;
			if (innerList != null)
			{
				int count = innerList.Count;
				for (int i = 0; i < count; i++)
				{
					if (value == innerList[i])
					{
						return i;
					}
				}
			}
		}
		return -1;
	}

	public override void Insert(int index, object value)
	{
		OnChange();
		ValidateType(value);
		Validate(-1, (OdbcParameter)value);
		InnerList.Insert(index, (OdbcParameter)value);
	}

	private void RangeCheck(int index)
	{
		if (index < 0 || Count <= index)
		{
			throw ADP.ParametersMappingIndex(index, this);
		}
	}

	public override void Remove(object value)
	{
		OnChange();
		ValidateType(value);
		int num = IndexOf(value);
		if (-1 != num)
		{
			RemoveIndex(num);
		}
		else if (this != ((OdbcParameter)value).CompareExchangeParent(null, this))
		{
			throw ADP.CollectionRemoveInvalidObject(s_itemType, this);
		}
	}

	public override void RemoveAt(int index)
	{
		OnChange();
		RangeCheck(index);
		RemoveIndex(index);
	}

	public override void RemoveAt(string parameterName)
	{
		OnChange();
		int index = CheckName(parameterName);
		RemoveIndex(index);
	}

	private void RemoveIndex(int index)
	{
		List<OdbcParameter> innerList = InnerList;
		OdbcParameter odbcParameter = innerList[index];
		innerList.RemoveAt(index);
		odbcParameter.ResetParent();
	}

	private void Replace(int index, object newValue)
	{
		List<OdbcParameter> innerList = InnerList;
		ValidateType(newValue);
		Validate(index, newValue);
		OdbcParameter odbcParameter = innerList[index];
		innerList[index] = (OdbcParameter)newValue;
		odbcParameter.ResetParent();
	}

	protected override void SetParameter(int index, DbParameter value)
	{
		OnChange();
		RangeCheck(index);
		Replace(index, value);
	}

	protected override void SetParameter(string parameterName, DbParameter value)
	{
		OnChange();
		int num = IndexOf(parameterName);
		if (num < 0)
		{
			throw ADP.ParametersSourceIndex(parameterName, this, s_itemType);
		}
		Replace(num, value);
	}

	private void Validate(int index, object value)
	{
		if (value == null)
		{
			throw ADP.ParameterNull("value", this, s_itemType);
		}
		object obj = ((OdbcParameter)value).CompareExchangeParent(this, null);
		if (obj != null)
		{
			if (this != obj)
			{
				throw ADP.ParametersIsNotParent(s_itemType, this);
			}
			if (index != IndexOf(value))
			{
				throw ADP.ParametersIsParent(s_itemType, this);
			}
		}
		string parameterName = ((OdbcParameter)value).ParameterName;
		if (parameterName.Length == 0)
		{
			index = 1;
			do
			{
				parameterName = "Parameter" + index.ToString(CultureInfo.CurrentCulture);
				index++;
			}
			while (-1 != IndexOf(parameterName));
			((OdbcParameter)value).ParameterName = parameterName;
		}
	}

	private void ValidateType(object value)
	{
		if (value == null)
		{
			throw ADP.ParameterNull("value", this, s_itemType);
		}
		if (!s_itemType.IsInstanceOfType(value))
		{
			throw ADP.InvalidParameterType(this, s_itemType, value);
		}
	}
}
