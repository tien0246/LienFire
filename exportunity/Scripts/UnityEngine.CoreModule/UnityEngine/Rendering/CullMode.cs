using UnityEngine.Bindings;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/GfxDevice/GfxDeviceTypes.h")]
public enum CullMode
{
	Off = 0,
	Front = 1,
	Back = 2
}
