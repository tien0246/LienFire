using System;
using System.ComponentModel;

namespace UnityEngine;

public enum JointProjectionMode
{
	None = 0,
	PositionAndRotation = 1,
	[Obsolete("JointProjectionMode.PositionOnly is no longer supported", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	PositionOnly = 2
}
