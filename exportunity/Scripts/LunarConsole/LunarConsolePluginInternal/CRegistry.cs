using System;
using System.Collections.Generic;
using LunarConsolePlugin;

namespace LunarConsolePluginInternal;

public class CRegistry
{
	private readonly CActionList m_actions = new CActionList();

	private readonly CVarList m_vars = new CVarList();

	private ICRegistryDelegate m_delegate;

	public ICRegistryDelegate registryDelegate
	{
		get
		{
			return m_delegate;
		}
		set
		{
			m_delegate = value;
		}
	}

	public CActionList actions => m_actions;

	public CVarList cvars => m_vars;

	public CAction RegisterAction(string name, Delegate actionDelegate)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("Action's name is empty");
		}
		if ((object)actionDelegate == null)
		{
			throw new ArgumentNullException("actionDelegate");
		}
		CAction cAction = m_actions.Find(name);
		if (cAction != null)
		{
			cAction.ActionDelegate = actionDelegate;
		}
		else
		{
			cAction = new CAction(name, actionDelegate);
			m_actions.Add(cAction);
			if (m_delegate != null)
			{
				m_delegate.OnActionRegistered(this, cAction);
			}
		}
		return cAction;
	}

	public bool Unregister(string name)
	{
		return Unregister((CAction action) => action.Name == name);
	}

	public bool Unregister(int id)
	{
		return Unregister((CAction action) => action.Id == id);
	}

	public bool Unregister(Delegate del)
	{
		return Unregister((CAction action) => action.ActionDelegate == del);
	}

	public bool UnregisterAll(object target)
	{
		if (target != null)
		{
			return Unregister((CAction action) => action.ActionDelegate.Target == target);
		}
		return false;
	}

	private bool Unregister(CActionFilter filter)
	{
		if (filter == null)
		{
			throw new ArgumentNullException("filter");
		}
		IList<CAction> list = new List<CAction>();
		foreach (CAction action in m_actions)
		{
			if (filter(action))
			{
				list.Add(action);
			}
		}
		foreach (CAction item in list)
		{
			RemoveAction(item);
		}
		return list.Count > 0;
	}

	private bool RemoveAction(CAction action)
	{
		if (m_actions.Remove(action.Id))
		{
			if (m_delegate != null)
			{
				m_delegate.OnActionUnregistered(this, action);
			}
			return true;
		}
		return false;
	}

	public CAction FindAction(int id)
	{
		return m_actions.Find(id);
	}

	public void Register(CVar cvar)
	{
		m_vars.Add(cvar);
		if (m_delegate != null)
		{
			m_delegate.OnVariableRegistered(this, cvar);
		}
	}

	public CVar FindVariable(int variableId)
	{
		return m_vars.Find(variableId);
	}

	public CVar FindVariable(string variableName)
	{
		return m_vars.Find(variableName);
	}

	public void Destroy()
	{
		m_actions.Clear();
		m_vars.Clear();
		m_delegate = null;
	}
}
