using UnityEngine;

namespace Mirror;

public struct TransformSnapshot : Snapshot
{
	public Vector3 position;

	public Quaternion rotation;

	public Vector3 scale;

	public double remoteTime { get; set; }

	public double localTime { get; set; }

	public TransformSnapshot(double remoteTime, double localTime, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		this.remoteTime = remoteTime;
		this.localTime = localTime;
		this.position = position;
		this.rotation = rotation;
		this.scale = scale;
	}

	public static TransformSnapshot Interpolate(TransformSnapshot from, TransformSnapshot to, double t)
	{
		return new TransformSnapshot(0.0, 0.0, Vector3.LerpUnclamped(from.position, to.position, (float)t), Quaternion.SlerpUnclamped(from.rotation, to.rotation, (float)t), Vector3.LerpUnclamped(from.scale, to.scale, (float)t));
	}
}
