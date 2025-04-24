using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class ChamTB : NetworkBehaviour
{
	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active && other.transform.CompareTag("tuong"))
		{
			TuHuy();
		}
	}

	[Command(requiresAuthority = false)]
	private void TuHuy()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void ChamTB::TuHuy()", -695788471, writer, 0, requiresAuthority: false);
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
			((ChamTB)obj).UserCode_TuHuy();
		}
	}

	static ChamTB()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(ChamTB), "System.Void ChamTB::TuHuy()", InvokeUserCode_TuHuy, requiresAuthority: false);
	}
}
