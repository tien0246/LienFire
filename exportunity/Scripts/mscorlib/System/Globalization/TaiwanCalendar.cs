using System.Runtime.InteropServices;

namespace System.Globalization;

[Serializable]
[ComVisible(true)]
public class TaiwanCalendar : Calendar
{
	internal static EraInfo[] taiwanEraInfo = new EraInfo[1]
	{
		new EraInfo(1, 1912, 1, 1, 1911, 1, 8088)
	};

	internal static volatile Calendar s_defaultInstance;

	internal GregorianCalendarHelper helper;

	internal static readonly DateTime calendarMinValue = new DateTime(1912, 1, 1);

	private const int DEFAULT_TWO_DIGIT_YEAR_MAX = 99;

	[ComVisible(false)]
	public override DateTime MinSupportedDateTime => calendarMinValue;

	[ComVisible(false)]
	public override DateTime MaxSupportedDateTime => DateTime.MaxValue;

	[ComVisible(false)]
	public override CalendarAlgorithmType AlgorithmType => CalendarAlgorithmType.SolarCalendar;

	internal override int ID => 4;

	public override int[] Eras => helper.Eras;

	public override int TwoDigitYearMax
	{
		get
		{
			if (twoDigitYearMax == -1)
			{
				twoDigitYearMax = Calendar.GetSystemTwoDigitYearSetting(ID, 99);
			}
			return twoDigitYearMax;
		}
		set
		{
			VerifyWritable();
			if (value < 99 || value > helper.MaxYear)
			{
				throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), 99, helper.MaxYear));
			}
			twoDigitYearMax = value;
		}
	}

	internal static Calendar GetDefaultInstance()
	{
		if (s_defaultInstance == null)
		{
			s_defaultInstance = new TaiwanCalendar();
		}
		return s_defaultInstance;
	}

	public TaiwanCalendar()
	{
		try
		{
			new CultureInfo("zh-TW");
		}
		catch (ArgumentException innerException)
		{
			throw new TypeInitializationException(GetType().FullName, innerException);
		}
		helper = new GregorianCalendarHelper(this, taiwanEraInfo);
	}

	public override DateTime AddMonths(DateTime time, int months)
	{
		return helper.AddMonths(time, months);
	}

	public override DateTime AddYears(DateTime time, int years)
	{
		return helper.AddYears(time, years);
	}

	public override int GetDaysInMonth(int year, int month, int era)
	{
		return helper.GetDaysInMonth(year, month, era);
	}

	public override int GetDaysInYear(int year, int era)
	{
		return helper.GetDaysInYear(year, era);
	}

	public override int GetDayOfMonth(DateTime time)
	{
		return helper.GetDayOfMonth(time);
	}

	public override DayOfWeek GetDayOfWeek(DateTime time)
	{
		return helper.GetDayOfWeek(time);
	}

	public override int GetDayOfYear(DateTime time)
	{
		return helper.GetDayOfYear(time);
	}

	public override int GetMonthsInYear(int year, int era)
	{
		return helper.GetMonthsInYear(year, era);
	}

	[ComVisible(false)]
	public override int GetWeekOfYear(DateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
	{
		return helper.GetWeekOfYear(time, rule, firstDayOfWeek);
	}

	public override int GetEra(DateTime time)
	{
		return helper.GetEra(time);
	}

	public override int GetMonth(DateTime time)
	{
		return helper.GetMonth(time);
	}

	public override int GetYear(DateTime time)
	{
		return helper.GetYear(time);
	}

	public override bool IsLeapDay(int year, int month, int day, int era)
	{
		return helper.IsLeapDay(year, month, day, era);
	}

	public override bool IsLeapYear(int year, int era)
	{
		return helper.IsLeapYear(year, era);
	}

	[ComVisible(false)]
	public override int GetLeapMonth(int year, int era)
	{
		return helper.GetLeapMonth(year, era);
	}

	public override bool IsLeapMonth(int year, int month, int era)
	{
		return helper.IsLeapMonth(year, month, era);
	}

	public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
	{
		return helper.ToDateTime(year, month, day, hour, minute, second, millisecond, era);
	}

	public override int ToFourDigitYear(int year)
	{
		if (year <= 0)
		{
			throw new ArgumentOutOfRangeException("year", Environment.GetResourceString("Positive number required."));
		}
		if (year > helper.MaxYear)
		{
			throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), 1, helper.MaxYear));
		}
		return year;
	}
}
