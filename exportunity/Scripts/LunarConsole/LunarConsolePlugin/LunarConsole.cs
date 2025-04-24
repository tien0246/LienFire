using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using LunarConsolePluginInternal;
using UnityEngine;

namespace LunarConsolePlugin;

public sealed class LunarConsole : MonoBehaviour
{
	private interface IPlatform : ICRegistryDelegate
	{
		void Update();

		void OnLogMessageReceived(string message, string stackTrace, LogType type);

		bool ShowConsole();

		bool HideConsole();

		void ClearConsole();

		void Destroy();
	}

	private class PlatformAndroid : IPlatform, ICRegistryDelegate
	{
		private readonly int m_mainThreadId;

		private readonly jvalue[] m_args0 = new jvalue[0];

		private readonly jvalue[] m_args1 = new jvalue[1];

		private readonly jvalue[] m_args2 = new jvalue[2];

		private readonly jvalue[] m_args3 = new jvalue[3];

		private readonly jvalue[] m_args10 = new jvalue[10];

		private static readonly string kPluginClassName = "spacemadness.com.lunarconsole.console.NativeBridge";

		private readonly AndroidJavaClass m_pluginClass;

		private readonly IntPtr m_pluginClassRaw;

		private readonly IntPtr m_methodLogMessage;

		private readonly IntPtr m_methodShowConsole;

		private readonly IntPtr m_methodHideConsole;

		private readonly IntPtr m_methodClearConsole;

		private readonly IntPtr m_methodRegisterAction;

		private readonly IntPtr m_methodUnregisterAction;

		private readonly IntPtr m_methodRegisterVariable;

		private readonly IntPtr m_methodUpdateVariable;

		private readonly IntPtr m_methodDestroy;

		private readonly Queue<LogMessageEntry> m_messageQueue;

		public PlatformAndroid(string targetName, string methodName, string version, LunarConsoleSettings settings)
		{
			string value = JsonUtility.ToJson(settings);
			m_mainThreadId = Thread.CurrentThread.ManagedThreadId;
			m_pluginClass = new AndroidJavaClass(kPluginClassName);
			m_pluginClassRaw = m_pluginClass.GetRawClass();
			IntPtr staticMethod = GetStaticMethod(m_pluginClassRaw, "init", "(Ljava.lang.String;Ljava.lang.String;Ljava.lang.String;Ljava.lang.String;)V");
			jvalue[] array = new jvalue[4]
			{
				jval(targetName),
				jval(methodName),
				jval(version),
				jval(value)
			};
			CallStaticVoidMethod(staticMethod, array);
			AndroidJNI.DeleteLocalRef(array[0].l);
			AndroidJNI.DeleteLocalRef(array[1].l);
			AndroidJNI.DeleteLocalRef(array[2].l);
			AndroidJNI.DeleteLocalRef(array[3].l);
			m_methodLogMessage = GetStaticMethod(m_pluginClassRaw, "logMessage", "(Ljava.lang.String;Ljava.lang.String;I)V");
			m_methodShowConsole = GetStaticMethod(m_pluginClassRaw, "showConsole", "()V");
			m_methodHideConsole = GetStaticMethod(m_pluginClassRaw, "hideConsole", "()V");
			m_methodClearConsole = GetStaticMethod(m_pluginClassRaw, "clearConsole", "()V");
			m_methodRegisterAction = GetStaticMethod(m_pluginClassRaw, "registerAction", "(ILjava.lang.String;)V");
			m_methodUnregisterAction = GetStaticMethod(m_pluginClassRaw, "unregisterAction", "(I)V");
			m_methodRegisterVariable = GetStaticMethod(m_pluginClassRaw, "registerVariable", "(ILjava/lang/String;Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;IZFFLjava/lang/String;)V");
			m_methodUpdateVariable = GetStaticMethod(m_pluginClassRaw, "updateVariable", "(ILjava/lang/String;)V");
			m_methodDestroy = GetStaticMethod(m_pluginClassRaw, "destroy", "()V");
			m_messageQueue = new Queue<LogMessageEntry>();
		}

		~PlatformAndroid()
		{
			m_pluginClass.Dispose();
		}

		public void Update()
		{
			lock (m_messageQueue)
			{
				while (m_messageQueue.Count > 0)
				{
					LogMessageEntry logMessageEntry = m_messageQueue.Dequeue();
					OnLogMessageReceived(logMessageEntry.message, logMessageEntry.stackTrace, logMessageEntry.type);
				}
			}
		}

