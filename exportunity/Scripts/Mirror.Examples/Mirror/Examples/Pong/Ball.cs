using UnityEngine;

namespace Mirror.Examples.Pong;

public class Ball : NetworkBehaviour
{
	public float speed = 30f;

	public Rigidbody2D rigidbody2d;

	public override void OnStartServer()
	{
		base.OnStartServer();
		rigidbody2d.simulated = true;
		rigidbody2d.velocity = Vector2.right * speed;
	}

	private float HitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight)
	{
		return (ballPos.y - racketPos.y) / racketHeight;
	}

	[ServerCallback]
	private void OnCollisionEnter2D(Collision2D col)
	{
		if (NetworkServer.active && (bool)col.transform.GetComponent<Player>())
		{
			float y = HitFactor(base.transform.position, col.transform.position, col.collider.bounds.size.y);
			Vector2 normalized = new Vector2((col.relativeVelocity.x > 0f) ? 1 : (-1), y).normalized;
			rigidbody2d.velocity = normalized * speed;
		}
	}

	private void MirrorProcessed()
	{
	}
}
