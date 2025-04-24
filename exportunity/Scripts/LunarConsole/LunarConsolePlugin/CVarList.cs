using System.Collections;
using System.Collections.Generic;

namespace LunarConsolePlugin;

public class CVarList : IEnumerable<CVar>, IEnumerable
{
	private readonly List<CVar> m_variables;

	private readonly Dictionary<int, CVar> m_lookupById;

	public int Count => m_variables.Count;

	public CVarList()
	{
		m_variables = new List<CVar>();
		m_lookupById = new Dictionary<int, CVar>();
	}

	public void Add(CVar variable)
	{
		m_variables.Add(variable);
		m_lookupById.Add(variable.Id, variable);
	}

	public bool Remove(int id)
	{
		if (m_lookupById.TryGetValue(id, out var value))
		{
			m_lookupById.Remove(id);
			m_variables.Remove(value);
			return true;
		}
		return false;
	}

	public CVar Find(int id)
	{
		if (!m_lookupById.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public CVar Find(string name)
	{
		foreach (CVar variable in m_variables)
		{
			if (variable.Name == name)
			{
				return variable;
			}
		}
		return null;
	}

	public void Clear()
	{
		m_variables.Clear();
		m_lookupById.Clear();
	}

	public IEnumerator<CVar> GetEnumerator()
	{
		return m_variables.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return m_variables.GetEnumerator();
	}
}
