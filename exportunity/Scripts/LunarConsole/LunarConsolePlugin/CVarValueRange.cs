namespace LunarConsolePlugin;

public struct CVarValueRange
{
	public static readonly CVarValueRange Undefined = new CVarValueRange(float.NaN, float.NaN);

	public readonly float min;

	public readonly float max;

	public bool IsValid
	{
		get
		{
			if (!float.IsNaN(min))
			{
				return !float.IsNaN(max);
			}
			return false;
		}
	}

	public CVarValueRange(float min, float max)
	{
		this.min = min;
		this.max = max;
	}
}
