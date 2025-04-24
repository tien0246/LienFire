using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[HelpURL("https://mirror-networking.gitbook.io/docs/guides/interest-management")]
public abstract class InterestManagementBase : MonoBehaviour
{
	protected virtual void OnEnable()
	{
		if (NetworkServer.aoi == null)
		{
			NetworkServer.aoi = this;
		}
		else
		{
			Debug.LogError($"Only one InterestManagement component allowed. {NetworkServer.aoi.GetType()} has been set up already.");
		}
		if (NetworkClient.aoi == null)
		{
			NetworkClient.aoi = this;
		}
		else
		{
			Debug.LogError($"Only one InterestManagement component allowed. {NetworkClient.aoi.GetType()} has been set up already.");
		}
	}

	[ServerCallback]
	public virtual void Reset()
	{
		if (NetworkServer.active)
		{
		}
	}

	public abstract bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver);

	[ServerCallback]
	public virtual void SetHostVisibility(NetworkIdentity identity, bool visible)
	{
		if (NetworkServer.active)
		{
			Renderer[] componentsInChildren = identity.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = visible;
			}
		}
	}

	[ServerCallback]
	public virtual void OnSpawned(NetworkIdentity identity)
	{
		if (NetworkServer.active)
		{
		}
	}

	[ServerCallback]
	public virtual void OnDestroyed(NetworkIdentity identity)
	{
		if (NetworkServer.active)
		{
		}
	}

	public abstract void Rebuild(NetworkIdentity identity, bool initialize);

	protected void AddObserver(NetworkConnectionToClient connection, NetworkIdentity identity)
	{
		connection.AddToObserving(identity);
		identity.observers.Add(connection.connectionId, connection);
	}

	protected void RemoveObserver(NetworkConnectionToClient connection, NetworkIdentity identity)
	{
		connection.RemoveFromObserving(identity, isDestroyed: false);
		identity.observers.Remove(connection.connectionId);
	}
}
