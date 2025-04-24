using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
public static class RendererExtensions
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("RendererScripting::UpdateGIMaterialsForRenderer")]
	internal static extern void UpdateGIMaterialsForRenderer(Renderer renderer);

	public static void UpdateGIMaterials(this Renderer renderer)
	{
		UpdateGIMaterialsForRenderer(renderer);
	}
}
