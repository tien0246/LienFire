using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[Serializable]
[ComVisible(false)]
public sealed class ServicedComponentException : SystemException
{
	public ServicedComponentException()
	{
	}

	public ServicedComponentException(string message)
		: base(message)
	{
	}

	public ServicedComponentException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
