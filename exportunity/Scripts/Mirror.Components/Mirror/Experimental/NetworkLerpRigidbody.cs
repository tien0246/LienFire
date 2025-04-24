using System.Runtime.InteropServices;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror.Experimental;

[AddComponentMenu("Network/ Experimental/Network Lerp Rigidbody")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-lerp-rigidbody")]
public class NetworkLerpRigidbody : NetworkBehaviour
{
	[Header("Settings")]
	[SerializeField]
	internal Rigidbody target;

	[Tooltip("How quickly current velocity approaches target velocity")]
	[SerializeField]
	private float lerpVelocityAmount = 0.5f;

	[Tooltip("How quickly current position approaches target position")]
	[SerializeField]
	private float lerpPositionAmount = 0.5f;

	[Tooltip("Set to true if moves come from owner client, set to false if moves always come from server")]
	[SerializeField]
	private bool clientAuthority;

	private double nextSyncTime;

	[SyncVar]
	private Vector3 targetVelocity;

	[SyncVar]
	private Vector3 targetPosition;

	private bool IgnoreSync
	{
		get
		{
			if (!base.isServer)
			{
				return ClientWithAuthority;
			}
			return true;
		}
	}

	private bool ClientWithAuthority
	{
		get
		{
			if (clientAuthority)
			{
				return base.isOwned;
			}
			return false;
		}
	}

	public Vector3 NetworktargetVelocity
	{
		get
		{
			return targetVelocity;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref targetVelocity, 1uL, null);
		}
	}

	public Vector3 NetworktargetPosition
	{
		get
		{
			return targetPosition;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref targetPosition, 2uL, null);
		}
	}

	private void OnValidate()
	{
		if (target == null)
		{
			target = GetComponent<Rigidbody>();
		}
	}

	private void Update()
	{
		if (base.isServer)
		{
			SyncToClients();
		}
		else if (ClientWithAuthority)
		{
			SendToServer();
		}
	}

	private void SyncToClients()
	{
		NetworktargetVelocity = target.velocity;
		NetworktargetPosition = target.position;
	}

	private void SendToServer()
	{
		double localTime = NetworkTime.localTime;
		if (localTime > nextSyncTime)
		{
			nextSyncTime = localTime + (double)syncInterval;
			CmdSendState(target.velocity, target.position);
		}
	}

	[Command]
	private void CmdSendState(Vector3 velocity, Vector3 position)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(velocity);
		writer.WriteVector3(position);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkLerpRigidbody::CmdSendState(UnityEngine.Vector3,UnityEngine.Vector3)", 222547705, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void FixedUpdate()
	{
		if (!IgnoreSync)
		{
			target.velocity = Vector3.Lerp(target.velocity, targetVelocity, lerpVelocityAmount);
			target.position = Vector3.Lerp(target.position, targetPosition, lerpPositionAmount);
			target.position += target.velocity * Time.fixedDeltaTime;
		}
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdSendState__Vector3__Vector3(Vector3 velocity, Vector3 position)
	{
		target.velocity = velocity;
		target.position = position;
		NetworktargetVelocity = velocity;
		NetworktargetPosition = position;
	}

	protected static void InvokeUserCode_CmdSendState__Vector3__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendState called on client.");
		}
		else
		{
			((NetworkLerpRigidbody)obj).UserCode_CmdSendState__Vector3__Vector3(reader.ReadVector3(), reader.ReadVector3());
		}
	}

	static NetworkLerpRigidbody()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkLerpRigidbody), "System.Void Mirror.Experimental.NetworkLerpRigidbody::CmdSendState(UnityEngine.Vector3,UnityEngine.Vector3)", InvokeUserCode_CmdSendState__Vector3__Vector3, requiresAuthority: true);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteVector3(targetVelocity);
			writer.WriteVector3(targetPosition);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVector3(targetVelocity);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteVector3(targetPosition);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref targetVelocity, null, reader.ReadVector3());
			GeneratedSyncVarDeserialize(ref targetPosition, null, reader.ReadVector3());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref targetVelocity, null, reader.ReadVector3());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref targetPosition, null, reader.ReadVector3());
		}
	}
}
