using System.Collections;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Class)]
[ComVisible(false)]
public sealed class ObjectPoolingAttribute : Attribute, IConfigurationAttribute
{
	private int creationTimeout;

	private bool enabled;

	private int minPoolSize;

	private int maxPoolSize;

	public int CreationTimeout
	{
		get
		{
			return creationTimeout;
		}
		set
		{
			creationTimeout = value;
		}
	}

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

	public int MaxPoolSize
	{
		get
		{
			return maxPoolSize;
		}
		set
		{
			maxPoolSize = value;
		}
	}

	public int MinPoolSize
	{
		get
		{
			return minPoolSize;
		}
		set
		{
			minPoolSize = value;
		}
	}

	public ObjectPoolingAttribute()
		: this(enable: true)
	{
	}

	public ObjectPoolingAttribute(bool enable)
	{
		enabled = enable;
	}

	public ObjectPoolingAttribute(int minPoolSize, int maxPoolSize)
		: this(enable: true, minPoolSize, maxPoolSize)
	{
	}

	public ObjectPoolingAttribute(bool enable, int minPoolSize, int maxPoolSize)
	{
		enabled = enable;
		this.minPoolSize = minPoolSize;
		this.maxPoolSize = maxPoolSize;
	}

	[System.MonoTODO]
	public bool AfterSaveChanges(Hashtable info)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public bool Apply(Hashtable info)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public bool IsValidTarget(string s)
	{
		throw new NotImplementedException();
	}
}
