using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/ Interest Management/ Match/Match Interest Management")]
public class MatchInterestManagement : InterestManagement
{
	private readonly Dictionary<Guid, HashSet<NetworkIdentity>> matchObjects = new Dictionary<Guid, HashSet<NetworkIdentity>>();

	private readonly Dictionary<NetworkIdentity, Guid> lastObjectMatch = new Dictionary<NetworkIdentity, Guid>();

	private readonly HashSet<Guid> dirtyMatches = new HashSet<Guid>();

	[ServerCallback]
	public override void OnSpawned(NetworkIdentity identity)
	{
		if (!NetworkServer.active || !identity.TryGetComponent<NetworkMatch>(out var component))
		{
			return;
		}
		Guid matchId = component.matchId;
		lastObjectMatch[identity] = matchId;
		if (!(matchId == Guid.Empty))
		{
			if (!matchObjects.TryGetValue(matchId, out var value))
			{
				value = new HashSet<NetworkIdentity>();
				matchObjects.Add(matchId, value);
			}
			value.Add(identity);
			dirtyMatches.Add(matchId);
		}
	}

	[ServerCallback]
	public override void OnDestroyed(NetworkIdentity identity)
	{
		if (NetworkServer.active && lastObjectMatch.TryGetValue(identity, out var value))
		{
			lastObjectMatch.Remove(identity);
			if (value != Guid.Empty && matchObjects.TryGetValue(value, out var value2) && value2.Remove(identity))
			{
				dirtyMatches.Add(value);
			}
		}
	}

	[ServerCallback]
	internal void Update()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (NetworkIdentity value2 in NetworkServer.spawned.Values)
		{
			if (value2.TryGetComponent<NetworkMatch>(out var component))
			{
				Guid matchId = component.matchId;
				if (lastObjectMatch.TryGetValue(value2, out var value) && !(matchId == Guid.Empty) && !(matchId == value))
				{
					UpdateDirtyMatches(matchId, value);
					UpdateMatchObjects(value2, matchId, value);
				}
			}
		}
		foreach (Guid dirtyMatch in dirtyMatches)
		{
			RebuildMatchObservers(dirtyMatch);
		}
		dirtyMatches.Clear();
	}

	private void UpdateDirtyMatches(Guid newMatch, Guid currentMatch)
	{
		if (currentMatch != Guid.Empty)
		{
			dirtyMatches.Add(currentMatch);
		}
		dirtyMatches.Add(newMatch);
	}

	private void UpdateMatchObjects(NetworkIdentity netIdentity, Guid newMatch, Guid currentMatch)
	{
		if (currentMatch != Guid.Empty)
		{
			matchObjects[currentMatch].Remove(netIdentity);
		}
		lastObjectMatch[netIdentity] = newMatch;
		if (!matchObjects.ContainsKey(newMatch))
		{
			matchObjects.Add(newMatch, new HashSet<NetworkIdentity>());
		}
		matchObjects[newMatch].Add(netIdentity);
	}

	private void RebuildMatchObservers(Guid matchId)
	{
		foreach (NetworkIdentity item in matchObjects[matchId])
		{
			if (item != null)
			{
				NetworkServer.RebuildObservers(item, initialize: false);
			}
		}
	}

	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		if (!identity.TryGetComponent<NetworkMatch>(out var component))
		{
			return false;
		}
		if (component.matchId == Guid.Empty)
		{
			return false;
		}
		if (!newObserver.identity.TryGetComponent<NetworkMatch>(out var component2))
		{
			return false;
		}
		if (component2.matchId == Guid.Empty)
		{
			return false;
		}
		return component.matchId == component2.matchId;
	}

	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		if (!identity.TryGetComponent<NetworkMatch>(out var component))
		{
			return;
		}
		Guid matchId = component.matchId;
		if (matchId == Guid.Empty || !matchObjects.TryGetValue(matchId, out var value))
		{
			return;
		}
		foreach (NetworkIdentity item in value)
		{
			if (item != null && item.connectionToClient != null)
			{
				newObservers.Add(item.connectionToClient);
			}
		}
	}
}
