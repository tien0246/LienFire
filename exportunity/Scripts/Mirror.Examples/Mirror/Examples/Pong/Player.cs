using UnityEngine;

namespace Mirror.Examples.Pong;

public class Player : NetworkBehaviour
{
	public float speed = 30f;

	public Rigidbody2D rigidbody2d;

	private void FixedUpdate()
	{
		if (base.isLocalPlayer)
		{
			rigidbody2d.velocity = new Vector2(0f, Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
		}
	}

	private void MirrorProcessed()
	{
	}
}
