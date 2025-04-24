using System;
using System.Text;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
internal sealed class _AndroidJNIHelper
{
	public static IntPtr CreateJavaProxy(IntPtr player, IntPtr delegateHandle, AndroidJavaProxy proxy)
	{
		return AndroidReflection.NewProxyInstance(player, delegateHandle, proxy.javaInterface.GetRawClass());
	}

	public static IntPtr CreateJavaRunnable(AndroidJavaRunnable jrunnable)
	{
		return AndroidJNIHelper.CreateJavaProxy(new AndroidJavaRunnableProxy(jrunnable));
	}

	[RequiredByNativeCode]
	public static IntPtr InvokeJavaProxyMethod(AndroidJavaProxy proxy, IntPtr jmethodName, IntPtr jargs)
	{
		try
		{
			int num = 0;
			if (jargs != IntPtr.Zero)
			{
				num = AndroidJNISafe.GetArrayLength(jargs);
			}
			AndroidJavaObject[] array = new AndroidJavaObject[num];
			for (int i = 0; i < num; i++)
			{
				IntPtr objectArrayElement = AndroidJNISafe.GetObjectArrayElement(jargs, i);
				array[i] = ((objectArrayElement != IntPtr.Zero) ? new AndroidJavaObject(objectArrayElement) : null);
			}
			using AndroidJavaObject androidJavaObject = proxy.Invoke(AndroidJNI.GetStringChars(jmethodName), array);
			if (androidJavaObject == null)
			{
				return IntPtr.Zero;
			}
			return AndroidJNI.NewLocalRef(androidJavaObject.GetRawObject());
		}
		catch (Exception e)
		{
			AndroidReflection.SetNativeExceptionOnProxy(proxy.GetRawProxy(), e, methodNotFound: false);
			return IntPtr.Zero;
		}
	}

	public static jvalue[] CreateJNIArgArray(object[] args)
	{
		jvalue[] array = new jvalue[args.GetLength(0)];
		int num = 0;
		foreach (object obj in args)
		{
			if (obj == null)
			{
				array[num].l = IntPtr.Zero;
			}
			else if (AndroidReflection.IsPrimitive(obj.GetType()))
			{
				if (obj is int)
				{
					array[num].i = (int)obj;
				}
				else if (obj is bool)
				{
					array[num].z = (bool)obj;
				}
				else if (obj is byte)
				{
					Debug.LogWarning("Passing Byte arguments to Java methods is obsolete, pass SByte parameters instead");
					array[num].b = (sbyte)(byte)obj;
				}
				else if (obj is sbyte)
				{
					array[num].b = (sbyte)obj;
				}
				else if (obj is short)
				{
					array[num].s = (short)obj;
				}
				else if (obj is long)
				{
					array[num].j = (long)obj;
				}
				else if (obj is float)
				{
					array[num].f = (float)obj;
				}
				else if (obj is double)
				{
					array[num].d = (double)obj;
				}
				else if (obj is char)
				{
					array[num].c = (char)obj;
				}
			}
			else if (obj is string)
			{
				array[num].l = AndroidJNISafe.NewString((string)obj);
			}
			else if (obj is AndroidJavaClass)
			{
				array[num].l = ((AndroidJavaClass)obj).GetRawClass();
			}
			else if (obj is AndroidJavaObject)
			{
				array[num].l = ((AndroidJavaObject)obj).GetRawObject();
			}
			else if (obj is Array)
			{
				array[num].l = ConvertToJNIArray((Array)obj);
			}
			else if (obj is AndroidJavaProxy)
			{
				array[num].l = ((AndroidJavaProxy)obj).GetRawProxy();
			}
			else
			{
				if (!(obj is AndroidJavaRunnable))
				{
					throw new Exception("JNI; Unknown argument type '" + obj.GetType()?.ToString() + "'");
				}
				array[num].l = AndroidJNIHelper.CreateJavaRunnable((AndroidJavaRunnable)obj);
			}
			num++;
		}
		return array;
	}

