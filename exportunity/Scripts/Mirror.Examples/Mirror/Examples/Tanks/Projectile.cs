using UnityEngine;

namespace Mirror.Examples.Tanks;

public class Projectile : NetworkBehaviour
{
	public float destroyAfter = 2f;

	public Rigidbody rigidBody;

	public float force = 1000f;

	public override void OnStartServer()
	{
		Invoke("DestroySelf", destroyAfter);
	}

	private void Start()
	{
		rigidBody.AddForce(base.transform.forward * force);
	}

	[Server]
	private void DestroySelf()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Mirror.Examples.Tanks.Projectile::DestroySelf()' called when server was not active");
		}
		else
		{
			NetworkServer.Destroy(base.gameObject);
		}
	}

	[ServerCallback]
	private void OnTriggerEnter(Collider co)
	{
		if (NetworkServer.active)
		{
			DestroySelf();
		}
	}

	private void MirrorProcessed()
	{
	}
}
