using System;
using System.Reflection;
using LunarConsolePlugin;
using UnityEngine;

namespace LunarConsolePluginInternal;

[Serializable]
public class LunarConsoleLegacyAction
{
	private static readonly object[] kEmptyArgs = new object[0];

	[SerializeField]
	private string m_name;

	[SerializeField]
	private GameObject m_target;

	[SerializeField]
	private string m_componentTypeName;

	[SerializeField]
	private string m_componentMethodName;

	private Type m_componentType;

	private MethodInfo m_componentMethod;

	public void Register()
	{
		if (string.IsNullOrEmpty(m_name))
		{
			Log.w("Unable to register action: name is null or empty");
		}
		else if (m_target == null)
		{
			Log.w("Unable to register action '{0}': target GameObject is missing", m_name);
		}
		else if (string.IsNullOrEmpty(m_componentMethodName))
		{
			Log.w("Unable to register action '{0}' for '{1}': function is missing", m_name, m_target.name);
		}
		else
		{
			LunarConsole.RegisterAction(m_name, Invoke);
		}
	}

	public void Unregister()
	{
		LunarConsole.UnregisterAction(Invoke);
	}

	private void Invoke()
	{
		if (m_target == null)
		{
			Log.e("Can't invoke action '{0}': target is not set", m_name);
		}
		else if (m_componentTypeName == null)
		{
			Log.e("Can't invoke action '{0}': method is not set", m_name);
		}
		else if (m_componentMethodName == null)
		{
			Log.e("Can't invoke action '{0}': method is not set", m_name);
		}
		else
		{
			if ((m_componentType == null || m_componentMethod == null) && !ResolveInvocation())
			{
				return;
			}
			Component component = m_target.GetComponent(m_componentType);
			if (!(component == null))
			{
				try
				{
					m_componentMethod.Invoke(component, kEmptyArgs);
					return;
				}
				catch (TargetInvocationException ex)
				{
					Log.e(ex.InnerException, "Exception while invoking action '{0}'", m_name);
					return;
				}
				catch (Exception exception)
				{
					Log.e(exception, "Exception while invoking action '{0}'", m_name);
					return;
				}
			}
			Log.w("Missing component {0}", m_componentType);
		}
	}

	private bool ResolveInvocation()
	{
		try
		{
			m_componentType = Type.GetType(m_componentTypeName);
			if (m_componentType == null)
			{
				Log.w("Can't resolve type {0}", m_componentTypeName);
				return false;
			}
			m_componentMethod = m_componentType.GetMethod(m_componentMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (m_componentMethod == null)
			{
				Log.w("Can't resolve method {0} of type {1}", m_componentMethod, m_componentType);
				return false;
			}
			return true;
		}
		catch (Exception exception)
		{
			Log.e(exception);
			return false;
		}
	}

	public void Validate()
	{
		if (string.IsNullOrEmpty(m_name))
		{
			Log.w("Missing action name");
		}
		if (m_target == null)
		{
			Log.w("Missing action target");
		}
		if (m_componentType != null && m_componentMethodName != null)
		{
			ResolveInvocation();
		}
	}
}
