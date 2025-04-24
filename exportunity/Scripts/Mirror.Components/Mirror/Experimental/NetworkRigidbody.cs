using System.Runtime.InteropServices;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror.Experimental;

[AddComponentMenu("Network/ Experimental/Network Rigidbody")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-rigidbody")]
public class NetworkRigidbody : NetworkBehaviour
{
	public class ClientSyncState
	{
		public double nextSyncTime;

		public Vector3 velocity;

		public Vector3 angularVelocity;

		public bool isKinematic;

		public bool useGravity;

		public float drag;

		public float angularDrag;
	}

	[Header("Settings")]
	[SerializeField]
	internal Rigidbody target;

	[Tooltip("Set to true if moves come from owner client, set to false if moves always come from server")]
	public bool clientAuthority;

	[Header("Velocity")]
	[Tooltip("Syncs Velocity every SyncInterval")]
	[SerializeField]
	private bool syncVelocity = true;

	[Tooltip("Set velocity to 0 each frame (only works if syncVelocity is false")]
	[SerializeField]
	private bool clearVelocity;

	[Tooltip("Only Syncs Value if distance between previous and current is great than sensitivity")]
	[SerializeField]
	private float velocitySensitivity = 0.1f;

	[Header("Angular Velocity")]
	[Tooltip("Syncs AngularVelocity every SyncInterval")]
	[SerializeField]
	private bool syncAngularVelocity = true;

	[Tooltip("Set angularVelocity to 0 each frame (only works if syncAngularVelocity is false")]
	[SerializeField]
	private bool clearAngularVelocity;

	[Tooltip("Only Syncs Value if distance between previous and current is great than sensitivity")]
	[SerializeField]
	private float angularVelocitySensitivity = 0.1f;

	private readonly ClientSyncState previousValue = new ClientSyncState();

	[SyncVar(hook = "OnVelocityChanged")]
	private Vector3 velocity;

	[SyncVar(hook = "OnAngularVelocityChanged")]
	private Vector3 angularVelocity;

	[SyncVar(hook = "OnIsKinematicChanged")]
	private bool isKinematic;

	[SyncVar(hook = "OnUseGravityChanged")]
	private bool useGravity;

	[SyncVar(hook = "OnuDragChanged")]
	private float drag;

	[SyncVar(hook = "OnAngularDragChanged")]
	private float angularDrag;

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

	public Vector3 Networkvelocity
	{
		get
		{
			return velocity;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref velocity, 1uL, OnVelocityChanged);
		}
	}

	public Vector3 NetworkangularVelocity
	{
		get
		{
			return angularVelocity;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref angularVelocity, 2uL, OnAngularVelocityChanged);
		}
	}

	public bool NetworkisKinematic
	{
		get
		{
			return isKinematic;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isKinematic, 4uL, OnIsKinematicChanged);
		}
	}

	public bool NetworkuseGravity
	{
		get
		{
			return useGravity;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref useGravity, 8uL, OnUseGravityChanged);
		}
	}

	public float Networkdrag
	{
		get
		{
			return drag;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref drag, 16uL, OnuDragChanged);
		}
	}

	public float NetworkangularDrag
	{
		get
		{
			return angularDrag;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref angularDrag, 32uL, OnAngularDragChanged);
		}
	}

	private void OnValidate()
	{
		if (target == null)
		{
			target = GetComponent<Rigidbody>();
		}
	}

	private void OnVelocityChanged(Vector3 _, Vector3 newValue)
	{
		if (!IgnoreSync)
		{
			target.velocity = newValue;
		}
	}

	private void OnAngularVelocityChanged(Vector3 _, Vector3 newValue)
	{
		if (!IgnoreSync)
		{
			target.angularVelocity = newValue;
		}
	}

	private void OnIsKinematicChanged(bool _, bool newValue)
	{
		if (!IgnoreSync)
		{
			target.isKinematic = newValue;
		}
	}

	private void OnUseGravityChanged(bool _, bool newValue)
	{
		if (!IgnoreSync)
		{
			target.useGravity = newValue;
		}
	}

	private void OnuDragChanged(float _, float newValue)
	{
		if (!IgnoreSync)
		{
			target.drag = newValue;
		}
	}

	private void OnAngularDragChanged(float _, float newValue)
	{
		if (!IgnoreSync)
		{
			target.angularDrag = newValue;
		}
	}

	internal void Update()
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

	internal void FixedUpdate()
	{
		if (clearAngularVelocity && !syncAngularVelocity)
		{
			target.angularVelocity = Vector3.zero;
		}
		if (clearVelocity && !syncVelocity)
		{
			target.velocity = Vector3.zero;
		}
	}

	[Server]
	private void SyncToClients()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Mirror.Experimental.NetworkRigidbody::SyncToClients()' called when server was not active");
			return;
		}
		Vector3 vector = (syncVelocity ? target.velocity : default(Vector3));
		Vector3 vector2 = (syncAngularVelocity ? target.angularVelocity : default(Vector3));
		bool num = syncVelocity && (previousValue.velocity - vector).sqrMagnitude > velocitySensitivity * velocitySensitivity;
		bool flag = syncAngularVelocity && (previousValue.angularVelocity - vector2).sqrMagnitude > angularVelocitySensitivity * angularVelocitySensitivity;
		if (num)
		{
			Networkvelocity = vector;
			previousValue.velocity = vector;
		}
		if (flag)
		{
			NetworkangularVelocity = vector2;
			previousValue.angularVelocity = vector2;
		}
		NetworkisKinematic = target.isKinematic;
		NetworkuseGravity = target.useGravity;
		Networkdrag = target.drag;
		NetworkangularDrag = target.angularDrag;
	}

	[Client]
	private void SendToServer()
	{
		if (!NetworkClient.active)
		{
			Debug.LogWarning("[Client] function 'System.Void Mirror.Experimental.NetworkRigidbody::SendToServer()' called when client was not active");
			return;
		}
		if (!base.isOwned)
		{
			Debug.LogWarning("SendToServer called without authority");
			return;
		}
		SendVelocity();
		SendRigidBodySettings();
	}

	[Client]
	private void SendVelocity()
	{
		if (!NetworkClient.active)
		{
			Debug.LogWarning("[Client] function 'System.Void Mirror.Experimental.NetworkRigidbody::SendVelocity()' called when client was not active");
			return;
		}
		double localTime = NetworkTime.localTime;
		if (localTime < previousValue.nextSyncTime)
		{
			return;
		}
		Vector3 vector = (syncVelocity ? target.velocity : default(Vector3));
		Vector3 vector2 = (syncAngularVelocity ? target.angularVelocity : default(Vector3));
		bool flag = syncVelocity && (previousValue.velocity - vector).sqrMagnitude > velocitySensitivity * velocitySensitivity;
		int num;
		if (syncAngularVelocity)
		{
			num = (((previousValue.angularVelocity - vector2).sqrMagnitude > angularVelocitySensitivity * angularVelocitySensitivity) ? 1 : 0);
			if (num != 0)
			{
				CmdSendVelocityAndAngular(vector, vector2);
				previousValue.velocity = vector;
				previousValue.angularVelocity = vector2;
				goto IL_010e;
			}
		}
		else
		{
			num = 0;
		}
		if (flag)
		{
			CmdSendVelocity(vector);
			previousValue.velocity = vector;
		}
		goto IL_010e;
		IL_010e:
		if (((uint)num | (flag ? 1u : 0u)) != 0)
		{
			previousValue.nextSyncTime = localTime + (double)syncInterval;
		}
	}

	[Client]
	private void SendRigidBodySettings()
	{
		if (!NetworkClient.active)
		{
			Debug.LogWarning("[Client] function 'System.Void Mirror.Experimental.NetworkRigidbody::SendRigidBodySettings()' called when client was not active");
			return;
		}
		if (previousValue.isKinematic != target.isKinematic)
		{
			CmdSendIsKinematic(target.isKinematic);
			previousValue.isKinematic = target.isKinematic;
		}
		if (previousValue.useGravity != target.useGravity)
		{
			CmdSendUseGravity(target.useGravity);
			previousValue.useGravity = target.useGravity;
		}
		if (previousValue.drag != target.drag)
		{
			CmdSendDrag(target.drag);
			previousValue.drag = target.drag;
		}
		if (previousValue.angularDrag != target.angularDrag)
		{
			CmdSendAngularDrag(target.angularDrag);
			previousValue.angularDrag = target.angularDrag;
		}
	}

	[Command]
	private void CmdSendVelocity(Vector3 velocity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(velocity);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody::CmdSendVelocity(UnityEngine.Vector3)", 778299891, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdSendVelocityAndAngular(Vector3 velocity, Vector3 angularVelocity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(velocity);
		writer.WriteVector3(angularVelocity);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody::CmdSendVelocityAndAngular(UnityEngine.Vector3,UnityEngine.Vector3)", 600596091, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdSendIsKinematic(bool isKinematic)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isKinematic);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody::CmdSendIsKinematic(System.Boolean)", -594703317, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdSendUseGravity(bool useGravity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(useGravity);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody::CmdSendUseGravity(System.Boolean)", -1841377529, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdSendDrag(float drag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(drag);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody::CmdSendDrag(System.Single)", -1542731250, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdSendAngularDrag(float angularDrag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(angularDrag);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody::CmdSendAngularDrag(System.Single)", -1552888100, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdSendVelocity__Vector3(Vector3 velocity)
	{
		if (clientAuthority)
		{
			Networkvelocity = velocity;
			target.velocity = velocity;
		}
	}

	protected static void InvokeUserCode_CmdSendVelocity__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendVelocity called on client.");
		}
		else
		{
			((NetworkRigidbody)obj).UserCode_CmdSendVelocity__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_CmdSendVelocityAndAngular__Vector3__Vector3(Vector3 velocity, Vector3 angularVelocity)
	{
		if (clientAuthority)
		{
			if (syncVelocity)
			{
				Networkvelocity = velocity;
				target.velocity = velocity;
			}
			NetworkangularVelocity = angularVelocity;
			target.angularVelocity = angularVelocity;
		}
	}

	protected static void InvokeUserCode_CmdSendVelocityAndAngular__Vector3__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendVelocityAndAngular called on client.");
		}
		else
		{
			((NetworkRigidbody)obj).UserCode_CmdSendVelocityAndAngular__Vector3__Vector3(reader.ReadVector3(), reader.ReadVector3());
		}
	}

	protected void UserCode_CmdSendIsKinematic__Boolean(bool isKinematic)
	{
		if (clientAuthority)
		{
			NetworkisKinematic = isKinematic;
			target.isKinematic = isKinematic;
		}
	}

	protected static void InvokeUserCode_CmdSendIsKinematic__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendIsKinematic called on client.");
		}
		else
		{
			((NetworkRigidbody)obj).UserCode_CmdSendIsKinematic__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_CmdSendUseGravity__Boolean(bool useGravity)
	{
		if (clientAuthority)
		{
			NetworkuseGravity = useGravity;
			target.useGravity = useGravity;
		}
	}

	protected static void InvokeUserCode_CmdSendUseGravity__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendUseGravity called on client.");
		}
		else
		{
			((NetworkRigidbody)obj).UserCode_CmdSendUseGravity__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_CmdSendDrag__Single(float drag)
	{
		if (clientAuthority)
		{
			Networkdrag = drag;
			target.drag = drag;
		}
	}

	protected static void InvokeUserCode_CmdSendDrag__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendDrag called on client.");
		}
		else
		{
			((NetworkRigidbody)obj).UserCode_CmdSendDrag__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_CmdSendAngularDrag__Single(float angularDrag)
	{
		if (clientAuthority)
		{
			NetworkangularDrag = angularDrag;
			target.angularDrag = angularDrag;
		}
	}

	protected static void InvokeUserCode_CmdSendAngularDrag__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendAngularDrag called on client.");
		}
		else
		{
			((NetworkRigidbody)obj).UserCode_CmdSendAngularDrag__Single(reader.ReadFloat());
		}
	}

	static NetworkRigidbody()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody), "System.Void Mirror.Experimental.NetworkRigidbody::CmdSendVelocity(UnityEngine.Vector3)", InvokeUserCode_CmdSendVelocity__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody), "System.Void Mirror.Experimental.NetworkRigidbody::CmdSendVelocityAndAngular(UnityEngine.Vector3,UnityEngine.Vector3)", InvokeUserCode_CmdSendVelocityAndAngular__Vector3__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody), "System.Void Mirror.Experimental.NetworkRigidbody::CmdSendIsKinematic(System.Boolean)", InvokeUserCode_CmdSendIsKinematic__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody), "System.Void Mirror.Experimental.NetworkRigidbody::CmdSendUseGravity(System.Boolean)", InvokeUserCode_CmdSendUseGravity__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody), "System.Void Mirror.Experimental.NetworkRigidbody::CmdSendDrag(System.Single)", InvokeUserCode_CmdSendDrag__Single, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody), "System.Void Mirror.Experimental.NetworkRigidbody::CmdSendAngularDrag(System.Single)", InvokeUserCode_CmdSendAngularDrag__Single, requiresAuthority: true);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteVector3(velocity);
			writer.WriteVector3(angularVelocity);
			writer.WriteBool(isKinematic);
			writer.WriteBool(useGravity);
			writer.WriteFloat(drag);
			writer.WriteFloat(angularDrag);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVector3(velocity);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteVector3(angularVelocity);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(isKinematic);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteBool(useGravity);
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteFloat(drag);
		}
		if ((base.syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteFloat(angularDrag);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref velocity, OnVelocityChanged, reader.ReadVector3());
			GeneratedSyncVarDeserialize(ref angularVelocity, OnAngularVelocityChanged, reader.ReadVector3());
			GeneratedSyncVarDeserialize(ref isKinematic, OnIsKinematicChanged, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref useGravity, OnUseGravityChanged, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref drag, OnuDragChanged, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref angularDrag, OnAngularDragChanged, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref velocity, OnVelocityChanged, reader.ReadVector3());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref angularVelocity, OnAngularVelocityChanged, reader.ReadVector3());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isKinematic, OnIsKinematicChanged, reader.ReadBool());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref useGravity, OnUseGravityChanged, reader.ReadBool());
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref drag, OnuDragChanged, reader.ReadFloat());
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref angularDrag, OnAngularDragChanged, reader.ReadFloat());
		}
	}
}