	public static object UnboxArray(AndroidJavaObject obj)
	{
		if (obj == null)
		{
			return null;
		}
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("java/lang/reflect/Array");
		AndroidJavaObject androidJavaObject = obj.Call<AndroidJavaObject>("getClass", new object[0]);
		AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getComponentType", new object[0]);
		string text = androidJavaObject2.Call<string>("getName", new object[0]);
		int num = androidJavaClass.CallStatic<int>("getLength", new object[1] { obj });
		Array array;
		if (!androidJavaObject2.Call<bool>("isPrimitive", new object[0]))
		{
			array = (("java.lang.String" == text) ? ((Array)new string[num]) : ((Array)((!("java.lang.Class" == text)) ? new AndroidJavaObject[num] : new AndroidJavaClass[num])));
		}
		else if ("int" == text)
		{
			array = new int[num];
		}
		else if ("boolean" == text)
		{
			array = new bool[num];
		}
		else if ("byte" == text)
		{
			array = new sbyte[num];
		}
		else if ("short" == text)
		{
			array = new short[num];
		}
		else if ("long" == text)
		{
			array = new long[num];
		}
		else if ("float" == text)
		{
			array = new float[num];
		}
		else if ("double" == text)
		{
			array = new double[num];
		}
		else
		{
			if (!("char" == text))
			{
				throw new Exception("JNI; Unknown argument type '" + text + "'");
			}
			array = new char[num];
		}
		for (int i = 0; i < num; i++)
		{
			array.SetValue(Unbox(androidJavaClass.CallStatic<AndroidJavaObject>("get", new object[2] { obj, i })), i);
		}
		androidJavaClass.Dispose();
		return array;
	}

	public static object Unbox(AndroidJavaObject obj)
	{
		if (obj == null)
		{
			return null;
		}
		using AndroidJavaObject androidJavaObject = obj.Call<AndroidJavaObject>("getClass", new object[0]);
		string text = androidJavaObject.Call<string>("getName", new object[0]);
		if ("java.lang.Integer" == text)
		{
			return obj.Call<int>("intValue", new object[0]);
		}
		if ("java.lang.Boolean" == text)
		{
			return obj.Call<bool>("booleanValue", new object[0]);
		}
		if ("java.lang.Byte" == text)
		{
			return obj.Call<sbyte>("byteValue", new object[0]);
		}
		if ("java.lang.Short" == text)
		{
			return obj.Call<short>("shortValue", new object[0]);
		}
		if ("java.lang.Long" == text)
		{
			return obj.Call<long>("longValue", new object[0]);
		}
		if ("java.lang.Float" == text)
		{
			return obj.Call<float>("floatValue", new object[0]);
		}
		if ("java.lang.Double" == text)
		{
			return obj.Call<double>("doubleValue", new object[0]);
		}
		if ("java.lang.Character" == text)
		{
			return obj.Call<char>("charValue", new object[0]);
		}
		if ("java.lang.String" == text)
		{
			return obj.Call<string>("toString", new object[0]);
		}
		if ("java.lang.Class" == text)
		{
			return new AndroidJavaClass(obj.GetRawObject());
		}
		if (androidJavaObject.Call<bool>("isArray", new object[0]))
		{
			return UnboxArray(obj);
		}
		return obj;
	}

	public static AndroidJavaObject Box(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (AndroidReflection.IsPrimitive(obj.GetType()))
		{
			if (obj is int)
			{
				return new AndroidJavaObject("java.lang.Integer", (int)obj);
			}
			if (obj is bool)
			{
				return new AndroidJavaObject("java.lang.Boolean", (bool)obj);
			}
			if (obj is byte)
			{
				return new AndroidJavaObject("java.lang.Byte", (sbyte)obj);
			}
			if (obj is sbyte)
			{
				return new AndroidJavaObject("java.lang.Byte", (sbyte)obj);
			}
			if (obj is short)
			{
				return new AndroidJavaObject("java.lang.Short", (short)obj);
			}
			if (obj is long)
			{
				return new AndroidJavaObject("java.lang.Long", (long)obj);
			}
			if (obj is float)
			{
				return new AndroidJavaObject("java.lang.Float", (float)obj);
			}
			if (obj is double)
			{
				return new AndroidJavaObject("java.lang.Double", (double)obj);
			}
			if (obj is char)
			{
				return new AndroidJavaObject("java.lang.Character", (char)obj);
			}
			throw new Exception("JNI; Unknown argument type '" + obj.GetType()?.ToString() + "'");
		}
		if (obj is string)
		{
			return new AndroidJavaObject("java.lang.String", (string)obj);
		}
		if (obj is AndroidJavaClass)
		{
			return new AndroidJavaObject(((AndroidJavaClass)obj).GetRawClass());
		}
		if (obj is AndroidJavaObject)
		{
			return (AndroidJavaObject)obj;
		}
		if (obj is Array)
		{
			return AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(ConvertToJNIArray((Array)obj));
		}
		if (obj is AndroidJavaProxy)
		{
			return ((AndroidJavaProxy)obj).GetProxyObject();
		}
		if (obj is AndroidJavaRunnable)
		{
			return AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(AndroidJNIHelper.CreateJavaRunnable((AndroidJavaRunnable)obj));
		}
		throw new Exception("JNI; Unknown argument type '" + obj.GetType()?.ToString() + "'");
	}

