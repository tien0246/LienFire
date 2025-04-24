using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngineInternal;

namespace UnityEngine;

[NativeHeader("Runtime/Misc/ResourceManagerUtility.h")]
[NativeHeader("Runtime/Export/Resources/Resources.bindings.h")]
internal static class ResourcesAPIInternal
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
	[FreeFunction("Resources_Bindings::FindObjectsOfTypeAll")]
	public static extern Object[] FindObjectsOfTypeAll(Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetShaderNameRegistry().FindShader")]
	public static extern Shader FindShaderByName(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Resources_Bindings::Load")]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
	[NativeThrows]
	public static extern Object Load(string path, [NotNull("ArgumentNullException")] Type systemTypeInstance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Resources_Bindings::LoadAll")]
	[NativeThrows]
	public static extern Object[] LoadAll([NotNull("ArgumentNullException")] string path, [NotNull("ArgumentNullException")] Type systemTypeInstance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Resources_Bindings::LoadAsyncInternal")]
	internal static extern ResourceRequest LoadAsyncInternal(string path, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::UnloadAssetFromScripting")]
	public static extern void UnloadAsset(Object assetToUnload);
}
