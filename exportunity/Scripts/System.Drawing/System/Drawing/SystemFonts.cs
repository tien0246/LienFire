namespace System.Drawing;

public sealed class SystemFonts
{
	public static Font CaptionFont => new Font("Microsoft Sans Serif", 11f, "CaptionFont");

	public static Font DefaultFont => new Font("Microsoft Sans Serif", 8.25f, "DefaultFont");

	public static Font DialogFont => new Font("Tahoma", 8f, "DialogFont");

	public static Font IconTitleFont => new Font("Microsoft Sans Serif", 11f, "IconTitleFont");

	public static Font MenuFont => new Font("Microsoft Sans Serif", 11f, "MenuFont");

	public static Font MessageBoxFont => new Font("Microsoft Sans Serif", 11f, "MessageBoxFont");

	public static Font SmallCaptionFont => new Font("Microsoft Sans Serif", 11f, "SmallCaptionFont");

	public static Font StatusFont => new Font("Microsoft Sans Serif", 11f, "StatusFont");

	static SystemFonts()
	{
	}

	private SystemFonts()
	{
	}

	public static Font GetFontByName(string systemFontName)
	{
		return systemFontName switch
		{
			"CaptionFont" => CaptionFont, 
			"DefaultFont" => DefaultFont, 
			"DialogFont" => DialogFont, 
			"IconTitleFont" => IconTitleFont, 
			"MenuFont" => MenuFont, 
			"MessageBoxFont" => MessageBoxFont, 
			"SmallCaptionFont" => SmallCaptionFont, 
			"StatusFont" => StatusFont, 
			_ => null, 
		};
	}
}
