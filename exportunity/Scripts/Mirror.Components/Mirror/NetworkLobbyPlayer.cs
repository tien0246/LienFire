using System;
using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[AddComponentMenu("Network/Network Lobby Player")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-room-player")]
[Obsolete("Use / inherit from NetworkRoomPlayer instead")]
public class NetworkLobbyPlayer : NetworkRoomPlayer
{
	private void MirrorProcessed()
	{
	}
}
