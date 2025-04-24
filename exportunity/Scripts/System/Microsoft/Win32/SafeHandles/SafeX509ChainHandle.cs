using System;
using Unity;

namespace Microsoft.Win32.SafeHandles;

public sealed class SafeX509ChainHandle : SafeHandleZeroOrMinusOneIsInvalid
{
	internal SafeX509ChainHandle(IntPtr handle)
		: base(ownsHandle: true)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected override bool ReleaseHandle()
	{
		throw new NotImplementedException();
	}

	internal SafeX509ChainHandle()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
