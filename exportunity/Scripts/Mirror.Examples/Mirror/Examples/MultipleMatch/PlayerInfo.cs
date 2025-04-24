using System;

namespace Mirror.Examples.MultipleMatch;

[Serializable]
public struct PlayerInfo
{
	public int playerIndex;

	public bool ready;

	public Guid matchId;
}
