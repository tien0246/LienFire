using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class ProjDC : NetworkBehaviour
{
	public float td;

	public float tg;

	private void Start()
	{
		Invoke("TuHuy", tg);
	}

	private void Update()
	{
		base.transform.Translate(Vector3.forward * td * Time.deltaTime);
	}

	[Command(requiresAuthority = false)]
	private void TuHuy()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void ProjDC::TuHuy()", -748088884, writer, 0, requiresAuthority: false);
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
			((ProjDC)obj).UserCode_TuHuy();
		}
	}

	static ProjDC()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(ProjDC), "System.Void ProjDC::TuHuy()", InvokeUserCode_TuHuy, requiresAuthority: false);
	}
}
