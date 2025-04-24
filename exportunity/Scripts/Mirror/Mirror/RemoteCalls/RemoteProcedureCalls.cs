using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.RemoteCalls;

public static class RemoteProcedureCalls
{
	private static readonly Dictionary<ushort, Invoker> remoteCallDelegates = new Dictionary<ushort, Invoker>();

	private static bool CheckIfDelegateExists(Type componentType, RemoteCallType remoteCallType, RemoteCallDelegate func, ushort functionHash)
	{
		if (remoteCallDelegates.ContainsKey(functionHash))
		{
			Invoker invoker = remoteCallDelegates[functionHash];
			if (invoker.AreEqual(componentType, remoteCallType, func))
			{
				return true;
			}
			Debug.LogError($"Function {invoker.componentType}.{invoker.function.GetMethodName()} and {componentType}.{func.GetMethodName()} have the same hash. Please rename one of them. To save bandwidth, we only use 2 bytes for the hash, which has a small chance of collisions.");
		}
		return false;
	}

	internal static ushort RegisterDelegate(Type componentType, string functionFullName, RemoteCallType remoteCallType, RemoteCallDelegate func, bool cmdRequiresAuthority = true)
	{
		ushort num = (ushort)(functionFullName.GetStableHashCode() & 0xFFFF);
		if (CheckIfDelegateExists(componentType, remoteCallType, func, num))
		{
			return num;
		}
		remoteCallDelegates[num] = new Invoker
		{
			callType = remoteCallType,
			componentType = componentType,
			function = func,
			cmdRequiresAuthority = cmdRequiresAuthority
		};
		return num;
	}

	public static void RegisterCommand(Type componentType, string functionFullName, RemoteCallDelegate func, bool requiresAuthority)
	{
		RegisterDelegate(componentType, functionFullName, RemoteCallType.Command, func, requiresAuthority);
	}

	public static void RegisterRpc(Type componentType, string functionFullName, RemoteCallDelegate func)
	{
		RegisterDelegate(componentType, functionFullName, RemoteCallType.ClientRpc, func);
	}

	internal static void RemoveDelegate(ushort hash)
	{
		remoteCallDelegates.Remove(hash);
	}

	private static bool GetInvokerForHash(ushort functionHash, RemoteCallType remoteCallType, out Invoker invoker)
	{
		if (remoteCallDelegates.TryGetValue(functionHash, out invoker) && invoker != null)
		{
			return invoker.callType == remoteCallType;
		}
		return false;
	}

	internal static bool Invoke(ushort functionHash, RemoteCallType remoteCallType, NetworkReader reader, NetworkBehaviour component, NetworkConnectionToClient senderConnection = null)
	{
		if (GetInvokerForHash(functionHash, remoteCallType, out var invoker) && invoker.componentType.IsInstanceOfType(component))
		{
			invoker.function(component, reader, senderConnection);
			return true;
		}
		return false;
	}

	internal static bool CommandRequiresAuthority(ushort cmdHash)
	{
		if (GetInvokerForHash(cmdHash, RemoteCallType.Command, out var invoker))
		{
			return invoker.cmdRequiresAuthority;
		}
		return false;
	}

	public static RemoteCallDelegate GetDelegate(ushort functionHash)
	{
		if (!remoteCallDelegates.TryGetValue(functionHash, out var value))
		{
			return null;
		}
		return value.function;
	}
}
