using UnityEngine;

namespace Mirror.Examples.CCU;

public class Monster : NetworkBehaviour
{
	public float speed = 1f;

	public float movementProbability = 0.5f;

	public float movementDistance = 20f;

	private bool moving;

	private Vector3 start;

	private Vector3 destination;

	private Transform tf;

	public override void OnStartServer()
	{
		tf = base.transform;
		start = tf.position;
	}

	[ServerCallback]
	private void Update()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (moving)
		{
			if (Vector3.Distance(tf.position, destination) <= 0.01f)
			{
				moving = false;
			}
			else
			{
				tf.position = Vector3.MoveTowards(tf.position, destination, speed * Time.deltaTime);
			}
		}
		else if (Random.value < movementProbability * Time.deltaTime)
		{
			Vector2 insideUnitCircle = Random.insideUnitCircle;
			Vector3 vector = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
			destination = start + vector * movementDistance;
			moving = true;
		}
	}

	private void MirrorProcessed()
	{
	}
}
