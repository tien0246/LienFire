using System.Runtime.InteropServices;
using UnityEngine;

namespace Mirror.Examples.AdditiveScenes;

public class ShootingTankBehaviour : NetworkBehaviour
{
	[SyncVar]
	public Quaternion rotation;

	private NetworkAnimator networkAnimator;

	[Range(0f, 1f)]
	public float turnSpeed = 0.1f;

	public Quaternion Networkrotation
	{
		get
		{
			return rotation;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref rotation, 1uL, null);
		}
	}

	[ServerCallback]
	private void Start()
	{
		if (NetworkServer.active)
		{
			networkAnimator = GetComponent<NetworkAnimator>();
		}
	}

	private void Update()
	{
		if (base.isServer && base.netIdentity.observers.Count > 0)
		{
			ShootNearestPlayer();
		}
		if (base.isClient)
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation, turnSpeed);
		}
	}

	[Server]
	private void ShootNearestPlayer()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Mirror.Examples.AdditiveScenes.ShootingTankBehaviour::ShootNearestPlayer()' called when server was not active");
			return;
		}
		GameObject gameObject = null;
		float num = 100f;
		foreach (NetworkConnectionToClient value in base.netIdentity.observers.Values)
		{
			GameObject gameObject2 = value.identity.gameObject;
			float num2 = Vector3.Distance(gameObject2.transform.position, base.transform.position);
			if (gameObject == null || num > num2)
			{
				gameObject = gameObject2;
				num = num2;
			}
		}
		if (gameObject != null)
		{
			base.transform.LookAt(gameObject.transform.position + Vector3.down);
			Networkrotation = base.transform.rotation;
			networkAnimator.SetTrigger("Fire");
		}
	}

	private void MirrorProcessed()
	{
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteQuaternion(rotation);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteQuaternion(rotation);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref rotation, null, reader.ReadQuaternion());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref rotation, null, reader.ReadQuaternion());
		}
	}
}
