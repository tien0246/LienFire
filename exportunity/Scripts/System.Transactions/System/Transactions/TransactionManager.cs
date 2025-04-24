using System.Configuration;
using System.Transactions.Configuration;

namespace System.Transactions;

public static class TransactionManager
{
	private static DefaultSettingsSection defaultSettings;

	private static MachineSettingsSection machineSettings;

	private static TimeSpan defaultTimeout;

	private static TimeSpan maxTimeout;

	public static TimeSpan DefaultTimeout
	{
		get
		{
			if (defaultSettings != null)
			{
				return defaultSettings.Timeout;
			}
			return defaultTimeout;
		}
	}

	[System.MonoTODO("Not implemented")]
	public static HostCurrentTransactionCallback HostCurrentCallback
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public static TimeSpan MaximumTimeout
	{
		get
		{
			if (machineSettings != null)
			{
				return machineSettings.MaxTimeout;
			}
			return maxTimeout;
		}
	}

	public static event TransactionStartedEventHandler DistributedTransactionStarted;

	static TransactionManager()
	{
		defaultTimeout = new TimeSpan(0, 1, 0);
		maxTimeout = new TimeSpan(0, 10, 0);
		defaultSettings = ConfigurationManager.GetSection("system.transactions/defaultSettings") as DefaultSettingsSection;
		machineSettings = ConfigurationManager.GetSection("system.transactions/machineSettings") as MachineSettingsSection;
	}

	[System.MonoTODO("Not implemented")]
	public static void RecoveryComplete(Guid resourceManagerIdentifier)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Not implemented")]
	public static Enlistment Reenlist(Guid resourceManagerIdentifier, byte[] recoveryInformation, IEnlistmentNotification enlistmentNotification)
	{
		throw new NotImplementedException();
	}
}
