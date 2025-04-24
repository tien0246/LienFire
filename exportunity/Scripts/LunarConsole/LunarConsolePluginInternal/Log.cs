using System;
using System.Diagnostics;
using UnityEngine;

namespace LunarConsolePluginInternal;

internal static class Log
{
	private static readonly string TAG = "[" + Constants.PluginDisplayName + "]";

	[Conditional("LUNAR_CONSOLE_DEVELOPMENT")]
	public static void dev(string format, params object[] args)
	{
		UnityEngine.Debug.Log(TAG + " " + StringUtils.TryFormat(format, args));
	}

	public static void e(Exception exception)
	{
		if (exception != null)
		{
			UnityEngine.Debug.LogError(TAG + " " + exception.Message + "\n" + exception.StackTrace);
		}
		else
		{
			UnityEngine.Debug.LogError(TAG + " Exception");
		}
	}

	public static void e(Exception exception, string format, params object[] args)
	{
		e(exception, StringUtils.TryFormat(format, args));
	}

	public static void e(Exception exception, string message)
	{
		if (exception != null)
		{
			UnityEngine.Debug.LogError(TAG + " " + message + "\n" + exception.Message + "\n" + exception.StackTrace);
			Exception ex = exception;
			while ((ex = ex.InnerException) != null)
			{
				UnityEngine.Debug.LogError(ex.Message + "\n" + ex.StackTrace);
			}
		}
		else
		{
			UnityEngine.Debug.LogError(TAG + " " + message);
		}
	}

	public static void e(string format, params object[] args)
	{
		e(StringUtils.TryFormat(format, args));
	}

	public static void e(string message)
	{
		UnityEngine.Debug.LogError(TAG + " " + message);
	}

	public static void w(string format, params object[] args)
	{
		w(StringUtils.TryFormat(format, args));
	}

	public static void w(string message)
	{
		UnityEngine.Debug.LogWarning(TAG + " " + message);
	}
}
