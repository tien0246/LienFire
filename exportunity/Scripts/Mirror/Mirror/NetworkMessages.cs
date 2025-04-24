using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public static class NetworkMessages
{
	public const int IdSize = 2;

	public static int MaxContentSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Transport.active.GetMaxPacketSize() - 2 - 8;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use NetworkMessageId<T>.Id instead")]
	public static ushort GetId<T>() where T : struct, NetworkMessage
	{
		return NetworkMessageId<T>.Id;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Pack<T>(T message, NetworkWriter writer) where T : struct, NetworkMessage
	{
		writer.WriteUShort(NetworkMessageId<T>.Id);
		writer.Write(message);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool UnpackId(NetworkReader reader, out ushort messageId)
	{
		try
		{
			messageId = reader.ReadUShort();
			return true;
		}
		catch (EndOfStreamException)
		{
			messageId = 0;
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static NetworkMessageDelegate WrapHandler<T, C>(Action<C, T, int> handler, bool requireAuthentication) where T : struct, NetworkMessage where C : NetworkConnection
	{
		return delegate(NetworkConnection conn, NetworkReader reader, int channelId)
		{
			T val = default(T);
			int position = reader.Position;
			try
			{
				if (requireAuthentication && !conn.isAuthenticated)
				{
					Debug.LogWarning($"Closing connection: {conn}. Received message {typeof(T)} that required authentication, but the user has not authenticated yet");
					conn.Disconnect();
					return;
				}
				val = reader.Read<T>();
			}
			catch (Exception arg)
			{
				Debug.LogError($"Closed connection: {conn}. This can happen if the other side accidentally (or an attacker intentionally) sent invalid data. Reason: {arg}");
				conn.Disconnect();
				return;
			}
			finally
			{
				int position2 = reader.Position;
				NetworkDiagnostics.OnReceive(val, channelId, position2 - position);
			}
			try
			{
				handler((C)conn, val, channelId);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Disconnecting connId={conn.connectionId} to prevent exploits from an Exception in MessageHandler: {ex.GetType().Name} {ex.Message}\n{ex.StackTrace}");
				conn.Disconnect();
			}
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static NetworkMessageDelegate WrapHandler<T, C>(Action<C, T> handler, bool requireAuthentication) where T : struct, NetworkMessage where C : NetworkConnection
	{
		return WrapHandler<T, C>(Wrapped, requireAuthentication);
		void Wrapped(C conn, T msg, int _)
		{
			handler(conn, msg);
		}
	}
}
