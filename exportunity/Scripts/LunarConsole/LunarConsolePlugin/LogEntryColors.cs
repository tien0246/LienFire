using System;
using UnityEngine;

namespace LunarConsolePlugin;

[Serializable]
public class LogEntryColors
{
	[SerializeField]
	public Color32 foreground;

	[SerializeField]
	public Color32 background;
}
