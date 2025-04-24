using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.VFX;

[UsedByNativeCode]
[NativeHeader("Modules/VFX/Public/VFXSystem.h")]
public struct VFXParticleSystemInfo
{
	public uint aliveCount;

	public uint capacity;

	public bool sleeping;

	public Bounds bounds;

	public VFXParticleSystemInfo(uint aliveCount, uint capacity, bool sleeping, Bounds bounds)
	{
		this.aliveCount = aliveCount;
		this.capacity = capacity;
		this.sleeping = sleeping;
		this.bounds = bounds;
	}
}
