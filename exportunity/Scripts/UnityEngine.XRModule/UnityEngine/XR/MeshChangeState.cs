using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR;

[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshBindings.h")]
[UsedByNativeCode]
public enum MeshChangeState
{
	Added = 0,
	Updated = 1,
	Removed = 2,
	Unchanged = 3
}
