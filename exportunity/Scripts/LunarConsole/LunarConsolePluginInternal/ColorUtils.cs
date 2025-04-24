using UnityEngine;

namespace LunarConsolePluginInternal;

internal static class ColorUtils
{
	private const float kMultiplier = 0.003921569f;

	public static Color FromRGBA(uint value)
	{
		float r = (float)((value >> 24) & 0xFF) * 0.003921569f;
		float g = (float)((value >> 16) & 0xFF) * 0.003921569f;
		float b = (float)((value >> 8) & 0xFF) * 0.003921569f;
		float a = (float)(value & 0xFF) * 0.003921569f;
		return new Color(r, g, b, a);
	}

	public static Color FromRGB(uint value)
	{
		float r = (float)((value >> 16) & 0xFF) * 0.003921569f;
		float g = (float)((value >> 8) & 0xFF) * 0.003921569f;
		float b = (float)(value & 0xFF) * 0.003921569f;
		float a = 1f;
		return new Color(r, g, b, a);
	}

	public static uint ToRGBA(ref Color value)
	{
		uint num = (uint)(value.r * 255f) & 0xFF;
		uint num2 = (uint)(value.g * 255f) & 0xFF;
		uint num3 = (uint)(value.b * 255f) & 0xFF;
		uint num4 = (uint)(value.a * 255f) & 0xFF;
		return (num << 24) | (num2 << 16) | (num3 << 8) | num4;
	}
}