		public void OnLogMessageReceived(string message, string stackTrace, LogType type)
		{
			if (Thread.CurrentThread.ManagedThreadId == m_mainThreadId)
			{
				m_args3[0] = jval(message);
				m_args3[1] = jval(stackTrace);
				m_args3[2] = jval((int)type);
				CallStaticVoidMethod(m_methodLogMessage, m_args3);
				AndroidJNI.DeleteLocalRef(m_args3[0].l);
				AndroidJNI.DeleteLocalRef(m_args3[1].l);
				return;
			}
			lock (m_messageQueue)
			{
				m_messageQueue.Enqueue(new LogMessageEntry(message, stackTrace, type));
			}
		}

		public bool ShowConsole()
		{
			try
			{
				CallStaticVoidMethod(m_methodShowConsole, m_args0);
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception while calling 'LunarConsole.ShowConsole': " + ex.Message);
				return false;
			}
		}

		public bool HideConsole()
		{
			try
			{
				CallStaticVoidMethod(m_methodHideConsole, m_args0);
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception while calling 'LunarConsole.HideConsole': " + ex.Message);
				return false;
			}
		}

		public void ClearConsole()
		{
			try
			{
				CallStaticVoidMethod(m_methodClearConsole, m_args0);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception while calling 'LunarConsole.ClearConsole': " + ex.Message);
			}
		}

		public void Destroy()
		{
			try
			{
				CallStaticVoidMethod(m_methodDestroy, m_args0);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception while destroying platform: " + ex.Message);
			}
		}

		public void OnActionRegistered(CRegistry registry, CAction action)
		{
			try
			{
				m_args2[0] = jval(action.Id);
				m_args2[1] = jval(action.Name);
				CallStaticVoidMethod(m_methodRegisterAction, m_args2);
				AndroidJNI.DeleteLocalRef(m_args2[1].l);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception while calling 'LunarConsole.OnActionRegistered': " + ex.Message);
			}
		}

		public void OnActionUnregistered(CRegistry registry, CAction action)
		{
			try
			{
				m_args1[0] = jval(action.Id);
				CallStaticVoidMethod(m_methodUnregisterAction, m_args1);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception while calling 'LunarConsole.OnActionUnregistered': " + ex.Message);
			}
		}

		public void OnVariableRegistered(CRegistry registry, CVar cvar)
		{
			try
			{
				m_args10[0] = jval(cvar.Id);
				m_args10[1] = jval(cvar.Name);
				m_args10[2] = jval(cvar.Type.ToString());
				m_args10[3] = jval(cvar.Value);
				m_args10[4] = jval(cvar.DefaultValue);
				m_args10[5] = jval((int)cvar.Flags);
				m_args10[6] = jval(cvar.HasRange);
				m_args10[7] = jval(cvar.Range.min);
				m_args10[8] = jval(cvar.Range.max);
				m_args10[9] = jval((cvar.AvailableValues != null) ? cvar.AvailableValues.Join() : null);
				CallStaticVoidMethod(m_methodRegisterVariable, m_args10);
				AndroidJNI.DeleteLocalRef(m_args10[1].l);
				AndroidJNI.DeleteLocalRef(m_args10[2].l);
				AndroidJNI.DeleteLocalRef(m_args10[3].l);
				AndroidJNI.DeleteLocalRef(m_args10[4].l);
				AndroidJNI.DeleteLocalRef(m_args10[9].l);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception while calling 'LunarConsole.OnVariableRegistered': " + ex.Message);
			}
		}

		public void OnVariableUpdated(CRegistry registry, CVar cvar)
		{
			try
			{
				m_args2[0] = jval(cvar.Id);
				m_args2[1] = jval(cvar.Value);
				CallStaticVoidMethod(m_methodUpdateVariable, m_args2);
				AndroidJNI.DeleteLocalRef(m_args2[1].l);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception while calling 'LunarConsole.OnVariableUpdated': " + ex.Message);
			}
		}

		private static IntPtr GetStaticMethod(IntPtr classRaw, string name, string signature)
		{
			return AndroidJNIHelper.GetMethodID(classRaw, name, signature, isStatic: true);
		}

