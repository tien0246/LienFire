using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Examples.Chat;

[AddComponentMenu("")]
public class ChatAuthenticator : NetworkAuthenticator
{
	public struct AuthRequestMessage : NetworkMessage
	{
		public string authUsername;
	}

	public struct AuthResponseMessage : NetworkMessage
	{
		public byte code;

		public string message;
	}

	private readonly HashSet<NetworkConnection> connectionsPendingDisconnect = new HashSet<NetworkConnection>();

	internal static readonly HashSet<string> playerNames = new HashSet<string>();

	[Header("Client Username")]
	public string playerName;

	[RuntimeInitializeOnLoadMethod]
	private static void ResetStatics()
	{
		playerNames.Clear();
	}

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
		Debug.Log("Authentication Request: " + msg.authUsername);
		if (!connectionsPendingDisconnect.Contains(conn))
		{
			if (!playerNames.Contains(msg.authUsername))
			{
				playerNames.Add(msg.authUsername);
				conn.authenticationData = msg.authUsername;
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
					message = "Username already in use...try again"
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

	public void SetPlayername(string username)
	{
		playerName = username;
		LoginUI.instance.errorText.text = string.Empty;
		LoginUI.instance.errorText.gameObject.SetActive(value: false);
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
			authUsername = playerName
		});
	}

	public void OnAuthResponseMessage(AuthResponseMessage msg)
	{
		if (msg.code == 100)
		{
			Debug.Log($"Authentication Response: {msg.code} {msg.message}");
			ClientAccept();
			return;
		}
		Debug.LogError($"Authentication Response: {msg.code} {msg.message}");
		NetworkManager.singleton.StopHost();
		LoginUI.instance.errorText.text = msg.message;
		LoginUI.instance.errorText.gameObject.SetActive(value: true);
	}
}
