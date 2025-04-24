using UnityEngine;

public class TuHuy : MonoBehaviour
{
	public float tgTonTai;

	private float tg;

	private void Update()
	{
		tg += Time.deltaTime;
		if (tg >= tgTonTai)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
