using System.Configuration;

namespace System.Runtime.Serialization.Configuration;

public sealed class TypeElement : ConfigurationElement
{
	private ConfigurationPropertyCollection properties;

	private string key = Guid.NewGuid().ToString();

	protected override ConfigurationPropertyCollection Properties
	{
		get
		{
			if (properties == null)
			{
				ConfigurationPropertyCollection configurationPropertyCollection = new ConfigurationPropertyCollection();
				configurationPropertyCollection.Add(new ConfigurationProperty("", typeof(ParameterElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection));
				configurationPropertyCollection.Add(new ConfigurationProperty("type", typeof(string), string.Empty, null, new StringValidator(0, int.MaxValue, null), ConfigurationPropertyOptions.None));
				configurationPropertyCollection.Add(new ConfigurationProperty("index", typeof(int), 0, null, new IntegerValidator(0, int.MaxValue, rangeIsExclusive: false), ConfigurationPropertyOptions.None));
				properties = configurationPropertyCollection;
			}
			return properties;
		}
	}

	internal string Key => key;

	[ConfigurationProperty("", DefaultValue = null, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
	public ParameterElementCollection Parameters => (ParameterElementCollection)base[""];

	[StringValidator(MinLength = 0)]
	[ConfigurationProperty("type", DefaultValue = "")]
	public string Type
	{
		get
		{
			return (string)base["type"];
		}
		set
		{
			base["type"] = value;
		}
	}

	[IntegerValidator(MinValue = 0)]
	[ConfigurationProperty("index", DefaultValue = 0)]
	public int Index
	{
		get
		{
			return (int)base["index"];
		}
		set
		{
			base["index"] = value;
		}
	}

	public TypeElement()
	{
	}

	public TypeElement(string typeName)
		: this()
	{
		if (string.IsNullOrEmpty(typeName))
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
		}
		Type = typeName;
	}

	protected override void Reset(ConfigurationElement parentElement)
	{
		TypeElement typeElement = (TypeElement)parentElement;
		key = typeElement.key;
		base.Reset(parentElement);
	}

	internal Type GetType(string rootType, Type[] typeArgs)
	{
		return GetType(rootType, typeArgs, Type, Index, Parameters);
	}

	internal static Type GetType(string rootType, Type[] typeArgs, string type, int index, ParameterElementCollection parameters)
	{
		if (string.IsNullOrEmpty(type))
		{
			if (typeArgs == null || index >= typeArgs.Length)
			{
				int num = ((typeArgs != null) ? typeArgs.Length : 0);
				if (num == 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("For known type configuration, index is out of bound. Root type: '{0}' has {1} type arguments, and index was {2}.", rootType, num, index));
				}
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("For known type configuration, index is out of bound. Root type: '{0}' has {1} type arguments, and index was {2}.", rootType, num, index));
			}
			return typeArgs[index];
		}
		Type type2 = System.Type.GetType(type, throwOnError: true);
		if (type2.IsGenericTypeDefinition)
		{
			if (parameters.Count != type2.GetGenericArguments().Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("Generic parameter count do not match between known type and configuration. Type is '{0}', known type has {1} parameters, configuration has {2} parameters.", type, type2.GetGenericArguments().Length, parameters.Count));
			}
			Type[] array = new Type[parameters.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = parameters[i].GetType(rootType, typeArgs);
			}
			type2 = type2.MakeGenericType(array);
		}
		return type2;
	}
}
