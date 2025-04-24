using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/RenderTexture.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Graphics/RenderBufferManager.h")]
[NativeHeader("Runtime/Camera/Camera.h")]
public class RenderTexture : Texture
{
	public override extern int width
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public override extern int height
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public override extern TextureDimension dimension
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("ColorFormat")]
	public new extern GraphicsFormat graphicsFormat
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("MipMap")]
	public extern bool useMipMap
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("SRGBReadWrite")]
	public extern bool sRGB
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[NativeProperty("VRUsage")]
	public extern VRTextureUsage vrUsage
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("Memoryless")]
	public extern RenderTextureMemoryless memorylessMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public RenderTextureFormat format
	{
		get
		{
			if (graphicsFormat != GraphicsFormat.None)
			{
				return GraphicsFormatUtility.GetRenderTextureFormat(graphicsFormat);
			}
			if (GetDescriptor().shadowSamplingMode != ShadowSamplingMode.None)
			{
				return RenderTextureFormat.Shadowmap;
			}
			return RenderTextureFormat.Depth;
		}
		set
		{
			graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(value, sRGB);
		}
	}

	public extern GraphicsFormat stencilFormat
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern GraphicsFormat depthStencilFormat
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool autoGenerateMips
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern int volumeDepth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern int antiAliasing
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool bindTextureMS
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool enableRandomWrite
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool useDynamicScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public bool isPowerOfTwo
	{
		get
		{
			return GetIsPowerOfTwo();
		}
		set
		{
		}
	}

	public static RenderTexture active
	{
		get
		{
			return GetActive();
		}
		set
		{
			SetActive(value);
		}
	}

	public RenderBuffer colorBuffer => GetColorBuffer();

	public RenderBuffer depthBuffer => GetDepthBuffer();

	public extern int depth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("RenderTextureScripting::GetDepth", HasExplicitThis = true)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("RenderTextureScripting::SetDepth", HasExplicitThis = true)]
		set;
	}

	[Obsolete("Use RenderTexture.dimension instead.", false)]
	public bool isCubemap
	{
		get
		{
			return dimension == TextureDimension.Cube;
		}
		set
		{
			dimension = (value ? TextureDimension.Cube : TextureDimension.Tex2D);
		}
	}

	[Obsolete("Use RenderTexture.dimension instead.", false)]
	public bool isVolume
	{
		get
		{
			return dimension == TextureDimension.Tex3D;
		}
		set
		{
			dimension = (value ? TextureDimension.Tex3D : TextureDimension.Tex2D);
		}
	}

	[Obsolete("RenderTexture.enabled is always now, no need to use it.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool enabled
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	public RenderTextureDescriptor descriptor
	{
		get
		{
			return GetDescriptor();
		}
		set
		{
			ValidateRenderTextureDesc(value);
			SetRenderTextureDescriptor(value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool GetIsPowerOfTwo();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("RenderTexture::GetActive")]
	private static extern RenderTexture GetActive();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("RenderTextureScripting::SetActive")]
	private static extern void SetActive(RenderTexture rt);

	[FreeFunction(Name = "RenderTextureScripting::GetColorBuffer", HasExplicitThis = true)]
	private RenderBuffer GetColorBuffer()
	{
		GetColorBuffer_Injected(out var ret);
		return ret;
	}

	[FreeFunction(Name = "RenderTextureScripting::GetDepthBuffer", HasExplicitThis = true)]
	private RenderBuffer GetDepthBuffer()
	{
		GetDepthBuffer_Injected(out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetMipMapCount(int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetShadowSamplingMode(ShadowSamplingMode samplingMode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern IntPtr GetNativeDepthBufferPtr();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void DiscardContents(bool discardColor, bool discardDepth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("This function has no effect.", false)]
	public extern void MarkRestoreExpected();

	public void DiscardContents()
	{
		DiscardContents(discardColor: true, discardDepth: true);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ResolveAntiAliasedSurface")]
	private extern void ResolveAA();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ResolveAntiAliasedSurface")]
	private extern void ResolveAATo(RenderTexture rt);

	public void ResolveAntiAliasedSurface()
	{
		ResolveAA();
	}

	public void ResolveAntiAliasedSurface(RenderTexture target)
	{
		ResolveAATo(target);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RenderTextureScripting::SetGlobalShaderProperty", HasExplicitThis = true)]
	public extern void SetGlobalShaderProperty(string propertyName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern bool Create();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void Release();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern bool IsCreated();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void GenerateMips();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	public extern void ConvertToEquirect(RenderTexture equirect, Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void SetSRGBReadWrite(bool srgb);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("RenderTextureScripting::Create")]
	private static extern void Internal_Create([Writable] RenderTexture rt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("RenderTextureSupportsStencil")]
	public static extern bool SupportsStencil(RenderTexture rt);

	[NativeName("SetRenderTextureDescFromScript")]
	private void SetRenderTextureDescriptor(RenderTextureDescriptor desc)
	{
		SetRenderTextureDescriptor_Injected(ref desc);
	}

	[NativeName("GetRenderTextureDesc")]
	private RenderTextureDescriptor GetDescriptor()
	{
		GetDescriptor_Injected(out var ret);
		return ret;
	}

	[FreeFunction("GetRenderBufferManager().GetTextures().GetTempBuffer")]
	private static RenderTexture GetTemporary_Internal(RenderTextureDescriptor desc)
	{
		return GetTemporary_Internal_Injected(ref desc);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetRenderBufferManager().GetTextures().ReleaseTempBuffer")]
	public static extern void ReleaseTemporary(RenderTexture temp);

	[Obsolete("GetTexelOffset always returns zero now, no point in using it.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Vector2 GetTexelOffset()
	{
		return Vector2.zero;
	}

	[RequiredByNativeCode]
	protected internal RenderTexture()
	{
	}

	public RenderTexture(RenderTextureDescriptor desc)
	{
		ValidateRenderTextureDesc(desc);
		Internal_Create(this);
		SetRenderTextureDescriptor(desc);
	}

	public RenderTexture(RenderTexture textureToCopy)
	{
		if (textureToCopy == null)
		{
			throw new ArgumentNullException("textureToCopy");
		}
		ValidateRenderTextureDesc(textureToCopy.descriptor);
		Internal_Create(this);
		SetRenderTextureDescriptor(textureToCopy.descriptor);
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth, DefaultFormat format)
		: this(width, height, depth, SystemInfo.GetGraphicsFormat(format))
	{
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth, GraphicsFormat format)
		: this(width, height, depth, format, Texture.GenerateAllMips)
	{
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth, GraphicsFormat format, int mipCount)
	{
		if (ValidateFormat(format, FormatUsage.Render))
		{
			Internal_Create(this);
			depthStencilFormat = GetDepthStencilFormatLegacy(depth, format);
			this.width = width;
			this.height = height;
			graphicsFormat = format;
			SetMipMapCount(mipCount);
			SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(format));
		}
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat, int mipCount)
	{
		if (colorFormat == GraphicsFormat.None || ValidateFormat(colorFormat, FormatUsage.Render))
		{
			Internal_Create(this);
			this.width = width;
			this.height = height;
			this.depthStencilFormat = depthStencilFormat;
			graphicsFormat = colorFormat;
			SetMipMapCount(mipCount);
			SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(colorFormat));
		}
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat)
		: this(width, height, colorFormat, depthStencilFormat, Texture.GenerateAllMips)
	{
	}

	public RenderTexture(int width, int height, int depth, [UnityEngine.Internal.DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [UnityEngine.Internal.DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite)
	{
		Initialize(width, height, depth, format, readWrite, Texture.GenerateAllMips);
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
		: this(width, height, depth, format, Texture.GenerateAllMips)
	{
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth)
		: this(width, height, depth, RenderTextureFormat.Default)
	{
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth, RenderTextureFormat format, int mipCount)
	{
		Initialize(width, height, depth, format, RenderTextureReadWrite.Default, mipCount);
	}

	private void Initialize(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite, int mipCount)
	{
		GraphicsFormat compatibleFormat = GetCompatibleFormat(format, readWrite);
		GraphicsFormat depthStencilFormatLegacy = GetDepthStencilFormatLegacy(depth, compatibleFormat);
		if (compatibleFormat == GraphicsFormat.None || ValidateFormat(compatibleFormat, FormatUsage.Render))
		{
			Internal_Create(this);
			this.width = width;
			this.height = height;
			depthStencilFormat = depthStencilFormatLegacy;
			graphicsFormat = compatibleFormat;
			SetMipMapCount(mipCount);
			SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(compatibleFormat));
		}
	}

	internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, GraphicsFormat colorFormat)
	{
		return (colorFormat == GraphicsFormat.ShadowAuto) ? GraphicsFormatUtility.GetDepthStencilFormat(depthBits, 0) : GraphicsFormatUtility.GetDepthStencilFormat(depthBits);
	}

	private static void ValidateRenderTextureDesc(RenderTextureDescriptor desc)
	{
		if (desc.graphicsFormat == GraphicsFormat.None && desc.depthStencilFormat == GraphicsFormat.None)
		{
			throw new ArgumentException("RenderTextureDesc graphicsFormat and depthStencilFormat cannot both be None.");
		}
		if (desc.graphicsFormat != GraphicsFormat.None && !SystemInfo.IsFormatSupported(desc.graphicsFormat, FormatUsage.Render))
		{
			throw new ArgumentException("RenderTextureDesc graphicsFormat must be a supported GraphicsFormat. " + desc.graphicsFormat.ToString() + " is not supported on this platform.", "desc.graphicsFormat");
		}
		if (desc.depthStencilFormat != GraphicsFormat.None && !GraphicsFormatUtility.IsDepthFormat(desc.depthStencilFormat) && !GraphicsFormatUtility.IsStencilFormat(desc.depthStencilFormat))
		{
			throw new ArgumentException("RenderTextureDesc depthStencilFormat must be a supported depth/stencil GraphicsFormat. " + desc.depthStencilFormat.ToString() + " is not supported on this platform.", "desc.depthStencilFormat");
		}
		if (desc.width <= 0)
		{
			throw new ArgumentException("RenderTextureDesc width must be greater than zero.", "desc.width");
		}
		if (desc.height <= 0)
		{
			throw new ArgumentException("RenderTextureDesc height must be greater than zero.", "desc.height");
		}
		if (desc.volumeDepth <= 0)
		{
			throw new ArgumentException("RenderTextureDesc volumeDepth must be greater than zero.", "desc.volumeDepth");
		}
		if (desc.msaaSamples != 1 && desc.msaaSamples != 2 && desc.msaaSamples != 4 && desc.msaaSamples != 8)
		{
			throw new ArgumentException("RenderTextureDesc msaaSamples must be 1, 2, 4, or 8.", "desc.msaaSamples");
		}
		if (desc.dimension == TextureDimension.CubeArray && desc.volumeDepth % 6 != 0)
		{
			throw new ArgumentException("RenderTextureDesc volumeDepth must be a multiple of 6 when dimension is CubeArray", "desc.volumeDepth");
		}
		if (desc.graphicsFormat != GraphicsFormat.ShadowAuto && desc.graphicsFormat != GraphicsFormat.DepthAuto && (GraphicsFormatUtility.IsDepthFormat(desc.graphicsFormat) || GraphicsFormatUtility.IsStencilFormat(desc.graphicsFormat)))
		{
			throw new ArgumentException("RenderTextureDesc graphicsFormat must not be a depth/stencil format. " + desc.graphicsFormat.ToString() + " is not supported.", "desc.graphicsFormat");
		}
	}

	internal static GraphicsFormat GetCompatibleFormat(RenderTextureFormat renderTextureFormat, RenderTextureReadWrite readWrite)
	{
		GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(renderTextureFormat, readWrite);
		GraphicsFormat compatibleFormat = SystemInfo.GetCompatibleFormat(graphicsFormat, FormatUsage.Render);
		if (graphicsFormat == compatibleFormat)
		{
			return graphicsFormat;
		}
		Debug.LogWarning($"'{graphicsFormat.ToString()}' is not supported. RenderTexture::GetTemporary fallbacks to {compatibleFormat.ToString()} format on this platform. Use 'SystemInfo.IsFormatSupported' C# API to check format support.");
		return compatibleFormat;
	}

	public static RenderTexture GetTemporary(RenderTextureDescriptor desc)
	{
		ValidateRenderTextureDesc(desc);
		desc.createdFromScript = true;
		return GetTemporary_Internal(desc);
	}

	private static RenderTexture GetTemporaryImpl(int width, int height, int depthBuffer, GraphicsFormat colorFormat, int antiAliasing = 1, RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, bool useDynamicScale = false)
	{
		RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height, colorFormat, GetDepthStencilFormatLegacy(depthBuffer, colorFormat));
		desc.msaaSamples = antiAliasing;
		desc.memoryless = memorylessMode;
		desc.vrUsage = vrUsage;
		desc.useDynamicScale = useDynamicScale;
		return GetTemporary(desc);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, [UnityEngine.Internal.DefaultValue("1")] int antiAliasing, [UnityEngine.Internal.DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [UnityEngine.Internal.DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage, [UnityEngine.Internal.DefaultValue("false")] bool useDynamicScale)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format, antiAliasing, memorylessMode, vrUsage, useDynamicScale);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing, RenderTextureMemoryless memorylessMode, VRTextureUsage vrUsage)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format, antiAliasing, memorylessMode, vrUsage);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing, RenderTextureMemoryless memorylessMode)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format, antiAliasing, memorylessMode);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format, antiAliasing);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format)
	{
		return GetTemporaryImpl(width, height, depthBuffer, format);
	}

	public static RenderTexture GetTemporary(int width, int height, [UnityEngine.Internal.DefaultValue("0")] int depthBuffer, [UnityEngine.Internal.DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [UnityEngine.Internal.DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [UnityEngine.Internal.DefaultValue("1")] int antiAliasing, [UnityEngine.Internal.DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [UnityEngine.Internal.DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage, [UnityEngine.Internal.DefaultValue("false")] bool useDynamicScale)
	{
		return GetTemporaryImpl(width, height, depthBuffer, GetCompatibleFormat(format, readWrite), antiAliasing, memorylessMode, vrUsage, useDynamicScale);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode, VRTextureUsage vrUsage)
	{
		return GetTemporaryImpl(width, height, depthBuffer, GetCompatibleFormat(format, readWrite), antiAliasing, memorylessMode, vrUsage);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode)
	{
		return GetTemporaryImpl(width, height, depthBuffer, GetCompatibleFormat(format, readWrite), antiAliasing, memorylessMode);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
	{
		return GetTemporaryImpl(width, height, depthBuffer, GetCompatibleFormat(format, readWrite), antiAliasing);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
	{
		return GetTemporaryImpl(width, height, depthBuffer, GetCompatibleFormat(format, readWrite));
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
	{
		return GetTemporaryImpl(width, height, depthBuffer, GetCompatibleFormat(format, RenderTextureReadWrite.Default));
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
	{
		return GetTemporaryImpl(width, height, depthBuffer, GetCompatibleFormat(RenderTextureFormat.Default, RenderTextureReadWrite.Default));
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height)
	{
		return GetTemporaryImpl(width, height, 0, GetCompatibleFormat(RenderTextureFormat.Default, RenderTextureReadWrite.Default));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetColorBuffer_Injected(out RenderBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetDepthBuffer_Injected(out RenderBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetRenderTextureDescriptor_Injected(ref RenderTextureDescriptor desc);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetDescriptor_Injected(out RenderTextureDescriptor ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RenderTexture GetTemporary_Internal_Injected(ref RenderTextureDescriptor desc);
}
