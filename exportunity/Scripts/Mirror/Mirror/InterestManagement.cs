using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[HelpURL("https://mirror-networking.gitbook.io/docs/guides/interest-management")]
public abstract class InterestManagement : InterestManagementBase
{
	private readonly HashSet<NetworkConnectionToClient> newObservers = new HashSet<NetworkConnectionToClient>();

	public abstract void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers);

	[ServerCallback]
	protected void RebuildAll()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (NetworkIdentity value in NetworkServer.spawned.Values)
		{
			NetworkServer.RebuildObservers(value, initialize: false);
		}
	}

	public override void Rebuild(NetworkIdentity identity, bool initialize)
	{
		newObservers.Clear();
		if (identity.visible != Visibility.ForceHidden)
		{
			OnRebuildObservers(identity, newObservers);
		}
		if (identity.connectionToClient != null)
		{
			newObservers.Add(identity.connectionToClient);
		}
		bool flag = false;
		foreach (NetworkConnectionToClient newObserver in newObservers)
		{
			if (newObserver != null && newObserver.isReady && (initialize || !identity.observers.ContainsKey(newObserver.connectionId)))
			{
				newObserver.AddToObserving(identity);
				flag = true;
			}
		}
		foreach (NetworkConnectionToClient value in identity.observers.Values)
		{
			if (!newObservers.Contains(value))
			{
				value.RemoveFromObserving(identity, isDestroyed: false);
				flag = true;
			}
		}
		if (flag)
		{
			identity.observers.Clear();
			foreach (NetworkConnectionToClient newObserver2 in newObservers)
			{
				if (newObserver2 != null && newObserver2.isReady)
				{
					identity.observers.Add(newObserver2.connectionId, newObserver2);
				}
			}
		}
		if (initialize && !newObservers.Contains(NetworkServer.localConnection))
		{
			SetHostVisibility(identity, visible: false);
		}
	}
}
