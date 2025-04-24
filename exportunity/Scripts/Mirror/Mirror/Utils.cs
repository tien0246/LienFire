using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror;

public static class Utils
{
	public static uint GetTrueRandomUInt()
	{
		using RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
		byte[] array = new byte[4];
		rNGCryptoServiceProvider.GetBytes(array);
		return BitConverter.ToUInt32(array, 0);
	}

	public static bool IsPrefab(GameObject obj)
	{
		return false;
	}

	public static bool IsSceneObject(NetworkIdentity identity)
	{
		if (identity.gameObject.hideFlags != HideFlags.NotEditable && identity.gameObject.hideFlags != HideFlags.HideAndDontSave)
		{
			return identity.sceneId != 0;
		}
		return false;
	}

	public static bool IsSceneObjectWithPrefabParent(GameObject gameObject, out GameObject prefab)
	{
		prefab = null;
		if (prefab == null)
		{
			Debug.LogError("Failed to find prefab parent for scene object [name:" + gameObject.name + "]");
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPointInScreen(Vector2 point)
	{
		if (0f <= point.x && point.x < (float)Screen.width && 0f <= point.y)
		{
			return point.y < (float)Screen.height;
		}
		return false;
	}

	public static string PrettyBytes(long bytes)
	{
		if (bytes < 1024)
		{
			return $"{bytes} B";
		}
		if (bytes < 1048576)
		{
			return $"{(float)bytes / 1024f:F2} KB";
		}
		if (bytes < 1073741824)
		{
			return $"{(float)bytes / 1048576f:F2} MB";
		}
		return $"{(float)bytes / 1.0737418E+09f:F2} GB";
	}

	public static string PrettySeconds(double seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		string text = "";
		if (timeSpan.Days > 0)
		{
			text += $"{timeSpan.Days}d";
		}
		if (timeSpan.Hours > 0)
		{
			text += string.Format("{0}{1}h", (text.Length > 0) ? " " : "", timeSpan.Hours);
		}
		if (timeSpan.Minutes > 0)
		{
			text += string.Format("{0}{1}m", (text.Length > 0) ? " " : "", timeSpan.Minutes);
		}
		if (timeSpan.Milliseconds > 0)
		{
			text += string.Format("{0}{1}.{2}s", (text.Length > 0) ? " " : "", timeSpan.Seconds, timeSpan.Milliseconds / 100);
		}
		else if (timeSpan.Seconds > 0)
		{
			text += string.Format("{0}{1}s", (text.Length > 0) ? " " : "", timeSpan.Seconds);
		}
		if (!(text != ""))
		{
			return "0s";
		}
		return text;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NetworkIdentity GetSpawnedInServerOrClient(uint netId)
	{
		if (NetworkServer.active)
		{
			NetworkServer.spawned.TryGetValue(netId, out var value);
			return value;
		}
		if (NetworkClient.active)
		{
			NetworkClient.spawned.TryGetValue(netId, out var value2);
			return value2;
		}
		return null;
	}

	public static Rect KeepInScreen(Rect rect)
	{
		rect.x = Math.Max(rect.x, 0f);
		rect.y = Math.Max(rect.y, 0f);
		rect.x = Math.Min(rect.x, (float)Screen.width - rect.width);
		rect.y = Math.Min(rect.y, (float)Screen.width - rect.height);
		return rect;
	}

	public static void CreateLocalConnections(out LocalConnectionToClient connectionToClient, out LocalConnectionToServer connectionToServer)
	{
		connectionToServer = new LocalConnectionToServer();
		connectionToClient = new LocalConnectionToClient();
		connectionToServer.connectionToClient = connectionToClient;
		connectionToClient.connectionToServer = connectionToServer;
	}

	public static bool IsSceneActive(string scene)
	{
		Scene activeScene = SceneManager.GetActiveScene();
		if (!(activeScene.path == scene))
		{
			return activeScene.name == scene;
		}
		return true;
	}
}
