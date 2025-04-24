using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public static class NetworkTime
{
	public static float PingFrequency = 2f;

	public static int PingWindowSize = 6;

	private static double lastPingTime;

	private static ExponentialMovingAverage _rtt = new ExponentialMovingAverage(PingWindowSize);

	public static double localTime
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Time.timeAsDouble;
		}
	}

	public static double time
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (!NetworkServer.active)
			{
				return NetworkClient.localTimeline;
			}
			return localTime;
		}
	}

	public static double offset => localTime - time;

	public static double rtt => _rtt.Value;

	[RuntimeInitializeOnLoadMethod]
	public static void ResetStatics()
	{
		PingFrequency = 2f;
		PingWindowSize = 6;
		lastPingTime = 0.0;
		_rtt = new ExponentialMovingAverage(PingWindowSize);
	}

	internal static void UpdateClient()
	{
		if (localTime - lastPingTime >= (double)PingFrequency)
		{
			NetworkClient.Send(new NetworkPingMessage(localTime), 1);
			lastPingTime = localTime;
		}
	}

	internal static void OnServerPing(NetworkConnectionToClient conn, NetworkPingMessage message)
	{
		NetworkPongMessage message2 = new NetworkPongMessage
		{
			clientTime = message.clientTime
		};
		conn.Send(message2, 1);
	}

	internal static void OnClientPong(NetworkPongMessage message)
	{
		double newValue = localTime - message.clientTime;
		_rtt.Add(newValue);
	}
}
