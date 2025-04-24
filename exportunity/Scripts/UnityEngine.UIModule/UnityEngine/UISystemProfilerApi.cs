using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/UI/Canvas.h")]
[StaticAccessor("UI::SystemProfilerApi", StaticAccessorType.DoubleColon)]
public static class UISystemProfilerApi
{
	public enum SampleType
	{
		Layout = 0,
		Render = 1
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void BeginSample(SampleType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void EndSample(SampleType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void AddMarker(string name, Object obj);
}
