using System;

namespace LunarConsolePluginInternal;

internal class ReflectionException : Exception
{
	public ReflectionException(string message)
		: base(message)
	{
	}

	public ReflectionException(string format, params object[] args)
		: this(StringUtils.TryFormat(format, args))
	{
	}
}
