using System;
using LunarConsolePluginInternal;

namespace LunarConsolePlugin;

public class CVar : IEquatable<CVar>, IComparable<CVar>
{
	private static int s_nextId;

	private readonly int m_id;

	private readonly string m_name;

	private readonly CVarType m_type;

	private readonly CFlags m_flags;

	private CValue m_value;

	private CValue m_defaultValue;

	private CVarValueRange m_range = CVarValueRange.Undefined;

	private CVarChangedDelegateList m_delegateList;

	public int Id => m_id;

	public string Name => m_name;

	public CVarType Type => m_type;

	public string DefaultValue
	{
		get
		{
			return m_defaultValue.stringValue;
		}
		protected set
		{
			m_defaultValue.stringValue = value;
		}
	}

	public bool IsString => m_type == CVarType.String;

	public string Value
	{
		get
		{
			return m_value.stringValue;
		}
		set
		{
			bool num = m_value.stringValue != value;
			m_value.stringValue = value;
			m_value.floatValue = ((IsInt || IsFloat) ? StringUtils.ParseFloat(value, 0f) : 0f);
			m_value.intValue = ((IsInt || IsFloat) ? ((int)FloatValue) : 0);
			if (num)
			{
				NotifyValueChanged();
			}
		}
	}

	public CVarValueRange Range
	{
		get
		{
			return m_range;
		}
		set
		{
			m_range = value;
		}
	}

	public bool HasRange => m_range.IsValid;

	public bool IsInt
	{
		get
		{
			if (m_type != CVarType.Integer)
			{
				return m_type == CVarType.Boolean;
			}
			return true;
		}
	}

	public int IntValue
	{
		get
		{
			return m_value.intValue;
		}
		set
		{
			bool num = m_value.intValue != value;
			m_value.stringValue = StringUtils.ToString(value);
			m_value.intValue = value;
			m_value.floatValue = value;
			if (num)
			{
				NotifyValueChanged();
			}
		}
	}

	public bool IsFloat => m_type == CVarType.Float;

	public float FloatValue
	{
		get
		{
			return m_value.floatValue;
		}
		set
		{
			float floatValue = m_value.floatValue;
			m_value.stringValue = StringUtils.ToString(value);
			m_value.intValue = (int)value;
			m_value.floatValue = value;
			if (floatValue != value)
			{
				NotifyValueChanged();
			}
		}
	}

	public bool IsBool => m_type == CVarType.Boolean;

	public bool BoolValue
	{
		get
		{
			return m_value.intValue != 0;
		}
		set
		{
			IntValue = (value ? 1 : 0);
		}
	}

	public virtual string[] AvailableValues => null;

	public bool IsDefault
	{
		get
		{
			return m_value.Equals(m_defaultValue);
		}
		set
		{
			bool num = IsDefault ^ value;
			m_value = m_defaultValue;
			if (num)
			{
				NotifyValueChanged();
			}
		}
	}

	public CFlags Flags => m_flags;

	public bool IsHidden => (m_flags & CFlags.Hidden) != 0;

	public CVar(string name, bool defaultValue, CFlags flags = CFlags.None)
		: this(name, CVarType.Boolean, flags)
	{
		IntValue = (defaultValue ? 1 : 0);
		m_defaultValue = m_value;
	}

	public CVar(string name, int defaultValue, CFlags flags = CFlags.None)
		: this(name, CVarType.Integer, flags)
	{
		IntValue = defaultValue;
		m_defaultValue = m_value;
	}

	public CVar(string name, float defaultValue, CFlags flags = CFlags.None)
		: this(name, CVarType.Float, flags)
	{
		FloatValue = defaultValue;
		m_defaultValue = m_value;
	}

	public CVar(string name, string defaultValue, CFlags flags = CFlags.None)
		: this(name, CVarType.String, flags)
	{
		Value = defaultValue;
		m_defaultValue = m_value;
	}

	protected CVar(string name, CVarType type, CFlags flags)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		m_id = ++s_nextId;
		m_name = name;
		m_type = type;
		m_flags = flags;
	}

	public void AddDelegate(CVarChangedDelegate del)
	{
		if (del == null)
		{
			throw new ArgumentNullException("del");
		}
		if (m_delegateList == null)
		{
			m_delegateList = new CVarChangedDelegateList(1);
			m_delegateList.Add(del);
		}
		else if (!m_delegateList.Contains(del))
		{
			m_delegateList.Add(del);
		}
	}

	public void RemoveDelegate(CVarChangedDelegate del)
	{
		if (del != null && m_delegateList != null)
		{
			m_delegateList.Remove(del);
			if (m_delegateList.Count == 0)
			{
				m_delegateList = null;
			}
		}
	}

	public void RemoveDelegates(object target)
	{
		if (target == null || m_delegateList == null)
		{
			return;
		}
		for (int num = m_delegateList.Count - 1; num >= 0; num--)
		{
			if (m_delegateList.Get(num).Target == target)
			{
				m_delegateList.RemoveAt(num);
			}
		}
		if (m_delegateList.Count == 0)
		{
			m_delegateList = null;
		}
	}

	private void NotifyValueChanged()
	{
		if (m_delegateList != null && m_delegateList.Count > 0)
		{
			m_delegateList.NotifyValueChanged(this);
		}
	}

	public bool Equals(CVar other)
	{
		if (other != null && other.m_name == m_name && other.m_value.Equals(ref m_value) && other.m_defaultValue.Equals(ref m_defaultValue))
		{
			return other.m_type == m_type;
		}
		return false;
	}

	public int CompareTo(CVar other)
	{
		return Name.CompareTo(other.Name);
	}

	public bool HasFlag(CFlags flag)
	{
		return (m_flags & flag) != 0;
	}

	public static implicit operator string(CVar cvar)
	{
		return cvar.m_value.stringValue;
	}

	public static implicit operator int(CVar cvar)
	{
		return cvar.m_value.intValue;
	}

	public static implicit operator float(CVar cvar)
	{
		return cvar.m_value.floatValue;
	}

	public static implicit operator bool(CVar cvar)
	{
		return cvar.m_value.intValue != 0;
	}
}
