using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;

namespace UnityEngine.Experimental.Playables;

[StaticAccessor("TexturePlayableGraphExtensionsBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
[NativeHeader("Runtime/Export/Director/TexturePlayableGraphExtensions.bindings.h")]
internal static class TexturePlayableGraphExtensions
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal static extern bool InternalCreateTextureOutput(ref PlayableGraph graph, string name, out PlayableOutputHandle handle);
}
