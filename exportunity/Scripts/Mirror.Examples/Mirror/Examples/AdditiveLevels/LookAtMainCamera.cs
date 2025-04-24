using UnityEngine;

namespace Mirror.Examples.AdditiveLevels;

public class LookAtMainCamera : MonoBehaviour
{
	private void OnValidate()
	{
		base.enabled = false;
	}

	[ClientCallback]
	private void LateUpdate()
	{
		if (NetworkClient.active)
		{
			base.transform.forward = Camera.main.transform.forward;
		}
	}
}
