namespace System.Runtime.InteropServices.WindowsRuntime;

internal static class UnsafeNativeMethods
{
	public unsafe static int WindowsCreateString(string sourceString, int length, IntPtr* hstring)
	{
		throw new NotImplementedException();
	}

	public static int WindowsDeleteString(IntPtr hstring)
	{
		throw new NotImplementedException();
	}

	public unsafe static char* WindowsGetStringRawBuffer(IntPtr hstring, uint* length)
	{
		throw new NotImplementedException();
	}

	public static bool RoOriginateLanguageException(int error, string message, IntPtr languageException)
	{
		throw new NotImplementedException();
	}

	public static void RoReportUnhandledError(IRestrictedErrorInfo error)
	{
		throw new NotImplementedException();
	}

	public static IRestrictedErrorInfo GetRestrictedErrorInfo()
	{
		throw new NotImplementedException();
	}
}
