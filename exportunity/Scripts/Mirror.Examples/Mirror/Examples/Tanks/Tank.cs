using System.Runtime.InteropServices;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.AI;

namespace Mirror.Examples.Tanks;

public class Tank : NetworkBehaviour
{
	[Header("Components")]
	public NavMeshAgent agent;

	public Animator animator;

	public TextMesh healthBar;

	public Transform turret;

	[Header("Movement")]
	public float rotationSpeed = 100f;

	[Header("Firing")]
	public KeyCode shootKey = KeyCode.Space;

	public GameObject projectilePrefab;

	public Transform projectileMount;

	[Header("Stats")]
	[SyncVar]
	public int health = 4;

	public int Networkhealth
	{
		get
		{
			return health;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref health, 1uL, null);
		}
	}

	private void Update()
	{
		healthBar.text = new string('-', health);
		if (Application.isFocused && base.isLocalPlayer)
		{
			float axis = Input.GetAxis("Horizontal");
			base.transform.Rotate(0f, axis * rotationSpeed * Time.deltaTime, 0f);
			float axis2 = Input.GetAxis("Vertical");
			Vector3 vector = base.transform.TransformDirection(Vector3.forward);
			agent.velocity = vector * Mathf.Max(axis2, 0f) * agent.speed;
			animator.SetBool("Moving", agent.velocity != Vector3.zero);
			if (Input.GetKeyDown(shootKey))
			{
				CmdFire();
			}
			RotateTurret();
		}
	}

	[Command]
	private void CmdFire()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Mirror.Examples.Tanks.Tank::CmdFire()", 1163962920, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcOnFire()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Mirror.Examples.Tanks.Tank::RpcOnFire()", -695433646, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active && other.GetComponent<Projectile>() != null)
		{
			Networkhealth = health - 1;
			if (health == 0)
			{
				NetworkServer.Destroy(base.gameObject);
			}
		}
	}

	private void RotateTurret()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out var hitInfo, 100f))
		{
			Debug.DrawLine(ray.origin, hitInfo.point);
			Vector3 worldPosition = new Vector3(hitInfo.point.x, turret.transform.position.y, hitInfo.point.z);
			turret.transform.LookAt(worldPosition);
		}
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdFire()
	{
		if (Physics.Raycast(projectileMount.position, projectileMount.forward, out var hitInfo, 10f))
		{
			Tank component = hitInfo.transform.GetComponent<Tank>();
			if (component != null)
			{
				component.Networkhealth = component.health - 1;
			}
		}
		RpcOnFire();
	}

	protected static void InvokeUserCode_CmdFire(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdFire called on client.");
		}
		else
		{
			((Tank)obj).UserCode_CmdFire();
		}
	}

	protected void UserCode_RpcOnFire()
	{
		animator.SetTrigger("Shoot");
	}

	protected static void InvokeUserCode_RpcOnFire(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcOnFire called on server.");
		}
		else
		{
			((Tank)obj).UserCode_RpcOnFire();
		}
	}

	static Tank()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Tank), "System.Void Mirror.Examples.Tanks.Tank::CmdFire()", InvokeUserCode_CmdFire, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(Tank), "System.Void Mirror.Examples.Tanks.Tank::RpcOnFire()", InvokeUserCode_RpcOnFire);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(health);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(health);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref health, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref health, null, reader.ReadInt());
		}
	}
}
