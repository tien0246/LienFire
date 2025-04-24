using System;
using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[AddComponentMenu("Network/Network Ping Display")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-ping-display")]
public class NetworkPingDisplay : MonoBehaviour
{
	public Color color = Color.white;

	public int padding = 2;

	public int width = 150;

	public int height = 25;

	private void OnGUI()
	{
		if (NetworkClient.active)
		{
			GUI.color = color;
			Rect position = new Rect(Screen.width - width - padding, Screen.height - height - padding, width, height);
			GUIStyle style = GUI.skin.GetStyle("Label");
			style.alignment = TextAnchor.MiddleRight;
			GUI.Label(position, $"RTT: {Math.Round(NetworkTime.rtt * 1000.0)}ms", style);
			GUI.color = Color.white;
		}
	}
}
