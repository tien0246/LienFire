using Mirror;
using UnityEngine;

public class PMoveW : NetworkBehaviour
{
	private CharacterController cc;

	public DynamicJoystick joy;

	public int bay;

	[SerializeField]
	private float td;

	[SerializeField]
	private float tdBay;

	private void Start()
	{
		cc = GetComponent<CharacterController>();
	}

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			Move();
		}
	}

	private void Move()
	{
		Vector3 zero = Vector3.zero;
		float horizontal = joy.Horizontal;
		float vertical = joy.Vertical;
		zero = base.transform.TransformDirection(new Vector3(horizontal * td, tdBay * (float)bay, vertical * td));
		cc.Move(zero * Time.deltaTime);
	}

	public void Bay(int i)
	{
		bay = i;
	}

	private void MirrorProcessed()
	{
	}
}
