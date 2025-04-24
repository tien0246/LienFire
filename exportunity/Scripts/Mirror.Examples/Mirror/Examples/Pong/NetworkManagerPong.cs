using UnityEngine;

namespace Mirror.Examples.Pong;

[AddComponentMenu("")]
public class NetworkManagerPong : NetworkManager
{
	public Transform leftRacketSpawn;

	public Transform rightRacketSpawn;

	private GameObject ball;

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		Transform transform = ((base.numPlayers == 0) ? leftRacketSpawn : rightRacketSpawn);
		GameObject player = Object.Instantiate(playerPrefab, transform.position, transform.rotation);
		NetworkServer.AddPlayerForConnection(conn, player);
		if (base.numPlayers == 2)
		{
			ball = Object.Instantiate(spawnPrefabs.Find((GameObject prefab) => prefab.name == "Ball"));
			NetworkServer.Spawn(ball);
		}
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		if (ball != null)
		{
			NetworkServer.Destroy(ball);
		}
		base.OnServerDisconnect(conn);
	}
}
