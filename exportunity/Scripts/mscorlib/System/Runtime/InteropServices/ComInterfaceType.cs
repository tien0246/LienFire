namespace System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
public enum ComInterfaceType
{
	InterfaceIsDual = 0,
	InterfaceIsIUnknown = 1,
	InterfaceIsIDispatch = 2,
	[ComVisible(false)]
	InterfaceIsIInspectable = 3
}
