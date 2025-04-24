using System;
using System.Text;

namespace UnityEngine;

public class AndroidJavaObject : IDisposable
{
	private static bool enableDebugPrints;

	internal GlobalJavaObjectRef m_jobject;

	internal GlobalJavaObjectRef m_jclass;

	public AndroidJavaObject(string className, string[] args)
		: this()
	{
		_AndroidJavaObject(className, new object[1] { args });
	}

	public AndroidJavaObject(string className, AndroidJavaObject[] args)
		: this()
	{
		_AndroidJavaObject(className, new object[1] { args });
	}

	public AndroidJavaObject(string className, AndroidJavaClass[] args)
		: this()
	{
		_AndroidJavaObject(className, new object[1] { args });
	}

	public AndroidJavaObject(string className, AndroidJavaProxy[] args)
		: this()
	{
		_AndroidJavaObject(className, new object[1] { args });
	}

	public AndroidJavaObject(string className, AndroidJavaRunnable[] args)
		: this()
	{
		_AndroidJavaObject(className, new object[1] { args });
	}

	public AndroidJavaObject(string className, params object[] args)
		: this()
	{
		_AndroidJavaObject(className, args);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void Call<T>(string methodName, T[] args)
	{
		_Call(methodName, args);
	}

	public void Call(string methodName, params object[] args)
	{
		_Call(methodName, args);
	}

	public void CallStatic<T>(string methodName, T[] args)
	{
		_CallStatic(methodName, args);
	}

	public void CallStatic(string methodName, params object[] args)
	{
		_CallStatic(methodName, args);
	}

	public FieldType Get<FieldType>(string fieldName)
	{
		return _Get<FieldType>(fieldName);
	}

	public void Set<FieldType>(string fieldName, FieldType val)
	{
		_Set(fieldName, val);
	}

	public FieldType GetStatic<FieldType>(string fieldName)
	{
		return _GetStatic<FieldType>(fieldName);
	}

	public void SetStatic<FieldType>(string fieldName, FieldType val)
	{
		_SetStatic(fieldName, val);
	}

	public IntPtr GetRawObject()
	{
		return _GetRawObject();
	}

	public IntPtr GetRawClass()
	{
		return _GetRawClass();
	}

	public AndroidJavaObject CloneReference()
	{
		if (m_jclass == null)
		{
			throw new Exception("Cannot clone a disposed reference");
		}
		if (m_jobject != null)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject();
			androidJavaObject.m_jobject = new GlobalJavaObjectRef(m_jobject);
			androidJavaObject.m_jclass = new GlobalJavaObjectRef(m_jclass);
			return androidJavaObject;
		}
		return new AndroidJavaClass(m_jclass);
	}

	public ReturnType Call<ReturnType, T>(string methodName, T[] args)
	{
		return _Call<ReturnType>(methodName, new object[1] { args });
	}

	public ReturnType Call<ReturnType>(string methodName, params object[] args)
	{
		return _Call<ReturnType>(methodName, args);
	}

	public ReturnType CallStatic<ReturnType, T>(string methodName, T[] args)
	{
		return _CallStatic<ReturnType>(methodName, new object[1] { args });
	}

	public ReturnType CallStatic<ReturnType>(string methodName, params object[] args)
	{
		return _CallStatic<ReturnType>(methodName, args);
	}

	protected void DebugPrint(string msg)
	{
		if (enableDebugPrints)
		{
			Debug.Log(msg);
		}
	}

