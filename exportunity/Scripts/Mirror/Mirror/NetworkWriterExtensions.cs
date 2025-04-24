using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mirror;

public static class NetworkWriterExtensions
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct UIntDouble
	{
		[FieldOffset(0)]
		public double doubleValue;

		[FieldOffset(0)]
		public ulong longValue;
	}

	public static void WriteByte(this NetworkWriter writer, byte value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteByteNullable(this NetworkWriter writer, byte? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteSByte(this NetworkWriter writer, sbyte value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteSByteNullable(this NetworkWriter writer, sbyte? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteChar(this NetworkWriter writer, char value)
	{
		writer.WriteBlittable((ushort)value);
	}

	public static void WriteCharNullable(this NetworkWriter writer, char? value)
	{
		writer.WriteBlittableNullable((ushort?)value);
	}

	public static void WriteBool(this NetworkWriter writer, bool value)
	{
		writer.WriteBlittable((byte)(value ? 1u : 0u));
	}

	public static void WriteBoolNullable(this NetworkWriter writer, bool? value)
	{
		writer.WriteBlittableNullable(value.HasValue ? new byte?((byte)(value.Value ? 1u : 0u)) : ((byte?)null));
	}

	public static void WriteShort(this NetworkWriter writer, short value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteShortNullable(this NetworkWriter writer, short? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteUShort(this NetworkWriter writer, ushort value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteUShortNullable(this NetworkWriter writer, ushort? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteInt(this NetworkWriter writer, int value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteIntNullable(this NetworkWriter writer, int? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteUInt(this NetworkWriter writer, uint value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteUIntNullable(this NetworkWriter writer, uint? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteLong(this NetworkWriter writer, long value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteLongNullable(this NetworkWriter writer, long? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteULong(this NetworkWriter writer, ulong value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteULongNullable(this NetworkWriter writer, ulong? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteFloat(this NetworkWriter writer, float value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteFloatNullable(this NetworkWriter writer, float? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteDouble(this NetworkWriter writer, double value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteDoubleNullable(this NetworkWriter writer, double? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteDecimal(this NetworkWriter writer, decimal value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteDecimalNullable(this NetworkWriter writer, decimal? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteString(this NetworkWriter writer, string value)
	{
		if (value == null)
		{
			writer.WriteUShort(0);
			return;
		}
		int maxByteCount = writer.encoding.GetMaxByteCount(value.Length);
		writer.EnsureCapacity(writer.Position + 2 + maxByteCount);
		int bytes = writer.encoding.GetBytes(value, 0, value.Length, writer.buffer, writer.Position + 2);
		if (bytes > 65534)
		{
			throw new IndexOutOfRangeException($"NetworkWriter.WriteString - Value too long: {bytes} bytes. Limit: {(ushort)65534} bytes");
		}
		writer.WriteUShort(checked((ushort)(bytes + 1)));
		writer.Position += bytes;
	}

	public static void WriteBytesAndSizeSegment(this NetworkWriter writer, ArraySegment<byte> buffer)
	{
		writer.WriteBytesAndSize(buffer.Array, buffer.Offset, buffer.Count);
	}

	public static void WriteBytesAndSize(this NetworkWriter writer, byte[] buffer)
	{
		writer.WriteBytesAndSize(buffer, 0, (buffer != null) ? buffer.Length : 0);
	}

	public static void WriteBytesAndSize(this NetworkWriter writer, byte[] buffer, int offset, int count)
	{
		if (buffer == null)
		{
			writer.WriteUInt(0u);
			return;
		}
		writer.WriteUInt(checked((uint)count) + 1);
		writer.WriteBytes(buffer, offset, count);
	}

	public static void WriteArraySegment<T>(this NetworkWriter writer, ArraySegment<T> segment)
	{
		int count = segment.Count;
		writer.WriteInt(count);
		for (int i = 0; i < count; i++)
		{
			writer.Write(segment.Array[segment.Offset + i]);
		}
	}

	public static void WriteVector2(this NetworkWriter writer, Vector2 value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteVector2Nullable(this NetworkWriter writer, Vector2? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteVector3(this NetworkWriter writer, Vector3 value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteVector3Nullable(this NetworkWriter writer, Vector3? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteVector4(this NetworkWriter writer, Vector4 value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteVector4Nullable(this NetworkWriter writer, Vector4? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteVector2Int(this NetworkWriter writer, Vector2Int value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteVector2IntNullable(this NetworkWriter writer, Vector2Int? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteVector3Int(this NetworkWriter writer, Vector3Int value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteVector3IntNullable(this NetworkWriter writer, Vector3Int? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteColor(this NetworkWriter writer, Color value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteColorNullable(this NetworkWriter writer, Color? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteColor32(this NetworkWriter writer, Color32 value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteColor32Nullable(this NetworkWriter writer, Color32? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteQuaternion(this NetworkWriter writer, Quaternion value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteQuaternionNullable(this NetworkWriter writer, Quaternion? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteRect(this NetworkWriter writer, Rect value)
	{
		writer.WriteVector2(value.position);
		writer.WriteVector2(value.size);
	}

	public static void WriteRectNullable(this NetworkWriter writer, Rect? value)
	{
		writer.WriteBool(value.HasValue);
		if (value.HasValue)
		{
			writer.WriteRect(value.Value);
		}
	}

	public static void WritePlane(this NetworkWriter writer, Plane value)
	{
		writer.WriteVector3(value.normal);
		writer.WriteFloat(value.distance);
	}

	public static void WritePlaneNullable(this NetworkWriter writer, Plane? value)
	{
		writer.WriteBool(value.HasValue);
		if (value.HasValue)
		{
			writer.WritePlane(value.Value);
		}
	}

	public static void WriteRay(this NetworkWriter writer, Ray value)
	{
		writer.WriteVector3(value.origin);
		writer.WriteVector3(value.direction);
	}

	public static void WriteRayNullable(this NetworkWriter writer, Ray? value)
	{
		writer.WriteBool(value.HasValue);
		if (value.HasValue)
		{
			writer.WriteRay(value.Value);
		}
	}

	public static void WriteMatrix4x4(this NetworkWriter writer, Matrix4x4 value)
	{
		writer.WriteBlittable(value);
	}

	public static void WriteMatrix4x4Nullable(this NetworkWriter writer, Matrix4x4? value)
	{
		writer.WriteBlittableNullable(value);
	}

	public static void WriteGuid(this NetworkWriter writer, Guid value)
	{
		writer.EnsureCapacity(writer.Position + 16);
		value.TryWriteBytes(new Span<byte>(writer.buffer, writer.Position, 16));
		writer.Position += 16;
	}

	public static void WriteGuidNullable(this NetworkWriter writer, Guid? value)
	{
		writer.WriteBool(value.HasValue);
		if (value.HasValue)
		{
			writer.WriteGuid(value.Value);
		}
	}

	public static void WriteNetworkIdentity(this NetworkWriter writer, NetworkIdentity value)
	{
		if (value == null)
		{
			writer.WriteUInt(0u);
			return;
		}
		if (value.netId == 0)
		{
			Debug.LogWarning("Attempted to serialize unspawned GameObject: " + value.name + ". Prefabs and unspawned GameObjects would always be null on the other side. Please spawn it before using it in [SyncVar]s/Rpcs/Cmds/NetworkMessages etc.");
		}
		writer.WriteUInt(value.netId);
	}

	public static void WriteNetworkBehaviour(this NetworkWriter writer, NetworkBehaviour value)
	{
		if (value == null)
		{
			writer.WriteUInt(0u);
		}
		else if (value.netId == 0)
		{
			Debug.LogWarning($"Attempted to serialize unspawned NetworkBehaviour: of type {value.GetType()} on GameObject {value.name}. Prefabs and unspawned GameObjects would always be null on the other side. Please spawn it before using it in [SyncVar]s/Rpcs/Cmds/NetworkMessages etc.");
			writer.WriteUInt(0u);
		}
		else
		{
			writer.WriteUInt(value.netId);
			writer.WriteByte(value.ComponentIndex);
		}
	}

	public static void WriteTransform(this NetworkWriter writer, Transform value)
	{
		if (value == null)
		{
			writer.WriteUInt(0u);
			return;
		}
		if (value.TryGetComponent<NetworkIdentity>(out var component))
		{
			writer.WriteUInt(component.netId);
			return;
		}
		Debug.LogWarning($"NetworkWriter {value} has no NetworkIdentity");
		writer.WriteUInt(0u);
	}

	public static void WriteGameObject(this NetworkWriter writer, GameObject value)
	{
		if (value == null)
		{
			writer.WriteUInt(0u);
			return;
		}
		if (!value.TryGetComponent<NetworkIdentity>(out var component))
		{
			Debug.LogWarning($"NetworkWriter {value} has no NetworkIdentity");
		}
		writer.WriteNetworkIdentity(component);
	}

	public static void WriteList<T>(this NetworkWriter writer, List<T> list)
	{
		if (list == null)
		{
			writer.WriteInt(-1);
			return;
		}
		writer.WriteInt(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			writer.Write(list[i]);
		}
	}

	public static void WriteArray<T>(this NetworkWriter writer, T[] array)
	{
		if (array == null)
		{
			writer.WriteInt(-1);
			return;
		}
		writer.WriteInt(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			writer.Write(array[i]);
		}
	}

	public static void WriteUri(this NetworkWriter writer, Uri uri)
	{
		writer.WriteString(uri?.ToString());
	}

	public static void WriteTexture2D(this NetworkWriter writer, Texture2D texture2D)
	{
		if (texture2D == null)
		{
			writer.WriteShort(-1);
			return;
		}
		writer.WriteShort((short)texture2D.width);
		writer.WriteShort((short)texture2D.height);
		writer.WriteArray(texture2D.GetPixels32());
	}

	public static void WriteSprite(this NetworkWriter writer, Sprite sprite)
	{
		if (sprite == null)
		{
			writer.WriteTexture2D(null);
			return;
		}
		writer.WriteTexture2D(sprite.texture);
		writer.WriteRect(sprite.rect);
		writer.WriteVector2(sprite.pivot);
	}

	public static void WriteDateTime(this NetworkWriter writer, DateTime dateTime)
	{
		writer.WriteDouble(dateTime.ToOADate());
	}

	public static void WriteDateTimeNullable(this NetworkWriter writer, DateTime? dateTime)
	{
		writer.WriteBool(dateTime.HasValue);
		if (dateTime.HasValue)
		{
			writer.WriteDouble(dateTime.Value.ToOADate());
		}
	}
}
