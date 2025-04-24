using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{
	public bool duocQuay;

	public bool m;

	public GameObject camHold;

	private float mX;

	public float senY;

	public float senX;

	[SyncVar]
	public float doNhay;

	public float NetworkdoNhay
	{
		get
		{
			return doNhay;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref doNhay, 1uL, null);
		}
	}

	private void Start()
	{
		duocQuay = true;
		if (!base.isLocalPlayer)
		{
			camHold.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (!base.isLocalPlayer || !duocQuay)
		{
			return;
		}
		for (int i = 0; i < Input.touches.Length; i++)
		{
			if (Input.GetTouch(i).position.x > (float)(Screen.width / 2))
			{
				float num = Input.GetTouch(i).deltaPosition.x * doNhay;
				float num2 = Input.GetTouch(i).deltaPosition.y * doNhay;
				mX -= num2 * Time.deltaTime * senY;
				mX = Mathf.Clamp(mX, -80f, 80f);
				camHold.transform.localRotation = Quaternion.Euler(mX, 0f, 0f);
				base.transform.Rotate(Vector3.up * senX * Time.deltaTime * num);
			}
		}
		if (m && Input.GetMouseButton(0))
		{
			float num3 = Input.GetAxisRaw("Mouse X") * doNhay;
			float num4 = Input.GetAxisRaw("Mouse Y") * doNhay;
			mX -= num4 * Time.deltaTime * senY;
			mX = Mathf.Clamp(mX, -80f, 80f);
			camHold.transform.localRotation = Quaternion.Euler(mX, 0f, 0f);
			base.transform.Rotate(Vector3.up * senX * Time.deltaTime * num3);
		}
	}

	private void MirrorProcessed()
	{
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteFloat(doNhay);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteFloat(doNhay);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref doNhay, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref doNhay, null, reader.ReadFloat());
		}
	}
}
