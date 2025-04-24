using System;
using System.Collections;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Toro : NetworkBehaviour
{
	public AudioClip chemA;

	public AudioClip noA;

	public GameObject vongDo;

	public float tgLamCham;

	private float tdLuot;

	private bool dangLuot;

	private Vector3 huongLuot;

	public int damC1;

	public float tgLuotC1;

	public float tgChoangC1;

	public float tdLuotC1;

	public float tamDanhC1;

	public float doLonC1;

	public float tgNapC2;

	public int damC2;

	public float doLonC2;

	public float tgDanhC3;

	public float tgC3;

	public float tgLuotC3;

	public float tgChoangC3;

	public int damC3;

	public float doLonC3;

	public float tdLuotC3;

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

	private CharacterController cc;

	private void Start()
	{
		cc = GetComponent<CharacterController>();
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
		if (dangLuot)
		{
			cc.Move(huongLuot * tdLuot * Time.deltaTime);
		}
	}

	private void OnDrawGizmos()
	{
	}

	private void Chieu1(object sender, EventArgs e)
	{
		C1Com();
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonC1, base.transform.forward, tamDanhC1);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && pS.team != component.team)
			{
				component.MatMau(damC1 * sonTang);
				component.DungYen(tgChoangC1);
			}
		}
		pS.DungYen(tgLuotC1);
		tdLuot = tdLuotC1;
		dangLuot = true;
		huongLuot = base.transform.forward;
		Invoke("HetLuot", tgLuotC1);
	}

	[Command]
	private void C1Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Toro::C1Com()", -124518248, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C1Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Toro::C1Rpc()", -110645252, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu2(object sender, EventArgs e)
	{
		pS.DungYen(tgNapC2 + 0.1f);
		ModelC(t: true);
		Invoke("NoC2", tgNapC2);
	}

	private void NoC2()
	{
		ModelC(t: false);
		C2Com();
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonC2, base.transform.forward, 1f);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && component.team != pS.team)
			{
				component.MatMau(damC2 * sonTang);
				component.LamCham(tgLamCham);
			}
		}
	}

	[Command]
	private void C2Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Toro::C2Com()", -95889097, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C2Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Toro::C2Rpc()", -82016101, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu3(object sender, EventArgs e)
	{
		StartCoroutine(Chieu3I());
	}

	private IEnumerator Chieu3I()
	{
		pS.DungYen(tgC3);
		tdLuot = tdLuotC3;
		dangLuot = true;
		huongLuot = base.transform.forward;
		yield return new WaitForSeconds(tgLuotC3);
		ModelC(t: true);
		dangLuot = false;
		for (int i = 0; i < 3; i++)
		{
			C2Com();
			RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonC3, base.transform.forward, 1f);
			for (int j = 0; j < array.Length; j++)
			{
				PlayerStat component = array[j].transform.GetComponent<PlayerStat>();
				if (component != null && component.team != pS.team)
				{
					component.MatMau(damC3 * sonTang);
					component.LamCham(tgLamCham);
					if (i == 2)
					{
						component.DungYen(tgChoangC3);
					}
				}
			}
			yield return new WaitForSeconds(tgDanhC3);
		}
		ModelC(t: false);
	}

	private void DanhThuong(object sender, EventArgs e)
	{
		C1Com();
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

	private void HetLuot()
	{
		dangLuot = false;
	}

	[Command]
	private void ModelC(bool t)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(t);
		SendCommandInternal("System.Void Toro::ModelC(System.Boolean)", -1621970802, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void ModelCR(bool t)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(t);
		SendRPCInternal("System.Void Toro::ModelCR(System.Boolean)", -1049972748, writer, 0, includeOwner: true);
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
		SendRPCInternal("System.Void Toro::SonRpc()", -1349833264, writer, 0, includeOwner: true);
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
			((Toro)obj).UserCode_C1Com();
		}
	}

	protected void UserCode_C1Rpc()
	{
		aS.PlayOneShot(chemA);
	}

	protected static void InvokeUserCode_C1Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C1Rpc called on server.");
		}
		else
		{
			((Toro)obj).UserCode_C1Rpc();
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
			((Toro)obj).UserCode_C2Com();
		}
	}

	protected void UserCode_C2Rpc()
	{
		aS.PlayOneShot(noA);
	}

	protected static void InvokeUserCode_C2Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C2Rpc called on server.");
		}
		else
		{
			((Toro)obj).UserCode_C2Rpc();
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
			((Toro)obj).UserCode_ModelC__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_ModelCR__Boolean(bool t)
	{
		vongDo.SetActive(t);
	}

	protected static void InvokeUserCode_ModelCR__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC ModelCR called on server.");
		}
		else
		{
			((Toro)obj).UserCode_ModelCR__Boolean(reader.ReadBool());
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
			((Toro)obj).UserCode_SonRpc();
		}
	}

	static Toro()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Toro), "System.Void Toro::C1Com()", InvokeUserCode_C1Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Toro), "System.Void Toro::C2Com()", InvokeUserCode_C2Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Toro), "System.Void Toro::ModelC(System.Boolean)", InvokeUserCode_ModelC__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(Toro), "System.Void Toro::C1Rpc()", InvokeUserCode_C1Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Toro), "System.Void Toro::C2Rpc()", InvokeUserCode_C2Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Toro), "System.Void Toro::ModelCR(System.Boolean)", InvokeUserCode_ModelCR__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(Toro), "System.Void Toro::SonRpc()", InvokeUserCode_SonRpc);
	}
}
