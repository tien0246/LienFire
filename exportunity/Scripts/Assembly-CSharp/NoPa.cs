using TMPro;
using UnityEngine;

public class NoPa : MonoBehaviour
{
	private TMP_Dropdown drop;

	private void Start()
	{
		drop = base.transform.GetComponent<TMP_Dropdown>();
		drop.onValueChanged.AddListener(delegate
		{
			DoiChatLuong();
		});
	}

	private void DoiChatLuong()
	{
		QualitySettings.SetQualityLevel(drop.value);
	}
}
