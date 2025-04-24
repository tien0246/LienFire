using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[VisibleToOtherModules]
[NativeHeader("Runtime/Export/Scripting/ScriptingRuntime.h")]
internal class ScriptingRuntime
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern string[] GetAllUserAssemblies();
}
