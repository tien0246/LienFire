using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class EnderP : NetworkBehaviour
{
	public GameObject nemA;

	public float offset;

	[SyncVar]
	public GameObject player;

	private CharacterController cc;

	public float force;

	private Rigidbody rb;

	protected uint ___playerNetId;

	public GameObject Networkplayer
	{
		get
		{
			return GetSyncVarGameObject(___playerNetId, ref player);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_GameObject(value, ref player, 1uL, null, ref ___playerNetId);
		}
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddForce(base.transform.forward * force, ForceMode.Impulse);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("matDat") || other.transform.CompareTag("tuong"))
		{
			TeleA();
			Networkplayer.GetComponent<Steve>().Tele(base.transform.position);
			base.gameObject.SetActive(value: false);
		}
	}

	[Command(requiresAuthority = false)]
	private void TeleA()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void EnderP::TeleA()", 417554796, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	private void Des()
	{
		NetworkServer.Destroy(base.gameObject);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_TeleA()
	{
		NetworkServer.Spawn(Object.Instantiate(nemA, base.transform.position, base.transform.rotation));
		Invoke("Des", 1f);
	}

	protected static void InvokeUserCode_TeleA(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command TeleA called on client.");
		}
		else
		{
			((EnderP)obj).UserCode_TeleA();
		}
	}

	static EnderP()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(EnderP), "System.Void EnderP::TeleA()", InvokeUserCode_TeleA, requiresAuthority: false);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteGameObject(Networkplayer);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteGameObject(Networkplayer);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_GameObject(ref player, null, reader, ref ___playerNetId);
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_GameObject(ref player, null, reader, ref ___playerNetId);
		}
	}
}
