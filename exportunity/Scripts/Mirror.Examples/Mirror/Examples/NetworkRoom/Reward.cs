using UnityEngine;

namespace Mirror.Examples.NetworkRoom;

[RequireComponent(typeof(RandomColor))]
public class Reward : NetworkBehaviour
{
	public bool available = true;

	public RandomColor randomColor;

	private void OnValidate()
	{
		if (randomColor == null)
		{
			randomColor = GetComponent<RandomColor>();
		}
	}

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active && other.gameObject.CompareTag("Player"))
		{
			ClaimPrize(other.gameObject);
		}
	}

	[ServerCallback]
	public void ClaimPrize(GameObject player)
	{
		if (NetworkServer.active && available)
		{
			available = false;
			Color32 color = randomColor.color;
			uint num = (uint)((color.r + color.g + color.b) / 3);
			PlayerScore component = player.GetComponent<PlayerScore>();
			component.Networkscore = component.score + num;
			Spawner.SpawnReward();
			NetworkServer.Destroy(base.gameObject);
		}
	}

	private void MirrorProcessed()
	{
	}
}
