using System;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using TMPro;
using UnityEngine;

public class PlayerAtack : NetworkBehaviour
{
	public AudioClip sonS;

	public AudioClip eatS;

	public Camera cam;

	public LayerMask layer;

	public TextMeshProUGUI tgHoiTC1;

	public TextMeshProUGUI tgHoiTC2;

	public TextMeshProUGUI tgHoiTC3;

	private Animator ani;

	public float tgHoiChieu1;

	public float tgHoiChieu2;

	public float tgHoiChieu3;

	private bool C1Hoi;

	private bool C2Hoi;

	private bool C3Hoi;

	private float koDanh;

	public float tgHoi1;

	public float tgHoi2;

	public float tgHoi3;

	public float tocDanh;

	private bool dungYen;

	[SyncVar]
	public int boTro;

	private bool btHoi;

	public float tgHoiBT;

	public TextMeshProUGUI tgHoiT_BT;

	public GameObject[] nutBT;

	public float kcBomKeo;

	public AudioClip bkA;

	public float tgHoiKeo;

	public GameObject bomKeo;

	public float tgTocBien;

	public float kcTocBien;

	public float doCaoTB;

	public AudioClip tbA;

	public float tgNhac;

	public GameObject nhacH;

	public AudioClip nhac;

	public float tgHB;

	public float tgHoa;

	private bool hb;

	public GameObject buiCay;

	public GameObject model;

	public GameObject thanh;

	[SyncVar]
	private float bX = 1f;