	protected void DebugPrint(string call, string methodName, string signature, object[] args)
	{
		if (enableDebugPrints)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in args)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append((obj == null) ? "<null>" : obj.GetType().ToString());
			}
			Debug.Log(call + "(\"" + methodName + "\"" + stringBuilder?.ToString() + ") = " + signature);
		}
	}

	private void _AndroidJavaObject(string className, params object[] args)
	{
		DebugPrint("Creating AndroidJavaObject from " + className);
		if (args == null)
		{
			args = new object[1];
		}
		IntPtr intPtr = AndroidJNISafe.FindClass(className.Replace('.', '/'));
		m_jclass = new GlobalJavaObjectRef(intPtr);
		AndroidJNISafe.DeleteLocalRef(intPtr);
		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
		try
		{
			IntPtr constructorID = AndroidJNIHelper.GetConstructorID(m_jclass, args);
			IntPtr intPtr2 = AndroidJNISafe.NewObject(m_jclass, constructorID, array);
			m_jobject = new GlobalJavaObjectRef(intPtr2);
			AndroidJNISafe.DeleteLocalRef(intPtr2);
		}
		finally
		{
			AndroidJNIHelper.DeleteJNIArgArray(args, array);
		}
	}

	internal AndroidJavaObject(IntPtr jobject)
		: this()
	{
		if (jobject == IntPtr.Zero)
		{
			throw new Exception("JNI: Init'd AndroidJavaObject with null ptr!");
		}
		IntPtr objectClass = AndroidJNISafe.GetObjectClass(jobject);
		m_jobject = new GlobalJavaObjectRef(jobject);
		m_jclass = new GlobalJavaObjectRef(objectClass);
		AndroidJNISafe.DeleteLocalRef(objectClass);
	}

	internal AndroidJavaObject()
	{
	}

	~AndroidJavaObject()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (m_jobject != null)
		{
			m_jobject.Dispose();
			m_jobject = null;
		}
		if (m_jclass != null)
		{
			m_jclass.Dispose();
			m_jclass = null;
		}
	}

	protected void _Call(string methodName, params object[] args)
	{
		if (args == null)
		{
			args = new object[1];
		}
		IntPtr methodID = AndroidJNIHelper.GetMethodID(m_jclass, methodName, args, isStatic: false);
		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
		try
		{
			AndroidJNISafe.CallVoidMethod(m_jobject, methodID, array);
		}
		finally
		{
			AndroidJNIHelper.DeleteJNIArgArray(args, array);
		}
	}

	protected ReturnType _Call<ReturnType>(string methodName, params object[] args)
	{
		if (args == null)
		{
			args = new object[1];
		}
		IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(m_jclass, methodName, args, isStatic: false);
		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
		try
		{
			if (AndroidReflection.IsPrimitive(typeof(ReturnType)))
			{
				if ((object)typeof(ReturnType) == typeof(int))
				{
					return (ReturnType)(object)AndroidJNISafe.CallIntMethod(m_jobject, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(bool))
				{
					return (ReturnType)(object)AndroidJNISafe.CallBooleanMethod(m_jobject, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(byte))
				{
					Debug.LogWarning("Return type <Byte> for Java method call is obsolete, use return type <SByte> instead");
					return (ReturnType)(object)(byte)AndroidJNISafe.CallSByteMethod(m_jobject, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(sbyte))
				{
					return (ReturnType)(object)AndroidJNISafe.CallSByteMethod(m_jobject, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(short))
				{
					return (ReturnType)(object)AndroidJNISafe.CallShortMethod(m_jobject, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(long))
				{
					return (ReturnType)(object)AndroidJNISafe.CallLongMethod(m_jobject, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(float))
				{
					return (ReturnType)(object)AndroidJNISafe.CallFloatMethod(m_jobject, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(double))
				{
					return (ReturnType)(object)AndroidJNISafe.CallDoubleMethod(m_jobject, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(char))
				{
					return (ReturnType)(object)AndroidJNISafe.CallCharMethod(m_jobject, methodID, array);
				}
				return default(ReturnType);
			}
			if ((object)typeof(ReturnType) == typeof(string))
			{
				return (ReturnType)(object)AndroidJNISafe.CallStringMethod(m_jobject, methodID, array);
			}
			if ((object)typeof(ReturnType) == typeof(AndroidJavaClass))
			{
				IntPtr intPtr = AndroidJNISafe.CallObjectMethod(m_jobject, methodID, array);
				return (intPtr == IntPtr.Zero) ? default(ReturnType) : ((ReturnType)(object)AndroidJavaClassDeleteLocalRef(intPtr));
			}
			if ((object)typeof(ReturnType) == typeof(AndroidJavaObject))
			{
				IntPtr intPtr2 = AndroidJNISafe.CallObjectMethod(m_jobject, methodID, array);
				return (intPtr2 == IntPtr.Zero) ? default(ReturnType) : ((ReturnType)(object)AndroidJavaObjectDeleteLocalRef(intPtr2));
			}
			if (AndroidReflection.IsAssignableFrom(typeof(Array), typeof(ReturnType)))
			{
				IntPtr jobject = AndroidJNISafe.CallObjectMethod(m_jobject, methodID, array);
				return FromJavaArrayDeleteLocalRef<ReturnType>(jobject);
			}
			throw new Exception("JNI: Unknown return type '" + typeof(ReturnType)?.ToString() + "'");
		}
		finally
		{
			AndroidJNIHelper.DeleteJNIArgArray(args, array);
		}
	}

	protected FieldType _Get<FieldType>(string fieldName)
	{
		IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, isStatic: false);
		if (AndroidReflection.IsPrimitive(typeof(FieldType)))
		{
			if ((object)typeof(FieldType) == typeof(int))
			{
				return (FieldType)(object)AndroidJNISafe.GetIntField(m_jobject, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(bool))
			{
				return (FieldType)(object)AndroidJNISafe.GetBooleanField(m_jobject, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(byte))
			{
				Debug.LogWarning("Field type <Byte> for Java get field call is obsolete, use field type <SByte> instead");
				return (FieldType)(object)(byte)AndroidJNISafe.GetSByteField(m_jobject, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(sbyte))
			{
				return (FieldType)(object)AndroidJNISafe.GetSByteField(m_jobject, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(short))
			{
				return (FieldType)(object)AndroidJNISafe.GetShortField(m_jobject, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(long))
			{
				return (FieldType)(object)AndroidJNISafe.GetLongField(m_jobject, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(float))
			{
				return (FieldType)(object)AndroidJNISafe.GetFloatField(m_jobject, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(double))
			{
				return (FieldType)(object)AndroidJNISafe.GetDoubleField(m_jobject, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(char))
			{
				return (FieldType)(object)AndroidJNISafe.GetCharField(m_jobject, fieldID);
			}
			return default(FieldType);
		}
		if ((object)typeof(FieldType) == typeof(string))
		{
			return (FieldType)(object)AndroidJNISafe.GetStringField(m_jobject, fieldID);
		}
		if ((object)typeof(FieldType) == typeof(AndroidJavaClass))
		{
			IntPtr objectField = AndroidJNISafe.GetObjectField(m_jobject, fieldID);
			return (objectField == IntPtr.Zero) ? default(FieldType) : ((FieldType)(object)AndroidJavaClassDeleteLocalRef(objectField));
		}
		if ((object)typeof(FieldType) == typeof(AndroidJavaObject))
		{
			IntPtr objectField2 = AndroidJNISafe.GetObjectField(m_jobject, fieldID);
			return (objectField2 == IntPtr.Zero) ? default(FieldType) : ((FieldType)(object)AndroidJavaObjectDeleteLocalRef(objectField2));
		}
		if (AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
		{
			IntPtr objectField3 = AndroidJNISafe.GetObjectField(m_jobject, fieldID);
			return FromJavaArrayDeleteLocalRef<FieldType>(objectField3);
		}
		throw new Exception("JNI: Unknown field type '" + typeof(FieldType)?.ToString() + "'");
	}

	protected void _Set<FieldType>(string fieldName, FieldType val)
	{
		IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, isStatic: false);
		if (AndroidReflection.IsPrimitive(typeof(FieldType)))
		{
			if ((object)typeof(FieldType) == typeof(int))
			{
				AndroidJNISafe.SetIntField(m_jobject, fieldID, (int)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(bool))
			{
				AndroidJNISafe.SetBooleanField(m_jobject, fieldID, (bool)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(byte))
			{
				Debug.LogWarning("Field type <Byte> for Java set field call is obsolete, use field type <SByte> instead");
				AndroidJNISafe.SetSByteField(m_jobject, fieldID, (sbyte)(byte)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(sbyte))
			{
				AndroidJNISafe.SetSByteField(m_jobject, fieldID, (sbyte)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(short))
			{
				AndroidJNISafe.SetShortField(m_jobject, fieldID, (short)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(long))
			{
				AndroidJNISafe.SetLongField(m_jobject, fieldID, (long)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(float))
			{
				AndroidJNISafe.SetFloatField(m_jobject, fieldID, (float)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(double))
			{
				AndroidJNISafe.SetDoubleField(m_jobject, fieldID, (double)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(char))
			{
				AndroidJNISafe.SetCharField(m_jobject, fieldID, (char)(object)val);
			}
		}
		else if ((object)typeof(FieldType) == typeof(string))
		{
			AndroidJNISafe.SetStringField(m_jobject, fieldID, (string)(object)val);
		}
		else if ((object)typeof(FieldType) == typeof(AndroidJavaClass))
		{
			AndroidJNISafe.SetObjectField(m_jobject, fieldID, (val == null) ? IntPtr.Zero : ((IntPtr)((AndroidJavaClass)(object)val).m_jclass));
		}
		else if ((object)typeof(FieldType) == typeof(AndroidJavaObject))
		{
			AndroidJNISafe.SetObjectField(m_jobject, fieldID, (val == null) ? IntPtr.Zero : ((IntPtr)((AndroidJavaObject)(object)val).m_jobject));
		}
		else
		{
			if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
			{
				throw new Exception("JNI: Unknown field type '" + typeof(FieldType)?.ToString() + "'");
			}
			IntPtr val2 = AndroidJNIHelper.ConvertToJNIArray((Array)(object)val);
			AndroidJNISafe.SetObjectField(m_jclass, fieldID, val2);
		}
	}

	protected void _CallStatic(string methodName, params object[] args)
	{
		if (args == null)
		{
			args = new object[1];
		}
		IntPtr methodID = AndroidJNIHelper.GetMethodID(m_jclass, methodName, args, isStatic: true);
		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
		try
		{
			AndroidJNISafe.CallStaticVoidMethod(m_jclass, methodID, array);
		}
		finally
		{
			AndroidJNIHelper.DeleteJNIArgArray(args, array);
		}
	}

	protected ReturnType _CallStatic<ReturnType>(string methodName, params object[] args)
	{
		if (args == null)
		{
			args = new object[1];
		}
		IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(m_jclass, methodName, args, isStatic: true);
		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
		try
		{
			if (AndroidReflection.IsPrimitive(typeof(ReturnType)))
			{
				if ((object)typeof(ReturnType) == typeof(int))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticIntMethod(m_jclass, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(bool))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticBooleanMethod(m_jclass, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(byte))
				{
					Debug.LogWarning("Return type <Byte> for Java method call is obsolete, use return type <SByte> instead");
					return (ReturnType)(object)(byte)AndroidJNISafe.CallStaticSByteMethod(m_jclass, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(sbyte))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticSByteMethod(m_jclass, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(short))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticShortMethod(m_jclass, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(long))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticLongMethod(m_jclass, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(float))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticFloatMethod(m_jclass, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(double))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticDoubleMethod(m_jclass, methodID, array);
				}
				if ((object)typeof(ReturnType) == typeof(char))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticCharMethod(m_jclass, methodID, array);
				}
				return default(ReturnType);
			}
			if ((object)typeof(ReturnType) == typeof(string))
			{
				return (ReturnType)(object)AndroidJNISafe.CallStaticStringMethod(m_jclass, methodID, array);
			}
			if ((object)typeof(ReturnType) == typeof(AndroidJavaClass))
			{
				IntPtr intPtr = AndroidJNISafe.CallStaticObjectMethod(m_jclass, methodID, array);
				return (intPtr == IntPtr.Zero) ? default(ReturnType) : ((ReturnType)(object)AndroidJavaClassDeleteLocalRef(intPtr));
			}
			if ((object)typeof(ReturnType) == typeof(AndroidJavaObject))
			{
				IntPtr intPtr2 = AndroidJNISafe.CallStaticObjectMethod(m_jclass, methodID, array);
				return (intPtr2 == IntPtr.Zero) ? default(ReturnType) : ((ReturnType)(object)AndroidJavaObjectDeleteLocalRef(intPtr2));
			}
			if (AndroidReflection.IsAssignableFrom(typeof(Array), typeof(ReturnType)))
			{
				IntPtr jobject = AndroidJNISafe.CallStaticObjectMethod(m_jclass, methodID, array);
				return FromJavaArrayDeleteLocalRef<ReturnType>(jobject);
			}
			throw new Exception("JNI: Unknown return type '" + typeof(ReturnType)?.ToString() + "'");
		}
		finally
		{
			AndroidJNIHelper.DeleteJNIArgArray(args, array);
		}
	}

	protected FieldType _GetStatic<FieldType>(string fieldName)
	{
		IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, isStatic: true);
		if (AndroidReflection.IsPrimitive(typeof(FieldType)))
		{
			if ((object)typeof(FieldType) == typeof(int))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticIntField(m_jclass, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(bool))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticBooleanField(m_jclass, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(byte))
			{
				Debug.LogWarning("Field type <Byte> for Java get field call is obsolete, use field type <SByte> instead");
				return (FieldType)(object)(byte)AndroidJNISafe.GetStaticSByteField(m_jclass, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(sbyte))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticSByteField(m_jclass, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(short))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticShortField(m_jclass, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(long))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticLongField(m_jclass, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(float))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticFloatField(m_jclass, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(double))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticDoubleField(m_jclass, fieldID);
			}
			if ((object)typeof(FieldType) == typeof(char))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticCharField(m_jclass, fieldID);
			}
			return default(FieldType);
		}
		if ((object)typeof(FieldType) == typeof(string))
		{
			return (FieldType)(object)AndroidJNISafe.GetStaticStringField(m_jclass, fieldID);
		}
		if ((object)typeof(FieldType) == typeof(AndroidJavaClass))
		{
			IntPtr staticObjectField = AndroidJNISafe.GetStaticObjectField(m_jclass, fieldID);
			return (staticObjectField == IntPtr.Zero) ? default(FieldType) : ((FieldType)(object)AndroidJavaClassDeleteLocalRef(staticObjectField));
		}
		if ((object)typeof(FieldType) == typeof(AndroidJavaObject))
		{
			IntPtr staticObjectField2 = AndroidJNISafe.GetStaticObjectField(m_jclass, fieldID);
			return (staticObjectField2 == IntPtr.Zero) ? default(FieldType) : ((FieldType)(object)AndroidJavaObjectDeleteLocalRef(staticObjectField2));
		}
		if (AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
		{
			IntPtr staticObjectField3 = AndroidJNISafe.GetStaticObjectField(m_jclass, fieldID);
			return FromJavaArrayDeleteLocalRef<FieldType>(staticObjectField3);
		}
		throw new Exception("JNI: Unknown field type '" + typeof(FieldType)?.ToString() + "'");
	}

	protected void _SetStatic<FieldType>(string fieldName, FieldType val)
	{
		IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, isStatic: true);
		if (AndroidReflection.IsPrimitive(typeof(FieldType)))
		{
			if ((object)typeof(FieldType) == typeof(int))
			{
				AndroidJNISafe.SetStaticIntField(m_jclass, fieldID, (int)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(bool))
			{
				AndroidJNISafe.SetStaticBooleanField(m_jclass, fieldID, (bool)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(byte))
			{
				Debug.LogWarning("Field type <Byte> for Java set field call is obsolete, use field type <SByte> instead");
				AndroidJNISafe.SetStaticSByteField(m_jclass, fieldID, (sbyte)(byte)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(sbyte))
			{
				AndroidJNISafe.SetStaticSByteField(m_jclass, fieldID, (sbyte)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(short))
			{
				AndroidJNISafe.SetStaticShortField(m_jclass, fieldID, (short)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(long))
			{
				AndroidJNISafe.SetStaticLongField(m_jclass, fieldID, (long)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(float))
			{
				AndroidJNISafe.SetStaticFloatField(m_jclass, fieldID, (float)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(double))
			{
				AndroidJNISafe.SetStaticDoubleField(m_jclass, fieldID, (double)(object)val);
			}
			else if ((object)typeof(FieldType) == typeof(char))
			{
				AndroidJNISafe.SetStaticCharField(m_jclass, fieldID, (char)(object)val);
			}
		}
		else if ((object)typeof(FieldType) == typeof(string))
		{
			AndroidJNISafe.SetStaticStringField(m_jclass, fieldID, (string)(object)val);
		}
		else if ((object)typeof(FieldType) == typeof(AndroidJavaClass))
		{
			AndroidJNISafe.SetStaticObjectField(m_jclass, fieldID, (val == null) ? IntPtr.Zero : ((IntPtr)((AndroidJavaClass)(object)val).m_jclass));
		}
		else if ((object)typeof(FieldType) == typeof(AndroidJavaObject))
		{
			AndroidJNISafe.SetStaticObjectField(m_jclass, fieldID, (val == null) ? IntPtr.Zero : ((IntPtr)((AndroidJavaObject)(object)val).m_jobject));
		}
		else
		{
			if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
			{
				throw new Exception("JNI: Unknown field type '" + typeof(FieldType)?.ToString() + "'");
			}
			IntPtr val2 = AndroidJNIHelper.ConvertToJNIArray((Array)(object)val);
			AndroidJNISafe.SetStaticObjectField(m_jclass, fieldID, val2);
		}
	}

	internal static AndroidJavaObject AndroidJavaObjectDeleteLocalRef(IntPtr jobject)
	{
		try
		{
			return new AndroidJavaObject(jobject);
		}
		finally
		{
			AndroidJNISafe.DeleteLocalRef(jobject);
		}
	}

	internal static AndroidJavaClass AndroidJavaClassDeleteLocalRef(IntPtr jclass)
	{
		try
		{
			return new AndroidJavaClass(jclass);
		}
		finally
		{
			AndroidJNISafe.DeleteLocalRef(jclass);
		}
	}

	internal static ReturnType FromJavaArrayDeleteLocalRef<ReturnType>(IntPtr jobject)
	{
		if (jobject == IntPtr.Zero)
		{
			return default(ReturnType);
		}
		try
		{
			return (ReturnType)(object)AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(jobject);
		}
		finally
		{
			AndroidJNISafe.DeleteLocalRef(jobject);
		}
	}

	protected IntPtr _GetRawObject()
	{
		return (m_jobject == null) ? IntPtr.Zero : ((IntPtr)m_jobject);
	}

	protected IntPtr _GetRawClass()
	{
		return m_jclass;
	}
}
