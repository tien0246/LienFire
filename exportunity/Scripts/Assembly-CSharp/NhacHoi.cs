using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class NhacHoi : NetworkBehaviour
{
	[SyncVar]
	public int team;

	[SyncVar]
	public Transform tr;

	public int mauHoi;

	public float tdHoi;

	public float doLon;

	private float tgDuocHoi;

	public int Networkteam
	{
		get
		{
			return team;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref team, 1uL, null);
		}
	}

	public Transform Networktr
	{
		get
		{
			return tr;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref tr, 2uL, null);
		}
	}

	private void Start()
	{
		base.transform.position = tr.position;
		base.transform.parent = tr;
	}

	[ServerCallback]
	private void Update()
	{
		if (!NetworkServer.active || !(Time.time > tgDuocHoi))
		{
			return;
		}
		tgDuocHoi = Time.time + tdHoi;
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, doLon, base.transform.forward, 1f);
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].transform.GetComponent<PlayerStat>();
			if (component != null && component.team == team)
			{
				component.HoiMau(mauHoi);
			}
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
			writer.WriteInt(team);
			writer.WriteTransform(tr);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(team);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteTransform(tr);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref team, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref tr, null, reader.ReadTransform());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref team, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref tr, null, reader.ReadTransform());
		}
	}
}