	public int NetworkboTro
	{
		get
		{
			return boTro;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref boTro, 1uL, null);
		}
	}

	public float NetworkbX
	{
		get
		{
			return bX;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref bX, 2uL, null);
		}
	}

	public event EventHandler DungChieu1;

	public event EventHandler DungChieu2;

	public event EventHandler DungChieu3;

	public event EventHandler DungDanhThuong;

	public event EventHandler SkinSung;

	private void Start()
	{
		ani = GetComponent<Animator>();
		if (nutBT[boTro] != null)
		{
			nutBT[boTro].SetActive(value: true);
		}
	}

	private void Update()
	{
		dungYen = GetComponent<PlayerStat>().dungYen;
		if (tgHoiChieu1 > 0f)
		{
			tgHoiChieu1 -= Time.deltaTime * bX;
			tgHoiTC1.text = MathF.Round(tgHoiChieu1).ToString();
		}
		else if (!C1Hoi)
		{
			tgHoiTC1.text = string.Empty;
			C1Hoi = true;
		}
		if (tgHoiChieu2 > 0f)
		{
			tgHoiChieu2 -= Time.deltaTime * bX;
			tgHoiTC2.text = MathF.Round(tgHoiChieu2).ToString();
		}
		else if (!C2Hoi)
		{
			tgHoiTC2.text = string.Empty;
			C2Hoi = true;
		}
		if (tgHoiChieu3 > 0f)
		{
			tgHoiChieu3 -= Time.deltaTime * bX;
			tgHoiTC3.text = MathF.Round(tgHoiChieu3).ToString();
		}
		else if (!C3Hoi)
		{
			tgHoiTC3.text = string.Empty;
			C3Hoi = true;
		}
		if (tgHoiBT > 0f)
		{
			tgHoiBT -= Time.deltaTime * bX;
			tgHoiT_BT.text = MathF.Round(tgHoiBT).ToString();
		}
		else if (!btHoi)
		{
			tgHoiT_BT.text = string.Empty;
			btHoi = true;
		}
		if (koDanh > 0f)
		{
			koDanh -= Time.deltaTime;
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			DanhThuong();
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			Chieu1();
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			Chieu2();
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			Chieu3();
		}
	}

	public void Chieu1()
	{
		if (base.isLocalPlayer && this.DungChieu1 != null && !dungYen && C1Hoi)
		{
			if (hb)
			{
				HetHB();
			}
			ani.SetTrigger("C1");
			C1Hoi = false;
			tgHoiChieu1 = tgHoi1;
			this.DungChieu1(this, EventArgs.Empty);
		}
	}

	public void Chieu2()
	{
		if (base.isLocalPlayer && this.DungChieu2 != null && !dungYen && C2Hoi)
		{
			if (hb)
			{
				HetHB();
			}
			ani.SetTrigger("C2");
			C2Hoi = false;
			tgHoiChieu2 = tgHoi2;
			this.DungChieu2(this, EventArgs.Empty);
		}
	}

	public void Chieu3()
	{
		if (base.isLocalPlayer && this.DungChieu3 != null && !dungYen && C3Hoi)
		{
			if (hb)
			{
				HetHB();
			}
			ani.SetTrigger("C3");
			C3Hoi = false;
			tgHoiChieu3 = tgHoi3;
			this.DungChieu3(this, EventArgs.Empty);
		}
	}

	public void DanhThuong()
	{
		if (base.isLocalPlayer && this.DungDanhThuong != null && !dungYen && koDanh <= 0f)
		{
			if (hb)
			{
				HetHB();
			}
			koDanh = tocDanh;
			ani.SetTrigger("DT");
			this.DungDanhThuong(this, EventArgs.Empty);
		}
	}

	public void BomKeo()
	{
		if (base.isLocalPlayer && !dungYen && btHoi && Physics.Raycast(cam.transform.position, cam.transform.forward, out var hitInfo, kcBomKeo, layer))
		{
			btHoi = false;
			tgHoiBT = tgHoiKeo;
			Vector3 point = hitInfo.point;
			Quaternion quay = Quaternion.LookRotation(base.transform.forward);
			BomKeoCmd(point, quay);
		}
	}

	[Command]
	private void BomKeoCmd(Vector3 vt, Quaternion quay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		writer.WriteQuaternion(quay);
		SendCommandInternal("System.Void PlayerAtack::BomKeoCmd(UnityEngine.Vector3,UnityEngine.Quaternion)", 1659035085, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void BomKeoRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerAtack::BomKeoRpc()", -1432372704, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void NhacHoi()
	{
		if (base.isLocalPlayer && !dungYen && btHoi)
		{
			btHoi = false;
			tgHoiBT = tgNhac;
			NhacHoiCmd(base.transform);
		}
	}

	[Command]
	private void NhacHoiCmd(Transform tr)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteTransform(tr);
		SendCommandInternal("System.Void PlayerAtack::NhacHoiCmd(UnityEngine.Transform)", -70303497, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void NhacHoiRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerAtack::NhacHoiRpc()", -947853327, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void HoaBui()
	{
		if (base.isLocalPlayer && !dungYen && btHoi)
		{
			btHoi = false;
			tgHoiBT = tgHoa;
			HoaBuiCmd();
		}
	}

	[Command]
	private void HoaBuiCmd()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerAtack::HoaBuiCmd()", -785512146, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void HBRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerAtack::HBRpc()", -1736719269, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void HetHB()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerAtack::HetHB()", -704522623, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void HetHBRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerAtack::HetHBRpc()", 1112713222, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void TocBien()
	{
		if (base.isLocalPlayer && !dungYen && btHoi && Physics.Raycast(cam.transform.position, cam.transform.forward, out var hitInfo, kcTocBien, layer))
		{
			TBCmd();
			btHoi = false;
			tgHoiBT = tgTocBien;
			Vector3 point = hitInfo.point;
			GetComponent<CharacterController>().enabled = false;
			base.transform.position = point + Vector3.up * doCaoTB;
			GetComponent<CharacterController>().enabled = true;
		}
	}

	[Command]
	private void TBCmd()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerAtack::TBCmd()", 309449084, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void TBRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerAtack::TBRpc()", 323390311, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void BXanh()
	{
		NetworkbX = 1.75f;
		Eat();
	}

	[ClientRpc]
	private void Eat()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerAtack::Eat()", -2028202616, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void SonSung()
	{
		this.SkinSung(this, EventArgs.Empty);
		Son();
	}

	[ClientRpc]
	private void Son()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerAtack::Son()", -2014862014, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_BomKeoCmd__Vector3__Quaternion(Vector3 vt, Quaternion quay)
	{
		NetworkServer.Spawn(UnityEngine.Object.Instantiate(bomKeo, vt, quay));
		BomKeoRpc();
	}

	protected static void InvokeUserCode_BomKeoCmd__Vector3__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command BomKeoCmd called on client.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_BomKeoCmd__Vector3__Quaternion(reader.ReadVector3(), reader.ReadQuaternion());
		}
	}

	protected void UserCode_BomKeoRpc()
	{
		base.gameObject.GetComponent<AudioSource>().PlayOneShot(bkA, 0.2f);
	}

	protected static void InvokeUserCode_BomKeoRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC BomKeoRpc called on server.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_BomKeoRpc();
		}
	}

	protected void UserCode_NhacHoiCmd__Transform(Transform tr)
	{
		GameObject obj = UnityEngine.Object.Instantiate(nhacH, base.transform.position, nhacH.transform.rotation);
		obj.GetComponent<NhacHoi>().Networkteam = GetComponent<PlayerStat>().team;
		obj.GetComponent<NhacHoi>().Networktr = tr;
		NetworkServer.Spawn(obj);
		NhacHoiRpc();
	}

	protected static void InvokeUserCode_NhacHoiCmd__Transform(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command NhacHoiCmd called on client.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_NhacHoiCmd__Transform(reader.ReadTransform());
		}
	}

	protected void UserCode_NhacHoiRpc()
	{
		base.gameObject.GetComponent<AudioSource>().PlayOneShot(nhac);
	}

	protected static void InvokeUserCode_NhacHoiRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC NhacHoiRpc called on server.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_NhacHoiRpc();
		}
	}

	protected void UserCode_HoaBuiCmd()
	{
		HBRpc();
		Invoke("HetHBRpc", tgHB);
	}

	protected static void InvokeUserCode_HoaBuiCmd(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command HoaBuiCmd called on client.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_HoaBuiCmd();
		}
	}

	protected void UserCode_HBRpc()
	{
		hb = true;
		model.SetActive(value: false);
		thanh.SetActive(value: false);
		buiCay.SetActive(value: true);
		GetComponent<PlayerStat>().NetworkdangKhieng = true;
	}

	protected static void InvokeUserCode_HBRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC HBRpc called on server.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_HBRpc();
		}
	}

	protected void UserCode_HetHB()
	{
		HetHBRpc();
	}

	protected static void InvokeUserCode_HetHB(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command HetHB called on client.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_HetHB();
		}
	}

	protected void UserCode_HetHBRpc()
	{
		hb = false;
		model.SetActive(value: true);
		GetComponent<PlayerStat>().NetworkdangKhieng = false;
		thanh.SetActive(value: true);
		buiCay.SetActive(value: false);
	}

	protected static void InvokeUserCode_HetHBRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC HetHBRpc called on server.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_HetHBRpc();
		}
	}

	protected void UserCode_TBCmd()
	{
		TBRpc();
	}

	protected static void InvokeUserCode_TBCmd(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command TBCmd called on client.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_TBCmd();
		}
	}

	protected void UserCode_TBRpc()
	{
		base.gameObject.GetComponent<AudioSource>().PlayOneShot(tbA, 0.2f);
	}

	protected static void InvokeUserCode_TBRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC TBRpc called on server.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_TBRpc();
		}
	}

	protected void UserCode_Eat()
	{
		GetComponent<AudioSource>().PlayOneShot(eatS);
	}

	protected static void InvokeUserCode_Eat(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Eat called on server.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_Eat();
		}
	}

	protected void UserCode_Son()
	{
		GetComponent<AudioSource>().PlayOneShot(sonS);
	}

	protected static void InvokeUserCode_Son(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Son called on server.");
		}
		else
		{
			((PlayerAtack)obj).UserCode_Son();
		}
	}

	static PlayerAtack()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAtack), "System.Void PlayerAtack::BomKeoCmd(UnityEngine.Vector3,UnityEngine.Quaternion)", InvokeUserCode_BomKeoCmd__Vector3__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAtack), "System.Void PlayerAtack::NhacHoiCmd(UnityEngine.Transform)", InvokeUserCode_NhacHoiCmd__Transform, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAtack), "System.Void PlayerAtack::HoaBuiCmd()", InvokeUserCode_HoaBuiCmd, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAtack), "System.Void PlayerAtack::HetHB()", InvokeUserCode_HetHB, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerAtack), "System.Void PlayerAtack::TBCmd()", InvokeUserCode_TBCmd, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAtack), "System.Void PlayerAtack::BomKeoRpc()", InvokeUserCode_BomKeoRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAtack), "System.Void PlayerAtack::NhacHoiRpc()", InvokeUserCode_NhacHoiRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAtack), "System.Void PlayerAtack::HBRpc()", InvokeUserCode_HBRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAtack), "System.Void PlayerAtack::HetHBRpc()", InvokeUserCode_HetHBRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAtack), "System.Void PlayerAtack::TBRpc()", InvokeUserCode_TBRpc);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAtack), "System.Void PlayerAtack::Eat()", InvokeUserCode_Eat);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerAtack), "System.Void PlayerAtack::Son()", InvokeUserCode_Son);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(boTro);
			writer.WriteFloat(bX);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(boTro);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteFloat(bX);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref boTro, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref bX, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref boTro, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref bX, null, reader.ReadFloat());
		}
	}
}
