using UnityEngine;

public class DTButt : MonoBehaviour
{
	private bool onP;

	private void Update()
	{
		if (onP)
		{
			GetComponentInParent<PlayerAtack>().DanhThuong();
		}
	}

	public void Clic()
	{
		onP = true;
	}

	public void unClic()
	{
		onP = false;
	}
}
