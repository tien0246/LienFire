using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LunarConsolePluginInternal;

[Serializable]
public class LunarConsoleActionCall
{
	private static readonly Type[] kParamTypes = new Type[4]
	{
		typeof(int),
		typeof(float),
		typeof(string),
		typeof(bool)
	};

	[SerializeField]
	private UnityEngine.Object m_target;

	[SerializeField]
	private string m_methodName;

	[SerializeField]
	private LunarPersistentListenerMode m_mode;

	[SerializeField]
	private LunarArgumentCache m_arguments;

	public UnityEngine.Object target => m_target;

	public string methodName => m_methodName;

	public LunarPersistentListenerMode mode => m_mode;

	public void Invoke()
	{
		MethodInfo methodInfo = null;
		object[] array = null;
		switch (m_mode)
		{
		case LunarPersistentListenerMode.Void:
			methodInfo = ResolveMethod(m_target, m_methodName, typeof(void));
			array = new object[0];
			break;
		case LunarPersistentListenerMode.Bool:
			methodInfo = ResolveMethod(m_target, m_methodName, typeof(bool));
			array = new object[1] { m_arguments.boolArgument };
			break;
		case LunarPersistentListenerMode.Float:
			methodInfo = ResolveMethod(m_target, m_methodName, typeof(float));
			array = new object[1] { m_arguments.floatArgument };
			break;
		case LunarPersistentListenerMode.Int:
			methodInfo = ResolveMethod(m_target, m_methodName, typeof(int));
			array = new object[1] { m_arguments.intArgument };
			break;
		case LunarPersistentListenerMode.String:
			methodInfo = ResolveMethod(m_target, m_methodName, typeof(string));
			array = new object[1] { m_arguments.stringArgument };
			break;
		case LunarPersistentListenerMode.Object:
			methodInfo = ResolveMethod(m_target, m_methodName, typeof(UnityEngine.Object));
			array = new object[1] { m_arguments.unityObjectArgument };
			break;
		default:
			Log.e("Unable to invoke action: unexpected invoke mode '{0}'", m_mode);
			return;
		}
		if (methodInfo != null)
		{
			methodInfo.Invoke(m_target, array);
			return;
		}
		Log.e("Unable to invoke action: can't resolve method '{0}'", m_methodName);
	}

	private static MethodInfo ResolveMethod(object target, string methodName, Type paramType)
	{
		List<MethodInfo> list = ClassUtils.ListInstanceMethods(target.GetType(), delegate(MethodInfo method)
		{
			if (method.Name != methodName)
			{
				return false;
			}
			ParameterInfo[] parameters = method.GetParameters();
			if (paramType == typeof(void))
			{
				return parameters.Length == 0;
			}
			return parameters.Length == 1 && (parameters[0].ParameterType == paramType || parameters[0].ParameterType.IsSubclassOf(paramType));
		});
		if (list.Count != 1)
		{
			return null;
		}
		return list[0];
	}

	public static bool IsPersistantListenerValid(UnityEngine.Object target, string methodName, LunarPersistentListenerMode mode)
	{
		if (target == null)
		{
			return false;
		}
		foreach (MethodInfo item in ListActionMethods(target))
		{
			if (!(item.Name == methodName))
			{
				continue;
			}
			ParameterInfo[] parameters = item.GetParameters();
			if (mode == LunarPersistentListenerMode.Void)
			{
				if (parameters.Length == 0)
				{
					return true;
				}
			}
			else if (parameters.Length == 1)
			{
				Type parameterType = parameters[0].ParameterType;
				if (mode == LunarPersistentListenerMode.Bool && parameterType == typeof(bool))
				{
					return true;
				}
				if (mode == LunarPersistentListenerMode.Float && parameterType == typeof(float))
				{
					return true;
				}
				if (mode == LunarPersistentListenerMode.Int && parameterType == typeof(int))
				{
					return true;
				}
				if (mode == LunarPersistentListenerMode.String && parameterType == typeof(string))
				{
					return true;
				}
				if (mode == LunarPersistentListenerMode.Object && parameterType.IsSubclassOf(typeof(UnityEngine.Object)))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static List<MethodInfo> ListActionMethods(object target)
	{
		List<MethodInfo> list = new List<MethodInfo>();
		ClassUtils.ListMethods(list, target.GetType(), IsValidActionMethod, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		return list;
	}

	private static bool IsValidActionMethod(MethodInfo method)
	{
		if (!method.IsPublic)
		{
			return false;
		}
		if (method.ReturnType != typeof(void))
		{
			return false;
		}
		if (method.IsAbstract)
		{
			return false;
		}
		ParameterInfo[] parameters = method.GetParameters();
		if (parameters.Length > 1)
		{
			return false;
		}
		object[] customAttributes = method.GetCustomAttributes(typeof(ObsoleteAttribute), inherit: false);
		if (customAttributes != null && customAttributes.Length != 0)
		{
			return false;
		}
		if (parameters.Length == 1)
		{
			Type parameterType = parameters[0].ParameterType;
			if (!parameterType.IsSubclassOf(typeof(UnityEngine.Object)) && Array.IndexOf(kParamTypes, parameterType) == -1)
			{
				return false;
			}
		}
		return true;
	}
}
