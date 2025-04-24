using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class TronNo : NetworkBehaviour
{
	public AudioClip noA;

	public float tg;

	public float td;

	private AudioSource aS;

	private MeshRenderer mR;

	private void Start()
	{
		mR = GetComponent<MeshRenderer>();
		aS = GetComponent<AudioSource>();
		aS.PlayOneShot(noA, 0.2f);
		Invoke("TuHuy", tg);
	}

	private void Update()
	{
		if (mR.material.color.a > 0f)
		{
			mR.material.color -= new Color(0f, 0f, 0f, td);
		}
		else
		{
			mR.enabled = false;
		}
	}

	[Command(requiresAuthority = false)]
	private void TuHuy()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void TronNo::TuHuy()", 657177898, writer, 0, requiresAuthority: false);
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
			((TronNo)obj).UserCode_TuHuy();
		}
	}

	static TronNo()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(TronNo), "System.Void TronNo::TuHuy()", InvokeUserCode_TuHuy, requiresAuthority: false);
	}
}
