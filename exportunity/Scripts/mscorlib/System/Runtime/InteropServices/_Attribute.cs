namespace System.Runtime.InteropServices;

[Guid("917B14D0-2D9E-38B8-92A9-381ACF52F7C0")]
[CLSCompliant(false)]
[ComVisible(true)]
[TypeLibImportClass(typeof(Attribute))]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface _Attribute
{
	void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

	void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

	void GetTypeInfoCount(out uint pcTInfo);

	void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
}
