using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.MultipleMatch;

public class RoomGUI : MonoBehaviour
{
	public GameObject playerList;

	public GameObject playerPrefab;

	public GameObject cancelButton;

	public GameObject leaveButton;

	public Button startButton;

	public bool owner;

	[ClientCallback]
	public void RefreshRoomPlayers(PlayerInfo[] playerInfos)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		foreach (Transform item in playerList.transform)
		{
			Object.Destroy(item.gameObject);
		}
		startButton.interactable = false;
		bool flag = true;
		for (int i = 0; i < playerInfos.Length; i++)
		{
			PlayerInfo playerInfo = playerInfos[i];
			GameObject obj = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
			obj.transform.SetParent(playerList.transform, worldPositionStays: false);
			obj.GetComponent<PlayerGUI>().SetPlayerInfo(playerInfo);
			if (!playerInfo.ready)
			{
				flag = false;
			}
		}
		startButton.interactable = flag && owner && playerInfos.Length > 1;
	}

	[ClientCallback]
	public void SetOwner(bool owner)
	{
		if (NetworkClient.active)
		{
			this.owner = owner;
			cancelButton.SetActive(owner);
			leaveButton.SetActive(!owner);
		}
	}
}
