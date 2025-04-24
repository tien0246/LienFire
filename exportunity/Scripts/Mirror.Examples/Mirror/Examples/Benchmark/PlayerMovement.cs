using UnityEngine;

namespace Mirror.Examples.Benchmark;

public class PlayerMovement : NetworkBehaviour
{
	public float speed = 5f;

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			Vector3 vector = new Vector3(axis, 0f, axis2);
			base.transform.position += vector.normalized * (Time.deltaTime * speed);
		}
	}

	private void MirrorProcessed()
	{
	}
}