		private void CallStaticVoidMethod(IntPtr method, jvalue[] args)
		{
			AndroidJNI.CallStaticVoidMethod(m_pluginClassRaw, method, args);
		}

		private bool CallStaticBoolMethod(IntPtr method, jvalue[] args)
		{
			return AndroidJNI.CallStaticBooleanMethod(m_pluginClassRaw, method, args);
		}

		private jvalue jval(string value)
		{
			return new jvalue
			{
				l = AndroidJNI.NewStringUTF(value)
			};
		}

		private jvalue jval(bool value)
		{
			return new jvalue
			{
				z = value
			};
		}

		private jvalue jval(int value)
		{
			return new jvalue
			{
				i = value
			};
		}

		private jvalue jval(float value)
		{
			return new jvalue
			{
				f = value
			};
		}
	}

	private struct LogMessageEntry
	{
		public readonly string message;

		public readonly string stackTrace;

		public readonly LogType type;

		public LogMessageEntry(string message, string stackTrace, LogType type)
		{
			this.message = message;
			this.stackTrace = stackTrace;
			this.type = type;
		}
	}

	[SerializeField]
	private LunarConsoleSettings m_settings = new LunarConsoleSettings();

	private static LunarConsole s_instance;

	private CRegistry m_registry;

	private bool m_variablesDirty;

	private IPlatform m_platform;

	private IDictionary<string, LunarConsoleNativeMessageHandler> m_nativeHandlerLookup;

	private IDictionary<string, LunarConsoleNativeMessageHandler> nativeHandlerLookup
	{
		get
		{
			if (m_nativeHandlerLookup == null)
			{
				m_nativeHandlerLookup = new Dictionary<string, LunarConsoleNativeMessageHandler>();
				m_nativeHandlerLookup["console_open"] = ConsoleOpenHandler;
				m_nativeHandlerLookup["console_close"] = ConsoleCloseHandler;
				m_nativeHandlerLookup["console_action"] = ConsoleActionHandler;
				m_nativeHandlerLookup["console_variable_set"] = ConsoleVariableSetHandler;
				m_nativeHandlerLookup["track_event"] = TrackEventHandler;
			}
			return m_nativeHandlerLookup;
		}
	}

	public static Action onConsoleOpened { get; set; }

	public static Action onConsoleClosed { get; set; }

	public static bool isConsoleEnabled => instance != null;

	public static LunarConsole instance => s_instance;

	public CRegistry registry => m_registry;

	private void Awake()
	{
		InitInstance();
	}

	private void OnEnable()
	{
		EnablePlatform();
	}

	private void OnDisable()
	{
		DisablePlatform();
	}

	private void Update()
	{
		if (m_platform != null)
		{
			m_platform.Update();
		}
		if (m_variablesDirty)
		{
			m_variablesDirty = false;
			SaveVariables();
		}
	}

	private void OnDestroy()
	{
		DestroyInstance();
	}

