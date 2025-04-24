using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class ThanhKhac : MonoBehaviour
{
	public Slider thanh;

	public Image im;

	private PlayerStat pS;

	private void Start()
	{
		pS = GetComponentInParent<PlayerStat>();
		if (pS.laLocal)
		{
			base.gameObject.SetActive(value: false);
		}
		thanh.maxValue = pS.maxHeath;
	}

	private void Update()
	{
		PlayerStat component = NetworkClient.localPlayer.gameObject.GetComponent<PlayerStat>();
		if (component != null)
		{
			if (component.team == pS.team)
			{
				im.color = Color.blue;
			}
			else
			{
				im.color = Color.red;
			}
		}
		if (thanh.value != (float)pS.heath)
		{
			thanh.value = pS.heath;
		}
	}
}
