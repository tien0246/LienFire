using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.Basic;

public class PlayerUI : MonoBehaviour
{
	[Header("Player Components")]
	public Image image;

	[Header("Child Text Objects")]
	public Text playerNameText;

	public Text playerDataText;

	public void SetLocalPlayer()
	{
		image.color = new Color(1f, 1f, 1f, 0.1f);
	}

	public void OnPlayerNumberChanged(byte newPlayerNumber)
	{
		playerNameText.text = $"Player {newPlayerNumber:00}";
	}

	public void OnPlayerColorChanged(Color32 newPlayerColor)
	{
		playerNameText.color = newPlayerColor;
	}

	public void OnPlayerDataChanged(ushort newPlayerData)
	{
		playerDataText.text = $"Data: {newPlayerData:000}";
	}
}
