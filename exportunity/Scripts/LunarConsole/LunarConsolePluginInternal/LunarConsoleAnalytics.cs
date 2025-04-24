using System.Collections;
using System.Text;
using UnityEngine;

namespace LunarConsolePluginInternal;

public static class LunarConsoleAnalytics
{
	public static readonly string TrackingURL;

	public const int kUndefinedValue = int.MinValue;

	private static readonly string DefaultPayload;

	static LunarConsoleAnalytics()
	{
		TrackingURL = "https://www.google-analytics.com/collect";
		string arg = "UA-91747018-1";
		StringBuilder stringBuilder = new StringBuilder("v=1&t=event");
		stringBuilder.AppendFormat("&tid={0}", arg);
		stringBuilder.AppendFormat("&cid={0}", WWW.EscapeURL(SystemInfo.deviceUniqueIdentifier));
		stringBuilder.AppendFormat("&ua={0}", WWW.EscapeURL(SystemInfo.operatingSystem));
		stringBuilder.AppendFormat("&av={0}", WWW.EscapeURL(Constants.Version));
		stringBuilder.AppendFormat("&ds={0}", "player");
		if (!string.IsNullOrEmpty(Application.productName))
		{
			string text = WWW.EscapeURL(Application.productName);
			if (text.Length <= 100)
			{
				stringBuilder.AppendFormat("&an={0}", text);
			}
		}
		string identifier = Application.identifier;
		if (!string.IsNullOrEmpty(identifier))
		{
			string text2 = WWW.EscapeURL(identifier);
			if (text2.Length <= 150)
			{
				stringBuilder.AppendFormat("&aid={0}", text2);
			}
		}
		if (!string.IsNullOrEmpty(Application.companyName))
		{
			string text3 = WWW.EscapeURL(Application.companyName);
			if (text3.Length <= 150)
			{
				stringBuilder.AppendFormat("&aiid={0}", text3);
			}
		}
		DefaultPayload = stringBuilder.ToString();
	}

	internal static IEnumerator TrackEvent(string category, string action, int value = int.MinValue)
	{
		string s = CreatePayload(category, action, value);
		yield return new WWW(TrackingURL, Encoding.UTF8.GetBytes(s));
	}

	public static string CreatePayload(string category, string action, int value)
	{
		StringBuilder stringBuilder = new StringBuilder(DefaultPayload);
		stringBuilder.AppendFormat("&ec={0}", WWW.EscapeURL(category));
		stringBuilder.AppendFormat("&ea={0}", WWW.EscapeURL(action));
		if (value != int.MinValue)
		{
			stringBuilder.AppendFormat("&ev={0}", value.ToString());
		}
		return stringBuilder.ToString();
	}
}
