using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[ComVisible(false)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class EventClassAttribute : Attribute
{
	private bool allowInProcSubscribers;

	private bool fireInParallel;

	private string publisherFilter;

	public bool AllowInprocSubscribers
	{
		get
		{
			return allowInProcSubscribers;
		}
		set
		{
			allowInProcSubscribers = value;
		}
	}

	public bool FireInParallel
	{
		get
		{
			return fireInParallel;
		}
		set
		{
			fireInParallel = value;
		}
	}

	public string PublisherFilter
	{
		get
		{
			return publisherFilter;
		}
		set
		{
			publisherFilter = value;
		}
	}

	public EventClassAttribute()
	{
		allowInProcSubscribers = true;
		fireInParallel = false;
		publisherFilter = null;
	}
}
