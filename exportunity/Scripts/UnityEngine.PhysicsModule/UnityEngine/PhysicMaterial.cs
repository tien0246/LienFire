using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Physics/PhysicMaterial.h")]
public class PhysicMaterial : Object
{
	[Obsolete("Use PhysicMaterial.bounciness instead (UnityUpgradable) -> bounciness")]
	public float bouncyness
	{
		get
		{
			return bounciness;
		}
		set
		{
			bounciness = value;
		}
	}

	[Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Vector3 frictionDirection2
	{
		get
		{
			return Vector3.zero;
		}
		set
		{
		}
	}

	[Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public float dynamicFriction2
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	[Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public float staticFriction2
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	[Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Vector3 frictionDirection
	{
		get
		{
			return Vector3.zero;
		}
		set
		{
		}
	}

	public extern float bounciness
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float dynamicFriction
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float staticFriction
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern PhysicMaterialCombine frictionCombine
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern PhysicMaterialCombine bounceCombine
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public PhysicMaterial()
	{
		Internal_CreateDynamicsMaterial(this, "DynamicMaterial");
	}

	public PhysicMaterial(string name)
	{
		Internal_CreateDynamicsMaterial(this, name);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CreateDynamicsMaterial([Writable] PhysicMaterial mat, string name);
}
