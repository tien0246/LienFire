using System;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Florentino : NetworkBehaviour
{
	public AudioClip nemA;

	public AudioClip chemA;

	public AudioClip nhacFloA;

	private float tdLuot;

	private bool dangLuot;

	private Vector3 huongLuot;

	public float tamNem;

	public GameObject hoa;

	public float tamDanhC2;

	public float doLonC2;

	public float tdLuotC2;

	public float tgLuotC2;

	public int damC2;

	public DynamicJoystick joy;

	public float tgNapC3;

	public float tdLuotC3;

	public float tgLuotC3;

	public float tamDanhC3;

	public float doLonC3;

	public int damC3;

	public GameObject baHoa;

	private bool dangC3;

	public float tgC3;

	public int hoiMauC3;

	public GameObject doNT;

	public float tdLuotNT;

	public float tgLuotNT;

	public float tamDanhNT;

	public float doLonNT;

	public int damNT;

	public bool noiTai;

	public float tGHoiNoiTai;

	public float tGHoiNoiTaiT;

	public int damDT;

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
		if (tGHoiNoiTai > 0f)
		{
			tGHoiNoiTai -= Time.deltaTime;
		}
		else if (!noiTai)
		{
			noiTai = true;
			doNT.SetActive(value: true);
		}
	}

	private void FixedUpdate()
	{
		if (dangLuot)
		{
			cc.Move(huongLuot * tdLuot * Time.deltaTime);
		}
	}

	private void Chieu1(object sender, EventArgs e)
	{
		Quaternion quay = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamNem - base.transform.position, Vector3.up);
		Chieu1cmd(quay);
	}

	[Command]
	private void Chieu1cmd(Quaternion quay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(quay);
		SendCommandInternal("System.Void Florentino::Chieu1cmd(UnityEngine.Quaternion)", 1452615453, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C1Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Florentino::C1Rpc()", -1792746132, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu2(object sender, EventArgs e)
	{
		C2Com();
		Vector3 direction = cam.transform.position + cam.transform.forward * 7f - base.transform.position;
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonC2, direction, tamDanhC2);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && pS.team != component.team)
			{
				component.MatMau(damC2 * sonTang);
				if (dangC3)
				{
					pS.HoiMau(hoiMauC3);
				}
			}
		}
		Vector2 vector = new Vector2(joy.Horizontal, joy.Vertical);
		if (vector != Vector2.zero)
		{
			pS.DungYen(tgLuotC2);
			dangLuot = true;
			tdLuot = tdLuotC2;
			huongLuot = base.transform.TransformDirection(new Vector3(vector.x, 0f, vector.y)).normalized;
			Invoke("HetLuot", tgLuotC2);
		}
	}

	[Command]
	private void C2Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Florentino::C2Com()", -1777989977, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C2Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Florentino::C2Rpc()", -1764116981, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Chieu3(object sender, EventArgs e)
	{
		pS.DungYen(tgNapC3);
		dangC3 = true;
		C3Com();
		Invoke("LuotC3", tgNapC3);
		Invoke("HetC3", tgC3);
	}

	private void LuotC3()
	{
		bool flag = true;
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonC3, base.transform.forward, tamDanhC3);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && pS.team != component.team)
			{
				if (flag)
				{
					TaoHoa(component.transform.position);
					flag = false;
				}
				Debug.Log("ok");
				component.MatMau(damC3 * sonTang);
			}
		}
		tdLuot = tdLuotC3;
		dangLuot = true;
		huongLuot = base.transform.forward;
		Invoke("HetLuot", tgLuotC3);
	}

	[Command]
	private void C3Com()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Florentino::C3Com()", -1749360826, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void C3Rpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Florentino::C3Rpc()", -1735487830, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void HetC3()
	{
		dangC3 = false;
	}

	[Command]
	private void TaoHoa(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendCommandInternal("System.Void Florentino::TaoHoa(UnityEngine.Vector3)", 62608468, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void DanhThuong(object sender, EventArgs e)
	{
		DTCom();
		if (!noiTai)
		{
			Vector3 direction = cam.transform.position + cam.transform.forward * 7f - base.transform.position;
			RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLonC2, direction, tamDanhC2);
			for (int i = 0; i < array.Length; i++)
			{
				PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
				if (component != null && pS.team != component.team)
				{
					component.MatMau(damDT * sonTang);
				}
			}
			return;
		}
		noiTai = false;
		doNT.SetActive(value: false);
		tGHoiNoiTai = tGHoiNoiTaiT;
		RaycastHit[] array2 = Physics.SphereCastAll(base.transform.position, doLonNT, base.transform.forward, tamDanhNT);
		for (int j = 0; j < array2.Length; j++)
		{
			PlayerStat component2 = array2[j].transform.GetComponent<PlayerStat>();
			if (component2 != null && pS.team != component2.team)
			{
				component2.MatMau(damNT * sonTang);
				if (dangC3)
				{
					pS.HoiMau(hoiMauC3);
				}
			}
		}
		pS.DungYen(tgLuotNT);
		tdLuot = tdLuotNT;
		dangLuot = true;
		huongLuot = base.transform.forward;
		Invoke("HetLuot", tgLuotNT);
	}

	[Command]
	private void DTCom()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Florentino::DTCom()", 82904838, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DTRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Florentino::DTRpc()", 96777834, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void OnDrawGizmos()
	{
	}

	[ClientRpc]
	public void NhatHoa()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Florentino::NhatHoa()", -1364584030, writer, 0, includeOwner: true);
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
		SendRPCInternal("System.Void Florentino::SonRpc()", -1955352992, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_Chieu1cmd__Quaternion(Quaternion quay)
	{
		C1Rpc();
		GameObject obj = UnityEngine.Object.Instantiate(hoa, base.transform.transform.position, quay);
		obj.GetComponent<ProjHit>().doi = pS.team;
		obj.GetComponent<ProjHit>().mauMat *= sonTang;
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
			((Florentino)obj).UserCode_Chieu1cmd__Quaternion(reader.ReadQuaternion());
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
			((Florentino)obj).UserCode_C1Rpc();
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
			((Florentino)obj).UserCode_C2Com();
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
			((Florentino)obj).UserCode_C2Rpc();
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
			((Florentino)obj).UserCode_C3Com();
		}
	}

	protected void UserCode_C3Rpc()
	{
		aS.PlayOneShot(nhacFloA);
	}

	protected static void InvokeUserCode_C3Rpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC C3Rpc called on server.");
		}
		else
		{
			((Florentino)obj).UserCode_C3Rpc();
		}
	}

	protected void UserCode_TaoHoa__Vector3(Vector3 vt)
	{
		for (int i = 0; i < 3; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(baHoa, vt, Quaternion.Euler(0f, base.transform.rotation.y + (float)(i * 120), 0f));
			obj.GetComponent<NhatHoa>().Networkdoi = pS.team;
			NetworkServer.Spawn(obj);
		}
	}

	protected static void InvokeUserCode_TaoHoa__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command TaoHoa called on client.");
		}
		else
		{
			((Florentino)obj).UserCode_TaoHoa__Vector3(reader.ReadVector3());
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
			((Florentino)obj).UserCode_DTCom();
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
			((Florentino)obj).UserCode_DTRpc();
		}
	}

	protected void UserCode_NhatHoa()
	{
		pA.tgHoiChieu2 = 0f;
		noiTai = true;
		doNT.SetActive(value: true);
		tGHoiNoiTai = 0f;
	}

	protected static void InvokeUserCode_NhatHoa(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC NhatHoa called on server.");
		}
		else
		{
			((Florentino)obj).UserCode_NhatHoa();
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
			((Florentino)obj).UserCode_SonRpc();
		}
	}

	static Florentino()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Florentino), "System.Void Florentino::Chieu1cmd(UnityEngine.Quaternion)", InvokeUserCode_Chieu1cmd__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Florentino), "System.Void Florentino::C2Com()", InvokeUserCode_C2Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Florentino), "System.Void Florentino::C3Com()", InvokeUserCode_C3Com, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Florentino), "System.Void Florentino::TaoHoa(UnityEngine.Vector3)", InvokeUserCode_TaoHoa__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Florentino), "System.Void Florentino::DTCom()", InvokeUserCode_DTCom, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(Florentino), "System.Void Florentino::C1Rpc()", InvokeUserCode_C1Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Florentino), "System.Void Florentino::C2Rpc()", InvokeUserCode_C2Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Florentino), "System.Void Florentino::C3Rpc()", InvokeUserCode_C3Rpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Florentino), "System.Void Florentino::DTRpc()", InvokeUserCode_DTRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(Florentino), "System.Void Florentino::NhatHoa()", InvokeUserCode_NhatHoa);
		RemoteProcedureCalls.RegisterRpc(typeof(Florentino), "System.Void Florentino::SonRpc()", InvokeUserCode_SonRpc);
	}
}
