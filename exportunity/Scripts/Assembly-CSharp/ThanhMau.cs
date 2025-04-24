using UnityEngine;
using UnityEngine.UI;

public class ThanhMau : MonoBehaviour
{
	public Slider thanh;

	private PlayerStat pS;

	private void Start()
	{
		pS = GetComponentInParent<PlayerStat>();
		thanh.maxValue = pS.maxHeath;
	}

	private void Update()
	{
		if (thanh.value != (float)pS.heath)
		{
			thanh.value = pS.heath;
		}
	}
}
