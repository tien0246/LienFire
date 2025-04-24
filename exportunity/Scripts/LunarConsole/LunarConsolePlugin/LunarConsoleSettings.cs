using System;
using UnityEngine;

namespace LunarConsolePlugin;

[Serializable]
public class LunarConsoleSettings
{
	[SerializeField]
	public ExceptionWarningSettings exceptionWarning = new ExceptionWarningSettings();

	[HideInInspector]
	[SerializeField]
	public LogOverlaySettings logOverlay = new LogOverlaySettings();

	[Range(128f, 65536f)]
	[Tooltip("Log output will never become bigger than this capacity")]
	[SerializeField]
	public int capacity = 4096;

	[Range(128f, 65536f)]
	[Tooltip("Log output will be trimmed this many lines when overflown")]
	[SerializeField]
	public int trim = 512;

	[Tooltip("Gesture type to open the console")]
	[SerializeField]
	public Gesture gesture = Gesture.SwipeDown;

	[Tooltip("If checked - enables Unity Rich Text in log output")]
	[SerializeField]
	public bool richTextTags;

	[HideInInspector]
	[SerializeField]
	public bool sortActions = true;

	[HideInInspector]
	[SerializeField]
	public bool sortVariables = true;

	[SerializeField]
	public string[] emails;
}
