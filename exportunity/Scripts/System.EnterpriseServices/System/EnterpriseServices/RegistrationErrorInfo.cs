using Unity;

namespace System.EnterpriseServices;

[Serializable]
public sealed class RegistrationErrorInfo
{
	private int errorCode;

	private string errorString;

	private string majorRef;

	private string minorRef;

	private string name;

	public int ErrorCode => errorCode;

	public string ErrorString => errorString;

	public string MajorRef => majorRef;

	public string MinorRef => minorRef;

	public string Name => name;

	[System.MonoTODO]
	internal RegistrationErrorInfo(string name, string majorRef, string minorRef, int errorCode)
	{
		this.name = name;
		this.majorRef = majorRef;
		this.minorRef = minorRef;
		this.errorCode = errorCode;
	}

	internal RegistrationErrorInfo()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
