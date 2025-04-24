using System;
using System.ComponentModel;

namespace UnityEngine;

[Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
[EditorBrowsable(EditorBrowsableState.Never)]
public enum ProceduralPropertyType
{
	Boolean = 0,
	Float = 1,
	Vector2 = 2,
	Vector3 = 3,
	Vector4 = 4,
	Color3 = 5,
	Color4 = 6,
	Enum = 7,
	Texture = 8,
	String = 9
}
