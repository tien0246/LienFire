using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D;

[Serializable]
[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
[MovedFrom("UnityEngine.Experimental.U2D")]
[RequiredByNativeCode]
[NativeType(CodegenOptions.Custom, "ScriptingSpriteBone")]
[NativeHeader("Runtime/2D/Common/SpriteDataMarshalling.h")]
public struct SpriteBone
{
	[SerializeField]
	[NativeName("name")]
	private string m_Name;

	[NativeName("guid")]
	[SerializeField]
	private string m_Guid;

	[NativeName("position")]
	[SerializeField]
	private Vector3 m_Position;

	[NativeName("rotation")]
	[SerializeField]
	private Quaternion m_Rotation;

	[SerializeField]
	[NativeName("length")]
	private float m_Length;

	[NativeName("parentId")]
	[SerializeField]
	private int m_ParentId;

	[SerializeField]
	[NativeName("color")]
	private Color32 m_Color;

	public string name
	{
		get
		{
			return m_Name;
		}
		set
		{
			m_Name = value;
		}
	}

	public string guid
	{
		get
		{
			return m_Guid;
		}
		set
		{
			m_Guid = value;
		}
	}

	public Vector3 position
	{
		get
		{
			return m_Position;
		}
		set
		{
			m_Position = value;
		}
	}

	public Quaternion rotation
	{
		get
		{
			return m_Rotation;
		}
		set
		{
			m_Rotation = value;
		}
	}

	public float length
	{
		get
		{
			return m_Length;
		}
		set
		{
			m_Length = value;
		}
	}

	public int parentId
	{
		get
		{
			return m_ParentId;
		}
		set
		{
			m_ParentId = value;
		}
	}

	public Color32 color
	{
		get
		{
			return m_Color;
		}
		set
		{
			m_Color = value;
		}
	}
}
