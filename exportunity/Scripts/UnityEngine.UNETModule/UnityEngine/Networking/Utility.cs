using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking;

public class Utility
{
	private static Dictionary<NetworkID, NetworkAccessToken> s_dictTokens = new Dictionary<NetworkID, NetworkAccessToken>();

	[Obsolete("This property is unused and should not be referenced in code.", true)]
	public static bool useRandomSourceID
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	private Utility()
	{
	}

	public static SourceID GetSourceID()
	{
		return (SourceID)SystemInfo.deviceUniqueIdentifier.GetHashCode();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("This function is unused and should not be referenced in code. Please sign in and setup your project in the editor instead.", true)]
	public static void SetAppID(AppID newAppID)
	{
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("This function is unused and should not be referenced in code. Please sign in and setup your project in the editor instead.", true)]
	public static AppID GetAppID()
	{
		return AppID.Invalid;
	}

	public static void SetAccessTokenForNetwork(NetworkID netId, NetworkAccessToken accessToken)
	{
		if (s_dictTokens.ContainsKey(netId))
		{
			s_dictTokens.Remove(netId);
		}
		s_dictTokens.Add(netId, accessToken);
	}

	public static NetworkAccessToken GetAccessTokenForNetwork(NetworkID netId)
	{
		if (!s_dictTokens.TryGetValue(netId, out var value))
		{
			return new NetworkAccessToken();
		}
		return value;
	}
}
