using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class VungBom : NetworkBehaviour
{
	public GameObject tron;

	[SyncVar]
	public int doi;

	public int mauMat;

	public float tamNo;

	private float tgNo;

	public float tdNo;

	public int Networkdoi
	{
		get
		{
			return doi;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref doi, 1uL, null);
		}
	}

	private void Update()
	{
		if (tgNo > 0f)
		{
			tgNo -= Time.deltaTime;
			return;
		}
		tgNo = tdNo;
		No();
	}

	[ServerCallback]
	private void No()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, tamNo, base.transform.forward, 1f);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && component.team != doi)
			{
				NoBom(component.transform.position);
				if (mauMat > 0)
				{
					component.MatMau(mauMat);
				}
			}
		}
	}

	private void NoBom(Vector3 vt)
	{
		GameObject obj = Object.Instantiate(tron, vt, base.transform.rotation);
		obj.transform.localScale = new Vector3(3f, 3f, 3f);
		NetworkServer.Spawn(obj);
	}

	private void MirrorProcessed()
	{
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(doi);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(doi);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref doi, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref doi, null, reader.ReadInt());
		}
	}
}
