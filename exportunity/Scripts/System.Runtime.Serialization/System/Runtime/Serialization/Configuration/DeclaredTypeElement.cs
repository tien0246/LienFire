using System.Configuration;
using System.Security;

namespace System.Runtime.Serialization.Configuration;

public sealed class DeclaredTypeElement : ConfigurationElement
{
	private ConfigurationPropertyCollection properties;

	[ConfigurationProperty("", DefaultValue = null, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
	public TypeElementCollection KnownTypes => (TypeElementCollection)base[""];

	[DeclaredTypeValidator]
	[ConfigurationProperty("type", DefaultValue = "", Options = ConfigurationPropertyOptions.IsKey)]
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

	protected override ConfigurationPropertyCollection Properties
	{
		get
		{
			if (properties == null)
			{
				ConfigurationPropertyCollection configurationPropertyCollection = new ConfigurationPropertyCollection();
				configurationPropertyCollection.Add(new ConfigurationProperty("", typeof(TypeElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection));
				configurationPropertyCollection.Add(new ConfigurationProperty("type", typeof(string), string.Empty, null, new DeclaredTypeValidator(), ConfigurationPropertyOptions.IsKey));
				properties = configurationPropertyCollection;
			}
			return properties;
		}
	}

	public DeclaredTypeElement()
	{
	}

	public DeclaredTypeElement(string typeName)
		: this()
	{
		if (string.IsNullOrEmpty(typeName))
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
		}
		Type = typeName;
	}

	[SecuritySafeCritical]
	protected override void PostDeserialize()
	{
		if (base.EvaluationContext.IsMachineLevel || PartialTrustHelpers.IsInFullTrust())
		{
			return;
		}
		throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(SR.GetString("Failed to load configuration section for dataContractSerializer.")));
	}
}
