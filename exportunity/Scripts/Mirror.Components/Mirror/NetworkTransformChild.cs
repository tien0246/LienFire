using System;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("")]
[Obsolete("NetworkTransformChild is not needed anymore. The .target is now exposed in NetworkTransform itself. Note you can open the Inspector in debug view and replace the source script instead of reassigning everything.")]
public class NetworkTransformChild : NetworkTransform
{
	private void MirrorProcessed()
	{
	}
}
