namespace System.Drawing;

internal static class KnownColors
{
	internal static uint[] ArgbValues;

	static KnownColors()
	{
		ArgbValues = new uint[175]
		{
			0u, 4292137160u, 4278211811u, 4294967295u, 4286611584u, 4293716440u, 4289505433u, 4285624164u, 4294045666u, 4294967295u,
			4278190080u, 4278210200u, 4289505433u, 4281428677u, 4294967295u, 4278190208u, 4292137160u, 4286224095u, 4292404472u, 4294967265u,
			4278190080u, 4294967295u, 4278190080u, 4292137160u, 4294967295u, 4278190080u, 4278190080u, 16777215u, 4293982463u, 4294634455u,
			4278255615u, 4286578644u, 4293984255u, 4294309340u, 4294960324u, 4278190080u, 4294962125u, 4278190335u, 4287245282u, 4289014314u,
			4292786311u, 4284456608u, 4286578432u, 4291979550u, 4294934352u, 4284782061u, 4294965468u, 4292613180u, 4278255615u, 4278190219u,
			4278225803u, 4290283019u, 4289309097u, 4278215680u, 4290623339u, 4287299723u, 4283788079u, 4294937600u, 4288230092u, 4287299584u,
			4293498490u, 4287609995u, 4282924427u, 4281290575u, 4278243025u, 4287889619u, 4294907027u, 4278239231u, 4285098345u, 4280193279u,
			4289864226u, 4294966000u, 4280453922u, 4294902015u, 4292664540u, 4294506751u, 4294956800u, 4292519200u, 4286611584u, 4278222848u,
			4289593135u, 4293984240u, 4294928820u, 4291648604u, 4283105410u, 4294967280u, 4293977740u, 4293322490u, 4294963445u, 4286381056u,
			4294965965u, 4289583334u, 4293951616u, 4292935679u, 4294638290u, 4292072403u, 4287688336u, 4294948545u, 4294942842u, 4280332970u,
			4287090426u, 4286023833u, 4289774814u, 4294967264u, 4278255360u, 4281519410u, 4294635750u, 4294902015u, 4286578688u, 4284927402u,
			4278190285u, 4290401747u, 4287852763u, 4282168177u, 4286277870u, 4278254234u, 4282962380u, 4291237253u, 4279834992u, 4294311930u,
			4294960353u, 4294960309u, 4294958765u, 4278190208u, 4294833638u, 4286611456u, 4285238819u, 4294944000u, 4294919424u, 4292505814u,
			4293847210u, 4288215960u, 4289720046u, 4292571283u, 4294963157u, 4294957753u, 4291659071u, 4294951115u, 4292714717u, 4289781990u,
			4286578816u, 4294901760u, 4290547599u, 4282477025u, 4287317267u, 4294606962u, 4294222944u, 4281240407u, 4294964718u, 4288696877u,
			4290822336u, 4287090411u, 4285160141u, 4285563024u, 4294966010u, 4278255487u, 4282811060u, 4291998860u, 4278222976u, 4292394968u,
			4294927175u, 4282441936u, 4293821166u, 4294303411u, 4294967295u, 4294309365u, 4294967040u, 4288335154u, 4293716440u, 4294967295u,
			4289505433u, 4282226175u, 4288526827u, 4293716440u, 4281428677u
		};
		if (GDIPlus.RunningOnWindows())
		{
			RetrieveWindowsSystemColors();
		}
	}

	private static uint GetSysColor(GetSysColorIndex index)
	{
		uint num = GDIPlus.Win32GetSysColor(index);
		return 0xFF000000u | ((num & 0xFF) << 16) | (num & 0xFF00) | (num >> 16);
	}

