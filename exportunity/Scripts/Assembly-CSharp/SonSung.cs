using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class SonSung : NetworkBehaviour
{
	public AudioClip eatS;

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active && other.CompareTag("Player"))
		{
			NetworkServer.Destroy(base.gameObject);
			NC(other.GetComponent<PlayerAtack>());
		}
	}

	private void NC(PlayerAtack pS)
	{
		pS.SonSung();
		Eat();
	}

	[ClientRpc]
	private void Eat()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void SonSung::Eat()", -1591060144, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void MirrorProcessed()
	{
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
			((SonSung)obj).UserCode_Eat();
		}
	}

	static SonSung()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(SonSung), "System.Void SonSung::Eat()", InvokeUserCode_Eat);
	}
}
