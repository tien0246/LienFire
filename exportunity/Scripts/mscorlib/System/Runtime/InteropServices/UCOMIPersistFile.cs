namespace System.Runtime.InteropServices;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Obsolete]
[Guid("0000010b-0000-0000-c000-000000000046")]
public interface UCOMIPersistFile
{
	void GetClassID(out Guid pClassID);

	[PreserveSig]
	int IsDirty();

	void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, int dwMode);

	void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);

	void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

	void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
}
