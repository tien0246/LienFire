using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/ Interest Management/ Spatial Hash/Spatial Hashing Interest Management")]
public class SpatialHashingInterestManagement : InterestManagement
{
	public enum CheckMethod
	{
		XZ_FOR_3D = 0,
		XY_FOR_2D = 1
	}

	[Tooltip("The maximum range that objects will be visible at.")]
	public int visRange = 30;

	[Tooltip("Rebuild all every 'rebuildInterval' seconds.")]
	public float rebuildInterval = 1f;

	private double lastRebuildTime;

	[Tooltip("Spatial Hashing supports 3D (XZ) and 2D (XY) games.")]
	public CheckMethod checkMethod;

	public bool showSlider;

	private Grid2D<NetworkConnectionToClient> grid = new Grid2D<NetworkConnectionToClient>(1024);

	public int resolution => visRange / 2;

	private Vector2Int ProjectToGrid(Vector3 position)
	{
		if (checkMethod != CheckMethod.XZ_FOR_3D)
		{
			return Vector2Int.RoundToInt(new Vector2(position.x, position.y) / resolution);
		}
		return Vector2Int.RoundToInt(new Vector2(position.x, position.z) / resolution);
	}

	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		Vector2Int vector2Int = ProjectToGrid(identity.transform.position);
		Vector2Int vector2Int2 = ProjectToGrid(newObserver.identity.transform.position);
		return (vector2Int - vector2Int2).sqrMagnitude <= 2;
	}

	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		Vector2Int position = ProjectToGrid(identity.transform.position);
		grid.GetWithNeighbours(position, newObservers);
	}

	[ServerCallback]
	public override void Reset()
	{
		if (NetworkServer.active)
		{
			lastRebuildTime = 0.0;
		}
	}

	[ServerCallback]
	internal void Update()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		grid.ClearNonAlloc();
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value.isAuthenticated && value.identity != null)
			{
				Vector2Int position = ProjectToGrid(value.identity.transform.position);
				grid.Add(position, value);
			}
		}
		if (NetworkTime.localTime >= lastRebuildTime + (double)rebuildInterval)
		{
			RebuildAll();
			lastRebuildTime = NetworkTime.localTime;
		}
	}
}
