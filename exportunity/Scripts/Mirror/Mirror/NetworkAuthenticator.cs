using UnityEngine;
using UnityEngine.Events;

namespace Mirror;

[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-authenticators")]
public abstract class NetworkAuthenticator : MonoBehaviour
{
	[Header("Event Listeners (optional)")]
	[Tooltip("Mirror has an internal subscriber to this event. You can add your own here.")]
	public UnityEventNetworkConnection OnServerAuthenticated = new UnityEventNetworkConnection();

	[Tooltip("Mirror has an internal subscriber to this event. You can add your own here.")]
	public UnityEvent OnClientAuthenticated = new UnityEvent();

	public virtual void OnStartServer()
	{
	}

	public virtual void OnStopServer()
	{
	}

	public virtual void OnServerAuthenticate(NetworkConnectionToClient conn)
	{
	}

	protected void ServerAccept(NetworkConnectionToClient conn)
	{
		OnServerAuthenticated.Invoke(conn);
	}

	protected void ServerReject(NetworkConnectionToClient conn)
	{
		conn.Disconnect();
	}

	public virtual void OnStartClient()
	{
	}

	public virtual void OnStopClient()
	{
	}

	public virtual void OnClientAuthenticate()
	{
	}

	protected void ClientAccept()
	{
		OnClientAuthenticated.Invoke();
	}

	protected void ClientReject()
	{
		NetworkClient.connection.isAuthenticated = false;
		NetworkClient.connection.Disconnect();
	}

	private void Reset()
	{
	}
}
