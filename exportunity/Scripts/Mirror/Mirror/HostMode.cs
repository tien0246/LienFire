using System;

namespace Mirror;

public static class HostMode
{
	internal static void SetupConnections()
	{
		Utils.CreateLocalConnections(out var connectionToClient, out var connectionToServer);
		NetworkClient.connection = connectionToServer;
		NetworkServer.SetLocalConnection(connectionToClient);
	}

	public static void InvokeOnConnected()
	{
		NetworkServer.OnConnected(NetworkServer.localConnection);
		((LocalConnectionToServer)NetworkClient.connection).QueueConnectedEvent();
	}

	[Obsolete("ActivateHostScene did nothing, since identities all had .isClient set in NetworkServer.SpawnObjects.")]
	public static void ActivateHostScene()
	{
	}
}
