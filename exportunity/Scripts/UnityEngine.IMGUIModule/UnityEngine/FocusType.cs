using System;

namespace UnityEngine;

public enum FocusType
{
	[Obsolete("FocusType.Native now behaves the same as FocusType.Passive in all OS cases. (UnityUpgradable) -> Passive", false)]
	Native = 0,
	Keyboard = 1,
	Passive = 2
}
