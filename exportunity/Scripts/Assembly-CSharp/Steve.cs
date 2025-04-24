using System;
using System.Collections;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Steve : NetworkBehaviour
{
	public AudioClip chemA;

	public AudioClip noA;

	public Transform vtBan2;

	public GameObject luuDan;

	public float tamNemC1;

	public AudioClip nemC1A;

	public GameObject enderP;

	public float tamNemC2;

	public float doCaoC2;

	public AudioClip nemC2A;

	public int mauHoi;

	public int soHoiC3;

	public float tg1HoiC3;

	public float tgAn;

	public GameObject paC3;

	public GameObject taoVang;

	public GameObject kiem;

	public int damDT;

	public float tamDanhDT;

	public float doLonDT;

	public Material kimCuong;

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
	}

	private void Chieu1(object sender, EventArgs e)
	{
		Quaternion quay = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamNemC1 - vtBan2.position, Vector3.up);
		Chieu1cmd(quay);
	}

	[Command]
	private void Chieu1cmd(Quaternion quay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(quay);
		SendCommandInternal("System.Void Steve::Chieu1cmd(UnityEngine.Quaternion)", -23166214, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C1Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Steve::C1Rpc()", -2010721399, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu2(object sender, EventArgs e)
	{
		Quaternion quay = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamNemC1 - vtBan2.position, Vector3.up);
		Chieu2cmd(quay, base.gameObject);
	}

	[Command]
	private void Chieu2cmd(Quaternion quay, GameObject tran)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(quay);
		writer.WriteGameObject(tran);
		SendCommandInternal("System.Void Steve::Chieu2cmd(UnityEngine.Quaternion,UnityEngine.GameObject)", 322734341, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C2Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Steve::C2Rpc()", -1982092248, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void Tele(Vector3 vt)
	{
		if (base.isLocalPlayer)
		{
			GetComponent<CharacterController>().enabled = false;
			base.transform.position = vt + Vector3.up * doCaoC2;
			GetComponent<CharacterController>().enabled = true;
		}
	}

	private void Chieu3(object sender, EventArgs e)
	{
		pS.DungYen(tgAn);
		C3Com();
	}

	[Command]
	private void C3Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Steve::C3Com()", -1967336093, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private IEnumerator Chieu3I()
	{
		C3Rpc();
		yield return new WaitForSeconds(tgAn);
		HetC3Rpc();
		for (int i = 0; i < soHoiC3; i++)
		{
			pS.HoiMau(mauHoi);
			yield return new WaitForSeconds(tg1HoiC3);
		}
		HetHieuUngRpc();
	}

	[ClientRpc]
	private void C3Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Steve::C3Rpc()", -1953463097, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void HetC3Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Steve::HetC3Rpc()", 831577390, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void HetHieuUngRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Steve::HetHieuUngRpc()", -2103980271, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void DanhThuong(object sender, EventArgs e)
	{
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
		SendCommandInternal("System.Void Steve::DTCom()", -135070429, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DTRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Steve::DTRpc()", -121197433, writer, 0, includeOwner: true);
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
		SendRPCInternal("System.Void Steve::SonRpc()", -122651677, writer, 0, includeOwner: true);
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
			((Steve)obj).UserCode_Chieu1cmd__Quaternion(reader.ReadQuaternion());
		}
	}

	protected void UserCode_C1Rpc()
	{
		aS.PlayOneShot(nemC1A);
	}

	protected static void InvokeUserCode_C1Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C1Rpc called on server.");
		}
		else
		{
			((Steve)obj).UserCode_C1Rpc();
		}
	}

	protected void UserCode_Chieu2cmd__Quaternion__GameObject(Quaternion quay, GameObject tran)
	{
		C2Rpc();
		GameObject obj = UnityEngine.Object.Instantiate(enderP, vtBan2.transform.position, quay);
		obj.GetComponent<EnderP>().Networkplayer = tran;
		NetworkServer.Spawn(obj);
	}

	protected static void InvokeUserCode_Chieu2cmd__Quaternion__GameObject(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Chieu2cmd called on client.");
		}
		else
		{
			((Steve)obj).UserCode_Chieu2cmd__Quaternion__GameObject(reader.ReadQuaternion(), reader.ReadGameObject());
		}
	}

	protected void UserCode_C2Rpc()
	{
		aS.PlayOneShot(nemC1A);
	}

	protected static void InvokeUserCode_C2Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C2Rpc called on server.");
		}
		else
		{
			((Steve)obj).UserCode_C2Rpc();
		}
	}

	protected void UserCode_C3Com()
	{
		StartCoroutine(Chieu3I());
	}

	protected static void InvokeUserCode_C3Com(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command C3Com called on client.");
		}
		else
		{
			((Steve)obj).UserCode_C3Com();
		}
	}

	protected void UserCode_C3Rpc()
	{
		taoVang.SetActive(value: true);
		kiem.SetActive(value: false);
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
			((Steve)obj).UserCode_C3Rpc();
		}
	}

	protected void UserCode_HetC3Rpc()
	{
		taoVang.SetActive(value: false);
		kiem.SetActive(value: true);
		paC3.SetActive(value: true);
	}

	protected static void InvokeUserCode_HetC3Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC HetC3Rpc called on server.");
		}
		else
		{
			((Steve)obj).UserCode_HetC3Rpc();
		}
	}

	protected void UserCode_HetHieuUngRpc()
	{
		paC3.SetActive(value: false);
	}

	protected static void InvokeUserCode_HetHieuUngRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC HetHieuUngRpc called on server.");
		}
		else
		{
			((Steve)obj).UserCode_HetHieuUngRpc();
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
			((Steve)obj).UserCode_DTCom();
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
			((Steve)obj).UserCode_DTRpc();
		}
	}

	protected void UserCode_SonRpc()
	{
		for (int i = 0; i < mesh.Length; i++)
		{
			mesh[i].material = kimCuong;
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
			((Steve)obj).UserCode_SonRpc();
		}
	}

	static Steve()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Steve), "System.Void Steve::Chieu1cmd(UnityEngine.Quaternion)", InvokeUserCode_Chieu1cmd__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Steve), "System.Void Steve::Chieu2cmd(UnityEngine.Quaternion,UnityEngine.GameObject)", InvokeUserCode_Chieu2cmd__Quaternion__GameObject, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Steve), "System.Void Steve::C3Com()", InvokeUserCode_C3Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Steve), "System.Void Steve::DTCom()", InvokeUserCode_DTCom, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(Steve), "System.Void Steve::C1Rpc()", InvokeUserCode_C1Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Steve), "System.Void Steve::C2Rpc()", InvokeUserCode_C2Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Steve), "System.Void Steve::C3Rpc()", InvokeUserCode_C3Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Steve), "System.Void Steve::HetC3Rpc()", InvokeUserCode_HetC3Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Steve), "System.Void Steve::HetHieuUngRpc()", InvokeUserCode_HetHieuUngRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Steve), "System.Void Steve::DTRpc()", InvokeUserCode_DTRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Steve), "System.Void Steve::SonRpc()", InvokeUserCode_SonRpc);
	}
}
