using System;

namespace Mirror.Examples.MultipleMatch;

[Serializable]
public struct MatchInfo
{
	public Guid matchId;

	public byte players;

	public byte maxPlayers;
}
