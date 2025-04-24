using System.Configuration;
using Unity;

namespace System.Net.Configuration;

public sealed class WebProxyScriptElement : ConfigurationElement
{
	private static ConfigurationProperty downloadTimeoutProp;

	private static ConfigurationPropertyCollection properties;

	[ConfigurationProperty("downloadTimeout", DefaultValue = "00:02:00")]
	public TimeSpan DownloadTimeout
	{
		get
		{
			return (TimeSpan)base[downloadTimeoutProp];
		}
		set
		{
			base[downloadTimeoutProp] = value;
		}
	}

	protected override ConfigurationPropertyCollection Properties => properties;

	public int AutoConfigUrlRetryInterval
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default(int);
		}
		set
		{
			Unity.ThrowStub.ThrowNotSupportedException();
		}
	}

	static WebProxyScriptElement()
	{
		downloadTimeoutProp = new ConfigurationProperty("downloadTimeout", typeof(TimeSpan), new TimeSpan(0, 0, 2, 0));
		properties = new ConfigurationPropertyCollection();
		properties.Add(downloadTimeoutProp);
	}

	protected override void PostDeserialize()
	{
	}
}
