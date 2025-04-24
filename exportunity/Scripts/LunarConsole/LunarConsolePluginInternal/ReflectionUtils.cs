using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LunarConsolePluginInternal;

internal static class ReflectionUtils
{
	private static readonly object[] EMPTY_INVOKE_ARGS = new object[0];

	public static bool Invoke(Delegate del, string[] invokeArgs)
	{
		if ((object)del == null)
		{
			throw new ArgumentNullException("del");
		}
		return Invoke(del.Target, del.Method, invokeArgs);
	}

	public static bool Invoke(object target, MethodInfo method, string[] invokeArgs)
	{
		ParameterInfo[] parameters = method.GetParameters();
		if (parameters.Length == 0)
		{
			return Invoke(target, method, EMPTY_INVOKE_ARGS);
		}
		List<object> list = new List<object>(invokeArgs.Length);
		Iterator<string> iter = new Iterator<string>(invokeArgs);
		ParameterInfo[] array = parameters;
		foreach (ParameterInfo param in array)
		{
			list.Add(ResolveInvokeParameter(param, iter));
		}
		return Invoke(target, method, list.ToArray());
	}

	private static bool Invoke(object target, MethodInfo method, object[] args)
	{
		if (method.ReturnType == typeof(bool))
		{
			return (bool)method.Invoke(target, args);
		}
		method.Invoke(target, args);
		return true;
	}

	private static object ResolveInvokeParameter(ParameterInfo param, Iterator<string> iter)
	{
		if (param.IsOptional && !iter.HasNext())
		{
			return param.DefaultValue;
		}
		Type parameterType = param.ParameterType;
		if (parameterType == typeof(string[]))
		{
			List<string> list = new List<string>();
			while (iter.HasNext())
			{
				list.Add(NextArg(iter));
			}
			return list.ToArray();
		}
		if (parameterType == typeof(string))
		{
			return NextArg(iter);
		}
		if (parameterType == typeof(float))
		{
			return NextFloatArg(iter);
		}
		if (parameterType == typeof(int))
		{
			return NextIntArg(iter);
		}
		if (parameterType == typeof(bool))
		{
			return NextBoolArg(iter);
		}
		if (parameterType == typeof(Vector2))
		{
			float x = NextFloatArg(iter);
			float y = NextFloatArg(iter);
			return new Vector2(x, y);
		}
		if (parameterType == typeof(Vector3))
		{
			float x2 = NextFloatArg(iter);
			float y2 = NextFloatArg(iter);
			float z = NextFloatArg(iter);
			return new Vector3(x2, y2, z);
		}
		if (parameterType == typeof(Vector4))
		{
			float x3 = NextFloatArg(iter);
			float y3 = NextFloatArg(iter);
			float z2 = NextFloatArg(iter);
			float w = NextFloatArg(iter);
			return new Vector4(x3, y3, z2, w);
		}
		if (parameterType == typeof(int[]))
		{
			List<int> list2 = new List<int>();
			while (iter.HasNext())
			{
				list2.Add(NextIntArg(iter));
			}
			return list2.ToArray();
		}
		if (parameterType == typeof(float[]))
		{
			List<float> list3 = new List<float>();
			while (iter.HasNext())
			{
				list3.Add(NextFloatArg(iter));
			}
			return list3.ToArray();
		}
		if (parameterType == typeof(bool[]))
		{
			List<bool> list4 = new List<bool>();
			while (iter.HasNext())
			{
				list4.Add(NextBoolArg(iter));
			}
			return list4.ToArray();
		}
		throw new ReflectionException("Unsupported value type: " + parameterType);
	}

	public static int NextIntArg(Iterator<string> iter)
	{
		string text = NextArg(iter);
		if (int.TryParse(text, out var result))
		{
			return result;
		}
		throw new ReflectionException("Can't parse int arg: '" + text + "'");
	}

	public static float NextFloatArg(Iterator<string> iter)
	{
		string text = NextArg(iter);
		if (float.TryParse(text, out var result))
		{
			return result;
		}
		throw new ReflectionException("Can't parse float arg: '" + text + "'");
	}

	public static bool NextBoolArg(Iterator<string> iter)
	{
		string text = NextArg(iter).ToLower();
		switch (text)
		{
		case "1":
		case "yes":
		case "true":
			return true;
		case "0":
		case "no":
		case "false":
			return false;
		default:
			throw new ReflectionException("Can't parse bool arg: '" + text + "'");
		}
	}

	public static string NextArg(Iterator<string> iter)
	{
		if (iter.HasNext())
		{
			string text = StringUtils.UnArg(iter.Next());
			if (!IsValidArg(text))
			{
				throw new ReflectionException("Invalid arg: " + text);
			}
			return text;
		}
		throw new ReflectionException("Unexpected end of args");
	}

	public static bool IsValidArg(string arg)
	{
		return true;
	}

	public static List<Assembly> ListAssemblies(Func<Assembly, bool> filter)
	{
		List<Assembly> list = new List<Assembly>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			if (filter(assembly))
			{
				list.Add(assembly);
			}
		}
		return list;
	}

	public static List<Type> FindAttributeTypes<T>(Assembly assembly) where T : Attribute
	{
		return FindAttributeTypes(assembly, typeof(T));
	}

	public static List<Type> FindAttributeTypes(Assembly assembly, Type attributeType)
	{
		return FindTypes(assembly, delegate(Type type)
		{
			object[] customAttributes = type.GetCustomAttributes(attributeType, inherit: false);
			return customAttributes != null && customAttributes.Length != 0;
		});
	}

	public static List<Type> FindTypes(Assembly assembly, ReflectionTypeFilter filter)
	{
		List<Type> list = new List<Type>();
		try
		{
			foreach (Type assemblyType in GetAssemblyTypes(assembly))
			{
				if (filter(assemblyType))
				{
					list.Add(assemblyType);
				}
			}
		}
		catch (Exception exception)
		{
			Log.e(exception, "Unable to list types for assembly: {0}", assembly);
		}
		return list;
	}

	private static IEnumerable<Type> GetAssemblyTypes(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			List<Type> list = new List<Type>();
			Type[] types = ex.Types;
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] != null)
				{
					list.Add(types[i]);
				}
			}
			return list;
		}
	}
}
