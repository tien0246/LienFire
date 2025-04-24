using System;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.UI;

public class Elsu : NetworkBehaviour
{
	public AudioClip banA;

	public int damDT;

	public float tamBan;

	public float tamNem;

	public float doCaoVungBom;

	public GameObject line;

	public GameObject lineRed;

	public GameObject rocket;

	public GameObject vungBom;

	public Camera cam;

	public Camera camXuyen;

	public Transform vtBan1;

	public Transform vtBan2;

	private bool dangBan;

	private float tGHetBan;

	public AudioClip tiengC1;

	public float tgC1;

	private bool th;

	public Material maC1;

	public Material maThuong;

	public SkinnedMeshRenderer skin;

	private MeshRenderer[] meshC1;

	public GameObject thanh;

	public GameObject ungTram;

	public AudioClip nhamA;

	private bool nham;

	public Button nutC2;

	private float fThuong;

	public float fZoom;

	public GameObject camH;

	private Vector3 vtThuong;

	public Vector3 vtZoom;

	private Vector3 vtCM;

	private Vector3 vtCMX;

	private float nhayThuong;

	public float nhayZoom;

	public GameObject nhamUI;

	public AudioClip tiengC2;

	public int damC2;

	private float tdLuot;

	private bool dangLuot;

	public float tgLuotC3;

	public float tdLuotC3;

	private Vector3 huongLuot;

	public GameObject luuDan;

	public AudioClip tiengC3;

	public Material vang;

	public MeshRenderer[] mesh;

	private int sonTang = 1;

	private PlayerAtack pA;

	private PlayerLook pL;

	private PlayerStat pS;

	private Animator ani;

	private AudioSource aS;

	private CharacterController cc;

	private void Start()
	{
		ani = GetComponent<Animator>();
		pS = GetComponent<PlayerStat>();
		aS = GetComponent<AudioSource>();
		pA = GetComponent<PlayerAtack>();
		pL = GetComponent<PlayerLook>();
		cc = GetComponent<CharacterController>();
		pA.DungChieu1 += Chieu1;
		pA.DungChieu2 += Chieu2;
		pA.DungChieu3 += Chieu3;
		pA.DungDanhThuong += DanhThuong;
		pA.SkinSung += Son;
		meshC1 = GetComponentsInChildren<MeshRenderer>();
		vtCM = cam.transform.localPosition;
		vtCMX = camXuyen.transform.localPosition;
		fThuong = cam.fieldOfView;
		vtThuong = camH.transform.localPosition;
		nhayThuong = pL.doNhay;
	}

	private void Update()
	{
		if (dangLuot)
		{
			cc.Move(huongLuot * tdLuot * Time.deltaTime);
		}
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
		if (nham)
		{
			TatNham();
		}
		Chieu1cmd();
	}

