using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

[NativeHeader("Runtime/Shaders/RayTracingAccelerationStructure.h")]
[Flags]
[NativeHeader("Runtime/Export/Graphics/RayTracingAccelerationStructure.bindings.h")]
[UsedByNativeCode]
public enum RayTracingSubMeshFlags
{
	Disabled = 0,
	Enabled = 1,
	ClosestHitOnly = 2,
	UniqueAnyHitCalls = 4
}
