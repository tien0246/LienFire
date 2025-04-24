using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[NativeHeader("Modules/Animation/ScriptBindings/AnimationClipPlayable.bindings.h")]
[NativeHeader("Modules/Animation/Director/AnimationClipPlayable.h")]
[StaticAccessor("AnimationClipPlayableBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
public struct AnimationClipPlayable : IPlayable, IEquatable<AnimationClipPlayable>
{
	private PlayableHandle m_Handle;

	public static AnimationClipPlayable Create(PlayableGraph graph, AnimationClip clip)
	{
		PlayableHandle handle = CreateHandle(graph, clip);
		return new AnimationClipPlayable(handle);
	}

	private static PlayableHandle CreateHandle(PlayableGraph graph, AnimationClip clip)
	{
		PlayableHandle handle = PlayableHandle.Null;
		if (!CreateHandleInternal(graph, clip, ref handle))
		{
			return PlayableHandle.Null;
		}
		return handle;
	}

	internal AnimationClipPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<AnimationClipPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AnimationClipPlayable.");
		}
		m_Handle = handle;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator Playable(AnimationClipPlayable playable)
	{
		return new Playable(playable.GetHandle());
	}

	public static explicit operator AnimationClipPlayable(Playable playable)
	{
		return new AnimationClipPlayable(playable.GetHandle());
	}

	public bool Equals(AnimationClipPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}

	public AnimationClip GetAnimationClip()
	{
		return GetAnimationClipInternal(ref m_Handle);
	}

	public bool GetApplyFootIK()
	{
		return GetApplyFootIKInternal(ref m_Handle);
	}

	public void SetApplyFootIK(bool value)
	{
		SetApplyFootIKInternal(ref m_Handle, value);
	}

	public bool GetApplyPlayableIK()
	{
		return GetApplyPlayableIKInternal(ref m_Handle);
	}

	public void SetApplyPlayableIK(bool value)
	{
		SetApplyPlayableIKInternal(ref m_Handle, value);
	}

	internal bool GetRemoveStartOffset()
	{
		return GetRemoveStartOffsetInternal(ref m_Handle);
	}

	internal void SetRemoveStartOffset(bool value)
	{
		SetRemoveStartOffsetInternal(ref m_Handle, value);
	}

	internal bool GetOverrideLoopTime()
	{
		return GetOverrideLoopTimeInternal(ref m_Handle);
	}

	internal void SetOverrideLoopTime(bool value)
	{
		SetOverrideLoopTimeInternal(ref m_Handle, value);
	}

	internal bool GetLoopTime()
	{
		return GetLoopTimeInternal(ref m_Handle);
	}

	internal void SetLoopTime(bool value)
	{
		SetLoopTimeInternal(ref m_Handle, value);
	}

	internal float GetSampleRate()
	{
		return GetSampleRateInternal(ref m_Handle);
	}

	internal void SetSampleRate(float value)
	{
		SetSampleRateInternal(ref m_Handle, value);
	}

	[NativeThrows]
	private static bool CreateHandleInternal(PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle)
	{
		return CreateHandleInternal_Injected(ref graph, clip, ref handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern AnimationClip GetAnimationClipInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern bool GetApplyFootIKInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern void SetApplyFootIKInternal(ref PlayableHandle handle, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern bool GetApplyPlayableIKInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern void SetApplyPlayableIKInternal(ref PlayableHandle handle, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern bool GetRemoveStartOffsetInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern void SetRemoveStartOffsetInternal(ref PlayableHandle handle, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern bool GetOverrideLoopTimeInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern void SetOverrideLoopTimeInternal(ref PlayableHandle handle, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern bool GetLoopTimeInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern void SetLoopTimeInternal(ref PlayableHandle handle, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern float GetSampleRateInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern void SetSampleRateInternal(ref PlayableHandle handle, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle);
}
