using System.Collections;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Channels;

internal class ServerDispatchSink : IServerChannelSink, IChannelSinkBase
{
	public IServerChannelSink NextChannelSink => null;

	public IDictionary Properties => null;

	public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream stream)
	{
		throw new NotSupportedException();
	}

	public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
	{
		return null;
	}

	public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
	{
		responseHeaders = null;
		responseStream = null;
		return ChannelServices.DispatchMessage(sinkStack, requestMsg, out responseMsg);
	}
}
