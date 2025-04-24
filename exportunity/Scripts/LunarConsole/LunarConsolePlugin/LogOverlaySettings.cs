using System;
using UnityEngine;

namespace LunarConsolePlugin;

[Serializable]
public class LogOverlaySettings
{
	[SerializeField]
	public bool enabled;

	[SerializeField]
	[Tooltip("Maximum visible lines count")]
	public int maxVisibleLines = 3;

	[SerializeField]
	[Tooltip("The amount of time each line would be displayed")]
	public float timeout = 1f;

	[SerializeField]
	public LogOverlayColors colors = new LogOverlayColors();
}