	private static void RetrieveWindowsSystemColors()
	{
		ArgbValues[1] = GetSysColor(GetSysColorIndex.COLOR_ACTIVEBORDER);
		ArgbValues[2] = GetSysColor(GetSysColorIndex.COLOR_ACTIVECAPTION);
		ArgbValues[3] = GetSysColor(GetSysColorIndex.COLOR_CAPTIONTEXT);
		ArgbValues[4] = GetSysColor(GetSysColorIndex.COLOR_APPWORKSPACE);
		ArgbValues[5] = GetSysColor(GetSysColorIndex.COLOR_BTNFACE);
		ArgbValues[6] = GetSysColor(GetSysColorIndex.COLOR_BTNSHADOW);
		ArgbValues[7] = GetSysColor(GetSysColorIndex.COLOR_3DDKSHADOW);
		ArgbValues[8] = GetSysColor(GetSysColorIndex.COLOR_3DLIGHT);
		ArgbValues[9] = GetSysColor(GetSysColorIndex.COLOR_BTNHIGHLIGHT);
		ArgbValues[10] = GetSysColor(GetSysColorIndex.COLOR_BTNTEXT);
		ArgbValues[11] = GetSysColor(GetSysColorIndex.COLOR_BACKGROUND);
		ArgbValues[12] = GetSysColor(GetSysColorIndex.COLOR_GRAYTEXT);
		ArgbValues[13] = GetSysColor(GetSysColorIndex.COLOR_HIGHLIGHT);
		ArgbValues[14] = GetSysColor(GetSysColorIndex.COLOR_HIGHLIGHTTEXT);
		ArgbValues[15] = GetSysColor(GetSysColorIndex.COLOR_HOTLIGHT);
		ArgbValues[16] = GetSysColor(GetSysColorIndex.COLOR_INACTIVEBORDER);
		ArgbValues[17] = GetSysColor(GetSysColorIndex.COLOR_INACTIVECAPTION);
		ArgbValues[18] = GetSysColor(GetSysColorIndex.COLOR_INACTIVECAPTIONTEXT);
		ArgbValues[19] = GetSysColor(GetSysColorIndex.COLOR_INFOBK);
		ArgbValues[20] = GetSysColor(GetSysColorIndex.COLOR_INFOTEXT);
		ArgbValues[21] = GetSysColor(GetSysColorIndex.COLOR_MENU);
		ArgbValues[22] = GetSysColor(GetSysColorIndex.COLOR_MENUTEXT);
		ArgbValues[23] = GetSysColor(GetSysColorIndex.COLOR_SCROLLBAR);
		ArgbValues[24] = GetSysColor(GetSysColorIndex.COLOR_WINDOW);
		ArgbValues[25] = GetSysColor(GetSysColorIndex.COLOR_WINDOWFRAME);
		ArgbValues[26] = GetSysColor(GetSysColorIndex.COLOR_WINDOWTEXT);
		ArgbValues[168] = GetSysColor(GetSysColorIndex.COLOR_BTNFACE);
		ArgbValues[169] = GetSysColor(GetSysColorIndex.COLOR_BTNHIGHLIGHT);
		ArgbValues[170] = GetSysColor(GetSysColorIndex.COLOR_BTNSHADOW);
		ArgbValues[171] = GetSysColor(GetSysColorIndex.COLOR_GRADIENTACTIVECAPTION);
		ArgbValues[172] = GetSysColor(GetSysColorIndex.COLOR_GRADIENTINACTIVECAPTION);
		ArgbValues[173] = GetSysColor(GetSysColorIndex.COLOR_MENUBAR);
		ArgbValues[174] = GetSysColor(GetSysColorIndex.COLOR_MENUHIGHLIGHT);
	}

	public static Color FromKnownColor(KnownColor kc)
	{
		return Color.FromKnownColor(kc);
	}

