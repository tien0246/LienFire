using System.Net;

namespace kcp2k;

public struct KcpServerConnection
{
	public KcpPeer peer;

	public readonly EndPoint remoteEndPoint;

	public KcpServerConnection(EndPoint remoteEndPoint)
	{
		peer = null;
		this.remoteEndPoint = remoteEndPoint;
	}
}
