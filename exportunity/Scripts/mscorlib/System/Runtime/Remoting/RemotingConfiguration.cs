using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using Mono.Xml;

namespace System.Runtime.Remoting;

[ComVisible(true)]
public static class RemotingConfiguration
{
	private static string applicationID = null;

	private static string applicationName = null;

	private static string processGuid = null;

	private static bool defaultConfigRead = false;

	private static bool defaultDelayedConfigRead = false;

	private static CustomErrorsModes _errorMode = CustomErrorsModes.RemoteOnly;

	private static Hashtable wellKnownClientEntries = new Hashtable();

	private static Hashtable activatedClientEntries = new Hashtable();

	private static Hashtable wellKnownServiceEntries = new Hashtable();

	private static Hashtable activatedServiceEntries = new Hashtable();

	private static Hashtable channelTemplates = new Hashtable();

	private static Hashtable clientProviderTemplates = new Hashtable();

	private static Hashtable serverProviderTemplates = new Hashtable();

	public static string ApplicationId
	{
		get
		{
			applicationID = ApplicationName;
			return applicationID;
		}
	}

	public static string ApplicationName
	{
		get
		{
			return applicationName;
		}
		set
		{
			applicationName = value;
		}
	}

	public static CustomErrorsModes CustomErrorsMode
	{
		get
		{
			return _errorMode;
		}
		set
		{
			_errorMode = value;
		}
	}

	public static string ProcessId
	{
		get
		{
			if (processGuid == null)
			{
				processGuid = AppDomain.GetProcessGuid();
			}
			return processGuid;
		}
	}

	[MonoTODO("ensureSecurity support has not been implemented")]
	public static void Configure(string filename, bool ensureSecurity)
	{
		lock (channelTemplates)
		{
			if (!defaultConfigRead)
			{
				string bundledMachineConfig = Environment.GetBundledMachineConfig();
				if (bundledMachineConfig != null)
				{
					ReadConfigString(bundledMachineConfig);
				}
				if (File.Exists(Environment.GetMachineConfigPath()))
				{
					ReadConfigFile(Environment.GetMachineConfigPath());
				}
				defaultConfigRead = true;
			}
			if (filename != null)
			{
				ReadConfigFile(filename);
			}
		}
	}

	[Obsolete("Use Configure(String,Boolean)")]
	public static void Configure(string filename)
	{
		Configure(filename, ensureSecurity: false);
	}

	private static void ReadConfigString(string filename)
	{
		try
		{
			SmallXmlParser smallXmlParser = new SmallXmlParser();
			using TextReader input = new StringReader(filename);
			ConfigHandler handler = new ConfigHandler(onlyDelayedChannels: false);
			smallXmlParser.Parse(input, handler);
		}
		catch (Exception ex)
		{
			throw new RemotingException("Configuration string could not be loaded: " + ex.Message, ex);
		}
	}

	private static void ReadConfigFile(string filename)
	{
		try
		{
			SmallXmlParser smallXmlParser = new SmallXmlParser();
			using TextReader input = new StreamReader(filename);
			ConfigHandler handler = new ConfigHandler(onlyDelayedChannels: false);
			smallXmlParser.Parse(input, handler);
		}
		catch (Exception ex)
		{
			throw new RemotingException("Configuration file '" + filename + "' could not be loaded: " + ex.Message, ex);
		}
	}

	internal static void LoadDefaultDelayedChannels()
	{
		lock (channelTemplates)
		{
			if (!defaultDelayedConfigRead && !defaultConfigRead)
			{
				SmallXmlParser smallXmlParser = new SmallXmlParser();
				using (TextReader input = new StreamReader(Environment.GetMachineConfigPath()))
				{
					ConfigHandler handler = new ConfigHandler(onlyDelayedChannels: true);
					smallXmlParser.Parse(input, handler);
				}
				defaultDelayedConfigRead = true;
			}
		}
	}

	public static ActivatedClientTypeEntry[] GetRegisteredActivatedClientTypes()
	{
		lock (channelTemplates)
		{
			ActivatedClientTypeEntry[] array = new ActivatedClientTypeEntry[activatedClientEntries.Count];
			activatedClientEntries.Values.CopyTo(array, 0);
			return array;
		}
	}

