using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ApplicationQueuingAttribute : Attribute
{
	private bool enabled;

	private int maxListenerThreads;

	private bool queueListenerEnabled;

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	public int MaxListenerThreads
	{
		get
		{
			return maxListenerThreads;
		}
		set
		{
			maxListenerThreads = value;
		}
	}

	public bool QueueListenerEnabled
	{
		get
		{
			return queueListenerEnabled;
		}
		set
		{
			queueListenerEnabled = value;
		}
	}

	public ApplicationQueuingAttribute()
	{
		enabled = true;
		queueListenerEnabled = false;
		maxListenerThreads = 0;
	}
}
