using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class BatTru : NetworkBehaviour
{
	public GameObject Bot;

	public GameObject tru;

	private bool dangBat;

	public void Sponu()
	{
		if (dangBat)
		{
			TatSponTru();
		}
		else
		{
			BatSponTru();
		}
	}

	[Command(requiresAuthority = false)]
	public void TatSponTru()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void BatTru::TatSponTru()", 403385083, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[Command(requiresAuthority = false)]
	public void BatSponTru()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void BatTru::BatSponTru()", -1920103859, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Bat()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void BatTru::Bat()", -1061305072, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Tat()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void BatTru::Tat()", -1044681694, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Des()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("linh");
		for (int i = 0; i < array.Length; i++)
		{
			NetworkServer.Destroy(array[i]);
		}
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("tru");
		for (int j = 0; j < array2.Length; j++)
		{
			NetworkServer.Destroy(array2[j]);
		}
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_TatSponTru()
	{
		Tat();
		Des();
		dangBat = false;
	}

	protected static void InvokeUserCode_TatSponTru(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command TatSponTru called on client.");
		}
		else
		{
			((BatTru)obj).UserCode_TatSponTru();
		}
	}

	protected void UserCode_BatSponTru()
	{
		Des();
		Bat();
		SponTru[] array = Object.FindObjectsOfType<SponTru>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Spon();
		}
		dangBat = true;
	}

	protected static void InvokeUserCode_BatSponTru(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command BatSponTru called on client.");
		}
		else
		{
			((BatTru)obj).UserCode_BatSponTru();
		}
	}

	protected void UserCode_Bat()
	{
		Bot.SetActive(value: false);
	}

	protected static void InvokeUserCode_Bat(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Bat called on server.");
		}
		else
		{
			((BatTru)obj).UserCode_Bat();
		}
	}

	protected void UserCode_Tat()
	{
		Bot.SetActive(value: true);
	}

	protected static void InvokeUserCode_Tat(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Tat called on server.");
		}
		else
		{
			((BatTru)obj).UserCode_Tat();
		}
	}

	static BatTru()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(BatTru), "System.Void BatTru::TatSponTru()", InvokeUserCode_TatSponTru, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(BatTru), "System.Void BatTru::BatSponTru()", InvokeUserCode_BatSponTru, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(BatTru), "System.Void BatTru::Bat()", InvokeUserCode_Bat);
		RemoteProcedureCalls.RegisterRpc(typeof(BatTru), "System.Void BatTru::Tat()", InvokeUserCode_Tat);
	}
}
