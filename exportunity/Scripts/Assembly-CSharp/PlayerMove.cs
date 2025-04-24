using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
	public AudioClip nhayA;

	public AudioClip[] buocA;

	private Animator ani;

	private PlayerStat pS;

	private CharacterController cc;

	public DynamicJoystick joy;

	private float tdHT;

	[SerializeField]
	private float td;

	[SerializeField]
	private float gravity;

	[SerializeField]
	private float lucNhay;

	private float yMove;

	private bool nhay;

	public float tdMoiBuoc;

	private float tgDuocBuoc;

	private void Start()
	{
		ani = GetComponent<Animator>();
		cc = GetComponent<CharacterController>();
		pS = GetComponent<PlayerStat>();
	}

	private void Update()
	{
		if (base.isLocalPlayer && !pS.dungYen)
		{
			if (pS.biCham)
			{
				tdHT = td / 2f;
			}
			else
			{
				tdHT = td;
			}
			Move();
		}
	}

	private void Move()
	{
		Vector3 zero = Vector3.zero;
		float horizontal = joy.Horizontal;
		float vertical = joy.Vertical;
		ani.SetBool("ChamDat", cc.isGrounded);
		zero = base.transform.TransformDirection(new Vector3(horizontal, 0f, vertical)) * tdHT;
		if (zero == Vector3.zero)
		{
			ani.SetBool("Chay", value: false);
		}
		else
		{
			ani.SetBool("Chay", value: true);
		}
		if (ani.GetBool("Chay") && Time.time > tgDuocBuoc && cc.isGrounded)
		{
			tgDuocBuoc = Time.time + tdMoiBuoc;
			BuocRpc();
		}
		if (cc.isGrounded)
		{
			yMove = -0.2f;
			if (Input.GetKeyDown(KeyCode.Space) || nhay)
			{
				yMove = lucNhay;
				NCom();
				nhay = false;
			}
		}
		else
		{
			yMove -= gravity * Time.deltaTime;
		}
		zero.y = yMove;
		cc.Move(zero * Time.deltaTime);
	}

	public void Nhay()
	{
		if (cc.isGrounded)
		{
			nhay = true;
			NCom();
		}
	}

	[Command]
	private void NCom()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerMove::NCom()", 1836640106, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void NRpc()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerMove::NRpc()", 1850513102, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void BuocRpc()
	{
		base.gameObject.GetComponent<AudioSource>().PlayOneShot(buocA[Random.Range(0, buocA.Length)], 0.2f);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_NCom()
	{
		NRpc();
	}

	protected static void InvokeUserCode_NCom(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command NCom called on client.");
		}
		else
		{
			((PlayerMove)obj).UserCode_NCom();
		}
	}

	protected void UserCode_NRpc()
	{
		base.gameObject.GetComponent<AudioSource>().PlayOneShot(nhayA);
	}

	protected static void InvokeUserCode_NRpc(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC NRpc called on server.");
		}
		else
		{
			((PlayerMove)obj).UserCode_NRpc();
		}
	}

	static PlayerMove()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMove), "System.Void PlayerMove::NCom()", InvokeUserCode_NCom, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMove), "System.Void PlayerMove::NRpc()", InvokeUserCode_NRpc);
	}
}
