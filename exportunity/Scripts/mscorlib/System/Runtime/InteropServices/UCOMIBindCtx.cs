namespace System.Runtime.InteropServices;

[ComImport]
[Guid("0000000e-0000-0000-c000-000000000046")]
[Obsolete]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface UCOMIBindCtx
{
	void RegisterObjectBound([MarshalAs(UnmanagedType.Interface)] object punk);

	void RevokeObjectBound([MarshalAs(UnmanagedType.Interface)] object punk);

	void ReleaseBoundObjects();

	void SetBindOptions([In] ref BIND_OPTS pbindopts);

	void GetBindOptions(ref BIND_OPTS pbindopts);

	void GetRunningObjectTable(out UCOMIRunningObjectTable pprot);

	void RegisterObjectParam([MarshalAs(UnmanagedType.LPWStr)] string pszKey, [MarshalAs(UnmanagedType.Interface)] object punk);

	void GetObjectParam([MarshalAs(UnmanagedType.LPWStr)] string pszKey, [MarshalAs(UnmanagedType.Interface)] out object ppunk);

	void EnumObjectParam(out UCOMIEnumString ppenum);

	void RevokeObjectParam([MarshalAs(UnmanagedType.LPWStr)] string pszKey);
}
