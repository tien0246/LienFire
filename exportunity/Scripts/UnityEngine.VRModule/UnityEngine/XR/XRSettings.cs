using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine.XR;

[NativeHeader("Modules/VR/VRModule.h")]
[NativeHeader("Runtime/GfxDevice/GfxDeviceTypes.h")]
[NativeHeader("Modules/VR/ScriptBindings/XR.bindings.h")]
[NativeHeader("Runtime/Interfaces/IVRDevice.h")]
[NativeConditional("ENABLE_VR")]
public static class XRSettings
{
	public enum StereoRenderingMode
	{
		MultiPass = 0,
		SinglePass = 1,
		SinglePassInstanced = 2,
		SinglePassMultiview = 3
	}

	public static extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	public static extern GameViewRenderMode gameViewRenderMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeName("Active")]
	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	public static extern bool isDeviceActive
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	public static extern bool showDeviceView
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeName("RenderScale")]
	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	public static extern float eyeTextureResolutionScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	public static extern int eyeTextureWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	public static extern int eyeTextureHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[NativeConditional("ENABLE_VR", "RenderTextureDesc()")]
	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	[NativeName("IntermediateEyeTextureDesc")]
	public static RenderTextureDescriptor eyeTextureDesc
	{
		get
		{
			get_eyeTextureDesc_Injected(out var ret);
			return ret;
		}
	}

	[NativeName("DeviceEyeTextureDimension")]
	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	public static extern TextureDimension deviceEyeTextureDimension
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static float renderViewportScale
	{
		get
		{
			return renderViewportScaleInternal;
		}
		set
		{
			if (value < 0f || value > 1f)
			{
				throw new ArgumentOutOfRangeException("value", "Render viewport scale should be between 0 and 1.");
			}
			renderViewportScaleInternal = value;
		}
	}

	[NativeName("RenderViewportScale")]
	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	internal static extern float renderViewportScaleInternal
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	public static extern float occlusionMaskScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	public static extern bool useOcclusionMesh
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	[NativeName("DeviceName")]
	public static extern string loadedDeviceName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern string[] supportedDevices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
	public static extern StereoRenderingMode stereoRenderingMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static void LoadDeviceByName(string deviceName)
	{
		LoadDeviceByName(new string[1] { deviceName });
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void LoadDeviceByName(string[] prioritizedDeviceNameList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_eyeTextureDesc_Injected(out RenderTextureDescriptor ret);
}
