using System;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Mirror;

public static class NetworkLoop
{
	internal enum AddMode
	{
		Beginning = 0,
		End = 1
	}

	public static Action OnEarlyUpdate;

	public static Action OnLateUpdate;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void ResetStatics()
	{
		OnEarlyUpdate = null;
		OnLateUpdate = null;
	}

	internal static int FindPlayerLoopEntryIndex(PlayerLoopSystem.UpdateFunction function, PlayerLoopSystem playerLoop, Type playerLoopSystemType)
	{
		if (playerLoop.type == playerLoopSystemType)
		{
			return Array.FindIndex(playerLoop.subSystemList, (PlayerLoopSystem elem) => elem.updateDelegate == function);
		}
		if (playerLoop.subSystemList != null)
		{
			for (int num = 0; num < playerLoop.subSystemList.Length; num++)
			{
				int num2 = FindPlayerLoopEntryIndex(function, playerLoop.subSystemList[num], playerLoopSystemType);
				if (num2 != -1)
				{
					return num2;
				}
			}
		}
		return -1;
	}

	internal static bool AddToPlayerLoop(PlayerLoopSystem.UpdateFunction function, Type ownerType, ref PlayerLoopSystem playerLoop, Type playerLoopSystemType, AddMode addMode)
	{
		if (playerLoop.type == playerLoopSystemType)
		{
			if (Array.FindIndex(playerLoop.subSystemList, (PlayerLoopSystem s) => s.updateDelegate == function) != -1)
			{
				return true;
			}
			int num = ((playerLoop.subSystemList != null) ? playerLoop.subSystemList.Length : 0);
			Array.Resize(ref playerLoop.subSystemList, num + 1);
			PlayerLoopSystem playerLoopSystem = new PlayerLoopSystem
			{
				type = ownerType,
				updateDelegate = function
			};
			switch (addMode)
			{
			case AddMode.Beginning:
				Array.Copy(playerLoop.subSystemList, 0, playerLoop.subSystemList, 1, playerLoop.subSystemList.Length - 1);
				playerLoop.subSystemList[0] = playerLoopSystem;
				break;
			case AddMode.End:
				playerLoop.subSystemList[num] = playerLoopSystem;
				break;
			}
			return true;
		}
		if (playerLoop.subSystemList != null)
		{
			for (int num2 = 0; num2 < playerLoop.subSystemList.Length; num2++)
			{
				if (AddToPlayerLoop(function, ownerType, ref playerLoop.subSystemList[num2], playerLoopSystemType, addMode))
				{
					return true;
				}
			}
		}
		return false;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void RuntimeInitializeOnLoad()
	{
		PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
		AddToPlayerLoop(NetworkEarlyUpdate, typeof(NetworkLoop), ref playerLoop, typeof(EarlyUpdate), AddMode.End);
		AddToPlayerLoop(NetworkLateUpdate, typeof(NetworkLoop), ref playerLoop, typeof(PreLateUpdate), AddMode.End);
		PlayerLoop.SetPlayerLoop(playerLoop);
	}

	private static void NetworkEarlyUpdate()
	{
		if (Application.isPlaying)
		{
			NetworkServer.NetworkEarlyUpdate();
			NetworkClient.NetworkEarlyUpdate();
			OnEarlyUpdate?.Invoke();
		}
	}

	private static void NetworkLateUpdate()
	{
		if (Application.isPlaying)
		{
			OnLateUpdate?.Invoke();
			NetworkServer.NetworkLateUpdate();
			NetworkClient.NetworkLateUpdate();
		}
	}
}