	public static ActivatedServiceTypeEntry[] GetRegisteredActivatedServiceTypes()
	{
		lock (channelTemplates)
		{
			ActivatedServiceTypeEntry[] array = new ActivatedServiceTypeEntry[activatedServiceEntries.Count];
			activatedServiceEntries.Values.CopyTo(array, 0);
			return array;
		}
	}

	public static WellKnownClientTypeEntry[] GetRegisteredWellKnownClientTypes()
	{
		lock (channelTemplates)
		{
			WellKnownClientTypeEntry[] array = new WellKnownClientTypeEntry[wellKnownClientEntries.Count];
			wellKnownClientEntries.Values.CopyTo(array, 0);
			return array;
		}
	}

	public static WellKnownServiceTypeEntry[] GetRegisteredWellKnownServiceTypes()
	{
		lock (channelTemplates)
		{
			WellKnownServiceTypeEntry[] array = new WellKnownServiceTypeEntry[wellKnownServiceEntries.Count];
			wellKnownServiceEntries.Values.CopyTo(array, 0);
			return array;
		}
	}

	public static bool IsActivationAllowed(Type svrType)
	{
		lock (channelTemplates)
		{
			return activatedServiceEntries.ContainsKey(svrType);
		}
	}

	public static ActivatedClientTypeEntry IsRemotelyActivatedClientType(Type svrType)
	{
		lock (channelTemplates)
		{
			return activatedClientEntries[svrType] as ActivatedClientTypeEntry;
		}
	}

	public static ActivatedClientTypeEntry IsRemotelyActivatedClientType(string typeName, string assemblyName)
	{
		return IsRemotelyActivatedClientType(Assembly.Load(assemblyName).GetType(typeName));
	}

	public static WellKnownClientTypeEntry IsWellKnownClientType(Type svrType)
	{
		lock (channelTemplates)
		{
			return wellKnownClientEntries[svrType] as WellKnownClientTypeEntry;
		}
	}

	public static WellKnownClientTypeEntry IsWellKnownClientType(string typeName, string assemblyName)
	{
		return IsWellKnownClientType(Assembly.Load(assemblyName).GetType(typeName));
	}

	public static void RegisterActivatedClientType(ActivatedClientTypeEntry entry)
	{
		lock (channelTemplates)
		{
			if (wellKnownClientEntries.ContainsKey(entry.ObjectType) || activatedClientEntries.ContainsKey(entry.ObjectType))
			{
				throw new RemotingException("Attempt to redirect activation of type '" + entry.ObjectType.FullName + "' which is already redirected.");
			}
			activatedClientEntries[entry.ObjectType] = entry;
			ActivationServices.EnableProxyActivation(entry.ObjectType, enable: true);
		}
	}

