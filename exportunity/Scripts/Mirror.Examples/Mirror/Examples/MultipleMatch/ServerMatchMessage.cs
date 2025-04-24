using System;

namespace Mirror.Examples.MultipleMatch;

public struct ServerMatchMessage : NetworkMessage
{
	public ServerMatchOperation serverMatchOperation;

	public Guid matchId;
}
