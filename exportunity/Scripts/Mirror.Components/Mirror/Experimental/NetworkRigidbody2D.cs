using System.Runtime.InteropServices;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror.Experimental;

[AddComponentMenu("Network/ Experimental/Network Rigidbody 2D")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-rigidbody")]
public class NetworkRigidbody2D : NetworkBehaviour
{
	public class ClientSyncState
	{
		public float nextSyncTime;

		public Vector2 velocity;

		public float angularVelocity;

		public bool isKinematic;

		public float gravityScale;

		public float drag;

		public float angularDrag;
	}

	[Header("Settings")]
	[SerializeField]
	internal Rigidbody2D target;

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
	private Vector2 velocity;

	[SyncVar(hook = "OnAngularVelocityChanged")]
	private float angularVelocity;

	[SyncVar(hook = "OnIsKinematicChanged")]
	private bool isKinematic;

	[SyncVar(hook = "OnGravityScaleChanged")]
	private float gravityScale;

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

	public Vector2 Networkvelocity
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

	public float NetworkangularVelocity
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

	public float NetworkgravityScale
	{
		get
		{
			return gravityScale;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref gravityScale, 8uL, OnGravityScaleChanged);
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
			target = GetComponent<Rigidbody2D>();
		}
	}

	private void OnVelocityChanged(Vector2 _, Vector2 newValue)
	{
		if (!IgnoreSync)
		{
			target.velocity = newValue;
		}
	}

	private void OnAngularVelocityChanged(float _, float newValue)
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

	private void OnGravityScaleChanged(float _, float newValue)
	{
		if (!IgnoreSync)
		{
			target.gravityScale = newValue;
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
			target.angularVelocity = 0f;
		}
		if (clearVelocity && !syncVelocity)
		{
			target.velocity = Vector2.zero;
		}
	}

	[Server]
	private void SyncToClients()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Mirror.Experimental.NetworkRigidbody2D::SyncToClients()' called when server was not active");
			return;
		}
		Vector2 vector = (syncVelocity ? target.velocity : default(Vector2));
		float num = (syncAngularVelocity ? target.angularVelocity : 0f);
		bool num2 = syncVelocity && (previousValue.velocity - vector).sqrMagnitude > velocitySensitivity * velocitySensitivity;
		bool flag = syncAngularVelocity && previousValue.angularVelocity - num > angularVelocitySensitivity;
		if (num2)
		{
			Networkvelocity = vector;
			previousValue.velocity = vector;
		}
		if (flag)
		{
			NetworkangularVelocity = num;
			previousValue.angularVelocity = num;
		}
		NetworkisKinematic = target.isKinematic;
		NetworkgravityScale = target.gravityScale;
		Networkdrag = target.drag;
		NetworkangularDrag = target.angularDrag;
	}

	[Client]
	private void SendToServer()
	{
		if (!NetworkClient.active)
		{
			Debug.LogWarning("[Client] function 'System.Void Mirror.Experimental.NetworkRigidbody2D::SendToServer()' called when client was not active");
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
			Debug.LogWarning("[Client] function 'System.Void Mirror.Experimental.NetworkRigidbody2D::SendVelocity()' called when client was not active");
			return;
		}
		float time = Time.time;
		if (time < previousValue.nextSyncTime)
		{
			return;
		}
		Vector2 vector = (syncVelocity ? target.velocity : default(Vector2));
		float num = (syncAngularVelocity ? target.angularVelocity : 0f);
		bool flag = syncVelocity && (previousValue.velocity - vector).sqrMagnitude > velocitySensitivity * velocitySensitivity;
		int num2;
		if (syncAngularVelocity)
		{
			num2 = ((previousValue.angularVelocity != num) ? 1 : 0);
			if (num2 != 0)
			{
				CmdSendVelocityAndAngular(vector, num);
				previousValue.velocity = vector;
				previousValue.angularVelocity = num;
				goto IL_00f1;
			}
		}
		else
		{
			num2 = 0;
		}
		if (flag)
		{
			CmdSendVelocity(vector);
			previousValue.velocity = vector;
		}
		goto IL_00f1;
		IL_00f1:
		if (((uint)num2 | (flag ? 1u : 0u)) != 0)
		{
			previousValue.nextSyncTime = time + syncInterval;
		}
	}

	[Client]
	private void SendRigidBodySettings()
	{
		if (!NetworkClient.active)
		{
			Debug.LogWarning("[Client] function 'System.Void Mirror.Experimental.NetworkRigidbody2D::SendRigidBodySettings()' called when client was not active");
			return;
		}
		if (previousValue.isKinematic != target.isKinematic)
		{
			CmdSendIsKinematic(target.isKinematic);
			previousValue.isKinematic = target.isKinematic;
		}
		if (previousValue.gravityScale != target.gravityScale)
		{
			CmdChangeGravityScale(target.gravityScale);
			previousValue.gravityScale = target.gravityScale;
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
	private void CmdSendVelocity(Vector2 velocity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector2(velocity);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody2D::CmdSendVelocity(UnityEngine.Vector2)", 220550182, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdSendVelocityAndAngular(Vector2 velocity, float angularVelocity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector2(velocity);
		writer.WriteFloat(angularVelocity);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody2D::CmdSendVelocityAndAngular(UnityEngine.Vector2,System.Single)", 134907968, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdSendIsKinematic(bool isKinematic)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isKinematic);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody2D::CmdSendIsKinematic(System.Boolean)", -488021251, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdChangeGravityScale(float gravityScale)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(gravityScale);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody2D::CmdChangeGravityScale(System.Single)", 1567375312, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdSendDrag(float drag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(drag);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody2D::CmdSendDrag(System.Single)", 327979744, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdSendAngularDrag(float angularDrag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(angularDrag);
		SendCommandInternal("System.Void Mirror.Experimental.NetworkRigidbody2D::CmdSendAngularDrag(System.Single)", -441068086, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdSendVelocity__Vector2(Vector2 velocity)
	{
		if (clientAuthority)
		{
			Networkvelocity = velocity;
			target.velocity = velocity;
		}
	}

	protected static void InvokeUserCode_CmdSendVelocity__Vector2(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendVelocity called on client.");
		}
		else
		{
			((NetworkRigidbody2D)obj).UserCode_CmdSendVelocity__Vector2(reader.ReadVector2());
		}
	}

	protected void UserCode_CmdSendVelocityAndAngular__Vector2__Single(Vector2 velocity, float angularVelocity)
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

	protected static void InvokeUserCode_CmdSendVelocityAndAngular__Vector2__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendVelocityAndAngular called on client.");
		}
		else
		{
			((NetworkRigidbody2D)obj).UserCode_CmdSendVelocityAndAngular__Vector2__Single(reader.ReadVector2(), reader.ReadFloat());
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
			((NetworkRigidbody2D)obj).UserCode_CmdSendIsKinematic__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_CmdChangeGravityScale__Single(float gravityScale)
	{
		if (clientAuthority)
		{
			NetworkgravityScale = gravityScale;
			target.gravityScale = gravityScale;
		}
	}

	protected static void InvokeUserCode_CmdChangeGravityScale__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdChangeGravityScale called on client.");
		}
		else
		{
			((NetworkRigidbody2D)obj).UserCode_CmdChangeGravityScale__Single(reader.ReadFloat());
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
			((NetworkRigidbody2D)obj).UserCode_CmdSendDrag__Single(reader.ReadFloat());
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
			((NetworkRigidbody2D)obj).UserCode_CmdSendAngularDrag__Single(reader.ReadFloat());
		}
	}

	static NetworkRigidbody2D()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody2D), "System.Void Mirror.Experimental.NetworkRigidbody2D::CmdSendVelocity(UnityEngine.Vector2)", InvokeUserCode_CmdSendVelocity__Vector2, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody2D), "System.Void Mirror.Experimental.NetworkRigidbody2D::CmdSendVelocityAndAngular(UnityEngine.Vector2,System.Single)", InvokeUserCode_CmdSendVelocityAndAngular__Vector2__Single, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody2D), "System.Void Mirror.Experimental.NetworkRigidbody2D::CmdSendIsKinematic(System.Boolean)", InvokeUserCode_CmdSendIsKinematic__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody2D), "System.Void Mirror.Experimental.NetworkRigidbody2D::CmdChangeGravityScale(System.Single)", InvokeUserCode_CmdChangeGravityScale__Single, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody2D), "System.Void Mirror.Experimental.NetworkRigidbody2D::CmdSendDrag(System.Single)", InvokeUserCode_CmdSendDrag__Single, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkRigidbody2D), "System.Void Mirror.Experimental.NetworkRigidbody2D::CmdSendAngularDrag(System.Single)", InvokeUserCode_CmdSendAngularDrag__Single, requiresAuthority: true);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteVector2(velocity);
			writer.WriteFloat(angularVelocity);
			writer.WriteBool(isKinematic);
			writer.WriteFloat(gravityScale);
			writer.WriteFloat(drag);
			writer.WriteFloat(angularDrag);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVector2(velocity);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteFloat(angularVelocity);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(isKinematic);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteFloat(gravityScale);
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
			GeneratedSyncVarDeserialize(ref velocity, OnVelocityChanged, reader.ReadVector2());
			GeneratedSyncVarDeserialize(ref angularVelocity, OnAngularVelocityChanged, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref isKinematic, OnIsKinematicChanged, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref gravityScale, OnGravityScaleChanged, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref drag, OnuDragChanged, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref angularDrag, OnAngularDragChanged, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref velocity, OnVelocityChanged, reader.ReadVector2());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref angularVelocity, OnAngularVelocityChanged, reader.ReadFloat());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isKinematic, OnIsKinematicChanged, reader.ReadBool());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref gravityScale, OnGravityScaleChanged, reader.ReadFloat());
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
