using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Drawing;

public class IconConverter : ExpandableObjectConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(byte[]))
		{
			return true;
		}
		return false;
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(byte[]) || destinationType == typeof(string))
		{
			return true;
		}
		return false;
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (!(value is byte[] buffer))
		{
			return base.ConvertFrom(context, culture, value);
		}
		return new Icon(new MemoryStream(buffer));
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Icon && destinationType == typeof(string))
		{
			return value.ToString();
		}
		if (value == null && destinationType == typeof(string))
		{
			return "(none)";
		}
		if (CanConvertTo(null, destinationType))
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				((Icon)value).Save(memoryStream);
				return memoryStream.ToArray();
			}
		}
		return new NotSupportedException("IconConverter can not convert from " + value.GetType());
	}
}
