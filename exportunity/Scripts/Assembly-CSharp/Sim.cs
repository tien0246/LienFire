using System;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Sim : NetworkBehaviour
{
	public float tamNem;

	public GameObject cauChoang;

	public Camera cam;

	public GameObject line;

	public Transform vtBan;

	public ParticleSystem pa;

	private PlayerAtack pA;

	private PlayerStat pS;

	private void Start()
	{
		pS = GetComponent<PlayerStat>();
		pA = GetComponent<PlayerAtack>();
		pA.DungChieu1 += Choang;
		pA.DungChieu2 += Choang;
		pA.DungChieu3 += Choang;
		pA.DungDanhThuong += Ban;
	}

	[Command]
	private void Choang(object sender, EventArgs e)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_System_002EObject(writer, sender);
		GeneratedNetworkCode._Write_System_002EEventArgs(writer, e);
		SendCommandInternal("System.Void Sim::Choang(System.Object,System.EventArgs)", -611866754, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void Cham(object sender, EventArgs e)
	{
	}

	private void DiChuyen(object sender, EventArgs e)
	{
	}

	private void Ban(object sender, EventArgs e)
	{
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hitInfo, 100f))
		{
			DrawLine(hitInfo.point);
			PlayerStat component = hitInfo.transform.GetComponent<PlayerStat>();
			if (component != null && component.team != pS.team)
			{
				UnityEngine.Object.Instantiate(pa, hitInfo.point, hitInfo.transform.rotation);
				component.MatMau(50);
			}
		}
		else
		{
			DrawLine(cam.transform.position + cam.transform.forward * 100f);
		}
	}

	[Command]
	private void DrawLine(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendCommandInternal("System.Void Sim::DrawLine(UnityEngine.Vector3)", 1095302925, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void DrawLineRPC(Vector3 vt)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(vt);
		SendRPCInternal("System.Void Sim::DrawLineRPC(UnityEngine.Vector3)", -644352354, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_Choang__Object__EventArgs(object sender, EventArgs e)
	{
		Quaternion rotation = Quaternion.LookRotation(cam.transform.position + cam.transform.forward * tamNem - vtBan.position, Vector3.up);
		GameObject obj = UnityEngine.Object.Instantiate(cauChoang, vtBan.transform.position, rotation);
		obj.GetComponent<ChoangProj>().doi = pS.team;
		NetworkServer.Spawn(obj);
	}

	protected static void InvokeUserCode_Choang__Object__EventArgs(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Choang called on client.");
		}
		else
		{
			((Sim)obj).UserCode_Choang__Object__EventArgs(GeneratedNetworkCode._Read_System_002EObject(reader), GeneratedNetworkCode._Read_System_002EEventArgs(reader));
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
			((Sim)obj).UserCode_DrawLine__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_DrawLineRPC__Vector3(Vector3 vt)
	{
		UnityEngine.Object.Instantiate(line, vtBan.position, vtBan.rotation).GetComponent<Line>().SetLine(vtBan.position, vt);
	}

	protected static void InvokeUserCode_DrawLineRPC__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC DrawLineRPC called on server.");
		}
		else
		{
			((Sim)obj).UserCode_DrawLineRPC__Vector3(reader.ReadVector3());
		}
	}

	static Sim()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Sim), "System.Void Sim::Choang(System.Object,System.EventArgs)", InvokeUserCode_Choang__Object__EventArgs, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Sim), "System.Void Sim::DrawLine(UnityEngine.Vector3)", InvokeUserCode_DrawLine__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(Sim), "System.Void Sim::DrawLineRPC(UnityEngine.Vector3)", InvokeUserCode_DrawLineRPC__Vector3);
	}
}
