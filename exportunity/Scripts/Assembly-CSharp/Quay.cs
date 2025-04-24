using UnityEngine;

public class Quay : MonoBehaviour
{
	public float td;

	private void Update()
	{
		base.transform.Rotate(0f, td * Time.deltaTime, 0f);
	}
}
