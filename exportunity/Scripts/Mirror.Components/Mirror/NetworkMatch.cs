using System;
using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[AddComponentMenu("Network/ Interest Management/ Match/Network Match")]
[HelpURL("https://mirror-networking.gitbook.io/docs/guides/interest-management")]
public class NetworkMatch : NetworkBehaviour
{
	public Guid matchId;

	private void MirrorProcessed()
	{
	}
}
