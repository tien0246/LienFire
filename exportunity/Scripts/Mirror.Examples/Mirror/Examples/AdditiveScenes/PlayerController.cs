using UnityEngine;

namespace Mirror.Examples.AdditiveScenes;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
	public enum GroundState : byte
	{
		Jumping = 0,
		Falling = 1,
		Grounded = 2
	}

	[Header("Avatar Components")]
	public CharacterController characterController;

	[Header("Movement")]
	[Range(1f, 20f)]
	public float moveSpeedMultiplier = 8f;

	[Header("Turning")]
	[Range(1f, 200f)]
	public float maxTurnSpeed = 100f;

	[Range(0.5f, 5f)]
	public float turnDelta = 3f;

	[Header("Jumping")]
	[Range(0.1f, 1f)]
	public float initialJumpSpeed = 0.2f;

	[Range(1f, 10f)]
	public float maxJumpSpeed = 5f;

	[Range(0.1f, 1f)]
	public float jumpDelta = 0.2f;

	[Header("Diagnostics - Do Not Modify")]
	public GroundState groundState = GroundState.Grounded;

	[Range(-1f, 1f)]
	public float horizontal;

	[Range(-1f, 1f)]
	public float vertical;

	[Range(-200f, 200f)]
	public float turnSpeed;

	[Range(-10f, 10f)]
	public float jumpSpeed;

	[Range(-1.5f, 1.5f)]
	public float animVelocity;

	[Range(-1.5f, 1.5f)]
	public float animRotation;

	public Vector3Int velocity;

	public Vector3 direction;

	private void OnValidate()
	{
		if (characterController == null)
		{
			characterController = GetComponent<CharacterController>();
		}
		characterController.enabled = false;
		characterController.skinWidth = 0.02f;
		characterController.minMoveDistance = 0f;
		GetComponent<Rigidbody>().isKinematic = true;
		base.enabled = false;
	}

	public override void OnStartAuthority()
	{
		characterController.enabled = true;
		base.enabled = true;
	}

	public override void OnStopAuthority()
	{
		base.enabled = false;
		characterController.enabled = false;
	}

	private void Update()
	{
		if (characterController.enabled)
		{
			HandleTurning();
			HandleJumping();
			HandleMove();
			if (characterController.isGrounded)
			{
				groundState = GroundState.Grounded;
			}
			else if (groundState != GroundState.Jumping)
			{
				groundState = GroundState.Falling;
			}
			velocity = Vector3Int.FloorToInt(characterController.velocity);
		}
	}

	private void HandleTurning()
	{
		if (Input.GetKey(KeyCode.Q))
		{
			turnSpeed = Mathf.MoveTowards(turnSpeed, 0f - maxTurnSpeed, turnDelta);
		}
		if (Input.GetKey(KeyCode.E))
		{
			turnSpeed = Mathf.MoveTowards(turnSpeed, maxTurnSpeed, turnDelta);
		}
		if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E))
		{
			turnSpeed = Mathf.MoveTowards(turnSpeed, 0f, turnDelta);
		}
		if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
		{
			turnSpeed = Mathf.MoveTowards(turnSpeed, 0f, turnDelta);
		}
		base.transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f);
	}

	private void HandleJumping()
	{
		if (groundState != GroundState.Falling && Input.GetKey(KeyCode.Space))
		{
			if (groundState != GroundState.Jumping)
			{
				groundState = GroundState.Jumping;
				jumpSpeed = initialJumpSpeed;
			}
			else
			{
				jumpSpeed = Mathf.MoveTowards(jumpSpeed, maxJumpSpeed, jumpDelta);
			}
			if (jumpSpeed == maxJumpSpeed)
			{
				groundState = GroundState.Falling;
			}
		}
		else if (groundState != GroundState.Grounded)
		{
			groundState = GroundState.Falling;
			jumpSpeed = Mathf.Min(jumpSpeed, maxJumpSpeed);
			jumpSpeed += Physics.gravity.y * Time.deltaTime;
		}
		else
		{
			jumpSpeed = Physics.gravity.y * Time.deltaTime;
		}
	}

	private void HandleMove()
	{
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");
		direction = new Vector3(horizontal, 0f, vertical);
		direction = Vector3.ClampMagnitude(direction, 1f);
		direction = base.transform.TransformDirection(direction);
		direction *= moveSpeedMultiplier;
		direction.y = jumpSpeed;
		characterController.Move(direction * Time.deltaTime);
	}

	private void MirrorProcessed()
	{
	}
}
