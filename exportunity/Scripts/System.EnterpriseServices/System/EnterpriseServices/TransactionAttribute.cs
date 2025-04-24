using System.Runtime.InteropServices;

namespace System.EnterpriseServices;

[AttributeUsage(AttributeTargets.Class)]
[ComVisible(false)]
public sealed class TransactionAttribute : Attribute
{
	private TransactionIsolationLevel isolation;

	private int timeout;

	private TransactionOption val;

	public TransactionIsolationLevel Isolation
	{
		get
		{
			return isolation;
		}
		set
		{
			isolation = value;
		}
	}

	public int Timeout
	{
		get
		{
			return timeout;
		}
		set
		{
			timeout = value;
		}
	}

	public TransactionOption Value => val;

	public TransactionAttribute()
		: this(TransactionOption.Required)
	{
	}

	public TransactionAttribute(TransactionOption val)
	{
		isolation = TransactionIsolationLevel.Serializable;
		timeout = -1;
		this.val = val;
	}
}
