using System;
using System.Net;

namespace Mirror.Discovery;

public struct ServerResponse : NetworkMessage
{
	public Uri uri;

	public long serverId;

	public IPEndPoint EndPoint { get; set; }
}
