using System.Collections;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Lifetime;

[ComVisible(true)]
public class ClientSponsor : MarshalByRefObject, ISponsor
{
	private TimeSpan renewal_time;

	private Hashtable registered_objects = new Hashtable();

	public TimeSpan RenewalTime
	{
		get
		{
			return renewal_time;
		}
		set
		{
			renewal_time = value;
		}
	}

	public ClientSponsor()
	{
		renewal_time = new TimeSpan(0, 2, 0);
	}

	public ClientSponsor(TimeSpan renewalTime)
	{
		renewal_time = renewalTime;
	}

	public void Close()
	{
		foreach (MarshalByRefObject value in registered_objects.Values)
		{
			(value.GetLifetimeService() as ILease).Unregister(this);
		}
		registered_objects.Clear();
	}

	~ClientSponsor()
	{
		Close();
	}

	public override object InitializeLifetimeService()
	{
		return base.InitializeLifetimeService();
	}

	public bool Register(MarshalByRefObject obj)
	{
		if (registered_objects.ContainsKey(obj))
		{
			return false;
		}
		if (!(obj.GetLifetimeService() is ILease lease))
		{
			return false;
		}
		lease.Register(this);
		registered_objects.Add(obj, obj);
		return true;
	}

	[SecurityCritical]
	public TimeSpan Renewal(ILease lease)
	{
		return renewal_time;
	}

	public void Unregister(MarshalByRefObject obj)
	{
		if (registered_objects.ContainsKey(obj))
		{
			(obj.GetLifetimeService() as ILease).Unregister(this);
			registered_objects.Remove(obj);
		}
	}
}
