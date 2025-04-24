using System;

namespace UnityEngine;

public enum NPOTSupport
{
	[Obsolete("NPOTSupport.None does not happen on any platforms")]
	None = 0,
	Restricted = 1,
	Full = 2
}
