using System.Configuration;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Serialization.Configuration;

public sealed class NetDataContractSerializerSection : ConfigurationSection
{
	private ConfigurationPropertyCollection properties;

	[ConfigurationProperty("enableUnsafeTypeForwarding", DefaultValue = false)]
	public bool EnableUnsafeTypeForwarding => (bool)base["enableUnsafeTypeForwarding"];

	protected override ConfigurationPropertyCollection Properties
	{
		get
		{
			if (properties == null)
			{
				ConfigurationPropertyCollection configurationPropertyCollection = new ConfigurationPropertyCollection();
				configurationPropertyCollection.Add(new ConfigurationProperty("enableUnsafeTypeForwarding", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
				properties = configurationPropertyCollection;
			}
			return properties;
		}
	}

	[SecurityCritical]
	[ConfigurationPermission(SecurityAction.Assert, Unrestricted = true)]
	internal static bool TryUnsafeGetSection(out NetDataContractSerializerSection section)
	{
		section = (NetDataContractSerializerSection)ConfigurationManager.GetSection(ConfigurationStrings.NetDataContractSerializerSectionPath);
		return section != null;
	}
}
