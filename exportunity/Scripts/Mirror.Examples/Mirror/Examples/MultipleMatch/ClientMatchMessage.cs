using System;

namespace Mirror.Examples.MultipleMatch;

public struct ClientMatchMessage : NetworkMessage
{
	public ClientMatchOperation clientMatchOperation;

	public Guid matchId;

	public MatchInfo[] matchInfos;

	public PlayerInfo[] playerInfos;
}
