using System;

namespace Mirror.Examples.MultipleMatch;

[Serializable]
public struct MatchPlayerData
{
	public int playerIndex;

	public int wins;

	public CellValue currentScore;
}
