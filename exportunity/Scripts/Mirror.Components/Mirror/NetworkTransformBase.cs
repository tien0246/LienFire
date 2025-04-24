using System;
using System.Collections.Generic;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror;

public abstract class NetworkTransformBase : NetworkBehaviour
{
	[Header("Target")]
	[Tooltip("The Transform component to sync. May be on on this GameObject, or on a child.")]
	public Transform target;

	[Obsolete("NetworkTransform clientAuthority was replaced with syncDirection. To enable client authority, set SyncDirection to ClientToServer in the Inspector.")]
	[Header("[Obsolete]")]
	[Tooltip("Obsolete: NetworkTransform clientAuthority was replaced with syncDirection. To enable client authority, set SyncDirection to ClientToServer in the Inspector.")]
	public bool clientAuthority;

	public readonly SortedList<double, TransformSnapshot> clientSnapshots = new SortedList<double, TransformSnapshot>();

	public readonly SortedList<double, TransformSnapshot> serverSnapshots = new SortedList<double, TransformSnapshot>();

	[Header("Selective Sync\nDon't change these at Runtime")]
	public bool syncPosition = true;

	public bool syncRotation = true;

	public bool syncScale;

	[Header("Interpolation")]
	[Tooltip("Set to false to have a snap-like effect on position movement.")]
	public bool interpolatePosition = true;

	[Tooltip("Set to false to have a snap-like effect on rotations.")]
	public bool interpolateRotation = true;

	[Tooltip("Set to false to remove scale smoothing. Example use-case: Instant flipping of sprites that use -X and +X for direction.")]
	public bool interpolateScale = true;

	[Header("Debug")]
	public bool showGizmos;

	public bool showOverlay;

	public Color overlayColor = new Color(0f, 0f, 0f, 0.5f);

	protected bool IsClientWithAuthority
	{
		get
		{
			if (base.isClient)
			{
				return base.authority;
			}
			return false;
		}
	}

	protected virtual void Awake()
	{
	}

	protected virtual void OnValidate()
	{
		if (target == null)
		{
			target = base.transform;
		}
		syncInterval = 0f;
		if (clientAuthority)
		{
			syncDirection = SyncDirection.ClientToServer;
			Debug.LogWarning(base.name + "'s NetworkTransform component has obsolete .clientAuthority enabled. Please disable it and set SyncDirection to ClientToServer instead.");
		}
	}

	protected virtual TransformSnapshot Construct()
	{
		return new TransformSnapshot(NetworkTime.localTime, 0.0, target.localPosition, target.localRotation, target.localScale);
	}

