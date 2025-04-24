using UnityEngine;

namespace Mirror.Examples.SnapshotInterpolationDemo;

public struct Snapshot3D : Snapshot
{
	public Vector3 position;

	public double remoteTime { get; set; }

	public double localTime { get; set; }

	public Snapshot3D(double remoteTime, double localTime, Vector3 position)
	{
		this.remoteTime = remoteTime;
		this.localTime = localTime;
		this.position = position;
	}

	public static Snapshot3D Interpolate(Snapshot3D from, Snapshot3D to, double t)
	{
		return new Snapshot3D(0.0, 0.0, Vector3.LerpUnclamped(from.position, to.position, (float)t));
	}
}