	public static void DeleteJNIArgArray(object[] args, jvalue[] jniArgs)
	{
		int num = 0;
		foreach (object obj in args)
		{
			if (obj is string || obj is AndroidJavaRunnable || obj is AndroidJavaProxy || obj is Array)
			{
				AndroidJNISafe.DeleteLocalRef(jniArgs[num].l);
			}
			num++;
		}
	}

	public static IntPtr ConvertToJNIArray(Array array)
	{
		Type elementType = array.GetType().GetElementType();
		if (AndroidReflection.IsPrimitive(elementType))
		{
			if ((object)elementType == typeof(int))
			{
				return AndroidJNISafe.ToIntArray((int[])array);
			}
			if ((object)elementType == typeof(bool))
			{
				return AndroidJNISafe.ToBooleanArray((bool[])array);
			}
			if ((object)elementType == typeof(byte))
			{
				Debug.LogWarning("AndroidJNIHelper: converting Byte array is obsolete, use SByte array instead");
				return AndroidJNISafe.ToByteArray((byte[])array);
			}
			if ((object)elementType == typeof(sbyte))
			{
				return AndroidJNISafe.ToSByteArray((sbyte[])array);
			}
			if ((object)elementType == typeof(short))
			{
				return AndroidJNISafe.ToShortArray((short[])array);
			}
			if ((object)elementType == typeof(long))
			{
				return AndroidJNISafe.ToLongArray((long[])array);
			}
			if ((object)elementType == typeof(float))
			{
				return AndroidJNISafe.ToFloatArray((float[])array);
			}
			if ((object)elementType == typeof(double))
			{
				return AndroidJNISafe.ToDoubleArray((double[])array);
			}
			if ((object)elementType == typeof(char))
			{
				return AndroidJNISafe.ToCharArray((char[])array);
			}
			return IntPtr.Zero;
		}
		if ((object)elementType == typeof(string))
		{
			string[] array2 = (string[])array;
			int length = array.GetLength(0);
			IntPtr intPtr = AndroidJNISafe.FindClass("java/lang/String");
			IntPtr intPtr2 = AndroidJNI.NewObjectArray(length, intPtr, IntPtr.Zero);
			for (int i = 0; i < length; i++)
			{
				IntPtr intPtr3 = AndroidJNISafe.NewString(array2[i]);
				AndroidJNI.SetObjectArrayElement(intPtr2, i, intPtr3);
				AndroidJNISafe.DeleteLocalRef(intPtr3);
			}
			AndroidJNISafe.DeleteLocalRef(intPtr);
			return intPtr2;
		}
		if ((object)elementType == typeof(AndroidJavaObject))
		{
			AndroidJavaObject[] array3 = (AndroidJavaObject[])array;
			int length2 = array.GetLength(0);
			IntPtr[] array4 = new IntPtr[length2];
			IntPtr intPtr4 = AndroidJNISafe.FindClass("java/lang/Object");
			IntPtr intPtr5 = IntPtr.Zero;
			for (int j = 0; j < length2; j++)
			{
				if (array3[j] != null)
				{
					array4[j] = array3[j].GetRawObject();
					IntPtr rawClass = array3[j].GetRawClass();
					if (intPtr5 != rawClass)
					{
						intPtr5 = ((!(intPtr5 == IntPtr.Zero)) ? intPtr4 : rawClass);
					}
				}
				else
				{
					array4[j] = IntPtr.Zero;
				}
			}
			IntPtr result = AndroidJNISafe.ToObjectArray(array4, intPtr5);
			AndroidJNISafe.DeleteLocalRef(intPtr4);
			return result;
		}
		throw new Exception("JNI; Unknown array type '" + elementType?.ToString() + "'");
	}

