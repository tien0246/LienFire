using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;

namespace System.Threading;

[SecurityPermission(SecurityAction.InheritanceDemand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
public class SynchronizationContext
{
	private delegate int WaitDelegate(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout);

	private SynchronizationContextProperties _props;

	private static Type s_cachedPreparedType1;

	private static Type s_cachedPreparedType2;

	private static Type s_cachedPreparedType3;

	private static Type s_cachedPreparedType4;

	private static Type s_cachedPreparedType5;

	public static SynchronizationContext Current => Thread.CurrentThread.GetExecutionContextReader().SynchronizationContext ?? GetThreadLocalContext();

	internal static SynchronizationContext CurrentNoFlow
	{
		[FriendAccessAllowed]
		get
		{
			return Thread.CurrentThread.GetExecutionContextReader().SynchronizationContextNoFlow ?? GetThreadLocalContext();
		}
	}

	internal static SynchronizationContext CurrentExplicit => Current;

	[SecuritySafeCritical]
	protected void SetWaitNotificationRequired()
	{
		Type type = GetType();
		if (s_cachedPreparedType1 != type && s_cachedPreparedType2 != type && s_cachedPreparedType3 != type && s_cachedPreparedType4 != type && s_cachedPreparedType5 != type)
		{
			RuntimeHelpers.PrepareDelegate(new WaitDelegate(Wait));
			if (s_cachedPreparedType1 == null)
			{
				s_cachedPreparedType1 = type;
			}
			else if (s_cachedPreparedType2 == null)
			{
				s_cachedPreparedType2 = type;
			}
			else if (s_cachedPreparedType3 == null)
			{
				s_cachedPreparedType3 = type;
			}
			else if (s_cachedPreparedType4 == null)
			{
				s_cachedPreparedType4 = type;
			}
			else if (s_cachedPreparedType5 == null)
			{
				s_cachedPreparedType5 = type;
			}
		}
		_props |= SynchronizationContextProperties.RequireWaitNotification;
	}

	public bool IsWaitNotificationRequired()
	{
		return (_props & SynchronizationContextProperties.RequireWaitNotification) != 0;
	}

	public virtual void Send(SendOrPostCallback d, object state)
	{
		d(state);
	}

	public virtual void Post(SendOrPostCallback d, object state)
	{
		ThreadPool.QueueUserWorkItem(d.Invoke, state);
	}

	public virtual void OperationStarted()
	{
	}

	public virtual void OperationCompleted()
	{
	}

	[CLSCompliant(false)]
	[SecurityCritical]
	[PrePrepareMethod]
	public virtual int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
	{
		if (waitHandles == null)
		{
			throw new ArgumentNullException("waitHandles");
		}
		return WaitHelper(waitHandles, waitAll, millisecondsTimeout);
	}

	[CLSCompliant(false)]
	[SecurityCritical]
	[PrePrepareMethod]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	protected unsafe static int WaitHelper(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
	{
		fixed (IntPtr* handles = waitHandles)
		{
			return WaitHandle.Wait_internal(handles, waitHandles.Length, waitAll, millisecondsTimeout);
		}
	}

	[SecurityCritical]
	public static void SetSynchronizationContext(SynchronizationContext syncContext)
	{
		ExecutionContext mutableExecutionContext = Thread.CurrentThread.GetMutableExecutionContext();
		mutableExecutionContext.SynchronizationContext = syncContext;
		mutableExecutionContext.SynchronizationContextNoFlow = syncContext;
	}

	private static SynchronizationContext GetThreadLocalContext()
	{
		return null;
	}

	public virtual SynchronizationContext CreateCopy()
	{
		return new SynchronizationContext();
	}

	[SecurityCritical]
	private static int InvokeWaitMethodHelper(SynchronizationContext syncContext, IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
	{
		return syncContext.Wait(waitHandles, waitAll, millisecondsTimeout);
	}
}
