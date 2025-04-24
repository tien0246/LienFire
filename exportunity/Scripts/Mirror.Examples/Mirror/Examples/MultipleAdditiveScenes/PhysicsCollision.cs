using UnityEngine;

namespace Mirror.Examples.MultipleAdditiveScenes;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsCollision : NetworkBehaviour
{
	[Tooltip("how forcefully to push this object")]
	public float force = 12f;

	public Rigidbody rigidbody3D;

	private void OnValidate()
	{
		if (rigidbody3D == null)
		{
			rigidbody3D = GetComponent<Rigidbody>();
		}
	}

	private void Start()
	{
		rigidbody3D.isKinematic = !base.isServer;
	}

	[ServerCallback]
	private void OnCollisionStay(Collision other)
	{
		if (NetworkServer.active && other.gameObject.CompareTag("Player"))
		{
			Vector3 normal = other.contacts[0].normal;
			normal.y = 0f;
			normal = normal.normalized;
			if (other.gameObject.GetComponent<NetworkIdentity>().connectionToClient.connectionId == 0)
			{
				rigidbody3D.AddForce(normal * force * 0.5f);
			}
			else
			{
				rigidbody3D.AddForce(normal * force);
			}
		}
	}

	private void MirrorProcessed()
	{
	}
}