	public static string GetName(short kc)
	{
		return kc switch
		{
			1 => "ActiveBorder", 
			2 => "ActiveCaption", 
			3 => "ActiveCaptionText", 
			4 => "AppWorkspace", 
			5 => "Control", 
			6 => "ControlDark", 
			7 => "ControlDarkDark", 
			8 => "ControlLight", 
			9 => "ControlLightLight", 
			10 => "ControlText", 
			11 => "Desktop", 
			12 => "GrayText", 
			13 => "Highlight", 
			14 => "HighlightText", 
			15 => "HotTrack", 
			16 => "InactiveBorder", 
			17 => "InactiveCaption", 
			18 => "InactiveCaptionText", 
			19 => "Info", 
			20 => "InfoText", 
			21 => "Menu", 
			22 => "MenuText", 
			23 => "ScrollBar", 
			24 => "Window", 
			25 => "WindowFrame", 
			26 => "WindowText", 
			27 => "Transparent", 
			28 => "AliceBlue", 
			29 => "AntiqueWhite", 
			30 => "Aqua", 
			31 => "Aquamarine", 
			32 => "Azure", 
			33 => "Beige", 
			34 => "Bisque", 
			35 => "Black", 
			36 => "BlanchedAlmond", 
			37 => "Blue", 
			38 => "BlueViolet", 
			39 => "Brown", 
			40 => "BurlyWood", 
			41 => "CadetBlue", 
			42 => "Chartreuse", 
			43 => "Chocolate", 
			44 => "Coral", 
			45 => "CornflowerBlue", 
			46 => "Cornsilk", 
			47 => "Crimson", 
			48 => "Cyan", 
			49 => "DarkBlue", 
			50 => "DarkCyan", 
			51 => "DarkGoldenrod", 
			52 => "DarkGray", 
			53 => "DarkGreen", 
			54 => "DarkKhaki", 
			55 => "DarkMagenta", 
			56 => "DarkOliveGreen", 
			57 => "DarkOrange", 
			58 => "DarkOrchid", 
			59 => "DarkRed", 
			60 => "DarkSalmon", 
			61 => "DarkSeaGreen", 
			62 => "DarkSlateBlue", 
			63 => "DarkSlateGray", 
			64 => "DarkTurquoise", 
			65 => "DarkViolet", 
			66 => "DeepPink", 
			67 => "DeepSkyBlue", 
			68 => "DimGray", 
			69 => "DodgerBlue", 
			70 => "Firebrick", 
			71 => "FloralWhite", 
			72 => "ForestGreen", 
			73 => "Fuchsia", 
			74 => "Gainsboro", 
			75 => "GhostWhite", 
			76 => "Gold", 
			77 => "Goldenrod", 
			78 => "Gray", 
			79 => "Green", 
			80 => "GreenYellow", 
			81 => "Honeydew", 
			82 => "HotPink", 
			83 => "IndianRed", 
			84 => "Indigo", 
			85 => "Ivory", 
			86 => "Khaki", 
			87 => "Lavender", 
			88 => "LavenderBlush", 
			89 => "LawnGreen", 
			90 => "LemonChiffon", 
			91 => "LightBlue", 
			92 => "LightCoral", 
			93 => "LightCyan", 
			94 => "LightGoldenrodYellow", 
			95 => "LightGray", 
			96 => "LightGreen", 
			97 => "LightPink", 
			98 => "LightSalmon", 
			99 => "LightSeaGreen", 
			100 => "LightSkyBlue", 
			101 => "LightSlateGray", 
			102 => "LightSteelBlue", 
			103 => "LightYellow", 
			104 => "Lime", 
			105 => "LimeGreen", 
			106 => "Linen", 
			107 => "Magenta", 
			108 => "Maroon", 
			109 => "MediumAquamarine", 
			110 => "MediumBlue", 
			111 => "MediumOrchid", 
			112 => "MediumPurple", 
			113 => "MediumSeaGreen", 
			114 => "MediumSlateBlue", 
			115 => "MediumSpringGreen", 
			116 => "MediumTurquoise", 
			117 => "MediumVioletRed", 
			118 => "MidnightBlue", 
			119 => "MintCream", 
			120 => "MistyRose", 
			121 => "Moccasin", 
			122 => "NavajoWhite", 
			123 => "Navy", 
			124 => "OldLace", 
			125 => "Olive", 
			126 => "OliveDrab", 
			127 => "Orange", 
			128 => "OrangeRed", 
			129 => "Orchid", 
			130 => "PaleGoldenrod", 
			131 => "PaleGreen", 
			132 => "PaleTurquoise", 
			133 => "PaleVioletRed", 
			134 => "PapayaWhip", 
			135 => "PeachPuff", 
			136 => "Peru", 
			137 => "Pink", 
			138 => "Plum", 
			139 => "PowderBlue", 
			140 => "Purple", 
			141 => "Red", 
			142 => "RosyBrown", 
			143 => "RoyalBlue", 
			144 => "SaddleBrown", 
			145 => "Salmon", 
			146 => "SandyBrown", 
			147 => "SeaGreen", 
			148 => "SeaShell", 
			149 => "Sienna", 
			150 => "Silver", 
			151 => "SkyBlue", 
			152 => "SlateBlue", 
			153 => "SlateGray", 
			154 => "Snow", 
			155 => "SpringGreen", 
			156 => "SteelBlue", 
			157 => "Tan", 
			158 => "Teal", 
			159 => "Thistle", 
			160 => "Tomato", 
			161 => "Turquoise", 
			162 => "Violet", 
			163 => "Wheat", 
			164 => "White", 
			165 => "WhiteSmoke", 
			166 => "Yellow", 
			167 => "YellowGreen", 
			168 => "ButtonFace", 
			169 => "ButtonHighlight", 
			170 => "ButtonShadow", 
			171 => "GradientActiveCaption", 
			172 => "GradientInactiveCaption", 
			173 => "MenuBar", 
			174 => "MenuHighlight", 
			_ => string.Empty, 
		};
	}

	public static string GetName(KnownColor kc)
	{
		return GetName((short)kc);
	}

	public static Color FindColorMatch(Color c)
	{
		uint num = (uint)c.ToArgb();
		for (int i = 27; i < 167; i++)
		{
			if (num == ArgbValues[i])
			{
				return FromKnownColor((KnownColor)i);
			}
		}
		return Color.Empty;
	}

	public static void Update(int knownColor, int color)
	{
		ArgbValues[knownColor] = (uint)color;
	}
}
