using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[PreventReadOnlyInstanceModification]
[NativeHeader("Runtime/Graphics/LightingSettings.h")]
public sealed class LightingSettings : Object
{
	[NativeName("EnableBakedLightmaps")]
	public extern bool bakedGI
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeName("EnableRealtimeLightmaps")]
	public extern bool realtimeGI
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeName("RealtimeEnvironmentLighting")]
	public extern bool realtimeEnvironmentLighting
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[RequiredByNativeCode]
	internal void LightingSettingsDontStripMe()
	{
	}

	public LightingSettings()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Create([Writable] LightingSettings self);
}
