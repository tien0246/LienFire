namespace System.Runtime.InteropServices;

[ComImport]
[Obsolete]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("00000101-0000-0000-c000-000000000046")]
public interface UCOMIEnumString
{
	[PreserveSig]
	int Next(int celt, [Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0)] string[] rgelt, out int pceltFetched);

	[PreserveSig]
	int Skip(int celt);

	[PreserveSig]
	int Reset();

	void Clone(out UCOMIEnumString ppenum);
}
