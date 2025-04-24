using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.InteropServices;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("1CF2B120-547D-101B-8E65-08002B2BD119")]
[SuppressUnmanagedCodeSecurity]
internal interface IErrorInfo
{
	[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	int GetGUID(out Guid pGuid);

	[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	int GetSource([MarshalAs(UnmanagedType.BStr)] out string pBstrSource);

	[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	int GetDescription([MarshalAs(UnmanagedType.BStr)] out string pbstrDescription);

	[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	int GetHelpFile([MarshalAs(UnmanagedType.BStr)] out string pBstrHelpFile);

	[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	int GetHelpContext(out uint pdwHelpContext);
}
