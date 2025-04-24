namespace System.Runtime.InteropServices;

[ComImport]
[Guid("00000102-0000-0000-c000-000000000046")]
[Obsolete]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface UCOMIEnumMoniker
{
	[PreserveSig]
	int Next(int celt, [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] UCOMIMoniker[] rgelt, out int pceltFetched);

	[PreserveSig]
	int Skip(int celt);

	[PreserveSig]
	int Reset();

	void Clone(out UCOMIEnumMoniker ppenum);
}
