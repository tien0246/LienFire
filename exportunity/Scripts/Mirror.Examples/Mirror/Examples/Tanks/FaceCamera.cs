using UnityEngine;

namespace Mirror.Examples.Tanks;

public class FaceCamera : MonoBehaviour
{
	private void LateUpdate()
	{
		base.transform.forward = Camera.main.transform.forward;
	}
}
