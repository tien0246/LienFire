using System;

namespace UnityEngine.Playables;

public enum PlayState
{
	Paused = 0,
	Playing = 1,
	[Obsolete("Delayed is obsolete; use a custom ScriptPlayable to implement this feature", false)]
	Delayed = 2
}
