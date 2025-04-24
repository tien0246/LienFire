using System.Runtime.Serialization;

namespace System.Transactions;

[Serializable]
public class TransactionInDoubtException : TransactionException
{
	public TransactionInDoubtException()
	{
	}

	public TransactionInDoubtException(string message)
		: base(message)
	{
	}

	public TransactionInDoubtException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected TransactionInDoubtException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
