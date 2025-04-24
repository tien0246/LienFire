using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace System.Diagnostics;

[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
public sealed class PerformanceCounterCategory
{
	private string categoryName;

	private string machineName;

	private PerformanceCounterCategoryType type = PerformanceCounterCategoryType.Unknown;

	public string CategoryHelp
	{
		get
		{
			string text = null;
			if (IsValidMachine(machineName))
			{
				text = CategoryHelpInternal(categoryName);
			}
			if (text != null)
			{
				return text;
			}
			throw new InvalidOperationException();
		}
	}

	public string CategoryName
	{
		get
		{
			return categoryName;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value == "")
			{
				throw new ArgumentException("value");
			}
			categoryName = value;
		}
	}

	public string MachineName
	{
		get
		{
			return machineName;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value == "")
			{
				throw new ArgumentException("value");
			}
			machineName = value;
		}
	}

	public PerformanceCounterCategoryType CategoryType => type;

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern bool CategoryDelete_icall(char* name, int name_length);

	private unsafe static bool CategoryDelete(string name)
	{
		fixed (char* name2 = name)
		{
			return CategoryDelete_icall(name2, name?.Length ?? 0);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern string CategoryHelp_icall(char* category, int category_length);

	private unsafe static string CategoryHelpInternal(string category)
	{
		fixed (char* category2 = category)
		{
			return CategoryHelp_icall(category2, category?.Length ?? 0);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern bool CounterCategoryExists_icall(char* counter, int counter_length, char* category, int category_length);

	private unsafe static bool CounterCategoryExists(string counter, string category)
	{
		fixed (char* counter2 = counter)
		{
			fixed (char* category2 = category)
			{
				return CounterCategoryExists_icall(counter2, counter?.Length ?? 0, category2, category?.Length ?? 0);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern bool Create_icall(char* categoryName, int categoryName_length, char* categoryHelp, int categoryHelp_length, PerformanceCounterCategoryType categoryType, CounterCreationData[] items);

	private unsafe static bool Create(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, CounterCreationData[] items)
	{
		fixed (char* ptr = categoryName)
		{
			fixed (char* categoryHelp2 = categoryHelp)
			{
				return Create_icall(ptr, categoryName?.Length ?? 0, categoryHelp2, categoryHelp?.Length ?? 0, categoryType, items);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern bool InstanceExistsInternal_icall(char* instance, int instance_length, char* category, int category_length);

	private unsafe static bool InstanceExistsInternal(string instance, string category)
	{
		fixed (char* instance2 = instance)
		{
			fixed (char* category2 = category)
			{
				return InstanceExistsInternal_icall(instance2, instance?.Length ?? 0, category2, category?.Length ?? 0);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string[] GetCategoryNames();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern string[] GetCounterNames_icall(char* category, int category_length);

	private unsafe static string[] GetCounterNames(string category)
	{
		fixed (char* category2 = category)
		{
			return GetCounterNames_icall(category2, category?.Length ?? 0);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern string[] GetInstanceNames_icall(char* category, int category_length);

	private unsafe static string[] GetInstanceNames(string category)
	{
		fixed (char* category2 = category)
		{
			return GetInstanceNames_icall(category2, category?.Length ?? 0);
		}
	}

	private static void CheckCategory(string categoryName)
	{
		if (categoryName == null)
		{
			throw new ArgumentNullException("categoryName");
		}
		if (categoryName == "")
		{
			throw new ArgumentException("categoryName");
		}
	}

	public PerformanceCounterCategory()
		: this("", ".")
	{
	}

	public PerformanceCounterCategory(string categoryName)
		: this(categoryName, ".")
	{
	}

	public PerformanceCounterCategory(string categoryName, string machineName)
	{
		CheckCategory(categoryName);
		if (machineName == null)
		{
			throw new ArgumentNullException("machineName");
		}
		this.categoryName = categoryName;
		this.machineName = machineName;
	}

	private static bool IsValidMachine(string machine)
	{
		return machine == ".";
	}

	public bool CounterExists(string counterName)
	{
		return CounterExists(counterName, categoryName, machineName);
	}

	public static bool CounterExists(string counterName, string categoryName)
	{
		return CounterExists(counterName, categoryName, ".");
	}

	public static bool CounterExists(string counterName, string categoryName, string machineName)
	{
		if (counterName == null)
		{
			throw new ArgumentNullException("counterName");
		}
		CheckCategory(categoryName);
		if (machineName == null)
		{
			throw new ArgumentNullException("machineName");
		}
		if (IsValidMachine(machineName))
		{
			return CounterCategoryExists(counterName, categoryName);
		}
		return false;
	}

	[Obsolete("Use another overload that uses PerformanceCounterCategoryType instead")]
	public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, CounterCreationDataCollection counterData)
	{
		return Create(categoryName, categoryHelp, PerformanceCounterCategoryType.Unknown, counterData);
	}

	[Obsolete("Use another overload that uses PerformanceCounterCategoryType instead")]
	public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, string counterName, string counterHelp)
	{
		return Create(categoryName, categoryHelp, PerformanceCounterCategoryType.Unknown, counterName, counterHelp);
	}

	public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, CounterCreationDataCollection counterData)
	{
		CheckCategory(categoryName);
		if (counterData == null)
		{
			throw new ArgumentNullException("counterData");
		}
		if (counterData.Count == 0)
		{
			throw new ArgumentException("counterData");
		}
		CounterCreationData[] array = new CounterCreationData[counterData.Count];
		counterData.CopyTo(array, 0);
		if (!Create(categoryName, categoryHelp, categoryType, array))
		{
			throw new InvalidOperationException();
		}
		return new PerformanceCounterCategory(categoryName, categoryHelp);
	}

	public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, string counterName, string counterHelp)
	{
		CheckCategory(categoryName);
		if (!Create(categoryName, categoryHelp, categoryType, new CounterCreationData[1]
		{
			new CounterCreationData(counterName, counterHelp, PerformanceCounterType.NumberOfItems32)
		}))
		{
			throw new InvalidOperationException();
		}
		return new PerformanceCounterCategory(categoryName, categoryHelp);
	}

	public static void Delete(string categoryName)
	{
		CheckCategory(categoryName);
		if (!CategoryDelete(categoryName))
		{
			throw new InvalidOperationException();
		}
	}

	public static bool Exists(string categoryName)
	{
		return Exists(categoryName, ".");
	}

	public static bool Exists(string categoryName, string machineName)
	{
		CheckCategory(categoryName);
		if (IsValidMachine(machineName))
		{
			return CounterCategoryExists(null, categoryName);
		}
		return false;
	}

	public static PerformanceCounterCategory[] GetCategories()
	{
		return GetCategories(".");
	}

	public static PerformanceCounterCategory[] GetCategories(string machineName)
	{
		if (machineName == null)
		{
			throw new ArgumentNullException("machineName");
		}
		if (!IsValidMachine(machineName))
		{
			return Array.Empty<PerformanceCounterCategory>();
		}
		string[] categoryNames = GetCategoryNames();
		PerformanceCounterCategory[] array = new PerformanceCounterCategory[categoryNames.Length];
		for (int i = 0; i < categoryNames.Length; i++)
		{
			array[i] = new PerformanceCounterCategory(categoryNames[i], machineName);
		}
		return array;
	}

	public PerformanceCounter[] GetCounters()
	{
		return GetCounters("");
	}

	public PerformanceCounter[] GetCounters(string instanceName)
	{
		if (!IsValidMachine(machineName))
		{
			return Array.Empty<PerformanceCounter>();
		}
		string[] counterNames = GetCounterNames(categoryName);
		PerformanceCounter[] array = new PerformanceCounter[counterNames.Length];
		for (int i = 0; i < counterNames.Length; i++)
		{
			array[i] = new PerformanceCounter(categoryName, counterNames[i], instanceName, machineName);
		}
		return array;
	}

	public string[] GetInstanceNames()
	{
		if (!IsValidMachine(machineName))
		{
			return Array.Empty<string>();
		}
		return GetInstanceNames(categoryName);
	}

	public bool InstanceExists(string instanceName)
	{
		return InstanceExists(instanceName, categoryName, machineName);
	}

	public static bool InstanceExists(string instanceName, string categoryName)
	{
		return InstanceExists(instanceName, categoryName, ".");
	}

	public static bool InstanceExists(string instanceName, string categoryName, string machineName)
	{
		if (instanceName == null)
		{
			throw new ArgumentNullException("instanceName");
		}
		CheckCategory(categoryName);
		if (machineName == null)
		{
			throw new ArgumentNullException("machineName");
		}
		return InstanceExistsInternal(instanceName, categoryName);
	}

	[System.MonoTODO]
	public InstanceDataCollectionCollection ReadCategory()
	{
		throw new NotImplementedException();
	}
}
