using System.Configuration;

namespace System.Transactions.Configuration;

public class TransactionsSectionGroup : ConfigurationSectionGroup
{
	[ConfigurationProperty("defaultSettings")]
	public DefaultSettingsSection DefaultSettings => (DefaultSettingsSection)base.Sections["defaultSettings"];

	[ConfigurationProperty("machineSettings")]
	public MachineSettingsSection MachineSettings => (MachineSettingsSection)base.Sections["machineSettings"];

	public static TransactionsSectionGroup GetSectionGroup(System.Configuration.Configuration config)
	{
		if (config == null)
		{
			throw new ArgumentNullException("config");
		}
		return config.GetSectionGroup("system.transactions") as TransactionsSectionGroup;
	}
}
