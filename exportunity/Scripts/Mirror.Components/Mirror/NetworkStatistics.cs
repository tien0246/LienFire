using System;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/Network Statistics")]
[DisallowMultipleComponent]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-statistics")]
public class NetworkStatistics : MonoBehaviour
{
	private double intervalStartTime;

	[HideInInspector]
	public int clientIntervalReceivedPackets;

	[HideInInspector]
	public long clientIntervalReceivedBytes;

	[HideInInspector]
	public int clientIntervalSentPackets;

	[HideInInspector]
	public long clientIntervalSentBytes;

	[HideInInspector]
	public int clientReceivedPacketsPerSecond;

	[HideInInspector]
	public long clientReceivedBytesPerSecond;

	[HideInInspector]
	public int clientSentPacketsPerSecond;

	[HideInInspector]
	public long clientSentBytesPerSecond;

	[HideInInspector]
	public int serverIntervalReceivedPackets;

	[HideInInspector]
	public long serverIntervalReceivedBytes;

	[HideInInspector]
	public int serverIntervalSentPackets;

	[HideInInspector]
	public long serverIntervalSentBytes;

	[HideInInspector]
	public int serverReceivedPacketsPerSecond;

	[HideInInspector]
	public long serverReceivedBytesPerSecond;

	[HideInInspector]
	public int serverSentPacketsPerSecond;

	[HideInInspector]
	public long serverSentBytesPerSecond;

	private void Start()
	{
		Transport active = Transport.active;
		if (active != null)
		{
			active.OnClientDataReceived = (Action<ArraySegment<byte>, int>)Delegate.Combine(active.OnClientDataReceived, new Action<ArraySegment<byte>, int>(OnClientReceive));
			active.OnClientDataSent = (Action<ArraySegment<byte>, int>)Delegate.Combine(active.OnClientDataSent, new Action<ArraySegment<byte>, int>(OnClientSend));
			active.OnServerDataReceived = (Action<int, ArraySegment<byte>, int>)Delegate.Combine(active.OnServerDataReceived, new Action<int, ArraySegment<byte>, int>(OnServerReceive));
			active.OnServerDataSent = (Action<int, ArraySegment<byte>, int>)Delegate.Combine(active.OnServerDataSent, new Action<int, ArraySegment<byte>, int>(OnServerSend));
		}
		else
		{
			Debug.LogError($"NetworkStatistics: no available or active Transport found on this platform: {Application.platform}");
		}
	}

	private void OnDestroy()
	{
		Transport active = Transport.active;
		if (active != null)
		{
			active.OnClientDataReceived = (Action<ArraySegment<byte>, int>)Delegate.Remove(active.OnClientDataReceived, new Action<ArraySegment<byte>, int>(OnClientReceive));
			active.OnClientDataSent = (Action<ArraySegment<byte>, int>)Delegate.Remove(active.OnClientDataSent, new Action<ArraySegment<byte>, int>(OnClientSend));
			active.OnServerDataReceived = (Action<int, ArraySegment<byte>, int>)Delegate.Remove(active.OnServerDataReceived, new Action<int, ArraySegment<byte>, int>(OnServerReceive));
			active.OnServerDataSent = (Action<int, ArraySegment<byte>, int>)Delegate.Remove(active.OnServerDataSent, new Action<int, ArraySegment<byte>, int>(OnServerSend));
		}
	}

	private void OnClientReceive(ArraySegment<byte> data, int channelId)
	{
		clientIntervalReceivedPackets++;
		clientIntervalReceivedBytes += data.Count;
	}

	private void OnClientSend(ArraySegment<byte> data, int channelId)
	{
		clientIntervalSentPackets++;
		clientIntervalSentBytes += data.Count;
	}

	private void OnServerReceive(int connectionId, ArraySegment<byte> data, int channelId)
	{
		serverIntervalReceivedPackets++;
		serverIntervalReceivedBytes += data.Count;
	}

	private void OnServerSend(int connectionId, ArraySegment<byte> data, int channelId)
	{
		serverIntervalSentPackets++;
		serverIntervalSentBytes += data.Count;
	}

	private void Update()
	{
		if (NetworkTime.localTime >= intervalStartTime + 1.0)
		{
			if (NetworkClient.active)
			{
				UpdateClient();
			}
			if (NetworkServer.active)
			{
				UpdateServer();
			}
			intervalStartTime = NetworkTime.localTime;
		}
	}

	private void UpdateClient()
	{
		clientReceivedPacketsPerSecond = clientIntervalReceivedPackets;
		clientReceivedBytesPerSecond = clientIntervalReceivedBytes;
		clientSentPacketsPerSecond = clientIntervalSentPackets;
		clientSentBytesPerSecond = clientIntervalSentBytes;
		clientIntervalReceivedPackets = 0;
		clientIntervalReceivedBytes = 0L;
		clientIntervalSentPackets = 0;
		clientIntervalSentBytes = 0L;
	}

	private void UpdateServer()
	{
		serverReceivedPacketsPerSecond = serverIntervalReceivedPackets;
		serverReceivedBytesPerSecond = serverIntervalReceivedBytes;
		serverSentPacketsPerSecond = serverIntervalSentPackets;
		serverSentBytesPerSecond = serverIntervalSentBytes;
		serverIntervalReceivedPackets = 0;
		serverIntervalReceivedBytes = 0L;
		serverIntervalSentPackets = 0;
		serverIntervalSentBytes = 0L;
	}

	private void OnGUI()
	{
		if (NetworkClient.active || NetworkServer.active)
		{
			GUILayout.BeginArea(new Rect(10f, 120f, 215f, 300f));
			if (NetworkClient.active)
			{
				OnClientGUI();
			}
			if (NetworkServer.active)
			{
				OnServerGUI();
			}
			GUILayout.EndArea();
		}
	}

	private void OnClientGUI()
	{
		GUILayout.BeginVertical("Box");
		GUILayout.Label("<b>Client Statistics</b>");
		GUILayout.Label($"Send: {clientSentPacketsPerSecond} msgs @ {Utils.PrettyBytes(clientSentBytesPerSecond)}/s");
		GUILayout.Label($"Recv: {clientReceivedPacketsPerSecond} msgs @ {Utils.PrettyBytes(clientReceivedBytesPerSecond)}/s");
		GUILayout.EndVertical();
	}

	private void OnServerGUI()
	{
		GUILayout.BeginVertical("Box");
		GUILayout.Label("<b>Server Statistics</b>");
		GUILayout.Label($"Send: {serverSentPacketsPerSecond} msgs @ {Utils.PrettyBytes(serverSentBytesPerSecond)}/s");
		GUILayout.Label($"Recv: {serverReceivedPacketsPerSecond} msgs @ {Utils.PrettyBytes(serverReceivedBytesPerSecond)}/s");
		GUILayout.EndVertical();
	}
}
