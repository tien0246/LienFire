using System;
using System.Reflection;

namespace LunarConsolePluginInternal;

public class CAction : IComparable<CAction>
{
	private static readonly string[] kEmptyArgs = new string[0];

	private static int s_nextActionId;

	private readonly int m_id;

	private readonly string m_name;

	private Delegate m_actionDelegate;

	public int Id => m_id;

	public string Name => m_name;

	public Delegate ActionDelegate
	{
		get
		{
			return m_actionDelegate;
		}
		set
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("actionDelegate");
			}
			m_actionDelegate = value;
		}
	}

	public CAction(string name, Delegate actionDelegate)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("Action name is empty");
		}
		if ((object)actionDelegate == null)
		{
			throw new ArgumentNullException("actionDelegate");
		}
		m_id = s_nextActionId++;
		m_name = name;
		m_actionDelegate = actionDelegate;
	}

	public bool Execute()
	{
		try
		{
			return ReflectionUtils.Invoke(ActionDelegate, kEmptyArgs);
		}
		catch (TargetInvocationException ex)
		{
			Log.e(ex.InnerException, "Exception while invoking action '{0}'", m_name);
		}
		catch (Exception exception)
		{
			Log.e(exception, "Exception while invoking action '{0}'", m_name);
		}
		return false;
	}

	internal bool StartsWith(string prefix)
	{
		return StringUtils.StartsWithIgnoreCase(Name, prefix);
	}

	public int CompareTo(CAction other)
	{
		return Name.CompareTo(other.Name);
	}

	public override string ToString()
	{
		return $"{Name} ({ActionDelegate})";
	}
}