	public static ArrayType ConvertFromJNIArray<ArrayType>(IntPtr array)
	{
		Type elementType = typeof(ArrayType).GetElementType();
		if (AndroidReflection.IsPrimitive(elementType))
		{
			if ((object)elementType == typeof(int))
			{
				return (ArrayType)(object)AndroidJNISafe.FromIntArray(array);
			}
			if ((object)elementType == typeof(bool))
			{
				return (ArrayType)(object)AndroidJNISafe.FromBooleanArray(array);
			}
			if ((object)elementType == typeof(byte))
			{
				Debug.LogWarning("AndroidJNIHelper: converting from Byte array is obsolete, use SByte array instead");
				return (ArrayType)(object)AndroidJNISafe.FromByteArray(array);
			}
			if ((object)elementType == typeof(sbyte))
			{
				return (ArrayType)(object)AndroidJNISafe.FromSByteArray(array);
			}
			if ((object)elementType == typeof(short))
			{
				return (ArrayType)(object)AndroidJNISafe.FromShortArray(array);
			}
			if ((object)elementType == typeof(long))
			{
				return (ArrayType)(object)AndroidJNISafe.FromLongArray(array);
			}
			if ((object)elementType == typeof(float))
			{
				return (ArrayType)(object)AndroidJNISafe.FromFloatArray(array);
			}
			if ((object)elementType == typeof(double))
			{
				return (ArrayType)(object)AndroidJNISafe.FromDoubleArray(array);
			}
			if ((object)elementType == typeof(char))
			{
				return (ArrayType)(object)AndroidJNISafe.FromCharArray(array);
			}
			return default(ArrayType);
		}
		if ((object)elementType == typeof(string))
		{
			int arrayLength = AndroidJNISafe.GetArrayLength(array);
			string[] array2 = new string[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				IntPtr objectArrayElement = AndroidJNI.GetObjectArrayElement(array, i);
				array2[i] = AndroidJNISafe.GetStringChars(objectArrayElement);
				AndroidJNISafe.DeleteLocalRef(objectArrayElement);
			}
			return (ArrayType)(object)array2;
		}
		if ((object)elementType == typeof(AndroidJavaObject))
		{
			int arrayLength2 = AndroidJNISafe.GetArrayLength(array);
			AndroidJavaObject[] array3 = new AndroidJavaObject[arrayLength2];
			for (int j = 0; j < arrayLength2; j++)
			{
				IntPtr objectArrayElement2 = AndroidJNI.GetObjectArrayElement(array, j);
				array3[j] = new AndroidJavaObject(objectArrayElement2);
				AndroidJNISafe.DeleteLocalRef(objectArrayElement2);
			}
			return (ArrayType)(object)array3;
		}
		throw new Exception("JNI: Unknown generic array type '" + elementType?.ToString() + "'");
	}

	public static IntPtr GetConstructorID(IntPtr jclass, object[] args)
	{
		return AndroidJNIHelper.GetConstructorID(jclass, GetSignature(args));
	}

	public static IntPtr GetMethodID(IntPtr jclass, string methodName, object[] args, bool isStatic)
	{
		return AndroidJNIHelper.GetMethodID(jclass, methodName, GetSignature(args), isStatic);
	}

	public static IntPtr GetMethodID<ReturnType>(IntPtr jclass, string methodName, object[] args, bool isStatic)
	{
		return AndroidJNIHelper.GetMethodID(jclass, methodName, GetSignature<ReturnType>(args), isStatic);
	}

	public static IntPtr GetFieldID<ReturnType>(IntPtr jclass, string fieldName, bool isStatic)
	{
		return AndroidJNIHelper.GetFieldID(jclass, fieldName, GetSignature(typeof(ReturnType)), isStatic);
	}

