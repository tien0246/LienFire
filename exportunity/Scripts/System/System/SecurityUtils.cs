using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace System;

internal static class SecurityUtils
{
	private static volatile ReflectionPermission memberAccessPermission;

	private static volatile ReflectionPermission restrictedMemberAccessPermission;

	private static ReflectionPermission MemberAccessPermission
	{
		get
		{
			if (memberAccessPermission == null)
			{
				memberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
			}
			return memberAccessPermission;
		}
	}

	private static ReflectionPermission RestrictedMemberAccessPermission
	{
		get
		{
			if (restrictedMemberAccessPermission == null)
			{
				restrictedMemberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess);
			}
			return restrictedMemberAccessPermission;
		}
	}

	private static void DemandReflectionAccess(Type type)
	{
	}

	[SecuritySafeCritical]
	private static void DemandGrantSet(Assembly assembly)
	{
	}

	private static bool HasReflectionPermission(Type type)
	{
		try
		{
			DemandReflectionAccess(type);
			return true;
		}
		catch (SecurityException)
		{
		}
		return false;
	}

	internal static object SecureCreateInstance(Type type)
	{
		return SecureCreateInstance(type, null, allowNonPublic: false);
	}

	internal static object SecureCreateInstance(Type type, object[] args, bool allowNonPublic)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;
		if (!type.IsVisible)
		{
			DemandReflectionAccess(type);
		}
		else if (allowNonPublic && !HasReflectionPermission(type))
		{
			allowNonPublic = false;
		}
		if (allowNonPublic)
		{
			bindingFlags |= BindingFlags.NonPublic;
		}
		return Activator.CreateInstance(type, bindingFlags, null, args, null);
	}

	internal static object SecureCreateInstance(Type type, object[] args)
	{
		return SecureCreateInstance(type, args, allowNonPublic: false);
	}

	internal static object SecureConstructorInvoke(Type type, Type[] argTypes, object[] args, bool allowNonPublic)
	{
		return SecureConstructorInvoke(type, argTypes, args, allowNonPublic, BindingFlags.Default);
	}

	internal static object SecureConstructorInvoke(Type type, Type[] argTypes, object[] args, bool allowNonPublic, BindingFlags extraFlags)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (!type.IsVisible)
		{
			DemandReflectionAccess(type);
		}
		else if (allowNonPublic && !HasReflectionPermission(type))
		{
			allowNonPublic = false;
		}
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | extraFlags;
		if (!allowNonPublic)
		{
			bindingFlags &= ~BindingFlags.NonPublic;
		}
		ConstructorInfo constructor = type.GetConstructor(bindingFlags, null, argTypes, null);
		if (constructor != null)
		{
			return constructor.Invoke(args);
		}
		return null;
	}

	private static bool GenericArgumentsAreVisible(MethodInfo method)
	{
		if (method.IsGenericMethod)
		{
			Type[] genericArguments = method.GetGenericArguments();
			for (int i = 0; i < genericArguments.Length; i++)
			{
				if (!genericArguments[i].IsVisible)
				{
					return false;
				}
			}
		}
		return true;
	}

	internal static object FieldInfoGetValue(FieldInfo field, object target)
	{
		Type declaringType = field.DeclaringType;
		if (declaringType == null)
		{
			if (!field.IsPublic)
			{
				DemandGrantSet(field.Module.Assembly);
			}
		}
		else if (!(declaringType != null) || !declaringType.IsVisible || !field.IsPublic)
		{
			DemandReflectionAccess(declaringType);
		}
		return field.GetValue(target);
	}

	internal static object MethodInfoInvoke(MethodInfo method, object target, object[] args)
	{
		Type declaringType = method.DeclaringType;
		if (declaringType == null)
		{
			if (!method.IsPublic || !GenericArgumentsAreVisible(method))
			{
				DemandGrantSet(method.Module.Assembly);
			}
		}
		else if (!declaringType.IsVisible || !method.IsPublic || !GenericArgumentsAreVisible(method))
		{
			DemandReflectionAccess(declaringType);
		}
		return method.Invoke(target, args);
	}

	internal static object ConstructorInfoInvoke(ConstructorInfo ctor, object[] args)
	{
		Type declaringType = ctor.DeclaringType;
		if (declaringType != null && (!declaringType.IsVisible || !ctor.IsPublic))
		{
			DemandReflectionAccess(declaringType);
		}
		return ctor.Invoke(args);
	}

	internal static object ArrayCreateInstance(Type type, int length)
	{
		if (!type.IsVisible)
		{
			DemandReflectionAccess(type);
		}
		return Array.CreateInstance(type, length);
	}
}
