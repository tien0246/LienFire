using UnityEngine;

public class MiniMap : MonoBehaviour
{
	public GameObject vtMiniM;

	public Vector2 goc;

	public Vector2 gocM;

	public float tiLe;

	public float offSet;

	private Vector2 vtHT;

	private void Update()
	{
		vtHT = new Vector2(0f - base.transform.position.z, base.transform.position.x);
		Vector2 vector = goc - vtHT;
		vtMiniM.GetComponent<RectTransform>().anchoredPosition = gocM + vector * tiLe;
		vtMiniM.GetComponent<RectTransform>().transform.localRotation = Quaternion.Euler(0f, 0f, 0f - base.transform.rotation.eulerAngles.y + offSet);
	}
}
