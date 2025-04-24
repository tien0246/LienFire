namespace System.Drawing;

public sealed class SystemIcons
{
	private static Icon[] icons;

	private const int Application_Winlogo = 0;

	private const int Asterisk_Information = 1;

	private const int Error_Hand = 2;

	private const int Exclamation_Warning = 3;

	private const int Question_ = 4;

	private const int Shield_ = 5;

	public static Icon Application => icons[0];

	public static Icon Asterisk => icons[1];

	public static Icon Error => icons[2];

	public static Icon Exclamation => icons[3];

	public static Icon Hand => icons[2];

	public static Icon Information => icons[1];

	public static Icon Question => icons[4];

	public static Icon Warning => icons[3];

	public static Icon WinLogo => icons[0];

	public static Icon Shield => icons[5];

	static SystemIcons()
	{
		icons = new Icon[6];
		icons[0] = new Icon("Mono.ico", undisposable: true);
		icons[1] = new Icon("Information.ico", undisposable: true);
		icons[2] = new Icon("Error.ico", undisposable: true);
		icons[3] = new Icon("Warning.ico", undisposable: true);
		icons[4] = new Icon("Question.ico", undisposable: true);
		icons[5] = new Icon("Shield.ico", undisposable: true);
	}

	private SystemIcons()
	{
	}
}
