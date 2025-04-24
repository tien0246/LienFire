using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[ExcludeFromObjectFactory]
[RequiredByNativeCode]
[NativeClass(null)]
internal class FailedToLoadScriptObject : Object
{
	private FailedToLoadScriptObject()
	{
	}
}
