using UnityEngine;

namespace LunarConsolePluginInternal;

public static class LunarConsoleConfig
{
	public static bool consoleEnabled;

	public static readonly bool consoleSupported;

	public static readonly bool freeVersion;

	public static readonly bool fullVersion;

	public static bool actionsEnabled
	{
		get
		{
			if (consoleSupported && consoleEnabled)
			{
				return Application.platform == RuntimePlatform.Android;
			}
			return false;
		}
	}

	static LunarConsoleConfig()
	{
		consoleEnabled = true;
		consoleSupported = true;
		freeVersion = true;
		fullVersion = false;
	}
}
