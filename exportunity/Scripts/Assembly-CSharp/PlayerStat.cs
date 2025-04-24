using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using TMPro;
using UnityEngine;

public class PlayerStat : NetworkBehaviour
{
	public AudioClip eatS;

	public AudioClip deathS;

	public bool khacT;

	public GameObject vatPham;

	public GameObject doiTuong;

	private CharacterController cc;

	public GameObject ragdoll;

	public GameObject modelG;

	public GameObject cavan;

	public GameObject xanh;

	[SyncVar]
	public int heath;

	[SyncVar]
	public int team;

	[SyncVar]
	public bool chet;

	[SyncVar]
	public bool khieng;

	[SyncVar]
	public bool dangKhieng;

	public float tgK;

	[SyncVar]
	public bool dungYen;

	public bool biCham;

	public bool laLocal;

	private Transform vtHoiSinh;

	protected float tgDungYen;

	protected float tgBiCham;

	public int maxHeath;

	public float tgHoiSinh;

	public GameObject pa;

	public GameObject hitT;

	public GameObject witherG;

	public float hitToff;

	public GameObject[] model;

	public int Networkheath
	{
		get
		{
			return heath;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref heath, 1uL, null);
		}
	}

	public int Networkteam
	{
		get
		{
			return team;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref team, 2uL, null);
		}
	}

	public bool Networkchet
	{
		get
		{
			return chet;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref chet, 4uL, null);
		}
	}

	public bool Networkkhieng
	{
		get
		{
			return khieng;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref khieng, 8uL, null);
		}
	}

	public bool NetworkdangKhieng
	{
		get
		{
			return dangKhieng;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref dangKhieng, 16uL, null);
		}
	}

	public bool NetworkdungYen
	{
		get
		{
			return dungYen;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref dungYen, 32uL, null);
		}
	}

	private void Start()
	{
		if (!khacT)
		{
			cc = GetComponent<CharacterController>();
			if (!base.isLocalPlayer)
			{
				cavan.SetActive(value: false);
			}
			else
			{
				laLocal = true;
			}
			Networkheath = maxHeath;
			if (team == 1)
			{
				vtHoiSinh = GameObject.Find("redSpon").transform;
			}
			else if (team == 2)
			{
				vtHoiSinh = GameObject.Find("blueSpon").transform;
			}
			else
			{
				vtHoiSinh = base.transform;
			}
		}
	}

	private void Update()
	{
		if (tgBiCham > 0f)
		{
			tgBiCham -= Time.deltaTime;
		}
		else if (biCham)
		{
			biCham = false;
		}
		if (tgDungYen > 0f)
		{
			tgDungYen -= Time.deltaTime;
		}
		else if (dungYen)
		{
			NetworkdungYen = false;
		}
	}

	[Command(requiresAuthority = false)]
	public void DungYen(float tg)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(tg);
		SendCommandInternal("System.Void PlayerStat::DungYen(System.Single)", 1883117899, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void DungYenRpc(float tg)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(tg);
		SendRPCInternal("System.Void PlayerStat::DungYenRpc(System.Single)", 1084728132, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command(requiresAuthority = false)]
	public void LamCham(float tg)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(tg);
		SendCommandInternal("System.Void PlayerStat::LamCham(System.Single)", 892605594, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void LamChamRpc(float tg)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(tg);
		SendRPCInternal("System.Void PlayerStat::LamChamRpc(System.Single)", -842026603, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command(requiresAuthority = false)]
	public virtual void MatMau(int mauMat)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(mauMat);
		SendCommandInternal("System.Void PlayerStat::MatMau(System.Int32)", 1757796268, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void PaRpc(int mauM)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(mauM);
		SendRPCInternal("System.Void PlayerStat::PaRpc(System.Int32)", -262400979, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void OffK()
	{
		NetworkdangKhieng = false;
		Khieng(t: false);
	}

	[ClientRpc]
	private void Khieng(bool t)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(t);
		SendRPCInternal("System.Void PlayerStat::Khieng(System.Boolean)", -664389149, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command(requiresAuthority = false)]
	public void HoiMau(int mauHoi)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(mauHoi);
		SendCommandInternal("System.Void PlayerStat::HoiMau(System.Int32)", -1347860214, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Chet()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerStat::Chet()", 425075790, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void HoiSinh()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerStat::HoiSinh()", -890824870, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void DoiTuongCmd()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerStat::DoiTuongCmd()", -2007781127, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public void BDo()
	{
		Networkkhieng = true;
		Eat();
	}

	[ClientRpc]
	private void Eat()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerStat::Eat()", -2062847296, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void NhinThay(bool t)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(t);
		SendRPCInternal("System.Void PlayerStat::NhinThay(System.Boolean)", -1615453796, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void Wither()
	{
		GameObject gameObject = Object.Instantiate(witherG);
		gameObject.GetComponent<PlayerStat>().Networkteam = team;
		gameObject.transform.position = base.transform.position;
		NetworkServer.ReplacePlayerForConnection(base.connectionToClient, gameObject, keepAuthority: true);
		Object.Destroy(base.gameObject, 0.05f);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_DungYen__Single(float tg)
	{
		DungYenRpc(tg);
	}

	protected static void InvokeUserCode_DungYen__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command DungYen called on client.");
		}
		else
		{
			((PlayerStat)obj).UserCode_DungYen__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_DungYenRpc__Single(float tg)
	{
		NetworkdungYen = true;
		tgDungYen = tg;
	}

	protected static void InvokeUserCode_DungYenRpc__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DungYenRpc called on server.");
		}
		else
		{
			((PlayerStat)obj).UserCode_DungYenRpc__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_LamCham__Single(float tg)
	{
		LamChamRpc(tg);
	}

	protected static void InvokeUserCode_LamCham__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command LamCham called on client.");
		}
		else
		{
			((PlayerStat)obj).UserCode_LamCham__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_LamChamRpc__Single(float tg)
	{
		biCham = true;
		tgBiCham = tg;
	}

	protected static void InvokeUserCode_LamChamRpc__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC LamChamRpc called on server.");
		}
		else
		{
			((PlayerStat)obj).UserCode_LamChamRpc__Single(reader.ReadFloat());
		}
	}

	protected virtual void UserCode_MatMau__Int32(int mauMat)
	{
		if (heath <= 20 && khieng)
		{
			Networkkhieng = false;
			NetworkdangKhieng = true;
			Khieng(t: true);
			Invoke("OffK", tgK);
		}
		if (!dangKhieng)
		{
			Networkheath = Mathf.Clamp(heath - mauMat, 0, maxHeath);
			PaRpc(mauMat);
		}
		if (heath <= 0 && !chet)
		{
			if (!khacT)
			{
				Networkchet = true;
				Chet();
				Invoke("HoiSinh", tgHoiSinh);
			}
			else
			{
				NetworkServer.Spawn(Object.Instantiate(vatPham, base.transform.position, base.transform.rotation));
				NetworkServer.Destroy(base.gameObject);
			}
		}
	}

	protected static void InvokeUserCode_MatMau__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command MatMau called on client.");
		}
		else
		{
			((PlayerStat)obj).UserCode_MatMau__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_PaRpc__Int32(int mauM)
	{
		Object.Instantiate(pa, base.transform.position, base.transform.rotation);
		GameObject obj = Object.Instantiate(hitT, base.transform.position, base.transform.rotation);
		obj.GetComponentInChildren<TextMeshProUGUI>().text = mauM.ToString();
		obj.transform.position += new Vector3(Random.Range(0f - hitToff, hitToff), Random.Range(0f - hitToff, hitToff), Random.Range(0f - hitToff, hitToff));
	}

	protected static void InvokeUserCode_PaRpc__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC PaRpc called on server.");
		}
		else
		{
			((PlayerStat)obj).UserCode_PaRpc__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_Khieng__Boolean(bool t)
	{
		xanh.SetActive(t);
	}

	protected static void InvokeUserCode_Khieng__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Khieng called on server.");
		}
		else
		{
			((PlayerStat)obj).UserCode_Khieng__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_HoiMau__Int32(int mauHoi)
	{
		Networkheath = Mathf.Clamp(heath + mauHoi, 0, maxHeath);
	}

	protected static void InvokeUserCode_HoiMau__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command HoiMau called on client.");
		}
		else
		{
			((PlayerStat)obj).UserCode_HoiMau__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_Chet()
	{
		GetComponent<AudioSource>().PlayOneShot(deathS);
		cc.enabled = false;
		modelG.SetActive(value: false);
		Object.Instantiate(ragdoll, base.transform.position, base.transform.rotation);
		DungYen(tgHoiSinh);
		Networkchet = true;
	}

	protected static void InvokeUserCode_Chet(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Chet called on server.");
		}
		else
		{
			((PlayerStat)obj).UserCode_Chet();
		}
	}

	protected void UserCode_HoiSinh()
	{
		cc.enabled = true;
		modelG.SetActive(value: true);
		Networkchet = false;
		Networkheath = maxHeath;
		base.transform.position = vtHoiSinh.position;
	}

	protected static void InvokeUserCode_HoiSinh(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC HoiSinh called on server.");
		}
		else
		{
			((PlayerStat)obj).UserCode_HoiSinh();
		}
	}

	protected void UserCode_DoiTuongCmd()
	{
		GameObject player = Object.Instantiate(doiTuong);
		NetworkServer.ReplacePlayerForConnection(base.connectionToClient, player, keepAuthority: true);
		Object.Destroy(base.gameObject, 0.1f);
	}

	protected static void InvokeUserCode_DoiTuongCmd(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command DoiTuongCmd called on client.");
		}
		else
		{
			((PlayerStat)obj).UserCode_DoiTuongCmd();
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
			((PlayerStat)obj).UserCode_Eat();
		}
	}

	protected void UserCode_NhinThay__Boolean(bool t)
	{
		GameObject[] array = model;
		foreach (GameObject gameObject in array)
		{
			if (t)
			{
				gameObject.layer = LayerMask.NameToLayer("nhin qua");
			}
			else
			{
				gameObject.layer = 0;
			}
		}
	}

	protected static void InvokeUserCode_NhinThay__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC NhinThay called on server.");
		}
		else
		{
			((PlayerStat)obj).UserCode_NhinThay__Boolean(reader.ReadBool());
		}
	}

	static PlayerStat()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerStat), "System.Void PlayerStat::DungYen(System.Single)", InvokeUserCode_DungYen__Single, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerStat), "System.Void PlayerStat::LamCham(System.Single)", InvokeUserCode_LamCham__Single, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerStat), "System.Void PlayerStat::MatMau(System.Int32)", InvokeUserCode_MatMau__Int32, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerStat), "System.Void PlayerStat::HoiMau(System.Int32)", InvokeUserCode_HoiMau__Int32, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerStat), "System.Void PlayerStat::DoiTuongCmd()", InvokeUserCode_DoiTuongCmd, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStat), "System.Void PlayerStat::DungYenRpc(System.Single)", InvokeUserCode_DungYenRpc__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStat), "System.Void PlayerStat::LamChamRpc(System.Single)", InvokeUserCode_LamChamRpc__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStat), "System.Void PlayerStat::PaRpc(System.Int32)", InvokeUserCode_PaRpc__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStat), "System.Void PlayerStat::Khieng(System.Boolean)", InvokeUserCode_Khieng__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStat), "System.Void PlayerStat::Chet()", InvokeUserCode_Chet);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStat), "System.Void PlayerStat::HoiSinh()", InvokeUserCode_HoiSinh);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStat), "System.Void PlayerStat::Eat()", InvokeUserCode_Eat);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStat), "System.Void PlayerStat::NhinThay(System.Boolean)", InvokeUserCode_NhinThay__Boolean);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(heath);
			writer.WriteInt(team);
			writer.WriteBool(chet);
			writer.WriteBool(khieng);
			writer.WriteBool(dangKhieng);
			writer.WriteBool(dungYen);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(heath);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteInt(team);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(chet);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteBool(khieng);
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteBool(dangKhieng);
		}
		if ((base.syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteBool(dungYen);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref heath, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref team, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref chet, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref khieng, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref dangKhieng, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref dungYen, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref heath, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref team, null, reader.ReadInt());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref chet, null, reader.ReadBool());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref khieng, null, reader.ReadBool());
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref dangKhieng, null, reader.ReadBool());
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref dungYen, null, reader.ReadBool());
		}
	}
}