	protected void AddSnapshot(SortedList<double, TransformSnapshot> snapshots, double timeStamp, Vector3? position, Quaternion? rotation, Vector3? scale)
	{
		if (!position.HasValue)
		{
			position = ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].position : target.localPosition);
		}
		if (!rotation.HasValue)
		{
			rotation = ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].rotation : target.localRotation);
		}
		if (!scale.HasValue)
		{
			scale = ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].scale : target.localScale);
		}
		SnapshotInterpolation.InsertIfNotExists(snapshots, new TransformSnapshot(timeStamp, NetworkTime.localTime, position.Value, rotation.Value, scale.Value));
	}

	protected virtual void Apply(TransformSnapshot interpolated, TransformSnapshot endGoal)
	{
		if (syncPosition)
		{
			target.localPosition = (interpolatePosition ? interpolated.position : endGoal.position);
		}
		if (syncRotation)
		{
			target.localRotation = (interpolateRotation ? interpolated.rotation : endGoal.rotation);
		}
		if (syncScale)
		{
			target.localScale = (interpolateScale ? interpolated.scale : endGoal.scale);
		}
	}

	[Command]
	public void CmdTeleport(Vector3 destination)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(destination);
		SendCommandInternal("System.Void Mirror.NetworkTransformBase::CmdTeleport(UnityEngine.Vector3)", -788685907, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void CmdTeleport(Vector3 destination, Quaternion rotation)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(destination);
		writer.WriteQuaternion(rotation);
		SendCommandInternal("System.Void Mirror.NetworkTransformBase::CmdTeleport(UnityEngine.Vector3,UnityEngine.Quaternion)", -840469116, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void RpcTeleport(Vector3 destination)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(destination);
		SendRPCInternal("System.Void Mirror.NetworkTransformBase::RpcTeleport(UnityEngine.Vector3)", 165611234, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void RpcTeleport(Vector3 destination, Quaternion rotation)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(destination);
		writer.WriteQuaternion(rotation);
		SendRPCInternal("System.Void Mirror.NetworkTransformBase::RpcTeleport(UnityEngine.Vector3,UnityEngine.Quaternion)", -84918609, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcReset()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Mirror.NetworkTransformBase::RpcReset()", 165401669, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	protected virtual void OnTeleport(Vector3 destination)
	{
		Reset();
		target.position = destination;
	}

	protected virtual void OnTeleport(Vector3 destination, Quaternion rotation)
	{
		Reset();
		target.position = destination;
		target.rotation = rotation;
	}

	public virtual void Reset()
	{
		serverSnapshots.Clear();
		clientSnapshots.Clear();
	}

	protected virtual void OnEnable()
	{
		Reset();
		if (NetworkServer.active)
		{
			NetworkIdentity.clientAuthorityCallback += OnClientAuthorityChanged;
		}
	}

	protected virtual void OnDisable()
	{
		Reset();
		if (NetworkServer.active)
		{
			NetworkIdentity.clientAuthorityCallback -= OnClientAuthorityChanged;
		}
	}

	[ServerCallback]
	private void OnClientAuthorityChanged(NetworkConnectionToClient conn, NetworkIdentity identity, bool authorityState)
	{
		if (NetworkServer.active && !(identity != base.netIdentity) && syncDirection == SyncDirection.ClientToServer)
		{
			Reset();
			RpcReset();
		}
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdTeleport__Vector3(Vector3 destination)
	{
		if (syncDirection == SyncDirection.ClientToServer)
		{
			OnTeleport(destination);
			RpcTeleport(destination);
		}
	}

	protected static void InvokeUserCode_CmdTeleport__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdTeleport called on client.");
		}
		else
		{
			((NetworkTransformBase)obj).UserCode_CmdTeleport__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_CmdTeleport__Vector3__Quaternion(Vector3 destination, Quaternion rotation)
	{
		if (syncDirection == SyncDirection.ClientToServer)
		{
			OnTeleport(destination, rotation);
			RpcTeleport(destination, rotation);
		}
	}

	protected static void InvokeUserCode_CmdTeleport__Vector3__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdTeleport called on client.");
		}
		else
		{
			((NetworkTransformBase)obj).UserCode_CmdTeleport__Vector3__Quaternion(reader.ReadVector3(), reader.ReadQuaternion());
		}
	}

	protected void UserCode_RpcTeleport__Vector3(Vector3 destination)
	{
		OnTeleport(destination);
	}

	protected static void InvokeUserCode_RpcTeleport__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcTeleport called on server.");
		}
		else
		{
			((NetworkTransformBase)obj).UserCode_RpcTeleport__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_RpcTeleport__Vector3__Quaternion(Vector3 destination, Quaternion rotation)
	{
		OnTeleport(destination, rotation);
	}

	protected static void InvokeUserCode_RpcTeleport__Vector3__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcTeleport called on server.");
		}
		else
		{
			((NetworkTransformBase)obj).UserCode_RpcTeleport__Vector3__Quaternion(reader.ReadVector3(), reader.ReadQuaternion());
		}
	}

	protected void UserCode_RpcReset()
	{
		Reset();
	}

	protected static void InvokeUserCode_RpcReset(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcReset called on server.");
		}
		else
		{
			((NetworkTransformBase)obj).UserCode_RpcReset();
		}
	}

	static NetworkTransformBase()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkTransformBase), "System.Void Mirror.NetworkTransformBase::CmdTeleport(UnityEngine.Vector3)", InvokeUserCode_CmdTeleport__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkTransformBase), "System.Void Mirror.NetworkTransformBase::CmdTeleport(UnityEngine.Vector3,UnityEngine.Quaternion)", InvokeUserCode_CmdTeleport__Vector3__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkTransformBase), "System.Void Mirror.NetworkTransformBase::RpcTeleport(UnityEngine.Vector3)", InvokeUserCode_RpcTeleport__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkTransformBase), "System.Void Mirror.NetworkTransformBase::RpcTeleport(UnityEngine.Vector3,UnityEngine.Quaternion)", InvokeUserCode_RpcTeleport__Vector3__Quaternion);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkTransformBase), "System.Void Mirror.NetworkTransformBase::RpcReset()", InvokeUserCode_RpcReset);
	}
}
