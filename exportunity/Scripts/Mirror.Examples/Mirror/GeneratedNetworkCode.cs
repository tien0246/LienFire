using System;
using System.Runtime.InteropServices;
using Mirror.Examples.Chat;
using Mirror.Examples.MultipleMatch;
using UnityEngine;

namespace Mirror;

[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
public static class GeneratedNetworkCode
{
	public static TimeSnapshotMessage _Read_Mirror_002ETimeSnapshotMessage(NetworkReader reader)
	{
		return default(TimeSnapshotMessage);
	}

	public static void _Write_Mirror_002ETimeSnapshotMessage(NetworkWriter writer, TimeSnapshotMessage value)
	{
	}

	public static ReadyMessage _Read_Mirror_002EReadyMessage(NetworkReader reader)
	{
		return default(ReadyMessage);
	}

	public static void _Write_Mirror_002EReadyMessage(NetworkWriter writer, ReadyMessage value)
	{
	}

	public static NotReadyMessage _Read_Mirror_002ENotReadyMessage(NetworkReader reader)
	{
		return default(NotReadyMessage);
	}

	public static void _Write_Mirror_002ENotReadyMessage(NetworkWriter writer, NotReadyMessage value)
	{
	}

	public static AddPlayerMessage _Read_Mirror_002EAddPlayerMessage(NetworkReader reader)
	{
		return default(AddPlayerMessage);
	}

	public static void _Write_Mirror_002EAddPlayerMessage(NetworkWriter writer, AddPlayerMessage value)
	{
	}

	public static SceneMessage _Read_Mirror_002ESceneMessage(NetworkReader reader)
	{
		return new SceneMessage
		{
			sceneName = reader.ReadString(),
			sceneOperation = _Read_Mirror_002ESceneOperation(reader),
			customHandling = reader.ReadBool()
		};
	}

	public static SceneOperation _Read_Mirror_002ESceneOperation(NetworkReader reader)
	{
		return (SceneOperation)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_Mirror_002ESceneMessage(NetworkWriter writer, SceneMessage value)
	{
		writer.WriteString(value.sceneName);
		_Write_Mirror_002ESceneOperation(writer, value.sceneOperation);
		writer.WriteBool(value.customHandling);
	}

	public static void _Write_Mirror_002ESceneOperation(NetworkWriter writer, SceneOperation value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static CommandMessage _Read_Mirror_002ECommandMessage(NetworkReader reader)
	{
		return new CommandMessage
		{
			netId = reader.ReadUInt(),
			componentIndex = NetworkReaderExtensions.ReadByte(reader),
			functionHash = reader.ReadUShort(),
			payload = reader.ReadBytesAndSizeSegment()
		};
	}

	public static void _Write_Mirror_002ECommandMessage(NetworkWriter writer, CommandMessage value)
	{
		writer.WriteUInt(value.netId);
		NetworkWriterExtensions.WriteByte(writer, value.componentIndex);
		writer.WriteUShort(value.functionHash);
		writer.WriteBytesAndSizeSegment(value.payload);
	}

	public static RpcMessage _Read_Mirror_002ERpcMessage(NetworkReader reader)
	{
		return new RpcMessage
		{
			netId = reader.ReadUInt(),
			componentIndex = NetworkReaderExtensions.ReadByte(reader),
			functionHash = reader.ReadUShort(),
			payload = reader.ReadBytesAndSizeSegment()
		};
	}

	public static void _Write_Mirror_002ERpcMessage(NetworkWriter writer, RpcMessage value)
	{
		writer.WriteUInt(value.netId);
		NetworkWriterExtensions.WriteByte(writer, value.componentIndex);
		writer.WriteUShort(value.functionHash);
		writer.WriteBytesAndSizeSegment(value.payload);
	}

	public static RpcBufferMessage _Read_Mirror_002ERpcBufferMessage(NetworkReader reader)
	{
		return new RpcBufferMessage
		{
			payload = reader.ReadBytesAndSizeSegment()
		};
	}

	public static void _Write_Mirror_002ERpcBufferMessage(NetworkWriter writer, RpcBufferMessage value)
	{
		writer.WriteBytesAndSizeSegment(value.payload);
	}

	public static SpawnMessage _Read_Mirror_002ESpawnMessage(NetworkReader reader)
	{
		return new SpawnMessage
		{
			netId = reader.ReadUInt(),
			isLocalPlayer = reader.ReadBool(),
			isOwner = reader.ReadBool(),
			sceneId = reader.ReadULong(),
			assetId = reader.ReadUInt(),
			position = reader.ReadVector3(),
			rotation = reader.ReadQuaternion(),
			scale = reader.ReadVector3(),
			payload = reader.ReadBytesAndSizeSegment()
		};
	}

	public static void _Write_Mirror_002ESpawnMessage(NetworkWriter writer, SpawnMessage value)
	{
		writer.WriteUInt(value.netId);
		writer.WriteBool(value.isLocalPlayer);
		writer.WriteBool(value.isOwner);
		writer.WriteULong(value.sceneId);
		writer.WriteUInt(value.assetId);
		writer.WriteVector3(value.position);
		writer.WriteQuaternion(value.rotation);
		writer.WriteVector3(value.scale);
		writer.WriteBytesAndSizeSegment(value.payload);
	}

	public static ChangeOwnerMessage _Read_Mirror_002EChangeOwnerMessage(NetworkReader reader)
	{
		return new ChangeOwnerMessage
		{
			netId = reader.ReadUInt(),
			isOwner = reader.ReadBool(),
			isLocalPlayer = reader.ReadBool()
		};
	}

	public static void _Write_Mirror_002EChangeOwnerMessage(NetworkWriter writer, ChangeOwnerMessage value)
	{
		writer.WriteUInt(value.netId);
		writer.WriteBool(value.isOwner);
		writer.WriteBool(value.isLocalPlayer);
	}

	public static ObjectSpawnStartedMessage _Read_Mirror_002EObjectSpawnStartedMessage(NetworkReader reader)
	{
		return default(ObjectSpawnStartedMessage);
	}

	public static void _Write_Mirror_002EObjectSpawnStartedMessage(NetworkWriter writer, ObjectSpawnStartedMessage value)
	{
	}

	public static ObjectSpawnFinishedMessage _Read_Mirror_002EObjectSpawnFinishedMessage(NetworkReader reader)
	{
		return default(ObjectSpawnFinishedMessage);
	}

	public static void _Write_Mirror_002EObjectSpawnFinishedMessage(NetworkWriter writer, ObjectSpawnFinishedMessage value)
	{
	}

	public static ObjectDestroyMessage _Read_Mirror_002EObjectDestroyMessage(NetworkReader reader)
	{
		return new ObjectDestroyMessage
		{
			netId = reader.ReadUInt()
		};
	}

	public static void _Write_Mirror_002EObjectDestroyMessage(NetworkWriter writer, ObjectDestroyMessage value)
	{
		writer.WriteUInt(value.netId);
	}

	public static ObjectHideMessage _Read_Mirror_002EObjectHideMessage(NetworkReader reader)
	{
		return new ObjectHideMessage
		{
			netId = reader.ReadUInt()
		};
	}

	public static void _Write_Mirror_002EObjectHideMessage(NetworkWriter writer, ObjectHideMessage value)
	{
		writer.WriteUInt(value.netId);
	}

	public static EntityStateMessage _Read_Mirror_002EEntityStateMessage(NetworkReader reader)
	{
		return new EntityStateMessage
		{
			netId = reader.ReadUInt(),
			payload = reader.ReadBytesAndSizeSegment()
		};
	}

	public static void _Write_Mirror_002EEntityStateMessage(NetworkWriter writer, EntityStateMessage value)
	{
		writer.WriteUInt(value.netId);
		writer.WriteBytesAndSizeSegment(value.payload);
	}

	public static NetworkPingMessage _Read_Mirror_002ENetworkPingMessage(NetworkReader reader)
	{
		return new NetworkPingMessage
		{
			clientTime = reader.ReadDouble()
		};
	}

	public static void _Write_Mirror_002ENetworkPingMessage(NetworkWriter writer, NetworkPingMessage value)
	{
		writer.WriteDouble(value.clientTime);
	}

	public static NetworkPongMessage _Read_Mirror_002ENetworkPongMessage(NetworkReader reader)
	{
		return new NetworkPongMessage
		{
			clientTime = reader.ReadDouble()
		};
	}

	public static void _Write_Mirror_002ENetworkPongMessage(NetworkWriter writer, NetworkPongMessage value)
	{
		writer.WriteDouble(value.clientTime);
	}

	public static ServerMatchMessage _Read_Mirror_002EExamples_002EMultipleMatch_002EServerMatchMessage(NetworkReader reader)
	{
		return new ServerMatchMessage
		{
			serverMatchOperation = _Read_Mirror_002EExamples_002EMultipleMatch_002EServerMatchOperation(reader),
			matchId = reader.ReadGuid()
		};
	}

	public static ServerMatchOperation _Read_Mirror_002EExamples_002EMultipleMatch_002EServerMatchOperation(NetworkReader reader)
	{
		return (ServerMatchOperation)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_Mirror_002EExamples_002EMultipleMatch_002EServerMatchMessage(NetworkWriter writer, ServerMatchMessage value)
	{
		_Write_Mirror_002EExamples_002EMultipleMatch_002EServerMatchOperation(writer, value.serverMatchOperation);
		writer.WriteGuid(value.matchId);
	}

	public static void _Write_Mirror_002EExamples_002EMultipleMatch_002EServerMatchOperation(NetworkWriter writer, ServerMatchOperation value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static ClientMatchMessage _Read_Mirror_002EExamples_002EMultipleMatch_002EClientMatchMessage(NetworkReader reader)
	{
		return new ClientMatchMessage
		{
			clientMatchOperation = _Read_Mirror_002EExamples_002EMultipleMatch_002EClientMatchOperation(reader),
			matchId = reader.ReadGuid(),
			matchInfos = _Read_Mirror_002EExamples_002EMultipleMatch_002EMatchInfo_005B_005D(reader),
			playerInfos = _Read_Mirror_002EExamples_002EMultipleMatch_002EPlayerInfo_005B_005D(reader)
		};
	}

	public static ClientMatchOperation _Read_Mirror_002EExamples_002EMultipleMatch_002EClientMatchOperation(NetworkReader reader)
	{
		return (ClientMatchOperation)NetworkReaderExtensions.ReadByte(reader);
	}

	public static MatchInfo[] _Read_Mirror_002EExamples_002EMultipleMatch_002EMatchInfo_005B_005D(NetworkReader reader)
	{
		return reader.ReadArray<MatchInfo>();
	}

	public static MatchInfo _Read_Mirror_002EExamples_002EMultipleMatch_002EMatchInfo(NetworkReader reader)
	{
		return new MatchInfo
		{
			matchId = reader.ReadGuid(),
			players = NetworkReaderExtensions.ReadByte(reader),
			maxPlayers = NetworkReaderExtensions.ReadByte(reader)
		};
	}

	public static PlayerInfo[] _Read_Mirror_002EExamples_002EMultipleMatch_002EPlayerInfo_005B_005D(NetworkReader reader)
	{
		return reader.ReadArray<PlayerInfo>();
	}

	public static PlayerInfo _Read_Mirror_002EExamples_002EMultipleMatch_002EPlayerInfo(NetworkReader reader)
	{
		return new PlayerInfo
		{
			playerIndex = reader.ReadInt(),
			ready = reader.ReadBool(),
			matchId = reader.ReadGuid()
		};
	}

	public static void _Write_Mirror_002EExamples_002EMultipleMatch_002EClientMatchMessage(NetworkWriter writer, ClientMatchMessage value)
	{
		_Write_Mirror_002EExamples_002EMultipleMatch_002EClientMatchOperation(writer, value.clientMatchOperation);
		writer.WriteGuid(value.matchId);
		_Write_Mirror_002EExamples_002EMultipleMatch_002EMatchInfo_005B_005D(writer, value.matchInfos);
		_Write_Mirror_002EExamples_002EMultipleMatch_002EPlayerInfo_005B_005D(writer, value.playerInfos);
	}

	public static void _Write_Mirror_002EExamples_002EMultipleMatch_002EClientMatchOperation(NetworkWriter writer, ClientMatchOperation value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static void _Write_Mirror_002EExamples_002EMultipleMatch_002EMatchInfo_005B_005D(NetworkWriter writer, MatchInfo[] value)
	{
		writer.WriteArray(value);
	}

	public static void _Write_Mirror_002EExamples_002EMultipleMatch_002EMatchInfo(NetworkWriter writer, MatchInfo value)
	{
		writer.WriteGuid(value.matchId);
		NetworkWriterExtensions.WriteByte(writer, value.players);
		NetworkWriterExtensions.WriteByte(writer, value.maxPlayers);
	}

	public static void _Write_Mirror_002EExamples_002EMultipleMatch_002EPlayerInfo_005B_005D(NetworkWriter writer, PlayerInfo[] value)
	{
		writer.WriteArray(value);
	}

	public static void _Write_Mirror_002EExamples_002EMultipleMatch_002EPlayerInfo(NetworkWriter writer, PlayerInfo value)
	{
		writer.WriteInt(value.playerIndex);
		writer.WriteBool(value.ready);
		writer.WriteGuid(value.matchId);
	}

	public static ChatAuthenticator.AuthRequestMessage _Read_Mirror_002EExamples_002EChat_002EChatAuthenticator_002FAuthRequestMessage(NetworkReader reader)
	{
		return new ChatAuthenticator.AuthRequestMessage
		{
			authUsername = reader.ReadString()
		};
	}

	public static void _Write_Mirror_002EExamples_002EChat_002EChatAuthenticator_002FAuthRequestMessage(NetworkWriter writer, ChatAuthenticator.AuthRequestMessage value)
	{
		writer.WriteString(value.authUsername);
	}

	public static ChatAuthenticator.AuthResponseMessage _Read_Mirror_002EExamples_002EChat_002EChatAuthenticator_002FAuthResponseMessage(NetworkReader reader)
	{
		return new ChatAuthenticator.AuthResponseMessage
		{
			code = NetworkReaderExtensions.ReadByte(reader),
			message = reader.ReadString()
		};
	}

	public static void _Write_Mirror_002EExamples_002EChat_002EChatAuthenticator_002FAuthResponseMessage(NetworkWriter writer, ChatAuthenticator.AuthResponseMessage value)
	{
		NetworkWriterExtensions.WriteByte(writer, value.code);
		writer.WriteString(value.message);
	}

	public static MatchPlayerData _Read_Mirror_002EExamples_002EMultipleMatch_002EMatchPlayerData(NetworkReader reader)
	{
		return new MatchPlayerData
		{
			playerIndex = reader.ReadInt(),
			wins = reader.ReadInt(),
			currentScore = _Read_Mirror_002EExamples_002EMultipleMatch_002ECellValue(reader)
		};
	}

	public static CellValue _Read_Mirror_002EExamples_002EMultipleMatch_002ECellValue(NetworkReader reader)
	{
		return (CellValue)reader.ReadUShort();
	}

	public static void _Write_Mirror_002EExamples_002EMultipleMatch_002EMatchPlayerData(NetworkWriter writer, MatchPlayerData value)
	{
		writer.WriteInt(value.playerIndex);
		writer.WriteInt(value.wins);
		_Write_Mirror_002EExamples_002EMultipleMatch_002ECellValue(writer, value.currentScore);
	}

	public static void _Write_Mirror_002EExamples_002EMultipleMatch_002ECellValue(NetworkWriter writer, CellValue value)
	{
		writer.WriteUShort((ushort)value);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void InitReadWriters()
	{
		Writer<byte>.write = NetworkWriterExtensions.WriteByte;
		Writer<byte?>.write = NetworkWriterExtensions.WriteByteNullable;
		Writer<sbyte>.write = NetworkWriterExtensions.WriteSByte;
		Writer<sbyte?>.write = NetworkWriterExtensions.WriteSByteNullable;
		Writer<char>.write = NetworkWriterExtensions.WriteChar;
		Writer<char?>.write = NetworkWriterExtensions.WriteCharNullable;
		Writer<bool>.write = NetworkWriterExtensions.WriteBool;
		Writer<bool?>.write = NetworkWriterExtensions.WriteBoolNullable;
		Writer<short>.write = NetworkWriterExtensions.WriteShort;
		Writer<short?>.write = NetworkWriterExtensions.WriteShortNullable;
		Writer<ushort>.write = NetworkWriterExtensions.WriteUShort;
		Writer<ushort?>.write = NetworkWriterExtensions.WriteUShortNullable;
		Writer<int>.write = NetworkWriterExtensions.WriteInt;
		Writer<int?>.write = NetworkWriterExtensions.WriteIntNullable;
		Writer<uint>.write = NetworkWriterExtensions.WriteUInt;
		Writer<uint?>.write = NetworkWriterExtensions.WriteUIntNullable;
		Writer<long>.write = NetworkWriterExtensions.WriteLong;
		Writer<long?>.write = NetworkWriterExtensions.WriteLongNullable;
		Writer<ulong>.write = NetworkWriterExtensions.WriteULong;
		Writer<ulong?>.write = NetworkWriterExtensions.WriteULongNullable;
		Writer<float>.write = NetworkWriterExtensions.WriteFloat;
		Writer<float?>.write = NetworkWriterExtensions.WriteFloatNullable;
		Writer<double>.write = NetworkWriterExtensions.WriteDouble;
		Writer<double?>.write = NetworkWriterExtensions.WriteDoubleNullable;
		Writer<decimal>.write = NetworkWriterExtensions.WriteDecimal;
		Writer<decimal?>.write = NetworkWriterExtensions.WriteDecimalNullable;
		Writer<string>.write = NetworkWriterExtensions.WriteString;
		Writer<ArraySegment<byte>>.write = NetworkWriterExtensions.WriteBytesAndSizeSegment;
		Writer<byte[]>.write = NetworkWriterExtensions.WriteBytesAndSize;
		Writer<Vector2>.write = NetworkWriterExtensions.WriteVector2;
		Writer<Vector2?>.write = NetworkWriterExtensions.WriteVector2Nullable;
		Writer<Vector3>.write = NetworkWriterExtensions.WriteVector3;
		Writer<Vector3?>.write = NetworkWriterExtensions.WriteVector3Nullable;
		Writer<Vector4>.write = NetworkWriterExtensions.WriteVector4;
		Writer<Vector4?>.write = NetworkWriterExtensions.WriteVector4Nullable;
		Writer<Vector2Int>.write = NetworkWriterExtensions.WriteVector2Int;
		Writer<Vector2Int?>.write = NetworkWriterExtensions.WriteVector2IntNullable;
		Writer<Vector3Int>.write = NetworkWriterExtensions.WriteVector3Int;
		Writer<Vector3Int?>.write = NetworkWriterExtensions.WriteVector3IntNullable;
		Writer<Color>.write = NetworkWriterExtensions.WriteColor;
		Writer<Color?>.write = NetworkWriterExtensions.WriteColorNullable;
		Writer<Color32>.write = NetworkWriterExtensions.WriteColor32;
		Writer<Color32?>.write = NetworkWriterExtensions.WriteColor32Nullable;
		Writer<Quaternion>.write = NetworkWriterExtensions.WriteQuaternion;
		Writer<Quaternion?>.write = NetworkWriterExtensions.WriteQuaternionNullable;
		Writer<Rect>.write = NetworkWriterExtensions.WriteRect;
		Writer<Rect?>.write = NetworkWriterExtensions.WriteRectNullable;
		Writer<Plane>.write = NetworkWriterExtensions.WritePlane;
		Writer<Plane?>.write = NetworkWriterExtensions.WritePlaneNullable;
		Writer<Ray>.write = NetworkWriterExtensions.WriteRay;
		Writer<Ray?>.write = NetworkWriterExtensions.WriteRayNullable;
		Writer<Matrix4x4>.write = NetworkWriterExtensions.WriteMatrix4x4;
		Writer<Matrix4x4?>.write = NetworkWriterExtensions.WriteMatrix4x4Nullable;
		Writer<Guid>.write = NetworkWriterExtensions.WriteGuid;
		Writer<Guid?>.write = NetworkWriterExtensions.WriteGuidNullable;
		Writer<NetworkIdentity>.write = NetworkWriterExtensions.WriteNetworkIdentity;
		Writer<NetworkBehaviour>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<Transform>.write = NetworkWriterExtensions.WriteTransform;
		Writer<GameObject>.write = NetworkWriterExtensions.WriteGameObject;
		Writer<Uri>.write = NetworkWriterExtensions.WriteUri;
		Writer<Texture2D>.write = NetworkWriterExtensions.WriteTexture2D;
		Writer<Sprite>.write = NetworkWriterExtensions.WriteSprite;
		Writer<DateTime>.write = NetworkWriterExtensions.WriteDateTime;
		Writer<DateTime?>.write = NetworkWriterExtensions.WriteDateTimeNullable;
		Writer<TimeSnapshotMessage>.write = _Write_Mirror_002ETimeSnapshotMessage;
		Writer<ReadyMessage>.write = _Write_Mirror_002EReadyMessage;
		Writer<NotReadyMessage>.write = _Write_Mirror_002ENotReadyMessage;
		Writer<AddPlayerMessage>.write = _Write_Mirror_002EAddPlayerMessage;
		Writer<SceneMessage>.write = _Write_Mirror_002ESceneMessage;
		Writer<SceneOperation>.write = _Write_Mirror_002ESceneOperation;
		Writer<CommandMessage>.write = _Write_Mirror_002ECommandMessage;
		Writer<RpcMessage>.write = _Write_Mirror_002ERpcMessage;
		Writer<RpcBufferMessage>.write = _Write_Mirror_002ERpcBufferMessage;
		Writer<SpawnMessage>.write = _Write_Mirror_002ESpawnMessage;
		Writer<ChangeOwnerMessage>.write = _Write_Mirror_002EChangeOwnerMessage;
		Writer<ObjectSpawnStartedMessage>.write = _Write_Mirror_002EObjectSpawnStartedMessage;
		Writer<ObjectSpawnFinishedMessage>.write = _Write_Mirror_002EObjectSpawnFinishedMessage;
		Writer<ObjectDestroyMessage>.write = _Write_Mirror_002EObjectDestroyMessage;
		Writer<ObjectHideMessage>.write = _Write_Mirror_002EObjectHideMessage;
		Writer<EntityStateMessage>.write = _Write_Mirror_002EEntityStateMessage;
		Writer<NetworkPingMessage>.write = _Write_Mirror_002ENetworkPingMessage;
		Writer<NetworkPongMessage>.write = _Write_Mirror_002ENetworkPongMessage;
		Writer<ServerMatchMessage>.write = _Write_Mirror_002EExamples_002EMultipleMatch_002EServerMatchMessage;
		Writer<ServerMatchOperation>.write = _Write_Mirror_002EExamples_002EMultipleMatch_002EServerMatchOperation;
		Writer<ClientMatchMessage>.write = _Write_Mirror_002EExamples_002EMultipleMatch_002EClientMatchMessage;
		Writer<ClientMatchOperation>.write = _Write_Mirror_002EExamples_002EMultipleMatch_002EClientMatchOperation;
		Writer<MatchInfo[]>.write = _Write_Mirror_002EExamples_002EMultipleMatch_002EMatchInfo_005B_005D;
		Writer<MatchInfo>.write = _Write_Mirror_002EExamples_002EMultipleMatch_002EMatchInfo;
		Writer<PlayerInfo[]>.write = _Write_Mirror_002EExamples_002EMultipleMatch_002EPlayerInfo_005B_005D;
		Writer<PlayerInfo>.write = _Write_Mirror_002EExamples_002EMultipleMatch_002EPlayerInfo;
		Writer<ChatAuthenticator.AuthRequestMessage>.write = _Write_Mirror_002EExamples_002EChat_002EChatAuthenticator_002FAuthRequestMessage;
		Writer<ChatAuthenticator.AuthResponseMessage>.write = _Write_Mirror_002EExamples_002EChat_002EChatAuthenticator_002FAuthResponseMessage;
		Writer<MatchPlayerData>.write = _Write_Mirror_002EExamples_002EMultipleMatch_002EMatchPlayerData;
		Writer<CellValue>.write = _Write_Mirror_002EExamples_002EMultipleMatch_002ECellValue;
		Reader<byte>.read = NetworkReaderExtensions.ReadByte;
		Reader<byte?>.read = NetworkReaderExtensions.ReadByteNullable;
		Reader<sbyte>.read = NetworkReaderExtensions.ReadSByte;
		Reader<sbyte?>.read = NetworkReaderExtensions.ReadSByteNullable;
		Reader<char>.read = NetworkReaderExtensions.ReadChar;
		Reader<char?>.read = NetworkReaderExtensions.ReadCharNullable;
		Reader<bool>.read = NetworkReaderExtensions.ReadBool;
		Reader<bool?>.read = NetworkReaderExtensions.ReadBoolNullable;
		Reader<short>.read = NetworkReaderExtensions.ReadShort;
		Reader<short?>.read = NetworkReaderExtensions.ReadShortNullable;
		Reader<ushort>.read = NetworkReaderExtensions.ReadUShort;
		Reader<ushort?>.read = NetworkReaderExtensions.ReadUShortNullable;
		Reader<int>.read = NetworkReaderExtensions.ReadInt;
		Reader<int?>.read = NetworkReaderExtensions.ReadIntNullable;
		Reader<uint>.read = NetworkReaderExtensions.ReadUInt;
		Reader<uint?>.read = NetworkReaderExtensions.ReadUIntNullable;
		Reader<long>.read = NetworkReaderExtensions.ReadLong;
		Reader<long?>.read = NetworkReaderExtensions.ReadLongNullable;
		Reader<ulong>.read = NetworkReaderExtensions.ReadULong;
		Reader<ulong?>.read = NetworkReaderExtensions.ReadULongNullable;
		Reader<float>.read = NetworkReaderExtensions.ReadFloat;
		Reader<float?>.read = NetworkReaderExtensions.ReadFloatNullable;
		Reader<double>.read = NetworkReaderExtensions.ReadDouble;
		Reader<double?>.read = NetworkReaderExtensions.ReadDoubleNullable;
		Reader<decimal>.read = NetworkReaderExtensions.ReadDecimal;
		Reader<decimal?>.read = NetworkReaderExtensions.ReadDecimalNullable;
		Reader<string>.read = NetworkReaderExtensions.ReadString;
		Reader<byte[]>.read = NetworkReaderExtensions.ReadBytesAndSize;
		Reader<ArraySegment<byte>>.read = NetworkReaderExtensions.ReadBytesAndSizeSegment;
		Reader<Vector2>.read = NetworkReaderExtensions.ReadVector2;
		Reader<Vector2?>.read = NetworkReaderExtensions.ReadVector2Nullable;
		Reader<Vector3>.read = NetworkReaderExtensions.ReadVector3;
		Reader<Vector3?>.read = NetworkReaderExtensions.ReadVector3Nullable;
		Reader<Vector4>.read = NetworkReaderExtensions.ReadVector4;
		Reader<Vector4?>.read = NetworkReaderExtensions.ReadVector4Nullable;
		Reader<Vector2Int>.read = NetworkReaderExtensions.ReadVector2Int;
		Reader<Vector2Int?>.read = NetworkReaderExtensions.ReadVector2IntNullable;
		Reader<Vector3Int>.read = NetworkReaderExtensions.ReadVector3Int;
		Reader<Vector3Int?>.read = NetworkReaderExtensions.ReadVector3IntNullable;
		Reader<Color>.read = NetworkReaderExtensions.ReadColor;
		Reader<Color?>.read = NetworkReaderExtensions.ReadColorNullable;
		Reader<Color32>.read = NetworkReaderExtensions.ReadColor32;
		Reader<Color32?>.read = NetworkReaderExtensions.ReadColor32Nullable;
		Reader<Quaternion>.read = NetworkReaderExtensions.ReadQuaternion;
		Reader<Quaternion?>.read = NetworkReaderExtensions.ReadQuaternionNullable;
		Reader<Rect>.read = NetworkReaderExtensions.ReadRect;
		Reader<Rect?>.read = NetworkReaderExtensions.ReadRectNullable;
		Reader<Plane>.read = NetworkReaderExtensions.ReadPlane;
		Reader<Plane?>.read = NetworkReaderExtensions.ReadPlaneNullable;
		Reader<Ray>.read = NetworkReaderExtensions.ReadRay;
		Reader<Ray?>.read = NetworkReaderExtensions.ReadRayNullable;
		Reader<Matrix4x4>.read = NetworkReaderExtensions.ReadMatrix4x4;
		Reader<Matrix4x4?>.read = NetworkReaderExtensions.ReadMatrix4x4Nullable;
		Reader<Guid>.read = NetworkReaderExtensions.ReadGuid;
		Reader<Guid?>.read = NetworkReaderExtensions.ReadGuidNullable;
		Reader<NetworkIdentity>.read = NetworkReaderExtensions.ReadNetworkIdentity;
		Reader<NetworkBehaviour>.read = NetworkReaderExtensions.ReadNetworkBehaviour;
		Reader<NetworkBehaviourSyncVar>.read = NetworkReaderExtensions.ReadNetworkBehaviourSyncVar;
		Reader<Transform>.read = NetworkReaderExtensions.ReadTransform;
		Reader<GameObject>.read = NetworkReaderExtensions.ReadGameObject;
		Reader<Uri>.read = NetworkReaderExtensions.ReadUri;
		Reader<Texture2D>.read = NetworkReaderExtensions.ReadTexture2D;
		Reader<Sprite>.read = NetworkReaderExtensions.ReadSprite;
		Reader<DateTime>.read = NetworkReaderExtensions.ReadDateTime;
		Reader<DateTime?>.read = NetworkReaderExtensions.ReadDateTimeNullable;
		Reader<TimeSnapshotMessage>.read = _Read_Mirror_002ETimeSnapshotMessage;
		Reader<ReadyMessage>.read = _Read_Mirror_002EReadyMessage;
		Reader<NotReadyMessage>.read = _Read_Mirror_002ENotReadyMessage;
		Reader<AddPlayerMessage>.read = _Read_Mirror_002EAddPlayerMessage;
		Reader<SceneMessage>.read = _Read_Mirror_002ESceneMessage;
		Reader<SceneOperation>.read = _Read_Mirror_002ESceneOperation;
		Reader<CommandMessage>.read = _Read_Mirror_002ECommandMessage;
		Reader<RpcMessage>.read = _Read_Mirror_002ERpcMessage;
		Reader<RpcBufferMessage>.read = _Read_Mirror_002ERpcBufferMessage;
		Reader<SpawnMessage>.read = _Read_Mirror_002ESpawnMessage;
		Reader<ChangeOwnerMessage>.read = _Read_Mirror_002EChangeOwnerMessage;
		Reader<ObjectSpawnStartedMessage>.read = _Read_Mirror_002EObjectSpawnStartedMessage;
		Reader<ObjectSpawnFinishedMessage>.read = _Read_Mirror_002EObjectSpawnFinishedMessage;
		Reader<ObjectDestroyMessage>.read = _Read_Mirror_002EObjectDestroyMessage;
		Reader<ObjectHideMessage>.read = _Read_Mirror_002EObjectHideMessage;
		Reader<EntityStateMessage>.read = _Read_Mirror_002EEntityStateMessage;
		Reader<NetworkPingMessage>.read = _Read_Mirror_002ENetworkPingMessage;
		Reader<NetworkPongMessage>.read = _Read_Mirror_002ENetworkPongMessage;
		Reader<ServerMatchMessage>.read = _Read_Mirror_002EExamples_002EMultipleMatch_002EServerMatchMessage;
		Reader<ServerMatchOperation>.read = _Read_Mirror_002EExamples_002EMultipleMatch_002EServerMatchOperation;
		Reader<ClientMatchMessage>.read = _Read_Mirror_002EExamples_002EMultipleMatch_002EClientMatchMessage;
		Reader<ClientMatchOperation>.read = _Read_Mirror_002EExamples_002EMultipleMatch_002EClientMatchOperation;
		Reader<MatchInfo[]>.read = _Read_Mirror_002EExamples_002EMultipleMatch_002EMatchInfo_005B_005D;
		Reader<MatchInfo>.read = _Read_Mirror_002EExamples_002EMultipleMatch_002EMatchInfo;
		Reader<PlayerInfo[]>.read = _Read_Mirror_002EExamples_002EMultipleMatch_002EPlayerInfo_005B_005D;
		Reader<PlayerInfo>.read = _Read_Mirror_002EExamples_002EMultipleMatch_002EPlayerInfo;
		Reader<ChatAuthenticator.AuthRequestMessage>.read = _Read_Mirror_002EExamples_002EChat_002EChatAuthenticator_002FAuthRequestMessage;
		Reader<ChatAuthenticator.AuthResponseMessage>.read = _Read_Mirror_002EExamples_002EChat_002EChatAuthenticator_002FAuthResponseMessage;
		Reader<MatchPlayerData>.read = _Read_Mirror_002EExamples_002EMultipleMatch_002EMatchPlayerData;
		Reader<CellValue>.read = _Read_Mirror_002EExamples_002EMultipleMatch_002ECellValue;
	}
}
