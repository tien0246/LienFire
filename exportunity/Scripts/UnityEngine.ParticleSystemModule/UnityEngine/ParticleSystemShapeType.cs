using System;

namespace UnityEngine;

public enum ParticleSystemShapeType
{
	Sphere = 0,
	[Obsolete("SphereShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.", false)]
	SphereShell = 1,
	Hemisphere = 2,
	[Obsolete("HemisphereShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.", false)]
	HemisphereShell = 3,
	Cone = 4,
	Box = 5,
	Mesh = 6,
	[Obsolete("ConeShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.", false)]
	ConeShell = 7,
	ConeVolume = 8,
	[Obsolete("ConeVolumeShell is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.", false)]
	ConeVolumeShell = 9,
	Circle = 10,
	[Obsolete("CircleEdge is deprecated and does nothing. Please use ShapeModule.radiusThickness instead, to control edge emission.", false)]
	CircleEdge = 11,
	SingleSidedEdge = 12,
	MeshRenderer = 13,
	SkinnedMeshRenderer = 14,
	BoxShell = 15,
	BoxEdge = 16,
	Donut = 17,
	Rectangle = 18,
	Sprite = 19,
	SpriteRenderer = 20
}
