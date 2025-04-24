using System;
using LunarConsolePluginInternal;

namespace LunarConsolePlugin;

internal class CVarChangedDelegateList : BaseList<CVarChangedDelegate>
{
	public CVarChangedDelegateList(int capacity)
		: base((CVarChangedDelegate)NullCVarChangedDelegate, capacity)
	{
	}

	public void NotifyValueChanged(CVar cvar)
	{
		try
		{
			Lock();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				try
				{
					list[i](cvar);
				}
				catch (Exception exception)
				{
					Log.e(exception, "Exception while calling value changed delegate for '{0}'", cvar.Name);
				}
			}
		}
		finally
		{
			Unlock();
		}
	}

	private static void NullCVarChangedDelegate(CVar cvar)
	{
	}
}
