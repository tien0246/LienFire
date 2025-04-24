using System.Security;

namespace System.Threading;

internal sealed class ThreadPoolWorkQueueThreadLocals
{
	[SecurityCritical]
	[ThreadStatic]
	public static ThreadPoolWorkQueueThreadLocals threadLocals;

	public readonly ThreadPoolWorkQueue workQueue;

	public readonly ThreadPoolWorkQueue.WorkStealingQueue workStealingQueue;

	public readonly Random random = new Random(Thread.CurrentThread.ManagedThreadId);

	public ThreadPoolWorkQueueThreadLocals(ThreadPoolWorkQueue tpq)
	{
		workQueue = tpq;
		workStealingQueue = new ThreadPoolWorkQueue.WorkStealingQueue();
		ThreadPoolWorkQueue.allThreadQueues.Add(workStealingQueue);
	}

	[SecurityCritical]
	private void CleanUp()
	{
		if (workStealingQueue == null)
		{
			return;
		}
		if (workQueue != null)
		{
			bool flag = false;
			while (!flag)
			{
				try
				{
				}
				finally
				{
					IThreadPoolWorkItem obj = null;
					if (workStealingQueue.LocalPop(out obj))
					{
						workQueue.Enqueue(obj, forceGlobal: true);
					}
					else
					{
						flag = true;
					}
				}
			}
		}
		ThreadPoolWorkQueue.allThreadQueues.Remove(workStealingQueue);
	}

	[SecuritySafeCritical]
	~ThreadPoolWorkQueueThreadLocals()
	{
		if (!Environment.HasShutdownStarted && !AppDomain.CurrentDomain.IsFinalizingForUnload())
		{
			CleanUp();
		}
	}
}
