using System.ComponentModel;
using System.Globalization;

namespace System.Configuration;

public sealed class TimeSpanSecondsOrInfiniteConverter : TimeSpanSecondsConverter
{
	public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
	{
		if ((string)data == "Infinite")
		{
			return TimeSpan.MaxValue;
		}
		return base.ConvertFrom(ctx, ci, data);
	}

	public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
	{
		if (value.GetType() != typeof(TimeSpan))
		{
			throw new ArgumentException();
		}
		if ((TimeSpan)value == TimeSpan.MaxValue)
		{
			return "Infinite";
		}
		return base.ConvertTo(ctx, ci, value, type);
	}
}
