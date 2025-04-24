using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[RequireComponent(typeof(Transform), typeof(SkinnedMeshRenderer))]
[NativeHeader("Modules/Cloth/Cloth.h")]
[NativeClass("Unity::Cloth")]
public sealed class Cloth : Component
{
	public extern Vector3[] vertices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetPositions")]
		get;
	}

	public extern Vector3[] normals
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetNormals")]
		get;
	}

	public extern ClothSkinningCoefficient[] coefficients
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetCoefficients")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetCoefficients")]
		set;
	}

	public extern CapsuleCollider[] capsuleColliders
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetCapsuleColliders")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetCapsuleColliders")]
		set;
	}

	public extern ClothSphereColliderPair[] sphereColliders
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetSphereColliders")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetSphereColliders")]
		set;
	}

	public extern float sleepThreshold
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float bendingStiffness
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float stretchingStiffness
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float damping
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Vector3 externalAcceleration
	{
		get
		{
			get_externalAcceleration_Injected(out var ret);
			return ret;
		}
		set
		{
			set_externalAcceleration_Injected(ref value);
		}
	}

	public Vector3 randomAcceleration
	{
		get
		{
			get_randomAcceleration_Injected(out var ret);
			return ret;
		}
		set
		{
			set_randomAcceleration_Injected(ref value);
		}
	}

	public extern bool useGravity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float friction
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float collisionMassScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool enableContinuousCollision
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float useVirtualParticles
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float worldVelocityScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float worldAccelerationScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float clothSolverFrequency
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[Obsolete("Parameter solverFrequency is obsolete and no longer supported. Please use clothSolverFrequency instead.")]
	public bool solverFrequency
	{
		get
		{
			return clothSolverFrequency > 0f;
		}
		set
		{
			clothSolverFrequency = (value ? 120f : 0f);
		}
	}

	public extern bool useTethers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float stiffnessFrequency
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float selfCollisionDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float selfCollisionStiffness
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[Obsolete("useContinuousCollision is no longer supported, use enableContinuousCollision instead")]
	public float useContinuousCollision { get; set; }

	[Obsolete("Deprecated.Cloth.selfCollisions is no longer supported since Unity 5.0.", true)]
	public bool selfCollision { get; }

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ClearTransformMotion();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void GetSelfAndInterCollisionIndices([NotNull("ArgumentNullException")] List<uint> indices);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetSelfAndInterCollisionIndices([NotNull("ArgumentNullException")] List<uint> indices);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void GetVirtualParticleIndices([NotNull("ArgumentNullException")] List<uint> indicesOutList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetVirtualParticleIndices([NotNull("ArgumentNullException")] List<uint> indicesIn);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void GetVirtualParticleWeights([NotNull("ArgumentNullException")] List<Vector3> weightsOutList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetVirtualParticleWeights([NotNull("ArgumentNullException")] List<Vector3> weights);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetEnabledFading(bool enabled, float interpolationTime);

	[ExcludeFromDocs]
	public void SetEnabledFading(bool enabled)
	{
		SetEnabledFading(enabled, 0.5f);
	}

	internal RaycastHit Raycast(Ray ray, float maxDistance, ref bool hasHit)
	{
		Raycast_Injected(ref ray, maxDistance, ref hasHit, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_externalAcceleration_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_externalAcceleration_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_randomAcceleration_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_randomAcceleration_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void Raycast_Injected(ref Ray ray, float maxDistance, ref bool hasHit, out RaycastHit ret);
}
