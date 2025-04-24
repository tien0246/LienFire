using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.SceneManagement;

[NativeHeader("Runtime/SceneManager/SceneManager.h")]
[NativeHeader("Runtime/Export/SceneManager/SceneManager.bindings.h")]
[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
internal static class SceneManagerAPIInternal
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern int GetNumScenesInBuildSettings();

	[NativeThrows]
	public static Scene GetSceneByBuildIndex(int buildIndex)
	{
		GetSceneByBuildIndex_Injected(buildIndex, out var ret);
		return ret;
	}

	[NativeThrows]
	public static AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, LoadSceneParameters parameters, bool mustCompleteNextFrame)
	{
		return LoadSceneAsyncNameIndexInternal_Injected(sceneName, sceneBuildIndex, ref parameters, mustCompleteNextFrame);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	public static extern AsyncOperation UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSceneByBuildIndex_Injected(int buildIndex, out Scene ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AsyncOperation LoadSceneAsyncNameIndexInternal_Injected(string sceneName, int sceneBuildIndex, ref LoadSceneParameters parameters, bool mustCompleteNextFrame);
}
