using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;

namespace System.Drawing;

public class RectangleConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
		{
			return true;
		}
		return base.CanConvertFrom(context, sourceType);
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			return true;
		}
		if (destinationType == typeof(InstanceDescriptor))
		{
			return true;
		}
		return base.CanConvertTo(context, destinationType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (!(value is string text))
		{
			return base.ConvertFrom(context, culture, value);
		}
		if (culture == null)
		{
			culture = CultureInfo.CurrentCulture;
		}
		string[] array = text.Split(culture.TextInfo.ListSeparator.ToCharArray());
		Int32Converter int32Converter = new Int32Converter();
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = (int)int32Converter.ConvertFromString(context, culture, array[i]);
		}
		if (array.Length != 4)
		{
			throw new ArgumentException("Failed to parse Text(" + text + ") expected text in the format \"x,y,Width,Height.\"");
		}
		return new Rectangle(array2[0], array2[1], array2[2], array2[3]);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Rectangle rectangle)
		{
			if (destinationType == typeof(string))
			{
				string listSeparator = culture.TextInfo.ListSeparator;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(rectangle.X.ToString(culture));
				stringBuilder.Append(listSeparator);
				stringBuilder.Append(" ");
				stringBuilder.Append(rectangle.Y.ToString(culture));
				stringBuilder.Append(listSeparator);
				stringBuilder.Append(" ");
				stringBuilder.Append(rectangle.Width.ToString(culture));
				stringBuilder.Append(listSeparator);
				stringBuilder.Append(" ");
				stringBuilder.Append(rectangle.Height.ToString(culture));
				return stringBuilder.ToString();
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				return new InstanceDescriptor(typeof(Rectangle).GetConstructor(new Type[4]
				{
					typeof(int),
					typeof(int),
					typeof(int),
					typeof(int)
				}), new object[4] { rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height });
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		object obj = propertyValues["X"];
		object obj2 = propertyValues["Y"];
		object obj3 = propertyValues["Width"];
		object obj4 = propertyValues["Height"];
		if (obj == null || obj2 == null || obj3 == null || obj4 == null)
		{
			throw new ArgumentException("propertyValues");
		}
		int x = (int)obj;
		int y = (int)obj2;
		int width = (int)obj3;
		int height = (int)obj4;
		return new Rectangle(x, y, width, height);
	}

	public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		if (value is Rectangle)
		{
			return TypeDescriptor.GetProperties(value, attributes);
		}
		return base.GetProperties(context, value, attributes);
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
