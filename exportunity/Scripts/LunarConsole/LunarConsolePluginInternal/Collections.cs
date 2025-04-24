using System.Collections.Generic;
using System.Text;

namespace LunarConsolePluginInternal;

public static class Collections
{
	public static string Join<T>(this IList<T> list, string separator = ",")
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < list.Count; i++)
		{
			stringBuilder.Append(list[i]);
			if (i < list.Count - 1)
			{
				stringBuilder.Append(separator);
			}
		}
		return stringBuilder.ToString();
	}
}