	private void InitInstance()
	{
		if (s_instance == null)
		{
			if (IsPlatformSupported())
			{
				s_instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else if (s_instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void EnablePlatform()
	{
		if (s_instance != null)
		{
			InitPlatform(m_settings);
		}
	}

	private void DisablePlatform()
	{
		if (s_instance != null)
		{
			DestroyPlatform();
		}
	}

	private static bool IsPlatformSupported()
	{
		return Application.platform == RuntimePlatform.Android;
	}

	private bool InitPlatform(LunarConsoleSettings settings)
	{
		try
		{
			if (m_platform == null)
			{
				m_platform = CreatePlatform(settings);
				if (m_platform != null)
				{
					m_registry = new CRegistry();
					m_registry.registryDelegate = m_platform;
					Application.logMessageReceivedThreaded += OnLogMessageReceived;
					return true;
				}
			}
		}
		catch (Exception exception)
		{
			Log.e(exception, "Can't init platform");
		}
		return false;
	}

	private bool DestroyPlatform()
	{
		if (m_platform != null)
		{
			Application.logMessageReceivedThreaded -= OnLogMessageReceived;
			if (m_registry != null)
			{
				m_registry.Destroy();
				m_registry = null;
			}
			m_platform.Destroy();
			m_platform = null;
			return true;
		}
		return false;
	}

	private IPlatform CreatePlatform(LunarConsoleSettings settings)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			LunarConsoleNativeMessageCallback lunarConsoleNativeMessageCallback = NativeMessageCallback;
			return new PlatformAndroid(base.gameObject.name, lunarConsoleNativeMessageCallback.Method.Name, Constants.Version, settings);
		}
		return null;
	}

	private void DestroyInstance()
	{
		if (s_instance == this)
		{
			DestroyPlatform();
			s_instance = null;
		}
	}

	private static string GetGestureName(Gesture gesture)
	{
		return gesture.ToString();
	}

	private void ResolveVariables()
	{
		try
		{
			foreach (Assembly item in ListAssemblies())
			{
				try
				{
					foreach (Type item2 in ReflectionUtils.FindAttributeTypes<CVarContainerAttribute>(item))
					{
						RegisterVariables(item2);
					}
				}
				catch (Exception exception)
				{
					Log.e(exception, "Unable to register variables from assembly: {0}", item);
				}
			}
		}
		catch (Exception exception2)
		{
			Log.e(exception2, "Unable to register variables");
		}
	}

	private static IList<Assembly> ListAssemblies()
	{
		return ReflectionUtils.ListAssemblies(delegate(Assembly assembly)
		{
			string fullName = assembly.FullName;
			return !fullName.StartsWith("Unity") && !fullName.StartsWith("System") && !fullName.StartsWith("Microsoft") && !fullName.StartsWith("SyntaxTree") && !fullName.StartsWith("Mono") && !fullName.StartsWith("ExCSS") && !fullName.StartsWith("nunit") && !fullName.StartsWith("netstandard") && !fullName.StartsWith("mscorlib") && fullName != "Accessibility";
		});
	}

	private void RegisterVariables(Type type)
	{
		try
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (fields.Length == 0)
			{
				return;
			}
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (!fieldInfo.FieldType.IsAssignableFrom(typeof(CVar)) && !fieldInfo.FieldType.IsSubclassOf(typeof(CVar)))
				{
					continue;
				}
				if (!(fieldInfo.GetValue(null) is CVar cVar))
				{
					Log.w("Unable to register variable {0}.{0}", type.Name, fieldInfo.Name);
					continue;
				}
				CVarValueRange range = ResolveVariableRange(fieldInfo);
				if (range.IsValid)
				{
					if (cVar.Type == CVarType.Float)
					{
						cVar.Range = range;
					}
					else
					{
						Log.w("'{0}' attribute is only available with 'float' variables", typeof(CVarRangeAttribute).Name);
					}
				}
				m_registry.Register(cVar);
			}
		}
		catch (Exception exception)
		{
			Log.e(exception, "Unable to initialize cvar container: {0}", type);
		}
	}

	private static CVarValueRange ResolveVariableRange(FieldInfo field)
	{
		try
		{
			object[] customAttributes = field.GetCustomAttributes(typeof(CVarRangeAttribute), inherit: true);
			if (customAttributes.Length != 0 && customAttributes[0] is CVarRangeAttribute { min: var min, max: var max })
			{
				if (max - min < 1E-05f)
				{
					Log.w("Invalid range [{0}, {1}] for variable '{2}'", min.ToString(), max.ToString(), field.Name);
					return CVarValueRange.Undefined;
				}
				return new CVarValueRange(min, max);
			}
		}
		catch (Exception exception)
		{
			Log.e(exception, "Exception while resolving variable's range: {0}", field.Name);
		}
		return CVarValueRange.Undefined;
	}