	[Command]
	private void Chieu1cmd()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Elsu::Chieu1cmd()", 107763633, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void THRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Elsu::THRpc()", -506716827, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void HetTH()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Elsu::HetTH()", -1706041409, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void HetTHRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Elsu::HetTHRpc()", 2004364808, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu2(object sender, EventArgs e)
	{
		if (th)
		{
			HetTH();
		}
		C2Com();
		RaycastHit[] array = Physics.RaycastAll(cam.transform.position, cam.transform.forward, tamBan);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && component.team != pS.team)
			{
				component.MatMau(damC2 * sonTang);
			}
		}
		DrawLineA(cam.transform.position + cam.transform.forward * tamBan);
	}

	[Command]
	private void C2Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Elsu::C2Com()", 941875462, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C2Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Elsu::C2Rpc()", 955748458, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu3(object sender, EventArgs e)
	{
		if (nham)
		{
			TatNham();
		}
		if (th)
		{
			HetTH();
		}
		C3Com();
		pS.DungYen(tgLuotC3);
		tdLuot = tdLuotC3;
		dangLuot = true;
		huongLuot = -base.transform.forward;
		Invoke("HetLuot", tgLuotC3);
	}

	[Command]
	private void C3Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Elsu::C3Com()", 970504613, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C3Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Elsu::C3Rpc()", 984377609, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void DanhThuong(object sender, EventArgs e)
	{
		if (th)
		{
			HetTH();
		}
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
		SendCommandInternal("System.Void Elsu::DTCom()", -1492197019, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DTRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Elsu::DTRpc()", -1478324023, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void DrawLine(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendCommandInternal("System.Void Elsu::DrawLine(UnityEngine.Vector3)", -1961873837, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DrawLineRPC(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendRPCInternal("System.Void Elsu::DrawLineRPC(UnityEngine.Vector3)", 2079209880, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void DrawLineA(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendCommandInternal("System.Void Elsu::DrawLineA(UnityEngine.Vector3)", 2116000220, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DrawLineRPCA(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendRPCInternal("System.Void Elsu::DrawLineRPCA(UnityEngine.Vector3)", -1459423433, writer, 0, includeOwner: true);
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
		SendRPCInternal("System.Void Elsu::SonRpc()", 756096993, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void Nham()
	{
		NCom();
		if (!nham)
		{
			BatNham();
		}
		else
		{
			TatNham();
		}
	}

	[Command]
	private void NCom()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Elsu::NCom()", 1332141665, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void NRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Elsu::NRpc()", 1346014661, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void BatNham()
	{
		nham = true;
		pL.NetworkdoNhay = nhayThuong * nhayZoom;
		nutC2.interactable = true;
		cam.fieldOfView = fZoom;
		camXuyen.fieldOfView = fZoom;
		cam.transform.localPosition = Vector3.zero;
		camXuyen.transform.localPosition = Vector3.zero;
		camH.transform.localPosition = vtZoom;
		nhamUI.SetActive(value: true);
		GetComponent<Animator>().SetBool("nham", value: true);
	}

	private void TatNham()
	{
		nham = false;
		pL.NetworkdoNhay = nhayThuong;
		nutC2.interactable = false;
		cam.fieldOfView = fThuong;
		camXuyen.fieldOfView = fThuong;
		cam.transform.localPosition = vtCM;
		camXuyen.transform.localPosition = vtCMX;
		camH.transform.localPosition = vtThuong;
		nhamUI.SetActive(value: false);
		GetComponent<Animator>().SetBool("nham", value: false);
	}

	private void HetLuot()
	{
		dangLuot = false;
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_Chieu1cmd()
	{
		THRpc();
		Invoke("HetTHRpc", tgC1);
		GameObject obj = UnityEngine.Object.Instantiate(ungTram, base.transform.position, base.transform.rotation);
		obj.GetComponent<UngTram>().Networkteam = pS.team;
		NetworkServer.Spawn(obj);
	}

	protected static void InvokeUserCode_Chieu1cmd(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Chieu1cmd called on client.");
		}
		else
		{
			((Elsu)obj).UserCode_Chieu1cmd();
		}
	}

	protected void UserCode_THRpc()
	{
		aS.PlayOneShot(tiengC1);
		th = true;
		for (int i = 0; i < meshC1.Length; i++)
		{
			meshC1[i].enabled = false;
		}
		skin.material = maC1;
		thanh.SetActive(value: false);
	}

	protected static void InvokeUserCode_THRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC THRpc called on server.");
		}
		else
		{
			((Elsu)obj).UserCode_THRpc();
		}
	}

	protected void UserCode_HetTH()
	{
		HetTHRpc();
	}

	protected static void InvokeUserCode_HetTH(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command HetTH called on client.");
		}
		else
		{
			((Elsu)obj).UserCode_HetTH();
		}
	}

	protected void UserCode_HetTHRpc()
	{
		if (th)
		{
			th = false;
			for (int i = 0; i < meshC1.Length; i++)
			{
				meshC1[i].enabled = true;
			}
			skin.material = maThuong;
			thanh.SetActive(value: true);
		}
	}

	protected static void InvokeUserCode_HetTHRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC HetTHRpc called on server.");
		}
		else
		{
			((Elsu)obj).UserCode_HetTHRpc();
		}
	}

	protected void UserCode_C2Com()
	{
		C2Rpc();
	}

	protected static void InvokeUserCode_C2Com(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command C2Com called on client.");
		}
		else
		{
			((Elsu)obj).UserCode_C2Com();
		}
	}

	protected void UserCode_C2Rpc()
	{
		aS.PlayOneShot(tiengC2);
	}

	protected static void InvokeUserCode_C2Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C2Rpc called on server.");
		}
		else
		{
			((Elsu)obj).UserCode_C2Rpc();
		}
	}

	protected void UserCode_C3Com()
	{
		C3Rpc();
		GameObject obj = UnityEngine.Object.Instantiate(luuDan, vtBan2.transform.position, Quaternion.LookRotation(base.transform.forward, Vector3.up));
		obj.GetComponent<Grenade>().doi = pS.team;
		obj.GetComponent<Grenade>().mauMat *= sonTang;
		NetworkServer.Spawn(obj);
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
			((Elsu)obj).UserCode_C3Com();
		}
	}

	protected void UserCode_C3Rpc()
	{
		aS.PlayOneShot(tiengC3);
	}

	protected static void InvokeUserCode_C3Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C3Rpc called on server.");
		}
		else
		{
			((Elsu)obj).UserCode_C3Rpc();
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
			((Elsu)obj).UserCode_DTCom();
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
			((Elsu)obj).UserCode_DTRpc();
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
			((Elsu)obj).UserCode_DrawLine__Vector3(reader.ReadVector3());
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
			((Elsu)obj).UserCode_DrawLineRPC__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_DrawLineA__Vector3(Vector3 vt)
	{
		DrawLineRPCA(vt);
	}

	protected static void InvokeUserCode_DrawLineA__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command DrawLineA called on client.");
		}
		else
		{
			((Elsu)obj).UserCode_DrawLineA__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_DrawLineRPCA__Vector3(Vector3 vt)
	{
		UnityEngine.Object.Instantiate(lineRed, vtBan1.position, vtBan1.rotation).GetComponent<Line>().SetLine(vtBan1.position, vt);
	}

	protected static void InvokeUserCode_DrawLineRPCA__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DrawLineRPCA called on server.");
		}
		else
		{
			((Elsu)obj).UserCode_DrawLineRPCA__Vector3(reader.ReadVector3());
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
			((Elsu)obj).UserCode_SonRpc();
		}
	}

	protected void UserCode_NCom()
	{
		NRpc();
	}

	protected static void InvokeUserCode_NCom(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command NCom called on client.");
		}
		else
		{
			((Elsu)obj).UserCode_NCom();
		}
	}

	protected void UserCode_NRpc()
	{
		aS.PlayOneShot(nhamA);
	}

	protected static void InvokeUserCode_NRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC NRpc called on server.");
		}
		else
		{
			((Elsu)obj).UserCode_NRpc();
		}
	}

	static Elsu()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Elsu), "System.Void Elsu::Chieu1cmd()", InvokeUserCode_Chieu1cmd, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Elsu), "System.Void Elsu::HetTH()", InvokeUserCode_HetTH, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Elsu), "System.Void Elsu::C2Com()", InvokeUserCode_C2Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Elsu), "System.Void Elsu::C3Com()", InvokeUserCode_C3Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Elsu), "System.Void Elsu::DTCom()", InvokeUserCode_DTCom, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Elsu), "System.Void Elsu::DrawLine(UnityEngine.Vector3)", InvokeUserCode_DrawLine__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Elsu), "System.Void Elsu::DrawLineA(UnityEngine.Vector3)", InvokeUserCode_DrawLineA__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Elsu), "System.Void Elsu::NCom()", InvokeUserCode_NCom, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(Elsu), "System.Void Elsu::THRpc()", InvokeUserCode_THRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Elsu), "System.Void Elsu::HetTHRpc()", InvokeUserCode_HetTHRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Elsu), "System.Void Elsu::C2Rpc()", InvokeUserCode_C2Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Elsu), "System.Void Elsu::C3Rpc()", InvokeUserCode_C3Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Elsu), "System.Void Elsu::DTRpc()", InvokeUserCode_DTRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Elsu), "System.Void Elsu::DrawLineRPC(UnityEngine.Vector3)", InvokeUserCode_DrawLineRPC__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(Elsu), "System.Void Elsu::DrawLineRPCA(UnityEngine.Vector3)", InvokeUserCode_DrawLineRPCA__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(Elsu), "System.Void Elsu::SonRpc()", InvokeUserCode_SonRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Elsu), "System.Void Elsu::NRpc()", InvokeUserCode_NRpc);
	}
}
