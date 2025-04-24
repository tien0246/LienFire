using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine.Bindings;

namespace Unity.Collections.LowLevel.Unsafe;

[NativeHeader("Runtime/Export/BurstLike/BurstLike.bindings.h")]
[StaticAccessor("BurstLike", StaticAccessorType.DoubleColon)]
internal static class BurstLike
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe(ThrowsException = false)]
	[BurstAuthorizedExternalMethod]
	internal static extern int NativeFunctionCall_Int_IntPtr_IntPtr(IntPtr function, IntPtr p0, IntPtr p1, out int error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[BurstAuthorizedExternalMethod]
	[ThreadSafe(ThrowsException = false)]
	internal static extern IntPtr StaticDataGetOrCreate(int key, int sizeInBytes, out int error);
}
