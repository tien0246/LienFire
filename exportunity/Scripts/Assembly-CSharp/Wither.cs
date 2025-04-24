using System;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Wither : NetworkBehaviour
{
	public AudioClip tenLuaA;

	public float tamBan;

	public GameObject rocket;

	public Camera cam;

	public Transform vtBan;

	private PlayerAtack pA;

	private PlayerStat pS;

	private AudioSource aS;

	private void Start()
	{
		pS = GetComponent<PlayerStat>();
		aS = GetComponent<AudioSource>();
		pA = GetComponent<PlayerAtack>();
		pA.DungDanhThuong += DanhThuong;
	}

	private void DanhThuong(object sender, EventArgs e)
	{
		Quaternion quay = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamBan - vtBan.position, Vector3.up);
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var _, tamBan))
		{
			quay = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamBan - vtBan.position, Vector3.up);
		}
		DTcom(quay);
	}

	[Command]
	private void DTcom(Quaternion quay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(quay);
		SendCommandInternal("System.Void Wither::DTcom(UnityEngine.Quaternion)", -1818095450, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DTRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Wither::DTRpc()", 1972779615, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_DTcom__Quaternion(Quaternion quay)
	{
		DTRpc();
		GameObject obj = UnityEngine.Object.Instantiate(rocket, vtBan.transform.position, quay);
		obj.GetComponent<ProjNo>().doi = pS.team;
		NetworkServer.Spawn(obj);
	}

	protected static void InvokeUserCode_DTcom__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command DTcom called on client.");
		}
		else
		{
			((Wither)obj).UserCode_DTcom__Quaternion(reader.ReadQuaternion());
		}
	}

	protected void UserCode_DTRpc()
	{
		aS.PlayOneShot(tenLuaA);
	}

	protected static void InvokeUserCode_DTRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DTRpc called on server.");
		}
		else
		{
			((Wither)obj).UserCode_DTRpc();
		}
	}

	static Wither()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Wither), "System.Void Wither::DTcom(UnityEngine.Quaternion)", InvokeUserCode_DTcom__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(Wither), "System.Void Wither::DTRpc()", InvokeUserCode_DTRpc);
	}
}
