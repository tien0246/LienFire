using System.Runtime.CompilerServices;

namespace Mirror;

public static class Mathd
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Clamp(double value, double min, double max)
	{
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Clamp01(double value)
	{
		return Clamp(value, 0.0, 1.0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double InverseLerp(double a, double b, double value)
	{
		if (a == b)
		{
			return 0.0;
		}
		return Clamp01((value - a) / (b - a));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double LerpUnclamped(double a, double b, double t)
	{
		return a + (b - a) * t;
	}
}
