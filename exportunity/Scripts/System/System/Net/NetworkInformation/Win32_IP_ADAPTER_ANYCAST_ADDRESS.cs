namespace System.Net.NetworkInformation;

internal struct Win32_IP_ADAPTER_ANYCAST_ADDRESS
{
	public Win32LengthFlagsUnion LengthFlags;

	public IntPtr Next;

	public Win32_SOCKET_ADDRESS Address;
}
