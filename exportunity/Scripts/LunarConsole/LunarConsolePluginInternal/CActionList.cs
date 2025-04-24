using System.Collections;
using System.Collections.Generic;

namespace LunarConsolePluginInternal;

public class CActionList : IEnumerable<CAction>, IEnumerable
{
	private readonly List<CAction> m_actions;

	private readonly Dictionary<int, CAction> m_actionLookupById;

	private readonly Dictionary<string, CAction> m_actionLookupByName;

	public CActionList()
	{
		m_actions = new List<CAction>();
		m_actionLookupById = new Dictionary<int, CAction>();
		m_actionLookupByName = new Dictionary<string, CAction>();
	}

	public void Add(CAction action)
	{
		m_actions.Add(action);
		m_actionLookupById.Add(action.Id, action);
		m_actionLookupByName.Add(action.Name, action);
	}

	public bool Remove(int id)
	{
		if (m_actionLookupById.TryGetValue(id, out var value))
		{
			m_actionLookupById.Remove(id);
			m_actionLookupByName.Remove(value.Name);
			m_actions.Remove(value);
			return true;
		}
		return false;
	}

	public CAction Find(string name)
	{
		if (!m_actionLookupByName.TryGetValue(name, out var value))
		{
			return null;
		}
		return value;
	}

	public CAction Find(int id)
	{
		if (!m_actionLookupById.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public void Clear()
	{
		m_actions.Clear();
		m_actionLookupById.Clear();
		m_actionLookupByName.Clear();
	}

	public IEnumerator<CAction> GetEnumerator()
	{
		return m_actions.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return m_actions.GetEnumerator();
	}
}
