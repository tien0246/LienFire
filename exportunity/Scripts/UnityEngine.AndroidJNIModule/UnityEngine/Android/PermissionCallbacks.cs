using System;
using UnityEngine.Scripting;

namespace UnityEngine.Android;

public class PermissionCallbacks : AndroidJavaProxy
{
	public event Action<string> PermissionGranted;

	public event Action<string> PermissionDenied;

	public event Action<string> PermissionDeniedAndDontAskAgain;

	public PermissionCallbacks()
		: base("com.unity3d.player.IPermissionRequestCallbacks")
	{
	}

	[Preserve]
	private void onPermissionGranted(string permissionName)
	{
		this.PermissionGranted?.Invoke(permissionName);
	}

	[Preserve]
	private void onPermissionDenied(string permissionName)
	{
		this.PermissionDenied?.Invoke(permissionName);
	}

	[Preserve]
	private void onPermissionDeniedAndDontAskAgain(string permissionName)
	{
		if (this.PermissionDeniedAndDontAskAgain != null)
		{
			this.PermissionDeniedAndDontAskAgain(permissionName);
		}
		else
		{
			this.PermissionDenied?.Invoke(permissionName);
		}
	}
}
