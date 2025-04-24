using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class ThuocNo : NetworkBehaviour
{
	public float tg;

	public AudioClip noA;

	private AudioSource aS;

	private void Start()
	{
		aS = GetComponent<AudioSource>();
		aS.PlayOneShot(noA, 0.2f);
		Invoke("TuHuy", tg);
	}

	[Command(requiresAuthority = false)]
	private void TuHuy()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void ThuocNo::TuHuy()", 1686016506, writer, 0, requiresAuthority: false);
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
			((ThuocNo)obj).UserCode_TuHuy();
		}
	}

	static ThuocNo()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(ThuocNo), "System.Void ThuocNo::TuHuy()", InvokeUserCode_TuHuy, requiresAuthority: false);
	}
}
