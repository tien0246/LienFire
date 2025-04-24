using System.Collections;
using UnityEngine;

namespace Mirror.Authenticators;

[AddComponentMenu("Network/ Authenticators/Timeout Authenticator")]
public class TimeoutAuthenticator : NetworkAuthenticator
{
	public NetworkAuthenticator authenticator;

	[Range(0f, 600f)]
	[Tooltip("Timeout to auto-disconnect in seconds. Set to 0 for no timeout.")]
	public float timeout = 60f;

	public void Awake()
	{
		authenticator.OnServerAuthenticated.AddListener(delegate(NetworkConnectionToClient connection)
		{
			OnServerAuthenticated.Invoke(connection);
		});
		authenticator.OnClientAuthenticated.AddListener(OnClientAuthenticated.Invoke);
	}

	public override void OnStartServer()
	{
		authenticator.OnStartServer();
	}

	public override void OnStopServer()
	{
		authenticator.OnStopServer();
	}

	public override void OnStartClient()
	{
		authenticator.OnStartClient();
	}

	public override void OnStopClient()
	{
		authenticator.OnStopClient();
	}

	public override void OnServerAuthenticate(NetworkConnectionToClient conn)
	{
		authenticator.OnServerAuthenticate(conn);
		if (timeout > 0f)
		{
			StartCoroutine(BeginAuthentication(conn));
		}
	}

	public override void OnClientAuthenticate()
	{
		authenticator.OnClientAuthenticate();
		if (timeout > 0f)
		{
			StartCoroutine(BeginAuthentication(NetworkClient.connection));
		}
	}

	private IEnumerator BeginAuthentication(NetworkConnection conn)
	{
		yield return new WaitForSecondsRealtime(timeout);
		if (!conn.isAuthenticated)
		{
			Debug.LogError($"Authentication Timeout - Disconnecting {conn}");
			conn.Disconnect();
		}
	}
}
