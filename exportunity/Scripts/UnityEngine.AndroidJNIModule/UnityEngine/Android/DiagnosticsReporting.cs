namespace UnityEngine.Android;

public static class DiagnosticsReporting
{
	public static void CallReportFullyDrawn()
	{
		Common.GetActivity().Call("reportFullyDrawn");
	}
}
