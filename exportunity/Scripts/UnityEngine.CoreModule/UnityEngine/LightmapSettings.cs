using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[StaticAccessor("GetLightmapSettings()")]
[NativeHeader("Runtime/Graphics/LightmapSettings.h")]
public sealed class LightmapSettings : Object
{
	[Obsolete("Use lightmapsMode instead.", false)]
	public static LightmapsModeLegacy lightmapsModeLegacy
	{
		get
		{
			return LightmapsModeLegacy.Single;
		}
		set
		{
		}
	}

	[Obsolete("Use QualitySettings.desiredColorSpace instead.", false)]
	public static ColorSpace bakedColorSpace
	{
		get
		{
			return QualitySettings.desiredColorSpace;
		}
		set
		{
		}
	}

	public static extern LightmapData[] lightmaps
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(ThrowsException = true)]
		set;
	}

	public static extern LightmapsMode lightmapsMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(ThrowsException = true)]
		set;
	}

	public static extern LightProbes lightProbes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetLightProbes")]
		[FreeFunction]
		set;
	}

	private LightmapSettings()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ResetAndAwakeFromLoad")]
	internal static extern void Reset();
}