	public static void RegisterActivatedClientType(Type type, string appUrl)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (appUrl == null)
		{
			throw new ArgumentNullException("appUrl");
		}
		RegisterActivatedClientType(new ActivatedClientTypeEntry(type, appUrl));
	}

	public static void RegisterActivatedServiceType(ActivatedServiceTypeEntry entry)
	{
		lock (channelTemplates)
		{
			activatedServiceEntries.Add(entry.ObjectType, entry);
		}
	}

	public static void RegisterActivatedServiceType(Type type)
	{
		RegisterActivatedServiceType(new ActivatedServiceTypeEntry(type));
	}

	public static void RegisterWellKnownClientType(Type type, string objectUrl)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (objectUrl == null)
		{
			throw new ArgumentNullException("objectUrl");
		}
		RegisterWellKnownClientType(new WellKnownClientTypeEntry(type, objectUrl));
	}

	public static void RegisterWellKnownClientType(WellKnownClientTypeEntry entry)
	{
		lock (channelTemplates)
		{
			if (wellKnownClientEntries.ContainsKey(entry.ObjectType) || activatedClientEntries.ContainsKey(entry.ObjectType))
			{
				throw new RemotingException("Attempt to redirect activation of type '" + entry.ObjectType.FullName + "' which is already redirected.");
			}
			wellKnownClientEntries[entry.ObjectType] = entry;
			ActivationServices.EnableProxyActivation(entry.ObjectType, enable: true);
		}
	}

	public static void RegisterWellKnownServiceType(Type type, string objectUri, WellKnownObjectMode mode)
	{
		RegisterWellKnownServiceType(new WellKnownServiceTypeEntry(type, objectUri, mode));
	}

	public static void RegisterWellKnownServiceType(WellKnownServiceTypeEntry entry)
	{
		lock (channelTemplates)
		{
			wellKnownServiceEntries[entry.ObjectUri] = entry;
			RemotingServices.CreateWellKnownServerIdentity(entry.ObjectType, entry.ObjectUri, entry.Mode);
		}
	}

	internal static void RegisterChannelTemplate(ChannelData channel)
	{
		channelTemplates[channel.Id] = channel;
	}

	internal static void RegisterClientProviderTemplate(ProviderData prov)
	{
		clientProviderTemplates[prov.Id] = prov;
	}

	internal static void RegisterServerProviderTemplate(ProviderData prov)
	{
		serverProviderTemplates[prov.Id] = prov;
	}

	internal static void RegisterChannels(ArrayList channels, bool onlyDelayed)
	{
		foreach (ChannelData channel in channels)
		{
			if ((onlyDelayed && channel.DelayLoadAsClientChannel != "true") || (defaultDelayedConfigRead && channel.DelayLoadAsClientChannel == "true"))
			{
				continue;
			}
			if (channel.Ref != null)
			{
				ChannelData channelData2 = (ChannelData)channelTemplates[channel.Ref];
				if (channelData2 == null)
				{
					throw new RemotingException("Channel template '" + channel.Ref + "' not found");
				}
				channel.CopyFrom(channelData2);
			}
			foreach (ProviderData serverProvider in channel.ServerProviders)
			{
				if (serverProvider.Ref != null)
				{
					ProviderData providerData2 = (ProviderData)serverProviderTemplates[serverProvider.Ref];
					if (providerData2 == null)
					{
						throw new RemotingException("Provider template '" + serverProvider.Ref + "' not found");
					}
					serverProvider.CopyFrom(providerData2);
				}
			}
			foreach (ProviderData clientProvider in channel.ClientProviders)
			{
				if (clientProvider.Ref != null)
				{
					ProviderData providerData4 = (ProviderData)clientProviderTemplates[clientProvider.Ref];
					if (providerData4 == null)
					{
						throw new RemotingException("Provider template '" + clientProvider.Ref + "' not found");
					}
					clientProvider.CopyFrom(providerData4);
				}
			}
			ChannelServices.RegisterChannelConfig(channel);
		}
	}

	internal static void RegisterTypes(ArrayList types)
	{
		foreach (TypeEntry type in types)
		{
			if (type is ActivatedClientTypeEntry)
			{
				RegisterActivatedClientType((ActivatedClientTypeEntry)type);
			}
			else if (type is ActivatedServiceTypeEntry)
			{
				RegisterActivatedServiceType((ActivatedServiceTypeEntry)type);
			}
			else if (type is WellKnownClientTypeEntry)
			{
				RegisterWellKnownClientType((WellKnownClientTypeEntry)type);
			}
			else if (type is WellKnownServiceTypeEntry)
			{
				RegisterWellKnownServiceType((WellKnownServiceTypeEntry)type);
			}
		}
	}

	public static bool CustomErrorsEnabled(bool isLocalRequest)
	{
		if (_errorMode == CustomErrorsModes.Off)
		{
			return false;
		}
		if (_errorMode == CustomErrorsModes.On)
		{
			return true;
		}
		return !isLocalRequest;
	}

	internal static void SetCustomErrorsMode(string mode)
	{
		if (mode == null)
		{
			throw new RemotingException("mode attribute is required");
		}
		string text = mode.ToLower();
		if (text != "on" && text != "off" && text != "remoteonly")
		{
			throw new RemotingException("Invalid custom error mode: " + mode);
		}
		_errorMode = (CustomErrorsModes)Enum.Parse(typeof(CustomErrorsModes), text, ignoreCase: true);
	}
}
