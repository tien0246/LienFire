using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.Threading;

[ComVisible(true)]
[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
public class EventWaitHandle : WaitHandle
{
	[SecuritySafeCritical]
	public EventWaitHandle(bool initialState, EventResetMode mode)
		: this(initialState, mode, null)
	{
	}

	[SecurityCritical]
	public EventWaitHandle(bool initialState, EventResetMode mode, string name)
	{
		if (name != null && 260 < name.Length)
		{
			throw new ArgumentException(Environment.GetResourceString("The name can be no more than 260 characters in length.", name));
		}
		SafeWaitHandle safeWaitHandle = null;
		safeWaitHandle = mode switch
		{
			EventResetMode.ManualReset => new SafeWaitHandle(NativeEventCalls.CreateEvent_internal(manual: true, initialState, name, out var errorCode), ownsHandle: true), 
			EventResetMode.AutoReset => new SafeWaitHandle(NativeEventCalls.CreateEvent_internal(manual: false, initialState, name, out errorCode), ownsHandle: true), 
			_ => throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid.", name)), 
		};
		if (safeWaitHandle.IsInvalid)
		{
			safeWaitHandle.SetHandleAsInvalid();
			if (name != null && name.Length != 0 && 6 == errorCode)
			{
				throw new WaitHandleCannotBeOpenedException(Environment.GetResourceString("A WaitHandle with system-wide name '{0}' cannot be created. A WaitHandle of a different type might have the same name.", name));
			}
			__Error.WinIOError(errorCode, name);
		}
		SetHandleInternal(safeWaitHandle);
	}

	[SecurityCritical]
	public EventWaitHandle(bool initialState, EventResetMode mode, string name, out bool createdNew)
		: this(initialState, mode, name, out createdNew, null)
	{
	}

	[SecurityCritical]
	public EventWaitHandle(bool initialState, EventResetMode mode, string name, out bool createdNew, EventWaitHandleSecurity eventSecurity)
	{
		if (name != null && 260 < name.Length)
		{
			throw new ArgumentException(Environment.GetResourceString("The name can be no more than 260 characters in length.", name));
		}
		SafeWaitHandle safeWaitHandle = null;
		safeWaitHandle = new SafeWaitHandle(NativeEventCalls.CreateEvent_internal(mode switch
		{
			EventResetMode.ManualReset => true, 
			EventResetMode.AutoReset => false, 
			_ => throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid.", name)), 
		}, initialState, name, out var errorCode), ownsHandle: true);
		if (safeWaitHandle.IsInvalid)
		{
			safeWaitHandle.SetHandleAsInvalid();
			if (name != null && name.Length != 0 && 6 == errorCode)
			{
				throw new WaitHandleCannotBeOpenedException(Environment.GetResourceString("A WaitHandle with system-wide name '{0}' cannot be created. A WaitHandle of a different type might have the same name.", name));
			}
			__Error.WinIOError(errorCode, name);
		}
		createdNew = errorCode != 183;
		SetHandleInternal(safeWaitHandle);
	}

	[SecurityCritical]
	private EventWaitHandle(SafeWaitHandle handle)
	{
		SetHandleInternal(handle);
	}

	[SecurityCritical]
	public static EventWaitHandle OpenExisting(string name)
	{
		return OpenExisting(name, EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize);
	}

	[SecurityCritical]
	public static EventWaitHandle OpenExisting(string name, EventWaitHandleRights rights)
	{
		EventWaitHandle result;
		switch (OpenExistingWorker(name, rights, out result))
		{
		case OpenExistingResult.NameNotFound:
			throw new WaitHandleCannotBeOpenedException();
		case OpenExistingResult.NameInvalid:
			throw new WaitHandleCannotBeOpenedException(Environment.GetResourceString("A WaitHandle with system-wide name '{0}' cannot be created. A WaitHandle of a different type might have the same name.", name));
		case OpenExistingResult.PathNotFound:
			__Error.WinIOError(3, "");
			return result;
		default:
			return result;
		}
	}

	[SecurityCritical]
	public static bool TryOpenExisting(string name, out EventWaitHandle result)
	{
		return OpenExistingWorker(name, EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, out result) == OpenExistingResult.Success;
	}

	[SecurityCritical]
	public static bool TryOpenExisting(string name, EventWaitHandleRights rights, out EventWaitHandle result)
	{
		return OpenExistingWorker(name, rights, out result) == OpenExistingResult.Success;
	}

	[SecurityCritical]
	private static OpenExistingResult OpenExistingWorker(string name, EventWaitHandleRights rights, out EventWaitHandle result)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name", Environment.GetResourceString("Parameter '{0}' cannot be null."));
		}
		if (name.Length == 0)
		{
			throw new ArgumentException(Environment.GetResourceString("Empty name is not legal."), "name");
		}
		if (name != null && 260 < name.Length)
		{
			throw new ArgumentException(Environment.GetResourceString("The name can be no more than 260 characters in length.", name));
		}
		result = null;
		int errorCode;
		SafeWaitHandle safeWaitHandle = new SafeWaitHandle(NativeEventCalls.OpenEvent_internal(name, rights, out errorCode), ownsHandle: true);
		if (safeWaitHandle.IsInvalid)
		{
			if (2 == errorCode || 123 == errorCode)
			{
				return OpenExistingResult.NameNotFound;
			}
			if (3 == errorCode)
			{
				return OpenExistingResult.PathNotFound;
			}
			if (name != null && name.Length != 0 && 6 == errorCode)
			{
				return OpenExistingResult.NameInvalid;
			}
			__Error.WinIOError(errorCode, "");
		}
		result = new EventWaitHandle(safeWaitHandle);
		return OpenExistingResult.Success;
	}

	[SecuritySafeCritical]
	public bool Reset()
	{
		bool num = NativeEventCalls.ResetEvent(safeWaitHandle);
		if (!num)
		{
			throw new IOException();
		}
		return num;
	}

	[SecuritySafeCritical]
	public bool Set()
	{
		bool num = NativeEventCalls.SetEvent(safeWaitHandle);
		if (!num)
		{
			throw new IOException();
		}
		return num;
	}

	[SecuritySafeCritical]
	public EventWaitHandleSecurity GetAccessControl()
	{
		return new EventWaitHandleSecurity(safeWaitHandle, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
	}

	[SecuritySafeCritical]
	public void SetAccessControl(EventWaitHandleSecurity eventSecurity)
	{
		if (eventSecurity == null)
		{
			throw new ArgumentNullException("eventSecurity");
		}
		eventSecurity.Persist(safeWaitHandle);
	}
}
