using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/ Interest Management/ Team/Team Interest Management")]
public class TeamInterestManagement : InterestManagement
{
	private readonly Dictionary<string, HashSet<NetworkIdentity>> teamObjects = new Dictionary<string, HashSet<NetworkIdentity>>();

	private readonly Dictionary<NetworkIdentity, string> lastObjectTeam = new Dictionary<NetworkIdentity, string>();

	private readonly HashSet<string> dirtyTeams = new HashSet<string>();

	[ServerCallback]
	public override void OnSpawned(NetworkIdentity identity)
	{
		if (!NetworkServer.active || !identity.TryGetComponent<NetworkTeam>(out var component))
		{
			return;
		}
		string teamId = component.teamId;
		lastObjectTeam[identity] = teamId;
		if (!string.IsNullOrWhiteSpace(teamId))
		{
			if (!teamObjects.TryGetValue(teamId, out var value))
			{
				value = new HashSet<NetworkIdentity>();
				teamObjects.Add(teamId, value);
			}
			value.Add(identity);
			dirtyTeams.Add(teamId);
		}
	}

	[ServerCallback]
	public override void OnDestroyed(NetworkIdentity identity)
	{
		if (NetworkServer.active && lastObjectTeam.TryGetValue(identity, out var value))
		{
			lastObjectTeam.Remove(identity);
			if (!string.IsNullOrWhiteSpace(value) && teamObjects.TryGetValue(value, out var value2) && value2.Remove(identity))
			{
				dirtyTeams.Add(value);
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
			if (value2.TryGetComponent<NetworkTeam>(out var component))
			{
				string teamId = component.teamId;
				if (lastObjectTeam.TryGetValue(value2, out var value) && !string.IsNullOrWhiteSpace(teamId) && !(teamId == value))
				{
					UpdateDirtyTeams(teamId, value);
					UpdateTeamObjects(value2, teamId, value);
				}
			}
		}
		foreach (string dirtyTeam in dirtyTeams)
		{
			RebuildTeamObservers(dirtyTeam);
		}
		dirtyTeams.Clear();
	}

	private void UpdateDirtyTeams(string newTeam, string currentTeam)
	{
		if (!string.IsNullOrWhiteSpace(currentTeam))
		{
			dirtyTeams.Add(currentTeam);
		}
		dirtyTeams.Add(newTeam);
	}

	private void UpdateTeamObjects(NetworkIdentity netIdentity, string newTeam, string currentTeam)
	{
		if (!string.IsNullOrWhiteSpace(currentTeam))
		{
			teamObjects[currentTeam].Remove(netIdentity);
		}
		lastObjectTeam[netIdentity] = newTeam;
		if (!teamObjects.ContainsKey(newTeam))
		{
			teamObjects.Add(newTeam, new HashSet<NetworkIdentity>());
		}
		teamObjects[newTeam].Add(netIdentity);
	}

	private void RebuildTeamObservers(string teamId)
	{
		foreach (NetworkIdentity item in teamObjects[teamId])
		{
			if (item != null)
			{
				NetworkServer.RebuildObservers(item, initialize: false);
			}
		}
	}

	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		if (!identity.TryGetComponent<NetworkTeam>(out var component))
		{
			return true;
		}
		if (component.forceShown)
		{
			return true;
		}
		if (string.IsNullOrWhiteSpace(component.teamId))
		{
			return false;
		}
		if (!newObserver.identity.TryGetComponent<NetworkTeam>(out var component2))
		{
			return true;
		}
		if (string.IsNullOrWhiteSpace(component2.teamId))
		{
			return false;
		}
		return component.teamId == component2.teamId;
	}

	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		if (!identity.TryGetComponent<NetworkTeam>(out var component))
		{
			AddAllConnections(newObservers);
		}
		else if (component.forceShown)
		{
			AddAllConnections(newObservers);
		}
		else
		{
			if (string.IsNullOrWhiteSpace(component.teamId) || !teamObjects.TryGetValue(component.teamId, out var value))
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

	private void AddAllConnections(HashSet<NetworkConnectionToClient> newObservers)
	{
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != null && value.isAuthenticated && value.identity != null)
			{
				newObservers.Add(value);
			}
		}
	}
}
