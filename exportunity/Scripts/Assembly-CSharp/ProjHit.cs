using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class ProjHit : NetworkBehaviour
{
	public float tgChoang;

	public float tgCham;

	public int mauMat;

	public int doi;

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		PlayerStat component = other.GetComponent<PlayerStat>();
		if (component != null && component.team != doi)
		{
			if (mauMat > 0)
			{
				component.MatMau(mauMat);
			}
			if (tgChoang > 0f)
			{
				component.DungYen(tgChoang);
			}
			if (tgCham > 0f)
			{
				component.LamCham(tgCham);
			}
			TuHuy();
		}
	}

	[Command(requiresAuthority = false)]
	private void TuHuy()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void ProjHit::TuHuy()", 383321530, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_TuHuy()
	{
		NetworkServer.Destroy(base.gameObject);
	}

	protected static void InvokeUserCode_TuHuy(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command TuHuy called on client.");
		}
		else
		{
			((ProjHit)obj).UserCode_TuHuy();
		}
	}

	static ProjHit()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(ProjHit), "System.Void ProjHit::TuHuy()", InvokeUserCode_TuHuy, requiresAuthority: false);
	}
}
