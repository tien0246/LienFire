namespace System.Runtime.InteropServices;

[ComImport]
[Guid("00020402-0000-0000-c000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Obsolete]
public interface UCOMITypeLib
{
	[PreserveSig]
	int GetTypeInfoCount();

	void GetTypeInfo(int index, out UCOMITypeInfo ppTI);

	void GetTypeInfoType(int index, out TYPEKIND pTKind);

	void GetTypeInfoOfGuid(ref Guid guid, out UCOMITypeInfo ppTInfo);

	void GetLibAttr(out IntPtr ppTLibAttr);

	void GetTypeComp(out UCOMITypeComp ppTComp);

	void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile);

	[return: MarshalAs(UnmanagedType.Bool)]
	bool IsName([MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, int lHashVal);

	void FindName([MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, int lHashVal, [Out][MarshalAs(UnmanagedType.LPArray)] UCOMITypeInfo[] ppTInfo, [Out][MarshalAs(UnmanagedType.LPArray)] int[] rgMemId, ref short pcFound);

	[PreserveSig]
	void ReleaseTLibAttr(IntPtr pTLibAttr);
}
