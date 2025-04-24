using UnityEngine;

public class HitDam : MonoBehaviour
{
	public float td;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.position += new Vector3(0f, td, 0f) * Time.deltaTime;
	}
}
