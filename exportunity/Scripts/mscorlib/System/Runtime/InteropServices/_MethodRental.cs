using System.Reflection.Emit;

namespace System.Runtime.InteropServices;

[CLSCompliant(false)]
[TypeLibImportClass(typeof(MethodRental))]
[ComVisible(true)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("C2323C25-F57F-3880-8A4D-12EBEA7A5852")]
public interface _MethodRental
{
	void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

	void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

	void GetTypeInfoCount(out uint pcTInfo);

	void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
}
