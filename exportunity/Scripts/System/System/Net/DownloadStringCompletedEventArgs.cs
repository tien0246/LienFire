using System.ComponentModel;
using Unity;

namespace System.Net;

public class DownloadStringCompletedEventArgs : AsyncCompletedEventArgs
{
	private readonly string _result;

	public string Result
	{
		get
		{
			RaiseExceptionIfNecessary();
			return _result;
		}
	}

	internal DownloadStringCompletedEventArgs(string result, Exception exception, bool cancelled, object userToken)
		: base(exception, cancelled, userToken)
	{
		_result = result;
	}

	internal DownloadStringCompletedEventArgs()
	{
		Unity.ThrowStub.ThrowNotSupportedException();
	}
}
