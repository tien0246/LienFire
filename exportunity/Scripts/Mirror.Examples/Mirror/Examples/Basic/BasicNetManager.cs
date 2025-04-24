using UnityEngine;

namespace Mirror.Examples.Basic;

[AddComponentMenu("")]
public class BasicNetManager : NetworkManager
{
	public new static BasicNetManager singleton { get; private set; }

	public override void Awake()
	{
		base.Awake();
		singleton = this;
	}

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		base.OnServerAddPlayer(conn);
		Player.ResetPlayerNumbers();
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		base.OnServerDisconnect(conn);
		Player.ResetPlayerNumbers();
	}
}
