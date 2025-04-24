using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class UngTram : NetworkBehaviour
{
	[SyncVar]
	public float mau;

	[SyncVar]
	public int team;

	private bool die;

	public List<GameObject> list = new List<GameObject>();

	public float tam;

	public Color red;

	public Color blue;

	public SpriteRenderer sR;

	public MeshRenderer tron;

	public float Networkmau
	{
		get
		{
			return mau;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref mau, 1uL, null);
		}
	}

	public int Networkteam
	{
		get
		{
			return team;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref team, 2uL, null);
		}
	}

	private void Start()
	{
		if (team == 1)
		{
			sR.color = red;
			tron.material.color = Color.red;
		}
		else
		{
			sR.color = blue;
			tron.material.color = Color.blue;
		}
	}

	[ServerCallback]
	private void Update()
	{
		if (!NetworkServer.active || die)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PlayerStat component = array[i].GetComponent<PlayerStat>();
			float num = Vector3.Distance(base.transform.position, array[i].transform.position);
			if (num < tam && !list.Contains(array[i]) && component.team != team)
			{
				list.Add(array[i]);
				component.NhinThay(t: true);
			}
			else if (num > tam && list.Contains(array[i]) && component.team != team)
			{
				list.Remove(array[i]);
				component.NhinThay(t: false);
			}
		}
	}

	[ServerCallback]
	private void OnTriggerStay(Collider other)
	{
		if (!NetworkServer.active || !other.CompareTag("Player") || other.GetComponent<PlayerStat>().team == team)
		{
			return;
		}
		Networkmau = mau - Time.deltaTime;
		if (!(mau <= 0f))
		{
			return;
		}
		die = true;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null)
			{
				list[i].GetComponent<PlayerStat>().NhinThay(t: false);
			}
		}
		Invoke("Des", 0.25f);
	}

	private void Des()
	{
		NetworkServer.Destroy(base.gameObject);
	}

	private void MirrorProcessed()
	{
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteFloat(mau);
			writer.WriteInt(team);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteFloat(mau);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteInt(team);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref mau, null, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref team, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref mau, null, reader.ReadFloat());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref team, null, reader.ReadInt());
		}
	}
}
