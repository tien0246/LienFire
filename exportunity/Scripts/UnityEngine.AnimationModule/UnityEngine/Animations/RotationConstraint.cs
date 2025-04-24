using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[UsedByNativeCode]
[RequireComponent(typeof(Transform))]
[NativeHeader("Modules/Animation/Constraints/RotationConstraint.h")]
[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
public sealed class RotationConstraint : Behaviour, IConstraint, IConstraintInternal
{
	public extern float weight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Vector3 rotationAtRest
	{
		get
		{
			get_rotationAtRest_Injected(out var ret);
			return ret;
		}
		set
		{
			set_rotationAtRest_Injected(ref value);
		}
	}

	public Vector3 rotationOffset
	{
		get
		{
			get_rotationOffset_Injected(out var ret);
			return ret;
		}
		set
		{
			set_rotationOffset_Injected(ref value);
		}
	}

	public extern Axis rotationAxis
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool constraintActive
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool locked
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public int sourceCount => GetSourceCountInternal(this);

	private RotationConstraint()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Create([Writable] RotationConstraint self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ConstraintBindings::GetSourceCount")]
	private static extern int GetSourceCountInternal([NotNull("ArgumentNullException")] RotationConstraint self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
	public extern void GetSources([NotNull("ArgumentNullException")] List<ConstraintSource> sources);

	public void SetSources(List<ConstraintSource> sources)
	{
		if (sources == null)
		{
			throw new ArgumentNullException("sources");
		}
		SetSourcesInternal(this, sources);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
	private static extern void SetSourcesInternal([NotNull("ArgumentNullException")] RotationConstraint self, List<ConstraintSource> sources);

	public int AddSource(ConstraintSource source)
	{
		return AddSource_Injected(ref source);
	}

	public void RemoveSource(int index)
	{
		ValidateSourceIndex(index);
		RemoveSourceInternal(index);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("RemoveSource")]
	private extern void RemoveSourceInternal(int index);

	public ConstraintSource GetSource(int index)
	{
		ValidateSourceIndex(index);
		return GetSourceInternal(index);
	}

	[NativeName("GetSource")]
	private ConstraintSource GetSourceInternal(int index)
	{
		GetSourceInternal_Injected(index, out var ret);
		return ret;
	}

	public void SetSource(int index, ConstraintSource source)
	{
		ValidateSourceIndex(index);
		SetSourceInternal(index, source);
	}

	[NativeName("SetSource")]
	private void SetSourceInternal(int index, ConstraintSource source)
	{
		SetSourceInternal_Injected(index, ref source);
	}

	private void ValidateSourceIndex(int index)
	{
		if (sourceCount == 0)
		{
			throw new InvalidOperationException("The RotationConstraint component has no sources.");
		}
		if (index < 0 || index >= sourceCount)
		{
			throw new ArgumentOutOfRangeException("index", $"Constraint source index {index} is out of bounds (0-{sourceCount}).");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_rotationAtRest_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_rotationAtRest_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_rotationOffset_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_rotationOffset_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int AddSource_Injected(ref ConstraintSource source);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);
}
