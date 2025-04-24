namespace System.Net;

internal enum FtpLoginState : byte
{
	NotLoggedIn = 0,
	LoggedIn = 1,
	LoggedInButNeedsRelogin = 2,
	ReloginFailed = 3
}
