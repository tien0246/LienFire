using System;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class VanHeo : NetworkBehaviour
{
	public AudioClip nemA;

	public AudioClip socLoA;

	public int soPhat;

	public int damC3;

	public float doGiat;

	public float tamBan;

	public float tamNem;

	public GameObject line;

	public ParticleSystem pa;

	public GameObject depThuong;

	public GameObject depChoang;

	public GameObject depNo;

	public Material vang;

	public MeshRenderer[] mesh;

	private int sonTang = 1;

	public Camera cam;

	public Transform vtBan1;

	public Transform vtBan2;

	private PlayerAtack pA;

	private PlayerStat pS;

	private AudioSource aS;

	private void Start()
	{
		pS = GetComponent<PlayerStat>();
		pA = GetComponent<PlayerAtack>();
		aS = GetComponent<AudioSource>();
		pA.DungChieu1 += Chieu1;
		pA.DungChieu2 += Chieu2;
		pA.DungChieu3 += Chieu3;
		pA.DungDanhThuong += DanhThuong;
		pA.SkinSung += Son;
	}

	private void Chieu1(object sender, EventArgs e)
	{
		Quaternion quay = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamNem - vtBan1.position, Vector3.up);
		Chieu1cmd(quay);
	}

	[Command]
	private void Chieu1cmd(Quaternion quay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(quay);
		SendCommandInternal("System.Void VanHeo::Chieu1cmd(UnityEngine.Quaternion)", -2035861418, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C1Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void VanHeo::C1Rpc()", 839484389, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu2(object sender, EventArgs e)
	{
		Quaternion quay = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamNem - vtBan1.position, Vector3.up);
		Chieu2cmd(quay);
	}

	[Command]
	private void Chieu2cmd(Quaternion quay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(quay);
		SendCommandInternal("System.Void VanHeo::Chieu2cmd(UnityEngine.Quaternion)", 2004369333, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C2Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void VanHeo::C2Rpc()", 868113540, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu3(object sender, EventArgs e)
	{
		Vector3 zero = Vector3.zero;
		C3Com();
		zero = ((!Physics.Raycast(cam.transform.position, cam.transform.forward, out var hitInfo, 100f)) ? (cam.transform.position + cam.transform.forward * tamBan * 5f) : hitInfo.point);
		for (int i = 0; i < soPhat; i++)
		{
			Vector3 vector = (zero - base.transform.position).normalized + new Vector3(UnityEngine.Random.Range(0f - doGiat, doGiat), UnityEngine.Random.Range(0f - doGiat, doGiat), UnityEngine.Random.Range(0f - doGiat, doGiat));
			if (Physics.Raycast(base.transform.position, vector.normalized, out var hitInfo2, tamBan))
			{
				DrawLine(hitInfo2.point);
				PlayerStat component = hitInfo2.transform.GetComponent<PlayerStat>();
				if (component != null && component.team != pS.team)
				{
					UnityEngine.Object.Instantiate(pa, hitInfo2.point, hitInfo2.transform.rotation);
					component.MatMau(damC3 * sonTang);
				}
			}
			else
			{
				DrawLine(vtBan2.transform.position + vector.normalized * tamBan);
			}
		}
	}

	[Command]
	private void DrawLine(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendCommandInternal("System.Void VanHeo::DrawLine(UnityEngine.Vector3)", -1669362963, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DrawLineRPC(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendRPCInternal("System.Void VanHeo::DrawLineRPC(UnityEngine.Vector3)", 1782013630, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void C3Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void VanHeo::C3Com()", 882869695, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C3Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void VanHeo::C3Rpc()", 896742691, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void DanhThuong(object sender, EventArgs e)
	{
		Quaternion quay = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamNem - vtBan1.position, Vector3.up);
		DanhThuongcmd(quay);
	}

	[Command]
	private void DanhThuongcmd(Quaternion quay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(quay);
		SendCommandInternal("System.Void VanHeo::DanhThuongcmd(UnityEngine.Quaternion)", -973465739, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DTRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void VanHeo::DTRpc()", -1565958941, writer, 0, includeOwner: true);
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
		SendRPCInternal("System.Void VanHeo::SonRpc()", -1960585465, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_Chieu1cmd__Quaternion(Quaternion quay)
	{
		C1Rpc();
		GameObject obj = UnityEngine.Object.Instantiate(depNo, vtBan1.transform.position, quay);
		obj.GetComponent<ProjNo>().doi = pS.team;
		obj.GetComponent<ProjNo>().mauMat *= sonTang;
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
			((VanHeo)obj).UserCode_Chieu1cmd__Quaternion(reader.ReadQuaternion());
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
			((VanHeo)obj).UserCode_C1Rpc();
		}
	}

	protected void UserCode_Chieu2cmd__Quaternion(Quaternion quay)
	{
		C2Rpc();
		GameObject obj = UnityEngine.Object.Instantiate(depChoang, vtBan1.transform.position, quay);
		obj.GetComponent<ProjHit>().doi = pS.team;
		obj.GetComponent<ProjHit>().mauMat *= sonTang;
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
			((VanHeo)obj).UserCode_Chieu2cmd__Quaternion(reader.ReadQuaternion());
		}
	}

	protected void UserCode_C2Rpc()
	{
		aS.PlayOneShot(nemA);
	}

	protected static void InvokeUserCode_C2Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C2Rpc called on server.");
		}
		else
		{
			((VanHeo)obj).UserCode_C2Rpc();
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
			((VanHeo)obj).UserCode_DrawLine__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_DrawLineRPC__Vector3(Vector3 vt)
	{
		UnityEngine.Object.Instantiate(line, vtBan2.position, vtBan2.rotation).GetComponent<Line>().SetLine(vtBan2.position, vt);
	}

	protected static void InvokeUserCode_DrawLineRPC__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DrawLineRPC called on server.");
		}
		else
		{
			((VanHeo)obj).UserCode_DrawLineRPC__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_C3Com()
	{
		C3Rpc();
	}

	protected static void InvokeUserCode_C3Com(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command C3Com called on client.");
		}
		else
		{
			((VanHeo)obj).UserCode_C3Com();
		}
	}

	protected void UserCode_C3Rpc()
	{
		aS.PlayOneShot(socLoA);
	}

	protected static void InvokeUserCode_C3Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C3Rpc called on server.");
		}
		else
		{
			((VanHeo)obj).UserCode_C3Rpc();
		}
	}

	protected void UserCode_DanhThuongcmd__Quaternion(Quaternion quay)
	{
		DTRpc();
		GameObject obj = UnityEngine.Object.Instantiate(depThuong, vtBan1.transform.position, quay);
		obj.GetComponent<ProjHit>().doi = pS.team;
		obj.GetComponent<ProjHit>().mauMat *= sonTang;
		NetworkServer.Spawn(obj);
	}

	protected static void InvokeUserCode_DanhThuongcmd__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command DanhThuongcmd called on client.");
		}
		else
		{
			((VanHeo)obj).UserCode_DanhThuongcmd__Quaternion(reader.ReadQuaternion());
		}
	}

	protected void UserCode_DTRpc()
	{
		aS.PlayOneShot(nemA);
	}

	protected static void InvokeUserCode_DTRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DTRpc called on server.");
		}
		else
		{
			((VanHeo)obj).UserCode_DTRpc();
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
			((VanHeo)obj).UserCode_SonRpc();
		}
	}

	static VanHeo()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(VanHeo), "System.Void VanHeo::Chieu1cmd(UnityEngine.Quaternion)", InvokeUserCode_Chieu1cmd__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(VanHeo), "System.Void VanHeo::Chieu2cmd(UnityEngine.Quaternion)", InvokeUserCode_Chieu2cmd__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(VanHeo), "System.Void VanHeo::DrawLine(UnityEngine.Vector3)", InvokeUserCode_DrawLine__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(VanHeo), "System.Void VanHeo::C3Com()", InvokeUserCode_C3Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(VanHeo), "System.Void VanHeo::DanhThuongcmd(UnityEngine.Quaternion)", InvokeUserCode_DanhThuongcmd__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(VanHeo), "System.Void VanHeo::C1Rpc()", InvokeUserCode_C1Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(VanHeo), "System.Void VanHeo::C2Rpc()", InvokeUserCode_C2Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(VanHeo), "System.Void VanHeo::DrawLineRPC(UnityEngine.Vector3)", InvokeUserCode_DrawLineRPC__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(VanHeo), "System.Void VanHeo::C3Rpc()", InvokeUserCode_C3Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(VanHeo), "System.Void VanHeo::DTRpc()", InvokeUserCode_DTRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(VanHeo), "System.Void VanHeo::SonRpc()", InvokeUserCode_SonRpc);
	}
}
