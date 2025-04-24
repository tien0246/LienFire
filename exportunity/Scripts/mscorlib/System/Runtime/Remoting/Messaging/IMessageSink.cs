using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging;

[ComVisible(true)]
public interface IMessageSink
{
	IMessageSink NextSink { get; }

	IMessage SyncProcessMessage(IMessage msg);

	IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink);
}
