namespace UnityEngine.Analytics;

public static class UGSAnalyticsInternalTools
{
	public static void SetPrivacyStatus(bool status)
	{
		AnalyticsCommon.ugsAnalyticsEnabled = status;
	}
}
