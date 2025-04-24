using System;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Yon : NetworkBehaviour
{
	public AudioClip nemA;

	public AudioClip tenLuaA;

	public AudioClip vungBomA;

	public AudioClip banA;

	public int damDT;

	public float tamBan;

	public float tamNem;

	public float doCaoVungBom;

	public GameObject line;

	public GameObject rocket;

	public GameObject luuDan;

	public GameObject vungBom;

	public Camera cam;

	public Transform vtBan1;

	public Transform vtBan2;

	private bool dangBan;

	private float tGHetBan;

	public Material vang;

	public MeshRenderer[] mesh;

	private int sonTang = 1;

	private PlayerAtack pA;

	private PlayerStat pS;

	private Animator ani;

	private AudioSource aS;

	private void Start()
	{
		ani = GetComponent<Animator>();
		pS = GetComponent<PlayerStat>();
		aS = GetComponent<AudioSource>();
		pA = GetComponent<PlayerAtack>();
		pA.DungChieu1 += Chieu1;
		pA.DungChieu2 += Chieu2;
		pA.DungChieu3 += Chieu3;
		pA.DungDanhThuong += DanhThuong;
		pA.SkinSung += Son;
	}

	private void Update()
	{
		if (tGHetBan > 0f)
		{
			tGHetBan -= Time.deltaTime;
		}
		else if (dangBan)
		{
			dangBan = false;
			ani.SetBool("dangBan", dangBan);
		}
	}

	private void Chieu1(object sender, EventArgs e)
	{
		Quaternion quay = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamNem - vtBan2.position, Vector3.up);
		Chieu1cmd(quay);
	}

	[Command]
	private void Chieu1cmd(Quaternion quay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(quay);
		SendCommandInternal("System.Void Yon::Chieu1cmd(UnityEngine.Quaternion)", 897021333, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C1Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Yon::C1Rpc()", 1647333860, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu2(object sender, EventArgs e)
	{
		Quaternion quay = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamNem - vtBan2.position, Vector3.up);
		Chieu2cmd(quay);
	}

	[Command]
	private void Chieu2cmd(Quaternion quay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(quay);
		SendCommandInternal("System.Void Yon::Chieu2cmd(UnityEngine.Quaternion)", 642284788, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C2Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Yon::C2Rpc()", 1675963011, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu3(object sender, EventArgs e)
	{
		Quaternion quay = Quaternion.LookRotation(base.transform.forward, Vector3.up);
		Chieu3cmd(quay);
	}

	[Command]
	private void Chieu3cmd(Quaternion quay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(quay);
		SendCommandInternal("System.Void Yon::Chieu3cmd(UnityEngine.Quaternion)", 387548243, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C3Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Yon::C3Rpc()", 1704592162, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void DanhThuong(object sender, EventArgs e)
	{
		DTCom();
		dangBan = true;
		tGHetBan = 0.25f;
		ani.SetBool("dangBan", dangBan);
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hitInfo, tamBan))
		{
			DrawLine(hitInfo.point);
			PlayerStat component = hitInfo.transform.GetComponent<PlayerStat>();
			if (component != null && component.team != pS.team)
			{
				component.MatMau(damDT * sonTang);
			}
		}
		else
		{
			DrawLine(cam.transform.position + cam.transform.forward * tamBan);
		}
	}

	[Command]
	private void DTCom()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Yon::DTCom()", -771982466, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DTRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Yon::DTRpc()", -758109470, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void DrawLine(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendCommandInternal("System.Void Yon::DrawLine(UnityEngine.Vector3)", 726024108, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DrawLineRPC(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendRPCInternal("System.Void Yon::DrawLineRPC(UnityEngine.Vector3)", 1876622751, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Son(object sender, EventArgs e)
	{
		sonTang = 2;
		SonRpc();
	}

	[ClientRpc]
	private void SonRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Yon::SonRpc()", 1607911656, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_Chieu1cmd__Quaternion(Quaternion quay)
	{
		C1Rpc();
		GameObject obj = UnityEngine.Object.Instantiate(luuDan, vtBan2.transform.position, quay);
		obj.GetComponent<Grenade>().doi = pS.team;
		obj.GetComponent<Grenade>().mauMat *= sonTang;
		NetworkServer.Spawn(obj);
	}

	protected static void InvokeUserCode_Chieu1cmd__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Chieu1cmd called on client.");
		}
		else
		{
			((Yon)obj).UserCode_Chieu1cmd__Quaternion(reader.ReadQuaternion());
		}
	}

	protected void UserCode_C1Rpc()
	{
		aS.PlayOneShot(nemA);
	}

	protected static void InvokeUserCode_C1Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C1Rpc called on server.");
		}
		else
		{
			((Yon)obj).UserCode_C1Rpc();
		}
	}

	protected void UserCode_Chieu2cmd__Quaternion(Quaternion quay)
	{
		C2Rpc();
		GameObject obj = UnityEngine.Object.Instantiate(rocket, vtBan2.transform.position, quay);
		obj.GetComponent<ProjNo>().doi = pS.team;
		obj.GetComponent<ProjNo>().mauMat *= sonTang;
		NetworkServer.Spawn(obj);
	}

	protected static void InvokeUserCode_Chieu2cmd__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Chieu2cmd called on client.");
		}
		else
		{
			((Yon)obj).UserCode_Chieu2cmd__Quaternion(reader.ReadQuaternion());
		}
	}

	protected void UserCode_C2Rpc()
	{
		aS.PlayOneShot(tenLuaA);
	}

	protected static void InvokeUserCode_C2Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C2Rpc called on server.");
		}
		else
		{
			((Yon)obj).UserCode_C2Rpc();
		}
	}

	protected void UserCode_Chieu3cmd__Quaternion(Quaternion quay)
	{
		C3Rpc();
		GameObject obj = UnityEngine.Object.Instantiate(position: new Vector3(base.transform.position.x, doCaoVungBom, base.transform.position.z), original: vungBom, rotation: quay);
		obj.GetComponent<VungBom>().Networkdoi = pS.team;
		obj.GetComponent<VungBom>().mauMat *= sonTang;
		NetworkServer.Spawn(obj);
	}

	protected static void InvokeUserCode_Chieu3cmd__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Chieu3cmd called on client.");
		}
		else
		{
			((Yon)obj).UserCode_Chieu3cmd__Quaternion(reader.ReadQuaternion());
		}
	}

	protected void UserCode_C3Rpc()
	{
		aS.PlayOneShot(vungBomA);
	}

	protected static void InvokeUserCode_C3Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C3Rpc called on server.");
		}
		else
		{
			((Yon)obj).UserCode_C3Rpc();
		}
	}

	protected void UserCode_DTCom()
	{
		DTRpc();
	}

	protected static void InvokeUserCode_DTCom(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command DTCom called on client.");
		}
		else
		{
			((Yon)obj).UserCode_DTCom();
		}
	}

	protected void UserCode_DTRpc()
	{
		aS.PlayOneShot(banA);
	}

	protected static void InvokeUserCode_DTRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DTRpc called on server.");
		}
		else
		{
			((Yon)obj).UserCode_DTRpc();
		}
	}

	protected void UserCode_DrawLine__Vector3(Vector3 vt)
	{
		DrawLineRPC(vt);
	}

	protected static void InvokeUserCode_DrawLine__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command DrawLine called on client.");
		}
		else
		{
			((Yon)obj).UserCode_DrawLine__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_DrawLineRPC__Vector3(Vector3 vt)
	{
		UnityEngine.Object.Instantiate(line, vtBan1.position, vtBan1.rotation).GetComponent<Line>().SetLine(vtBan1.position, vt);
	}

	protected static void InvokeUserCode_DrawLineRPC__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DrawLineRPC called on server.");
		}
		else
		{
			((Yon)obj).UserCode_DrawLineRPC__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_SonRpc()
	{
		for (int i = 0; i < mesh.Length; i++)
		{
			mesh[i].material = vang;
		}
	}

	protected static void InvokeUserCode_SonRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC SonRpc called on server.");
		}
		else
		{
			((Yon)obj).UserCode_SonRpc();
		}
	}

	static Yon()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Yon), "System.Void Yon::Chieu1cmd(UnityEngine.Quaternion)", InvokeUserCode_Chieu1cmd__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Yon), "System.Void Yon::Chieu2cmd(UnityEngine.Quaternion)", InvokeUserCode_Chieu2cmd__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Yon), "System.Void Yon::Chieu3cmd(UnityEngine.Quaternion)", InvokeUserCode_Chieu3cmd__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Yon), "System.Void Yon::DTCom()", InvokeUserCode_DTCom, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Yon), "System.Void Yon::DrawLine(UnityEngine.Vector3)", InvokeUserCode_DrawLine__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(Yon), "System.Void Yon::C1Rpc()", InvokeUserCode_C1Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Yon), "System.Void Yon::C2Rpc()", InvokeUserCode_C2Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Yon), "System.Void Yon::C3Rpc()", InvokeUserCode_C3Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Yon), "System.Void Yon::DTRpc()", InvokeUserCode_DTRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Yon), "System.Void Yon::DrawLineRPC(UnityEngine.Vector3)", InvokeUserCode_DrawLineRPC__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(Yon), "System.Void Yon::SonRpc()", InvokeUserCode_SonRpc);
	}
}
