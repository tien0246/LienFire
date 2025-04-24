using System.Runtime.Serialization;

namespace System.Runtime.InteropServices;

[Serializable]
public class SafeArrayTypeMismatchException : SystemException
{
	public SafeArrayTypeMismatchException()
		: base("Specified array was not of the expected type.")
	{
		base.HResult = -2146233037;
	}

	public SafeArrayTypeMismatchException(string message)
		: base(message)
	{
		base.HResult = -2146233037;
	}

	public SafeArrayTypeMismatchException(string message, Exception inner)
		: base(message, inner)
	{
		base.HResult = -2146233037;
	}

	protected SafeArrayTypeMismatchException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
