using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class ChoangProj : NetworkBehaviour
{
	public int doi;

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

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active)
		{
			PlayerStat component = other.GetComponent<PlayerStat>();
			if (component != null && component.team != doi)
			{
				component.MatMau(30);
				component.DungYen(3f);
				TuHuy();
			}
		}
	}

	[Command(requiresAuthority = false)]
	private void TuHuy()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void ChoangProj::TuHuy()", -1679476965, writer, 0, requiresAuthority: false);
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
			((ChoangProj)obj).UserCode_TuHuy();
		}
	}

	static ChoangProj()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(ChoangProj), "System.Void ChoangProj::TuHuy()", InvokeUserCode_TuHuy, requiresAuthority: false);
	}
}
