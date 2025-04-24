using System.Runtime.Serialization;

namespace System.Transactions;

[Serializable]
public class TransactionManagerCommunicationException : TransactionException
{
	public TransactionManagerCommunicationException()
	{
	}

	public TransactionManagerCommunicationException(string message)
		: base(message)
	{
	}

	public TransactionManagerCommunicationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected TransactionManagerCommunicationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
