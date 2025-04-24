using System.Collections.Generic;

namespace UnityEngine.UIElements.StyleSheets;

internal static class ShorthandApplicator
{
	private static List<TimeValue> s_TransitionDelayList = new List<TimeValue>();

	private static List<TimeValue> s_TransitionDurationList = new List<TimeValue>();

	private static List<StylePropertyName> s_TransitionPropertyList = new List<StylePropertyName>();

	private static List<EasingFunction> s_TransitionTimingFunctionList = new List<EasingFunction>();

	private static bool CompileFlexShorthand(StylePropertyReader reader, out float grow, out float shrink, out Length basis)
	{
		grow = 0f;
		shrink = 1f;
		basis = Length.Auto();
		bool flag = false;
		int valueCount = reader.valueCount;
		if (valueCount == 1 && reader.IsValueType(0, StyleValueType.Keyword))
		{
			if (reader.IsKeyword(0, StyleValueKeyword.None))
			{
				flag = true;
				grow = 0f;
				shrink = 0f;
				basis = Length.Auto();
			}
			else if (reader.IsKeyword(0, StyleValueKeyword.Auto))
			{
				flag = true;
				grow = 1f;
				shrink = 1f;
				basis = Length.Auto();
			}
		}
		else if (valueCount <= 3)
		{
			flag = true;
			grow = 0f;
			shrink = 1f;
			basis = Length.Percent(0f);
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < valueCount && flag; i++)
			{
				StyleValueType valueType = reader.GetValueType(i);
				if (valueType == StyleValueType.Dimension || valueType == StyleValueType.Keyword)
				{
					if (flag3)
					{
						flag = false;
						break;
					}
					flag3 = true;
					switch (valueType)
					{
					case StyleValueType.Keyword:
						if (reader.IsKeyword(i, StyleValueKeyword.Auto))
						{
							basis = Length.Auto();
						}
						break;
					case StyleValueType.Dimension:
						basis = reader.ReadLength(i);
						break;
					}
					if (flag2 && i != valueCount - 1)
					{
						flag = false;
					}
				}
				else if (valueType == StyleValueType.Float)
				{
					float num = reader.ReadFloat(i);
					if (!flag2)
					{
						flag2 = true;
						grow = num;
					}
					else
					{
						shrink = num;
					}
				}
				else
				{
					flag = false;
				}
			}
		}
		return flag;
	}

	private static void CompileBorderRadius(StylePropertyReader reader, out Length top, out Length right, out Length bottom, out Length left)
	{
		CompileBoxArea(reader, out top, out right, out bottom, out left);
		if (top.IsAuto() || top.IsNone())
		{
			top = 0f;
		}
		if (right.IsAuto() || right.IsNone())
		{
			right = 0f;
		}
		if (bottom.IsAuto() || bottom.IsNone())
		{
			bottom = 0f;
		}
		if (left.IsAuto() || left.IsNone())
		{
			left = 0f;
		}
	}

	private static void CompileBoxArea(StylePropertyReader reader, out Length top, out Length right, out Length bottom, out Length left)
	{
		top = 0f;
		right = 0f;
		bottom = 0f;
		left = 0f;
		switch (reader.valueCount)
		{
		case 0:
			break;
		case 1:
			top = (right = (bottom = (left = reader.ReadLength(0))));
			break;
		case 2:
			top = (bottom = reader.ReadLength(0));
			left = (right = reader.ReadLength(1));
			break;
		case 3:
			top = reader.ReadLength(0);
			left = (right = reader.ReadLength(1));
			bottom = reader.ReadLength(2);
			break;
		default:
			top = reader.ReadLength(0);
			right = reader.ReadLength(1);
			bottom = reader.ReadLength(2);
			left = reader.ReadLength(3);
			break;
		}
	}

	private static void CompileBoxArea(StylePropertyReader reader, out float top, out float right, out float bottom, out float left)
	{
		CompileBoxArea(reader, out Length top2, out Length right2, out Length bottom2, out Length left2);
		top = top2.value;
		right = right2.value;
		bottom = bottom2.value;
		left = left2.value;
	}

	private static void CompileBoxArea(StylePropertyReader reader, out Color top, out Color right, out Color bottom, out Color left)
	{
		top = Color.clear;
		right = Color.clear;
		bottom = Color.clear;
		left = Color.clear;
		switch (reader.valueCount)
		{
		case 0:
			break;
		case 1:
			top = (right = (bottom = (left = reader.ReadColor(0))));
			break;
		case 2:
			top = (bottom = reader.ReadColor(0));
			left = (right = reader.ReadColor(1));
			break;
		case 3:
			top = reader.ReadColor(0);
			left = (right = reader.ReadColor(1));
			bottom = reader.ReadColor(2);
			break;
		default:
			top = reader.ReadColor(0);
			right = reader.ReadColor(1);
			bottom = reader.ReadColor(2);
			left = reader.ReadColor(3);
			break;
		}
	}

	private static void CompileTextOutline(StylePropertyReader reader, out Color outlineColor, out float outlineWidth)
	{
		outlineColor = Color.clear;
		outlineWidth = 0f;
		int valueCount = reader.valueCount;
		for (int i = 0; i < valueCount; i++)
		{
			StyleValueType valueType = reader.GetValueType(i);
			switch (valueType)
			{
			case StyleValueType.Dimension:
				outlineWidth = reader.ReadFloat(i);
				continue;
			default:
				if (valueType != StyleValueType.Color)
				{
					continue;
				}
				break;
			case StyleValueType.Enum:
				break;
			}
			outlineColor = reader.ReadColor(i);
		}
	}

	private static void CompileTransition(StylePropertyReader reader, out List<TimeValue> outDelay, out List<TimeValue> outDuration, out List<StylePropertyName> outProperty, out List<EasingFunction> outTimingFunction)
	{
		s_TransitionDelayList.Clear();
		s_TransitionDurationList.Clear();
		s_TransitionPropertyList.Clear();
		s_TransitionTimingFunctionList.Clear();
		bool flag = true;
		bool flag2 = false;
		int valueCount = reader.valueCount;
		int num = 0;
		int i = 0;
		do
		{
			if (flag2)
			{
				flag = false;
				break;
			}
			StylePropertyName item = InitialStyle.transitionProperty[0];
			TimeValue item2 = InitialStyle.transitionDuration[0];
			TimeValue item3 = InitialStyle.transitionDelay[0];
			EasingFunction item4 = InitialStyle.transitionTimingFunction[0];
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			for (; i < valueCount; i++)
			{
				if (flag7)
				{
					break;
				}
				switch (reader.GetValueType(i))
				{
				case StyleValueType.Keyword:
					if (reader.IsKeyword(i, StyleValueKeyword.None) && num == 0)
					{
						flag2 = true;
						flag5 = true;
						item = new StylePropertyName("none");
					}
					else
					{
						flag = false;
					}
					break;
				case StyleValueType.Dimension:
				{
					TimeValue timeValue = reader.ReadTimeValue(i);
					if (!flag3)
					{
						flag3 = true;
						item2 = timeValue;
					}
					else if (!flag4)
					{
						flag4 = true;
						item3 = timeValue;
					}
					else
					{
						flag = false;
					}
					break;
				}
				case StyleValueType.Enum:
				{
					string text = reader.ReadAsString(i);
					if (!flag6 && StylePropertyUtil.TryGetEnumIntValue(StyleEnumType.EasingMode, text, out var intValue))
					{
						flag6 = true;
						item4 = (EasingMode)intValue;
					}
					else if (!flag5)
					{
						flag5 = true;
						item = new StylePropertyName(text);
					}
					else
					{
						flag = false;
					}
					break;
				}
				case StyleValueType.CommaSeparator:
					flag7 = true;
					num++;
					break;
				default:
					flag = false;
					break;
				}
			}
			s_TransitionDelayList.Add(item3);
			s_TransitionDurationList.Add(item2);
			s_TransitionPropertyList.Add(item);
			s_TransitionTimingFunctionList.Add(item4);
		}
		while (i < valueCount && flag);
		if (flag)
		{
			outProperty = s_TransitionPropertyList;
			outDelay = s_TransitionDelayList;
			outDuration = s_TransitionDurationList;
			outTimingFunction = s_TransitionTimingFunctionList;
		}
		else
		{
			outProperty = InitialStyle.transitionProperty;
			outDelay = InitialStyle.transitionDelay;
			outDuration = InitialStyle.transitionDuration;
			outTimingFunction = InitialStyle.transitionTimingFunction;
		}
	}

	public static void ApplyBorderColor(StylePropertyReader reader, ref ComputedStyle computedStyle)
	{
		CompileBoxArea(reader, out Color top, out Color right, out Color bottom, out Color left);
		computedStyle.visualData.Write().borderTopColor = top;
		computedStyle.visualData.Write().borderRightColor = right;
		computedStyle.visualData.Write().borderBottomColor = bottom;
		computedStyle.visualData.Write().borderLeftColor = left;
	}

	public static void ApplyBorderRadius(StylePropertyReader reader, ref ComputedStyle computedStyle)
	{
		CompileBorderRadius(reader, out var top, out var right, out var bottom, out var left);
		computedStyle.visualData.Write().borderTopLeftRadius = top;
		computedStyle.visualData.Write().borderTopRightRadius = right;
		computedStyle.visualData.Write().borderBottomRightRadius = bottom;
		computedStyle.visualData.Write().borderBottomLeftRadius = left;
	}

	public static void ApplyBorderWidth(StylePropertyReader reader, ref ComputedStyle computedStyle)
	{
		CompileBoxArea(reader, out float top, out float right, out float bottom, out float left);
		computedStyle.layoutData.Write().borderTopWidth = top;
		computedStyle.layoutData.Write().borderRightWidth = right;
		computedStyle.layoutData.Write().borderBottomWidth = bottom;
		computedStyle.layoutData.Write().borderLeftWidth = left;
	}

	public static void ApplyFlex(StylePropertyReader reader, ref ComputedStyle computedStyle)
	{
		CompileFlexShorthand(reader, out var grow, out var shrink, out var basis);
		computedStyle.layoutData.Write().flexGrow = grow;
		computedStyle.layoutData.Write().flexShrink = shrink;
		computedStyle.layoutData.Write().flexBasis = basis;
	}

	public static void ApplyMargin(StylePropertyReader reader, ref ComputedStyle computedStyle)
	{
		CompileBoxArea(reader, out Length top, out Length right, out Length bottom, out Length left);
		computedStyle.layoutData.Write().marginTop = top;
		computedStyle.layoutData.Write().marginRight = right;
		computedStyle.layoutData.Write().marginBottom = bottom;
		computedStyle.layoutData.Write().marginLeft = left;
	}

	public static void ApplyPadding(StylePropertyReader reader, ref ComputedStyle computedStyle)
	{
		CompileBoxArea(reader, out Length top, out Length right, out Length bottom, out Length left);
		computedStyle.layoutData.Write().paddingTop = top;
		computedStyle.layoutData.Write().paddingRight = right;
		computedStyle.layoutData.Write().paddingBottom = bottom;
		computedStyle.layoutData.Write().paddingLeft = left;
	}

	public static void ApplyTransition(StylePropertyReader reader, ref ComputedStyle computedStyle)
	{
		CompileTransition(reader, out var outDelay, out var outDuration, out var outProperty, out var outTimingFunction);
		computedStyle.transitionData.Write().transitionDelay.CopyFrom(outDelay);
		computedStyle.transitionData.Write().transitionDuration.CopyFrom(outDuration);
		computedStyle.transitionData.Write().transitionProperty.CopyFrom(outProperty);
		computedStyle.transitionData.Write().transitionTimingFunction.CopyFrom(outTimingFunction);
	}

	public static void ApplyUnityTextOutline(StylePropertyReader reader, ref ComputedStyle computedStyle)
	{
		CompileTextOutline(reader, out var outlineColor, out var outlineWidth);
		computedStyle.inheritedData.Write().unityTextOutlineColor = outlineColor;
		computedStyle.inheritedData.Write().unityTextOutlineWidth = outlineWidth;
	}
}
