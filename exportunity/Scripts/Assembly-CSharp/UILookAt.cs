using UnityEngine;

public class UILookAt : MonoBehaviour
{
	private void Update()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			base.transform.LookAt(base.transform.position + main.transform.rotation * Vector3.forward, main.transform.rotation * Vector3.up);
		}
	}
}
