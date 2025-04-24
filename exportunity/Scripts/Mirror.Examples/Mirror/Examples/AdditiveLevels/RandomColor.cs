using System.Runtime.InteropServices;
using UnityEngine;

namespace Mirror.Examples.AdditiveLevels;

public class RandomColor : NetworkBehaviour
{
	[SyncVar(hook = "SetColor")]
	public Color32 color = Color.black;

	private Material cachedMaterial;

	public Color32 Networkcolor
	{
		get
		{
			return color;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref color, 1uL, SetColor);
		}
	}

	private void SetColor(Color32 _, Color32 newColor)
	{
		if (cachedMaterial == null)
		{
			cachedMaterial = GetComponentInChildren<Renderer>().material;
		}
		cachedMaterial.color = newColor;
	}

	private void OnDestroy()
	{
		Object.Destroy(cachedMaterial);
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		if (color == Color.black)
		{
			Networkcolor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
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
			writer.WriteColor32(color);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteColor32(color);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref color, SetColor, reader.ReadColor32());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref color, SetColor, reader.ReadColor32());
		}
	}
}
