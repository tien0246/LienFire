using System;
using UnityEngine;

namespace LunarConsolePlugin;

[Serializable]
public class ExceptionWarningSettings
{
	[SerializeField]
	public ExceptionWarningDisplayMode displayMode = ExceptionWarningDisplayMode.All;
}
