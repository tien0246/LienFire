using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Authenticators;

[AddComponentMenu("Network/ Authenticators/Basic Authenticator")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-authenticators/basic-authenticator")]
public class BasicAuthenticator : NetworkAuthenticator
{
	public struct AuthRequestMessage : NetworkMessage
	{
		public string authUsername;

		public string authPassword;
	}

	public struct AuthResponseMessage : NetworkMessage
	{
		public byte code;

		public string message;
	}

	[Header("Server Credentials")]
	public string serverUsername;

	public string serverPassword;

	[Header("Client Credentials")]
	public string username;

	public string password;

	private readonly HashSet<NetworkConnection> connectionsPendingDisconnect = new HashSet<NetworkConnection>();

	public override void OnStartServer()
	{
		NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, requireAuthentication: false);
	}

	public override void OnStopServer()
	{
		NetworkServer.UnregisterHandler<AuthRequestMessage>();
	}

	public override void OnServerAuthenticate(NetworkConnectionToClient conn)
	{
	}

	public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
	{
		if (!connectionsPendingDisconnect.Contains(conn))
		{
			if (msg.authUsername == serverUsername && msg.authPassword == serverPassword)
			{
				AuthResponseMessage message = new AuthResponseMessage
				{
					code = 100,
					message = "Success"
				};
				conn.Send(message);
				ServerAccept(conn);
			}
			else
			{
				connectionsPendingDisconnect.Add(conn);
				AuthResponseMessage message2 = new AuthResponseMessage
				{
					code = 200,
					message = "Invalid Credentials"
				};
				conn.Send(message2);
				conn.isAuthenticated = false;
				StartCoroutine(DelayedDisconnect(conn, 1f));
			}
		}
	}

	private IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		ServerReject(conn);
		yield return null;
		connectionsPendingDisconnect.Remove(conn);
	}

	public override void OnStartClient()
	{
		NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, requireAuthentication: false);
	}

	public override void OnStopClient()
	{
		NetworkClient.UnregisterHandler<AuthResponseMessage>();
	}

	public override void OnClientAuthenticate()
	{
		NetworkClient.Send(new AuthRequestMessage
		{
			authUsername = username,
			authPassword = password
		});
	}

	public void OnAuthResponseMessage(AuthResponseMessage msg)
	{
		if (msg.code == 100)
		{
			ClientAccept();
			return;
		}
		Debug.LogError("Authentication Response: " + msg.message);
		ClientReject();
	}
}
