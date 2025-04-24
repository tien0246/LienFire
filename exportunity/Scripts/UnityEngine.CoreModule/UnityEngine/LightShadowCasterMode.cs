using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Camera/SharedLightData.h")]
public enum LightShadowCasterMode
{
	Default = 0,
	NonLightmappedOnly = 1,
	Everything = 2
}
