using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mirror.Authenticators;

[AddComponentMenu("Network/ Authenticators/Device Authenticator")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-authenticators/device-authenticator")]
public class DeviceAuthenticator : NetworkAuthenticator
{
	public struct AuthRequestMessage : NetworkMessage
	{
		public string clientDeviceID;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct AuthResponseMessage : NetworkMessage
	{
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

	private void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
	{
		Debug.Log($"connection {conn.connectionId} authenticated with id {msg.clientDeviceID}");
		conn.authenticationData = msg.clientDeviceID;
		conn.Send(default(AuthResponseMessage));
		ServerAccept(conn);
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
		string text = SystemInfo.deviceUniqueIdentifier;
		if (text == "n/a")
		{
			text = PlayerPrefs.GetString("deviceUniqueIdentifier", Guid.NewGuid().ToString());
			PlayerPrefs.SetString("deviceUniqueIdentifier", text);
		}
		NetworkClient.Send(new AuthRequestMessage
		{
			clientDeviceID = text
		});
	}

	public void OnAuthResponseMessage(AuthResponseMessage msg)
	{
		Debug.Log("Authentication Success");
		ClientAccept();
	}
}
