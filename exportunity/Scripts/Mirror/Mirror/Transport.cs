using System;
using UnityEngine;

namespace Mirror;

public abstract class Transport : MonoBehaviour
{
	public static Transport active;

	public Action OnClientConnected;

	public Action<ArraySegment<byte>, int> OnClientDataReceived;

	public Action<ArraySegment<byte>, int> OnClientDataSent;

	public Action<TransportError, string> OnClientError;

	public Action OnClientDisconnected;

	public Action<int> OnServerConnected;

	public Action<int, ArraySegment<byte>, int> OnServerDataReceived;

	public Action<int, ArraySegment<byte>, int> OnServerDataSent;

	public Action<int, TransportError, string> OnServerError;

	public Action<int> OnServerDisconnected;

	public abstract bool Available();

	public abstract bool ClientConnected();

	public abstract void ClientConnect(string address);

	public virtual void ClientConnect(Uri uri)
	{
		ClientConnect(uri.Host);
	}

	public abstract void ClientSend(ArraySegment<byte> segment, int channelId = 0);

	public abstract void ClientDisconnect();

	public abstract Uri ServerUri();

	public abstract bool ServerActive();

	public abstract void ServerStart();

	public abstract void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId = 0);

	public abstract void ServerDisconnect(int connectionId);

	public abstract string ServerGetClientAddress(int connectionId);

	public abstract void ServerStop();

	public abstract int GetMaxPacketSize(int channelId = 0);

	public virtual int GetBatchThreshold(int channelId = 0)
	{
		return GetMaxPacketSize(channelId);
	}

	public void Update()
	{
	}

	public void LateUpdate()
	{
	}

	public virtual void ClientEarlyUpdate()
	{
	}

	public virtual void ServerEarlyUpdate()
	{
	}

	public virtual void ClientLateUpdate()
	{
	}

	public virtual void ServerLateUpdate()
	{
	}

	public abstract void Shutdown();

	public virtual void OnApplicationQuit()
	{
		Shutdown();
	}
}
