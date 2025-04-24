namespace UnityEngine.Android;

internal static class Common
{
	private static AndroidJavaObject m_Activity;

	public static AndroidJavaObject GetActivity()
	{
		if (m_Activity != null)
		{
			return m_Activity;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			m_Activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
		return m_Activity;
	}
}
