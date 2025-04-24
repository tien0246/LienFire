using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Modules/Physics/MeshCollider.h")]
[RequiredByNativeCode]
[NativeHeader("Runtime/Graphics/Mesh/Mesh.h")]
public class MeshCollider : Collider
{
	[Obsolete("Configuring smooth sphere collisions is no longer needed.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool smoothSphereCollisions
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	[Obsolete("MeshCollider.skinWidth is no longer used.")]
	public float skinWidth
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	[Obsolete("MeshCollider.inflateMesh is no longer supported. The new cooking algorithm doesn't need inflation to be used.")]
	public bool inflateMesh
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public extern Mesh sharedMesh
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool convex
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern MeshColliderCookingOptions cookingOptions
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}
}