	public static IntPtr GetConstructorID(IntPtr jclass, string signature)
	{
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = AndroidReflection.GetConstructorMember(jclass, signature);
			return AndroidJNISafe.FromReflectedMethod(intPtr);
		}
		catch (Exception ex)
		{
			IntPtr methodID = AndroidJNISafe.GetMethodID(jclass, "<init>", signature);
			if (methodID != IntPtr.Zero)
			{
				return methodID;
			}
			throw ex;
		}
		finally
		{
			AndroidJNISafe.DeleteLocalRef(intPtr);
		}
	}

	public static IntPtr GetMethodID(IntPtr jclass, string methodName, string signature, bool isStatic)
	{
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = AndroidReflection.GetMethodMember(jclass, methodName, signature, isStatic);
			return AndroidJNISafe.FromReflectedMethod(intPtr);
		}
		catch (Exception ex)
		{
			IntPtr methodIDFallback = GetMethodIDFallback(jclass, methodName, signature, isStatic);
			if (methodIDFallback != IntPtr.Zero)
			{
				return methodIDFallback;
			}
			throw ex;
		}
		finally
		{
			AndroidJNISafe.DeleteLocalRef(intPtr);
		}
	}

	private static IntPtr GetMethodIDFallback(IntPtr jclass, string methodName, string signature, bool isStatic)
	{
		try
		{
			return isStatic ? AndroidJNISafe.GetStaticMethodID(jclass, methodName, signature) : AndroidJNISafe.GetMethodID(jclass, methodName, signature);
		}
		catch (Exception)
		{
		}
		return IntPtr.Zero;
	}

	public static IntPtr GetFieldID(IntPtr jclass, string fieldName, string signature, bool isStatic)
	{
		IntPtr zero = IntPtr.Zero;
		Exception ex = null;
		AndroidJNI.PushLocalFrame(10);
		try
		{
			IntPtr fieldMember = AndroidReflection.GetFieldMember(jclass, fieldName, signature, isStatic);
			if (!isStatic)
			{
				jclass = AndroidReflection.GetFieldClass(fieldMember);
			}
			signature = AndroidReflection.GetFieldSignature(fieldMember);
		}
		catch (Exception ex2)
		{
			ex = ex2;
		}
		try
		{
			zero = (isStatic ? AndroidJNISafe.GetStaticFieldID(jclass, fieldName, signature) : AndroidJNISafe.GetFieldID(jclass, fieldName, signature));
			if (zero == IntPtr.Zero)
			{
				if (ex != null)
				{
					throw ex;
				}
				throw new Exception($"Field {fieldName} or type signature {signature} not found");
			}
			return zero;
		}
		finally
		{
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
	}

	public static string GetSignature(object obj)
	{
		if (obj == null)
		{
			return "Ljava/lang/Object;";
		}
		Type type = ((obj is Type) ? ((Type)obj) : obj.GetType());
		if (AndroidReflection.IsPrimitive(type))
		{
			if (type.Equals(typeof(int)))
			{
				return "I";
			}
			if (type.Equals(typeof(bool)))
			{
				return "Z";
			}
			if (type.Equals(typeof(byte)))
			{
				Debug.LogWarning("AndroidJNIHelper.GetSignature: using Byte parameters is obsolete, use SByte parameters instead");
				return "B";
			}
			if (type.Equals(typeof(sbyte)))
			{
				return "B";
			}
			if (type.Equals(typeof(short)))
			{
				return "S";
			}
			if (type.Equals(typeof(long)))
			{
				return "J";
			}
			if (type.Equals(typeof(float)))
			{
				return "F";
			}
			if (type.Equals(typeof(double)))
			{
				return "D";
			}
			if (type.Equals(typeof(char)))
			{
				return "C";
			}
			return "";
		}
		if (type.Equals(typeof(string)))
		{
			return "Ljava/lang/String;";
		}
		if (obj is AndroidJavaProxy)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject(((AndroidJavaProxy)obj).javaInterface.GetRawClass()))
			{
				return "L" + androidJavaObject.Call<string>("getName", new object[0]) + ";";
			}
		}
		if (type.Equals(typeof(AndroidJavaRunnable)))
		{
			return "Ljava/lang/Runnable;";
		}
		if (type.Equals(typeof(AndroidJavaClass)))
		{
			return "Ljava/lang/Class;";
		}
		if (type.Equals(typeof(AndroidJavaObject)))
		{
			if (obj == type)
			{
				return "Ljava/lang/Object;";
			}
			AndroidJavaObject androidJavaObject2 = (AndroidJavaObject)obj;
			using AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getClass", new object[0]);
			return "L" + androidJavaObject3.Call<string>("getName", new object[0]) + ";";
		}
		if (AndroidReflection.IsAssignableFrom(typeof(Array), type))
		{
			if (type.GetArrayRank() != 1)
			{
				throw new Exception("JNI: System.Array in n dimensions is not allowed");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			stringBuilder.Append(GetSignature(type.GetElementType()));
			return stringBuilder.ToString();
		}
		throw new Exception("JNI: Unknown signature for type '" + type?.ToString() + "' (obj = " + obj?.ToString() + ") " + ((type == obj) ? "equal" : "instance"));
	}

	public static string GetSignature(object[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('(');
		foreach (object obj in args)
		{
			stringBuilder.Append(GetSignature(obj));
		}
		stringBuilder.Append(")V");
		return stringBuilder.ToString();
	}

	public static string GetSignature<ReturnType>(object[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('(');
		foreach (object obj in args)
		{
			stringBuilder.Append(GetSignature(obj));
		}
		stringBuilder.Append(')');
		stringBuilder.Append(GetSignature(typeof(ReturnType)));
		return stringBuilder.ToString();
	}
}
