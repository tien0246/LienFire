using System.Runtime.Serialization;

namespace System.IO;

[Serializable]
public sealed class InvalidDataException : SystemException
{
	private const int Result = -2146233085;

	public InvalidDataException()
		: base(global::Locale.GetText("Invalid data format."))
	{
		base.HResult = -2146233085;
	}

	public InvalidDataException(string message)
		: base(message)
	{
		base.HResult = -2146233085;
	}

	public InvalidDataException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.HResult = -2146233085;
	}

	private InvalidDataException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
