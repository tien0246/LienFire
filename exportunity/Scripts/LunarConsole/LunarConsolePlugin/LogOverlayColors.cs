using System;
using UnityEngine;

namespace LunarConsolePlugin;

[Serializable]
public class LogOverlayColors
{
	[SerializeField]
	public LogEntryColors exception = MakeColors(4293543494u, 4280163870u);

	[SerializeField]
	public LogEntryColors error = MakeColors(4293543494u, 4280163870u);

	[SerializeField]
	public LogEntryColors warning = MakeColors(4291545920u, 4280163870u);

	[SerializeField]
	public LogEntryColors debug = MakeColors(4288404991u, 4280163870u);

	private static LogEntryColors MakeColors(uint foreground, uint background)
	{
		return new LogEntryColors
		{
			foreground = MakeColor(foreground),
			background = MakeColor(background)
		};
	}

	private static Color32 MakeColor(uint argb)
	{
		byte a = (byte)((argb >> 24) & 0xFF);
		byte r = (byte)((argb >> 16) & 0xFF);
		byte g = (byte)((argb >> 8) & 0xFF);
		byte b = (byte)(argb & 0xFF);
		return new Color32(r, g, b, a);
	}
}
