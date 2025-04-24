using System;
using System.Net;
using UnityEngine;

namespace Mirror.Discovery;

[DisallowMultipleComponent]
[AddComponentMenu("Network/Network Discovery")]
public class NetworkDiscovery : NetworkDiscoveryBase<ServerRequest, ServerResponse>
{
	protected override ServerResponse ProcessRequest(ServerRequest request, IPEndPoint endpoint)
	{
		try
		{
			return new ServerResponse
			{
				serverId = base.ServerId,
				uri = transport.ServerUri()
			};
		}
		catch (NotImplementedException)
		{
			Debug.LogError($"Transport {transport} does not support network discovery");
			throw;
		}
	}

	protected override ServerRequest GetRequest()
	{
		return default(ServerRequest);
	}

	protected override void ProcessResponse(ServerResponse response, IPEndPoint endpoint)
	{
		response.EndPoint = endpoint;
		UriBuilder uriBuilder = new UriBuilder(response.uri)
		{
			Host = response.EndPoint.Address.ToString()
		};
		response.uri = uriBuilder.Uri;
		OnServerFound.Invoke(response);
	}
}
