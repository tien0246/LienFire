using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation;

internal class Win32IPAddressCollection : IPAddressCollection
{
	public static readonly Win32IPAddressCollection Empty = new Win32IPAddressCollection(IntPtr.Zero);

	private Win32IPAddressCollection()
	{
	}

	public Win32IPAddressCollection(params IntPtr[] heads)
	{
		foreach (IntPtr head in heads)
		{
			AddSubsequentlyString(head);
		}
	}

	public Win32IPAddressCollection(params Win32_IP_ADDR_STRING[] al)
	{
		for (int i = 0; i < al.Length; i++)
		{
			Win32_IP_ADDR_STRING win32_IP_ADDR_STRING = al[i];
			if (!string.IsNullOrEmpty(win32_IP_ADDR_STRING.IpAddress))
			{
				InternalAdd(IPAddress.Parse(win32_IP_ADDR_STRING.IpAddress));
				AddSubsequentlyString(win32_IP_ADDR_STRING.Next);
			}
		}
	}

	public static Win32IPAddressCollection FromAnycast(IntPtr ptr)
	{
		Win32IPAddressCollection win32IPAddressCollection = new Win32IPAddressCollection();
		IntPtr intPtr = ptr;
		while (intPtr != IntPtr.Zero)
		{
			Win32_IP_ADAPTER_ANYCAST_ADDRESS win32_IP_ADAPTER_ANYCAST_ADDRESS = (Win32_IP_ADAPTER_ANYCAST_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(Win32_IP_ADAPTER_ANYCAST_ADDRESS));
			win32IPAddressCollection.InternalAdd(win32_IP_ADAPTER_ANYCAST_ADDRESS.Address.GetIPAddress());
			intPtr = win32_IP_ADAPTER_ANYCAST_ADDRESS.Next;
		}
		return win32IPAddressCollection;
	}

	public static Win32IPAddressCollection FromDnsServer(IntPtr ptr)
	{
		Win32IPAddressCollection win32IPAddressCollection = new Win32IPAddressCollection();
		IntPtr intPtr = ptr;
		while (intPtr != IntPtr.Zero)
		{
			Win32_IP_ADAPTER_DNS_SERVER_ADDRESS win32_IP_ADAPTER_DNS_SERVER_ADDRESS = (Win32_IP_ADAPTER_DNS_SERVER_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(Win32_IP_ADAPTER_DNS_SERVER_ADDRESS));
			win32IPAddressCollection.InternalAdd(win32_IP_ADAPTER_DNS_SERVER_ADDRESS.Address.GetIPAddress());
			intPtr = win32_IP_ADAPTER_DNS_SERVER_ADDRESS.Next;
		}
		return win32IPAddressCollection;
	}

	public static Win32IPAddressCollection FromSocketAddress(Win32_SOCKET_ADDRESS addr)
	{
		Win32IPAddressCollection win32IPAddressCollection = new Win32IPAddressCollection();
		if (addr.Sockaddr != IntPtr.Zero)
		{
			win32IPAddressCollection.InternalAdd(addr.GetIPAddress());
		}
		return win32IPAddressCollection;
	}

	public static Win32IPAddressCollection FromWinsServer(IntPtr ptr)
	{
		Win32IPAddressCollection win32IPAddressCollection = new Win32IPAddressCollection();
		IntPtr intPtr = ptr;
		while (intPtr != IntPtr.Zero)
		{
			Win32_IP_ADAPTER_WINS_SERVER_ADDRESS win32_IP_ADAPTER_WINS_SERVER_ADDRESS = (Win32_IP_ADAPTER_WINS_SERVER_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(Win32_IP_ADAPTER_WINS_SERVER_ADDRESS));
			win32IPAddressCollection.InternalAdd(win32_IP_ADAPTER_WINS_SERVER_ADDRESS.Address.GetIPAddress());
			intPtr = win32_IP_ADAPTER_WINS_SERVER_ADDRESS.Next;
		}
		return win32IPAddressCollection;
	}

	private void AddSubsequentlyString(IntPtr head)
	{
		IntPtr intPtr = head;
		while (intPtr != IntPtr.Zero)
		{
			Win32_IP_ADDR_STRING win32_IP_ADDR_STRING = (Win32_IP_ADDR_STRING)Marshal.PtrToStructure(intPtr, typeof(Win32_IP_ADDR_STRING));
			InternalAdd(IPAddress.Parse(win32_IP_ADDR_STRING.IpAddress));
			intPtr = win32_IP_ADDR_STRING.Next;
		}
	}
}
