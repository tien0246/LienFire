using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity;

namespace System.Security.Principal;

[ComVisible(true)]
public class WindowsImpersonationContext : IDisposable
{
	private IntPtr _token;

	private bool undo;

	internal WindowsImpersonationContext(IntPtr token)
	{
		_token = DuplicateToken(token);
		if (!SetCurrentToken(token))
		{
			throw new SecurityException("Couldn't impersonate token.");
		}
		undo = false;
	}

	[ComVisible(false)]
	public void Dispose()
	{
		if (!undo)
		{
			Undo();
		}
	}

	[ComVisible(false)]
	protected virtual void Dispose(bool disposing)
	{
		if (!undo)
		{
			Undo();
		}
		if (disposing)
		{
			GC.SuppressFinalize(this);
		}
	}

	public void Undo()
	{
		if (!RevertToSelf())
		{
			CloseToken(_token);
			throw new SecurityException("Couldn't switch back to original token.");
		}
		CloseToken(_token);
		undo = true;
		GC.SuppressFinalize(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CloseToken(IntPtr token);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr DuplicateToken(IntPtr token);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetCurrentToken(IntPtr token);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RevertToSelf();

	internal WindowsImpersonationContext()
	{
		ThrowStub.ThrowNotSupportedException();
	}
}
