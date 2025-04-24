using System;
using UnityEngine;

namespace Mirror.Examples.CCU;

public class Player : NetworkBehaviour
{
	public Vector3 cameraOffset = new Vector3(0f, 40f, -40f);

	[Header("Automated Movement")]
	public bool autoMove = true;

	public float autoSpeed = 2f;

	public float movementProbability = 0.5f;

	public float movementDistance = 20f;

	private bool moving;

	private Vector3 start;

	private Vector3 destination;

	[Header("Manual Movement")]
	public float manualSpeed = 10f;

	private Transform tf;

	public override void OnStartLocalPlayer()
	{
		tf = base.transform;
		start = tf.position;
		Camera.main.transform.SetParent(base.transform, worldPositionStays: false);
		Camera.main.transform.localPosition = cameraOffset;
	}

	public override void OnStopLocalPlayer()
	{
		Camera.main.transform.SetParent(null, worldPositionStays: true);
	}

	private void AutoMove()
	{
		if (moving)
		{
			if (Vector3.Distance(tf.position, destination) <= 0.01f)
			{
				moving = false;
			}
			else
			{
				tf.position = Vector3.MoveTowards(tf.position, destination, autoSpeed * Time.deltaTime);
			}
		}
		else if (UnityEngine.Random.value < movementProbability * Time.deltaTime)
		{
			float x = Mathf.Cos(UnityEngine.Random.value * MathF.PI);
			float y = Mathf.Sin(UnityEngine.Random.value * MathF.PI);
			Vector2 vector = new Vector2(x, y);
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.y);
			destination = start + vector2 * movementDistance;
			moving = true;
		}
	}

	private void ManualMove()
	{
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		Vector3 vector = new Vector3(axis, 0f, axis2);
		base.transform.position += vector.normalized * (Time.deltaTime * manualSpeed);
	}

	private static bool Interrupted()
	{
		if (Input.GetAxis("Horizontal") == 0f)
		{
			return Input.GetAxis("Vertical") != 0f;
		}
		return true;
	}

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			if (Interrupted())
			{
				autoMove = false;
			}
			if (autoMove)
			{
				AutoMove();
			}
			else
			{
				ManualMove();
			}
		}
	}

	private void MirrorProcessed()
	{
	}
}
