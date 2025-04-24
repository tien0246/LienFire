using System;
using System.Collections;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Alen : NetworkBehaviour
{
	public AudioClip chemA;

	public AudioClip chemC1A;

	public AudioClip noA;

	private float tdLuot;

	private bool dangLuot;

	private Vector3 huongLuot;

	private Transform vtC3;

	private bool dangC1;

	public float tgC1;

	public int soChemC1;

	public float tg1ChemC1;

	public int damC1;

	public float tamDanhC1;

	public float doLonC1;

	public float tgLuotC2;

	public float tgChamC2;

	public float tdLuotC2;

	public float tamDanhC2;

	public float doLonC2;

	public GameObject vongDo;

	public GameObject model;

	public float tgC3;

	public float tamDanhC3;

	public float doLonC3;

	private bool dangC3;

	public int damC3;

	public float doLonNoC3;

	public int damDT;

	public float tamDanhDT;

	public float doLonDT;

	public Material vang;

	public MeshRenderer[] mesh;

	private int sonTang = 1;

	public Camera cam;

	private PlayerAtack pA;

	private PlayerStat pS;

	private AudioSource aS;

	private PlayerLook pL;

	private CharacterController cc;

	private void Start()
	{
		cc = GetComponent<CharacterController>();
		pS = GetComponent<PlayerStat>();
		aS = GetComponent<AudioSource>();
		pL = GetComponent<PlayerLook>();
		pA = GetComponent<PlayerAtack>();
		pA.DungChieu1 += Chieu1;
		pA.DungChieu2 += Chieu2;
		pA.DungChieu3 += Chieu3;
		pA.DungDanhThuong += DanhThuong;
		pA.SkinSung += Son;
	}

	private void Update()
	{
		if (dangLuot)
		{
			cc.Move(huongLuot * tdLuot * Time.deltaTime);
		}
		if (dangC3)
		{
			base.transform.position = vtC3.position;
		}
	}

	private void Chieu1(object sender, EventArgs e)
	{
		pL.duocQuay = false;
		dangC1 = true;
		pS.LamCham(tgC1);
		StartCoroutine(Chieu1I());
	}

	private IEnumerator Chieu1I()
	{
		for (int i = 0; i < soChemC1; i++)
		{
			if (dangC3)
			{
				continue;
			}
			C1Com();
			RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonC1, base.transform.forward, tamDanhC1);
			for (int j = 0; j < array.Length; j++)
			{
				PlayerStat component = array[j].transform.GetComponent<PlayerStat>();
				if (component != null && pS.team != component.team)
				{
					component.MatMau(damC1 * sonTang);
				}
			}
			yield return new WaitForSeconds(tg1ChemC1);
		}
		dangC1 = false;
		pL.duocQuay = true;
	}

	[Command]
	private void C1Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Alen::C1Com()", -1444999172, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C1Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Alen::C1Rpc()", -1431126176, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu2(object sender, EventArgs e)
	{
		C2Com();
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonC3, base.transform.forward, tamDanhC3);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && pS.team != component.team)
			{
				component.LamCham(tgChamC2);
			}
		}
		pS.DungYen(tgLuotC2);
		tdLuot = tdLuotC2;
		dangLuot = true;
		huongLuot = base.transform.forward;
		Invoke("HetLuot", tgLuotC2);
	}

	[Command]
	private void C2Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Alen::C2Com()", -1416370021, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C2Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Alen::C2Rpc()", -1402497025, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu3(object sender, EventArgs e)
	{
		bool flag = false;
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonC2, base.transform.forward, tamDanhC2);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && pS.team != component.team && !flag)
			{
				TrungC3(component.transform);
				flag = true;
			}
		}
		if (!flag)
		{
			pA.tgHoiChieu3 = 0f;
		}
	}

	private void TrungC3(Transform tran)
	{
		vtC3 = tran;
		dangC3 = true;
		ModelC(t: false);
		pS.DungYen(tgC3);
		Invoke("HetC3", tgC3);
	}

	private void HetC3()
	{
		C3Com();
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonNoC3, base.transform.forward, 1f);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && component.team != pS.team)
			{
				component.MatMau(damC3 * sonTang);
			}
		}
		dangC3 = false;
		ModelC(t: true);
	}

	[Command]
	private void C3Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Alen::C3Com()", -1387740870, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C3Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Alen::C3Rpc()", -1373867874, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void ModelC(bool t)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(t);
		SendCommandInternal("System.Void Alen::ModelC(System.Boolean)", 2007133354, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void ModelCR(bool t)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(t);
		SendRPCInternal("System.Void Alen::ModelCR(System.Boolean)", -216893608, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void DanhThuong(object sender, EventArgs e)
	{
		if (dangC1)
		{
			return;
		}
		DTCom();
		Vector3 direction = cam.transform.position + cam.transform.forward * 7f - base.transform.position;
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonDT, direction, tamDanhDT);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && pS.team != component.team)
			{
				component.MatMau(damDT * sonTang);
			}
		}
	}

	[Command]
	private void DTCom()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Alen::DTCom()", 444524794, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DTRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Alen::DTRpc()", 458397790, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void HetLuot()
	{
		dangLuot = false;
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
		SendRPCInternal("System.Void Alen::SonRpc()", 664931052, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_C1Com()
	{
		C1Rpc();
	}

	protected static void InvokeUserCode_C1Com(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command C1Com called on client.");
		}
		else
		{
			((Alen)obj).UserCode_C1Com();
		}
	}

	protected void UserCode_C1Rpc()
	{
		aS.PlayOneShot(chemC1A);
	}

	protected static void InvokeUserCode_C1Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C1Rpc called on server.");
		}
		else
		{
			((Alen)obj).UserCode_C1Rpc();
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
			((Alen)obj).UserCode_C2Com();
		}
	}

	protected void UserCode_C2Rpc()
	{
		aS.PlayOneShot(chemA);
	}

	protected static void InvokeUserCode_C2Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C2Rpc called on server.");
		}
		else
		{
			((Alen)obj).UserCode_C2Rpc();
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
			((Alen)obj).UserCode_C3Com();
		}
	}

	protected void UserCode_C3Rpc()
	{
		aS.PlayOneShot(noA);
	}

	protected static void InvokeUserCode_C3Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C3Rpc called on server.");
		}
		else
		{
			((Alen)obj).UserCode_C3Rpc();
		}
	}

	protected void UserCode_ModelC__Boolean(bool t)
	{
		ModelCR(t);
	}

	protected static void InvokeUserCode_ModelC__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command ModelC called on client.");
		}
		else
		{
			((Alen)obj).UserCode_ModelC__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_ModelCR__Boolean(bool t)
	{
		model.SetActive(t);
		vongDo.SetActive(!t);
		cc.enabled = t;
	}

	protected static void InvokeUserCode_ModelCR__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC ModelCR called on server.");
		}
		else
		{
			((Alen)obj).UserCode_ModelCR__Boolean(reader.ReadBool());
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
			((Alen)obj).UserCode_DTCom();
		}
	}

	protected void UserCode_DTRpc()
	{
		aS.PlayOneShot(chemA);
	}

	protected static void InvokeUserCode_DTRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DTRpc called on server.");
		}
		else
		{
			((Alen)obj).UserCode_DTRpc();
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
			((Alen)obj).UserCode_SonRpc();
		}
	}

	static Alen()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Alen), "System.Void Alen::C1Com()", InvokeUserCode_C1Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Alen), "System.Void Alen::C2Com()", InvokeUserCode_C2Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Alen), "System.Void Alen::C3Com()", InvokeUserCode_C3Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Alen), "System.Void Alen::ModelC(System.Boolean)", InvokeUserCode_ModelC__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Alen), "System.Void Alen::DTCom()", InvokeUserCode_DTCom, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(Alen), "System.Void Alen::C1Rpc()", InvokeUserCode_C1Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Alen), "System.Void Alen::C2Rpc()", InvokeUserCode_C2Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Alen), "System.Void Alen::C3Rpc()", InvokeUserCode_C3Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Alen), "System.Void Alen::ModelCR(System.Boolean)", InvokeUserCode_ModelCR__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(Alen), "System.Void Alen::DTRpc()", InvokeUserCode_DTRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Alen), "System.Void Alen::SonRpc()", InvokeUserCode_SonRpc);
	}
}
