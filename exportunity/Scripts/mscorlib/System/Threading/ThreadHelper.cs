using System.Security;

namespace System.Threading;

internal class ThreadHelper
{
	private Delegate _start;

	private object _startArg;

	private ExecutionContext _executionContext;

	[SecurityCritical]
	internal static ContextCallback _ccb = ThreadStart_Context;

	internal ThreadHelper(Delegate start)
	{
		_start = start;
	}

	internal void SetExecutionContextHelper(ExecutionContext ec)
	{
		_executionContext = ec;
	}

	[SecurityCritical]
	private static void ThreadStart_Context(object state)
	{
		ThreadHelper threadHelper = (ThreadHelper)state;
		if (threadHelper._start is ThreadStart)
		{
			((ThreadStart)threadHelper._start)();
		}
		else
		{
			((ParameterizedThreadStart)threadHelper._start)(threadHelper._startArg);
		}
	}

	[SecurityCritical]
	internal void ThreadStart(object obj)
	{
		_startArg = obj;
		if (_executionContext != null)
		{
			ExecutionContext.Run(_executionContext, _ccb, this);
		}
		else
		{
			((ParameterizedThreadStart)_start)(obj);
		}
	}

	[SecurityCritical]
	internal void ThreadStart()
	{
		if (_executionContext != null)
		{
			ExecutionContext.Run(_executionContext, _ccb, this);
		}
		else
		{
			((ThreadStart)_start)();
		}
	}
}
