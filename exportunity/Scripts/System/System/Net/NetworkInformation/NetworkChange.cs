using System.ComponentModel;
using Unity;

namespace System.Net.NetworkInformation;

public sealed class NetworkChange
{
	private static INetworkChange networkChange;

	private static bool IsWindows
	{
		get
		{
			PlatformID platform = Environment.OSVersion.Platform;
			if (platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.WinCE)
			{
				return true;
			}
			return false;
		}
	}

	public static event NetworkAddressChangedEventHandler NetworkAddressChanged
	{
		add
		{
			lock (typeof(INetworkChange))
			{
				MaybeCreate();
				if (networkChange != null)
				{
					networkChange.NetworkAddressChanged += value;
				}
			}
		}
		remove
		{
			lock (typeof(INetworkChange))
			{
				if (networkChange != null)
				{
					networkChange.NetworkAddressChanged -= value;
					MaybeDispose();
				}
			}
		}
	}

	public static event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged
	{
		add
		{
			lock (typeof(INetworkChange))
			{
				MaybeCreate();
				if (networkChange != null)
				{
					networkChange.NetworkAvailabilityChanged += value;
				}
			}
		}
		remove
		{
			lock (typeof(INetworkChange))
			{
				if (networkChange != null)
				{
					networkChange.NetworkAvailabilityChanged -= value;
					MaybeDispose();
				}
			}
		}
	}

	private static void MaybeCreate()
	{
		if (networkChange != null)
		{
			return;
		}
		if (IsWindows)
		{
			throw new PlatformNotSupportedException("NetworkInformation.NetworkChange is not supported on the current platform.");
		}
		try
		{
			networkChange = new MacNetworkChange();
		}
		catch
		{
			networkChange = new LinuxNetworkChange();
		}
	}

	private static void MaybeDispose()
	{
		if (networkChange != null && networkChange.HasRegisteredEvents)
		{
			networkChange.Dispose();
			networkChange = null;
		}
	}

	[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void RegisterNetworkChange(NetworkChange nc)
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
