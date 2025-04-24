using UnityEngine.Scripting;

namespace Unity.Collections;

[UsedByNativeCode]
internal enum LeakCategory
{
	Invalid = 0,
	Malloc = 1,
	TempJob = 2,
	Persistent = 3,
	LightProbesQuery = 4,
	NativeTest = 5,
	MeshDataArray = 6,
	TransformAccessArray = 7,
	NavMeshQuery = 8
}
