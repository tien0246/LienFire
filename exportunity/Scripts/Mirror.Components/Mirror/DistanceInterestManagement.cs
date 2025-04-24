using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/ Interest Management/ Distance/Distance Interest Management")]
public class DistanceInterestManagement : InterestManagement
{
	[Tooltip("The maximum range that objects will be visible at. Add DistanceInterestManagementCustomRange onto NetworkIdentities for custom ranges.")]
	public int visRange = 500;

	[Tooltip("Rebuild all every 'rebuildInterval' seconds.")]
	public float rebuildInterval = 1f;

	private double lastRebuildTime;

	private readonly Dictionary<NetworkIdentity, DistanceInterestManagementCustomRange> CustomRanges = new Dictionary<NetworkIdentity, DistanceInterestManagementCustomRange>();

	[ServerCallback]
	private int GetVisRange(NetworkIdentity identity)
	{
		if (!NetworkServer.active)
		{
			return default(int);
		}
		if (!CustomRanges.TryGetValue(identity, out var value))
		{
			return visRange;
		}
		return value.visRange;
	}

	[ServerCallback]
	public override void Reset()
	{
		if (NetworkServer.active)
		{
			lastRebuildTime = 0.0;
			CustomRanges.Clear();
		}
	}

	public override void OnSpawned(NetworkIdentity identity)
	{
		if (identity.TryGetComponent<DistanceInterestManagementCustomRange>(out var component))
		{
			CustomRanges[identity] = component;
		}
	}

	public override void OnDestroyed(NetworkIdentity identity)
	{
		CustomRanges.Remove(identity);
	}

	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		int num = GetVisRange(identity);
		return Vector3.Distance(identity.transform.position, newObserver.identity.transform.position) < (float)num;
	}

	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		int num = GetVisRange(identity);
		Vector3 position = identity.transform.position;
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != null && value.isAuthenticated && value.identity != null && Vector3.Distance(value.identity.transform.position, position) < (float)num)
			{
				newObservers.Add(value);
			}
		}
	}

	[ServerCallback]
	internal void Update()
	{
		if (NetworkServer.active && NetworkTime.localTime >= lastRebuildTime + (double)rebuildInterval)
		{
			RebuildAll();
			lastRebuildTime = NetworkTime.localTime;
		}
	}
}
