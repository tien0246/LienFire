using System;
using System.Collections.Generic;

namespace LunarConsolePlugin;

public class CEnumVar<T> : CVar where T : struct, IConvertible
{
	private readonly IDictionary<string, T> m_valueLookup;

	private readonly string[] m_names;

	public override string[] AvailableValues => m_names;

	public T EnumValue => m_valueLookup[base.Value];

	public CEnumVar(string name, T defaultValue, CFlags flags = CFlags.None)
		: base(name, CVarType.Enum, flags)
	{
		if (!typeof(T).IsEnum)
		{
			throw new ArgumentException("T must be an enumerated type");
		}
		base.DefaultValue = (base.Value = defaultValue.ToString());
		Array values = Enum.GetValues(typeof(T));
		m_names = Enum.GetNames(typeof(T));
		m_valueLookup = new Dictionary<string, T>();
		for (int i = 0; i < values.Length; i++)
		{
			m_valueLookup[m_names[i]] = (T)values.GetValue(i);
		}
	}

	public static implicit operator T(CEnumVar<T> cvar)
	{
		return cvar.EnumValue;
	}
}
