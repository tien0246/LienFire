using System.Configuration;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Serialization.Configuration;

public sealed class DataContractSerializerSection : ConfigurationSection
{
	private ConfigurationPropertyCollection properties;

	[ConfigurationProperty("declaredTypes", DefaultValue = null)]
	public DeclaredTypeElementCollection DeclaredTypes => (DeclaredTypeElementCollection)base["declaredTypes"];

	protected override ConfigurationPropertyCollection Properties
	{
		get
		{
			if (properties == null)
			{
				ConfigurationPropertyCollection configurationPropertyCollection = new ConfigurationPropertyCollection();
				configurationPropertyCollection.Add(new ConfigurationProperty("declaredTypes", typeof(DeclaredTypeElementCollection), null, null, null, ConfigurationPropertyOptions.None));
				properties = configurationPropertyCollection;
			}
			return properties;
		}
	}

	[SecurityCritical]
	[ConfigurationPermission(SecurityAction.Assert, Unrestricted = true)]
	internal static DataContractSerializerSection UnsafeGetSection()
	{
		return ((DataContractSerializerSection)ConfigurationManager.GetSection(ConfigurationStrings.DataContractSerializerSectionPath)) ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(SR.GetString("Failed to load configuration section for dataContractSerializer.")));
	}
}
