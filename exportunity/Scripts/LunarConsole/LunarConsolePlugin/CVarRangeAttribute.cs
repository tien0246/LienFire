using System;

namespace LunarConsolePlugin;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class CVarRangeAttribute : Attribute
{
	public readonly float min;

	public readonly float max;

	public CVarRangeAttribute(float min, float max)
	{
		this.min = min;
		this.max = max;
	}
}
