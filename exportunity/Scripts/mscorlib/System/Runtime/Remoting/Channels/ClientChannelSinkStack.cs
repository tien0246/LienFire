using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security;

namespace System.Runtime.Remoting.Channels;

[ComVisible(true)]
public class ClientChannelSinkStack : IClientChannelSinkStack, IClientResponseChannelSinkStack
{
	private IMessageSink _replySink;

	private ChanelSinkStackEntry _sinkStack;

	public ClientChannelSinkStack()
	{
	}

	public ClientChannelSinkStack(IMessageSink replySink)
	{
		_replySink = replySink;
	}

	[SecurityCritical]
	public void AsyncProcessResponse(ITransportHeaders headers, Stream stream)
	{
		if (_sinkStack == null)
		{
			throw new RemotingException("The current sink stack is empty");
		}
		ChanelSinkStackEntry sinkStack = _sinkStack;
		_sinkStack = _sinkStack.Next;
		((IClientChannelSink)sinkStack.Sink).AsyncProcessResponse(this, sinkStack.State, headers, stream);
	}

	[SecurityCritical]
	public void DispatchException(Exception e)
	{
		DispatchReplyMessage(new ReturnMessage(e, null));
	}

	[SecurityCritical]
	public void DispatchReplyMessage(IMessage msg)
	{
		if (_replySink != null)
		{
			_replySink.SyncProcessMessage(msg);
		}
	}

	[SecurityCritical]
	public object Pop(IClientChannelSink sink)
	{
		while (_sinkStack != null)
		{
			ChanelSinkStackEntry sinkStack = _sinkStack;
			_sinkStack = _sinkStack.Next;
			if (sinkStack.Sink == sink)
			{
				return sinkStack.State;
			}
		}
		throw new RemotingException("The current sink stack is empty, or the specified sink was never pushed onto the current stack");
	}

	[SecurityCritical]
	public void Push(IClientChannelSink sink, object state)
	{
		_sinkStack = new ChanelSinkStackEntry(sink, state, _sinkStack);
	}
}
