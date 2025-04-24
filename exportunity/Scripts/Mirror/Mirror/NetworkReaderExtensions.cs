using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mirror;

public static class NetworkReaderExtensions
{
	public static byte ReadByte(this NetworkReader reader)
	{
		return reader.ReadBlittable<byte>();
	}

	public static byte? ReadByteNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<byte>();
	}

	public static sbyte ReadSByte(this NetworkReader reader)
	{
		return reader.ReadBlittable<sbyte>();
	}

	public static sbyte? ReadSByteNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<sbyte>();
	}

	public static char ReadChar(this NetworkReader reader)
	{
		return (char)reader.ReadBlittable<ushort>();
	}

	public static char? ReadCharNullable(this NetworkReader reader)
	{
		return (char?)reader.ReadBlittableNullable<ushort>();
	}

	public static bool ReadBool(this NetworkReader reader)
	{
		return reader.ReadBlittable<byte>() != 0;
	}

	public static bool? ReadBoolNullable(this NetworkReader reader)
	{
		byte? b = reader.ReadBlittableNullable<byte>();
		if (!b.HasValue)
		{
			return null;
		}
		return b.Value != 0;
	}

	public static short ReadShort(this NetworkReader reader)
	{
		return (short)reader.ReadUShort();
	}

	public static short? ReadShortNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<short>();
	}

	public static ushort ReadUShort(this NetworkReader reader)
	{
		return reader.ReadBlittable<ushort>();
	}

	public static ushort? ReadUShortNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<ushort>();
	}

	public static int ReadInt(this NetworkReader reader)
	{
		return reader.ReadBlittable<int>();
	}

	public static int? ReadIntNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<int>();
	}

	public static uint ReadUInt(this NetworkReader reader)
	{
		return reader.ReadBlittable<uint>();
	}

	public static uint? ReadUIntNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<uint>();
	}

	public static long ReadLong(this NetworkReader reader)
	{
		return reader.ReadBlittable<long>();
	}

	public static long? ReadLongNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<long>();
	}

	public static ulong ReadULong(this NetworkReader reader)
	{
		return reader.ReadBlittable<ulong>();
	}

	public static ulong? ReadULongNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<ulong>();
	}

	public static float ReadFloat(this NetworkReader reader)
	{
		return reader.ReadBlittable<float>();
	}

	public static float? ReadFloatNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<float>();
	}

	public static double ReadDouble(this NetworkReader reader)
	{
		return reader.ReadBlittable<double>();
	}

	public static double? ReadDoubleNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<double>();
	}

	public static decimal ReadDecimal(this NetworkReader reader)
	{
		return reader.ReadBlittable<decimal>();
	}

	public static decimal? ReadDecimalNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<decimal>();
	}

	public static string ReadString(this NetworkReader reader)
	{
		ushort num = reader.ReadUShort();
		if (num == 0)
		{
			return null;
		}
		ushort num2 = (ushort)(num - 1);
		if (num2 > 65534)
		{
			throw new EndOfStreamException($"NetworkReader.ReadString - Value too long: {num2} bytes. Limit is: {(ushort)65534} bytes");
		}
		ArraySegment<byte> arraySegment = reader.ReadBytesSegment(num2);
		return reader.encoding.GetString(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
	}

	public static byte[] ReadBytesAndSize(this NetworkReader reader)
	{
		uint num = reader.ReadUInt();
		if (num != 0)
		{
			return ReadBytes(reader, checked((int)(num - 1)));
		}
		return null;
	}

	public static byte[] ReadBytes(this NetworkReader reader, int count)
	{
		byte[] array = new byte[count];
		reader.ReadBytes(array, count);
		return array;
	}

	public static ArraySegment<byte> ReadBytesAndSizeSegment(this NetworkReader reader)
	{
		uint num = reader.ReadUInt();
		if (num != 0)
		{
			return reader.ReadBytesSegment(checked((int)(num - 1)));
		}
		return default(ArraySegment<byte>);
	}

	public static Vector2 ReadVector2(this NetworkReader reader)
	{
		return reader.ReadBlittable<Vector2>();
	}

	public static Vector2? ReadVector2Nullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<Vector2>();
	}

	public static Vector3 ReadVector3(this NetworkReader reader)
	{
		return reader.ReadBlittable<Vector3>();
	}

	public static Vector3? ReadVector3Nullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<Vector3>();
	}

	public static Vector4 ReadVector4(this NetworkReader reader)
	{
		return reader.ReadBlittable<Vector4>();
	}

	public static Vector4? ReadVector4Nullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<Vector4>();
	}

	public static Vector2Int ReadVector2Int(this NetworkReader reader)
	{
		return reader.ReadBlittable<Vector2Int>();
	}

	public static Vector2Int? ReadVector2IntNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<Vector2Int>();
	}

	public static Vector3Int ReadVector3Int(this NetworkReader reader)
	{
		return reader.ReadBlittable<Vector3Int>();
	}

	public static Vector3Int? ReadVector3IntNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<Vector3Int>();
	}

	public static Color ReadColor(this NetworkReader reader)
	{
		return reader.ReadBlittable<Color>();
	}

	public static Color? ReadColorNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<Color>();
	}

	public static Color32 ReadColor32(this NetworkReader reader)
	{
		return reader.ReadBlittable<Color32>();
	}

	public static Color32? ReadColor32Nullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<Color32>();
	}

	public static Quaternion ReadQuaternion(this NetworkReader reader)
	{
		return reader.ReadBlittable<Quaternion>();
	}

	public static Quaternion? ReadQuaternionNullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<Quaternion>();
	}

	public static Rect ReadRect(this NetworkReader reader)
	{
		return new Rect(reader.ReadVector2(), reader.ReadVector2());
	}

	public static Rect? ReadRectNullable(this NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		return reader.ReadRect();
	}

	public static Plane ReadPlane(this NetworkReader reader)
	{
		return new Plane(reader.ReadVector3(), reader.ReadFloat());
	}

	public static Plane? ReadPlaneNullable(this NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		return reader.ReadPlane();
	}

	public static Ray ReadRay(this NetworkReader reader)
	{
		return new Ray(reader.ReadVector3(), reader.ReadVector3());
	}

	public static Ray? ReadRayNullable(this NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		return reader.ReadRay();
	}

	public static Matrix4x4 ReadMatrix4x4(this NetworkReader reader)
	{
		return reader.ReadBlittable<Matrix4x4>();
	}

	public static Matrix4x4? ReadMatrix4x4Nullable(this NetworkReader reader)
	{
		return reader.ReadBlittableNullable<Matrix4x4>();
	}

	public static Guid ReadGuid(this NetworkReader reader)
	{
		if (reader.Remaining >= 16)
		{
			ReadOnlySpan<byte> b = new ReadOnlySpan<byte>(reader.buffer.Array, reader.buffer.Offset + reader.Position, 16);
			reader.Position += 16;
			return new Guid(b);
		}
		throw new EndOfStreamException($"ReadGuid out of range: {reader}");
	}

	public static Guid? ReadGuidNullable(this NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		return reader.ReadGuid();
	}

	public static NetworkIdentity ReadNetworkIdentity(this NetworkReader reader)
	{
		uint num = reader.ReadUInt();
		if (num == 0)
		{
			return null;
		}
		return Utils.GetSpawnedInServerOrClient(num);
	}

	public static NetworkBehaviour ReadNetworkBehaviour(this NetworkReader reader)
	{
		uint num = reader.ReadUInt();
		if (num == 0)
		{
			return null;
		}
		byte b = reader.ReadByte();
		NetworkIdentity spawnedInServerOrClient = Utils.GetSpawnedInServerOrClient(num);
		if (!(spawnedInServerOrClient != null))
		{
			return null;
		}
		return spawnedInServerOrClient.NetworkBehaviours[b];
	}

	public static T ReadNetworkBehaviour<T>(this NetworkReader reader) where T : NetworkBehaviour
	{
		return reader.ReadNetworkBehaviour() as T;
	}

	public static NetworkBehaviourSyncVar ReadNetworkBehaviourSyncVar(this NetworkReader reader)
	{
		uint num = reader.ReadUInt();
		byte componentIndex = 0;
		if (num != 0)
		{
			componentIndex = reader.ReadByte();
		}
		return new NetworkBehaviourSyncVar(num, componentIndex);
	}

	public static Transform ReadTransform(this NetworkReader reader)
	{
		NetworkIdentity networkIdentity = reader.ReadNetworkIdentity();
		if (!(networkIdentity != null))
		{
			return null;
		}
		return networkIdentity.transform;
	}

	public static GameObject ReadGameObject(this NetworkReader reader)
	{
		NetworkIdentity networkIdentity = reader.ReadNetworkIdentity();
		if (!(networkIdentity != null))
		{
			return null;
		}
		return networkIdentity.gameObject;
	}

	public static List<T> ReadList<T>(this NetworkReader reader)
	{
		int num = reader.ReadInt();
		if (num < 0)
		{
			return null;
		}
		List<T> list = new List<T>(num);
		for (int i = 0; i < num; i++)
		{
			list.Add(reader.Read<T>());
		}
		return list;
	}

	public static T[] ReadArray<T>(this NetworkReader reader)
	{
		int num = reader.ReadInt();
		if (num < 0)
		{
			return null;
		}
		if (num > reader.Remaining)
		{
			throw new EndOfStreamException($"Received array that is too large: {num}");
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = reader.Read<T>();
		}
		return array;
	}

	public static Uri ReadUri(this NetworkReader reader)
	{
		string text = reader.ReadString();
		if (!string.IsNullOrWhiteSpace(text))
		{
			return new Uri(text);
		}
		return null;
	}

	public static Texture2D ReadTexture2D(this NetworkReader reader)
	{
		short num = reader.ReadShort();
		if (num == -1)
		{
			return null;
		}
		short height = reader.ReadShort();
		Texture2D texture2D = new Texture2D(num, height);
		Color32[] pixels = reader.ReadArray<Color32>();
		texture2D.SetPixels32(pixels);
		texture2D.Apply();
		return texture2D;
	}

	public static Sprite ReadSprite(this NetworkReader reader)
	{
		Texture2D texture2D = reader.ReadTexture2D();
		if (texture2D == null)
		{
			return null;
		}
		return Sprite.Create(texture2D, reader.ReadRect(), reader.ReadVector2());
	}

	public static DateTime ReadDateTime(this NetworkReader reader)
	{
		return DateTime.FromOADate(reader.ReadDouble());
	}

	public static DateTime? ReadDateTimeNullable(this NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		return reader.ReadDateTime();
	}
}