	private void LoadVariables()
	{
		try
		{
			string path = Path.Combine(Application.persistentDataPath, "lunar-mobile-console-variables.bin");
			if (!File.Exists(path))
			{
				return;
			}
			using FileStream input = File.OpenRead(path);
			using BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = binaryReader.ReadString();
				string value = binaryReader.ReadString();
				CVar cVar = m_registry.FindVariable(text);
				if (cVar == null)
				{
					Log.w("Variable '{0}' not registered. Ignoring...", text);
				}
				else
				{
					cVar.Value = value;
					m_platform.OnVariableUpdated(m_registry, cVar);
				}
			}
		}
		catch (Exception exception)
		{
			Log.e(exception, "Error while loading variables");
		}
	}

	private void SaveVariables()
	{
		try
		{
			using FileStream output = File.OpenWrite(Path.Combine(Application.persistentDataPath, "lunar-mobile-console-variables.bin"));
			using BinaryWriter binaryWriter = new BinaryWriter(output);
			CVarList cvars = m_registry.cvars;
			int num = 0;
			foreach (CVar item in cvars)
			{
				if (ShouldSaveVar(item))
				{
					num++;
				}
			}
			binaryWriter.Write(num);
			foreach (CVar item2 in cvars)
			{
				if (ShouldSaveVar(item2))
				{
					binaryWriter.Write(item2.Name);
					binaryWriter.Write(item2.Value);
				}
			}
		}
		catch (Exception exception)
		{
			Log.e(exception, "Error while saving variables");
		}
	}

	private bool ShouldSaveVar(CVar cvar)
	{
		if (!cvar.IsDefault)
		{
			return !cvar.HasFlag(CFlags.NoArchive);
		}
		return false;
	}

	private void OnLogMessageReceived(string message, string stackTrace, LogType type)
	{
		m_platform.OnLogMessageReceived(message, stackTrace, type);
	}

	private void NativeMessageCallback(string param)
	{
		IDictionary<string, string> dictionary = StringUtils.DeserializeString(param);
		string text = dictionary["name"];
		if (string.IsNullOrEmpty(text))
		{
			Log.w("Can't handle native callback: 'name' is undefined");
			return;
		}
		if (!nativeHandlerLookup.TryGetValue(text, out var value))
		{
			Log.w("Can't handle native callback: handler not found '" + text + "'");
			return;
		}
		try
		{
			value(dictionary);
		}
		catch (Exception exception)
		{
			Log.e(exception, "Exception while handling native callback '{0}'", text);
		}
	}

	private void ConsoleOpenHandler(IDictionary<string, string> data)
	{
		if (onConsoleOpened != null)
		{
			onConsoleOpened();
		}
		TrackEvent("Console", "console_open");
	}

	private void ConsoleCloseHandler(IDictionary<string, string> data)
	{
		if (onConsoleClosed != null)
		{
			onConsoleClosed();
		}
		TrackEvent("Console", "console_close");
	}

	private void ConsoleActionHandler(IDictionary<string, string> data)
	{
		if (!data.TryGetValue("id", out var value))
		{
			Log.w("Can't run action: data is not properly formatted");
			return;
		}
		if (!int.TryParse(value, out var result))
		{
			Log.w("Can't run action: invalid ID " + value);
			return;
		}
		if (m_registry == null)
		{
			Log.w("Can't run action: registry is not property initialized");
			return;
		}
		CAction cAction = m_registry.FindAction(result);
		if (cAction == null)
		{
			Log.w("Can't run action: ID not found " + value);
			return;
		}
		try
		{
			cAction.Execute();
		}
		catch (Exception exception)
		{
			Log.e(exception, "Can't run action {0}", cAction.Name);
		}
	}

	private void ConsoleVariableSetHandler(IDictionary<string, string> data)
	{
		if (!data.TryGetValue("id", out var value))
		{
			Log.w("Can't set variable: missing 'id' property");
			return;
		}
		if (!data.TryGetValue("value", out var value2))
		{
			Log.w("Can't set variable: missing 'value' property");
			return;
		}
		if (!int.TryParse(value, out var result))
		{
			Log.w("Can't set variable: invalid ID " + value);
			return;
		}
		if (m_registry == null)
		{
			Log.w("Can't set variable: registry is not property initialized");
			return;
		}
		CVar cVar = m_registry.FindVariable(result);
		if (cVar == null)
		{
			Log.w("Can't set variable: ID not found " + value);
			return;
		}
		try
		{
			switch (cVar.Type)
			{
			case CVarType.Boolean:
			{
				if (int.TryParse(value2, out var result3) && (result3 == 0 || result3 == 1))
				{
					cVar.BoolValue = result3 == 1;
					m_variablesDirty = true;
				}
				else
				{
					Log.e("Invalid boolean value: '{0}'", value2);
				}
				break;
			}
			case CVarType.Integer:
			{
				if (int.TryParse(value2, out var result2))
				{
					cVar.IntValue = result2;
					m_variablesDirty = true;
				}
				else
				{
					Log.e("Invalid integer value: '{0}'", value2);
				}
				break;
			}
			case CVarType.Float:
			{
				if (float.TryParse(value2, out var result4))
				{
					cVar.FloatValue = result4;
					m_variablesDirty = true;
				}
				else
				{
					Log.e("Invalid float value: '{0}'", value2);
				}
				break;
			}
			case CVarType.String:
				cVar.Value = value2;
				m_variablesDirty = true;
				break;
			case CVarType.Enum:
				if (Array.IndexOf(cVar.AvailableValues, cVar.Value) != -1)
				{
					cVar.Value = value2;
					m_variablesDirty = true;
				}
				else
				{
					Log.e("Unexpected variable '{0}' value: {1}", cVar.Name, cVar.Value);
				}
				break;
			default:
				Log.e("Unexpected variable type: {0}", cVar.Type);
				break;
			}
		}
		catch (Exception exception)
		{
			Log.e(exception, "Exception while trying to set variable '{0}'", cVar.Name);
		}
	}

	private void TrackEventHandler(IDictionary<string, string> data)
	{
		if (!data.TryGetValue("category", out var value) || value.Length == 0)
		{
			Log.w("Can't track event: missing 'category' parameter");
			return;
		}
		if (!data.TryGetValue("action", out var value2) || value2.Length == 0)
		{
			Log.w("Can't track event: missing 'action' parameter");
			return;
		}
		int result = int.MinValue;
		if (data.TryGetValue("value", out var value3) && !int.TryParse(value3, out result))
		{
			Log.w("Can't track event: invalid 'value' parameter: {0}", value3);
		}
		else
		{
			LunarConsoleAnalytics.TrackEvent(value, value2, result);
		}
	}

	private void TrackEvent(string category, string action, int value = int.MinValue)
	{
		StartCoroutine(LunarConsoleAnalytics.TrackEvent(category, action, value));
	}

	public static void Show()
	{
		if (s_instance != null)
		{
			s_instance.ShowConsole();
		}
		else
		{
			Log.w("Can't show console: instance is not initialized. Make sure you've installed it correctly");
		}
	}

	public static void Hide()
	{
		if (s_instance != null)
		{
			s_instance.HideConsole();
		}
		else
		{
			Log.w("Can't hide console: instance is not initialized. Make sure you've installed it correctly");
		}
	}

	public static void Clear()
	{
		if (s_instance != null)
		{
			s_instance.ClearConsole();
		}
		else
		{
			Log.w("Can't clear console: instance is not initialized. Make sure you've installed it correctly");
		}
	}

	public static void RegisterAction(string name, Action action)
	{
		Log.w("Can't register action: feature is not available in FREE version. Learn more about PRO version: https://goo.gl/TLInmD");
	}

	public static void UnregisterAction(Action action)
	{
	}

	public static void UnregisterAction(string name)
	{
	}

	public static void UnregisterAllActions(object target)
	{
	}

	public static void SetConsoleEnabled(bool enabled)
	{
	}

	public void MarkVariablesDirty()
	{
		m_variablesDirty = true;
	}

	private void ShowConsole()
	{
		if (m_platform != null)
		{
			m_platform.ShowConsole();
		}
	}

	private void HideConsole()
	{
		if (m_platform != null)
		{
			m_platform.HideConsole();
		}
	}

	private void ClearConsole()
	{
		if (m_platform != null)
		{
			m_platform.ClearConsole();
		}
	}

	private void RegisterConsoleAction(string name, Action actionDelegate)
	{
		if (m_registry != null)
		{
			m_registry.RegisterAction(name, actionDelegate);
			return;
		}
		Log.w("Can't register action '{0}': registry is not property initialized", name);
	}

	private void UnregisterConsoleAction(Action actionDelegate)
	{
		if (m_registry != null)
		{
			m_registry.Unregister(actionDelegate);
			return;
		}
		Log.w("Can't unregister action '{0}': registry is not property initialized", actionDelegate);
	}

	private void UnregisterConsoleAction(string name)
	{
		if (m_registry != null)
		{
			m_registry.Unregister(name);
			return;
		}
		Log.w("Can't unregister action '{0}': registry is not property initialized", name);
	}

	private void UnregisterAllConsoleActions(object target)
	{
		if (m_registry != null)
		{
			m_registry.UnregisterAll(target);
			return;
		}
		Log.w("Can't unregister actions for target '{0}': registry is not property initialized", target);
	}

	private void SetConsoleInstanceEnabled(bool enabled)
	{
		base.enabled = enabled;
	}
}
