using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Text;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace System.Drawing;

public class FontConverter : TypeConverter
{
	public sealed class FontNameConverter : TypeConverter, IDisposable
	{
		private FontFamily[] fonts;

		public FontNameConverter()
		{
			fonts = FontFamily.Families;
		}

		void IDisposable.Dispose()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				return value;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			string[] array = new string[fonts.Length];
			int num = fonts.Length;
			while (num > 0)
			{
				num--;
				array[num] = fonts[num].Name;
			}
			return new StandardValuesCollection(array);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}

	public class FontUnitConverter : EnumConverter
	{
		public FontUnitConverter()
			: base(typeof(GraphicsUnit))
		{
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return base.GetStandardValues(context);
		}
	}

	~FontConverter()
	{
	}

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

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string) && value is Font)
		{
			Font font = (Font)value;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(font.Name).Append(culture.TextInfo.ListSeparator[0] + " ");
			stringBuilder.Append(font.Size);
			switch (font.Unit)
			{
			case GraphicsUnit.Display:
				stringBuilder.Append("display");
				break;
			case GraphicsUnit.Document:
				stringBuilder.Append("doc");
				break;
			case GraphicsUnit.Point:
				stringBuilder.Append("pt");
				break;
			case GraphicsUnit.Inch:
				stringBuilder.Append("in");
				break;
			case GraphicsUnit.Millimeter:
				stringBuilder.Append("mm");
				break;
			case GraphicsUnit.Pixel:
				stringBuilder.Append("px");
				break;
			case GraphicsUnit.World:
				stringBuilder.Append("world");
				break;
			}
			if (font.Style != FontStyle.Regular)
			{
				stringBuilder.Append(culture.TextInfo.ListSeparator[0] + " style=").Append(font.Style);
			}
			return stringBuilder.ToString();
		}
		if (destinationType == typeof(InstanceDescriptor) && value is Font)
		{
			Font font2 = (Font)value;
			return new InstanceDescriptor(typeof(Font).GetTypeInfo().GetConstructor(new Type[4]
			{
				typeof(string),
				typeof(float),
				typeof(FontStyle),
				typeof(GraphicsUnit)
			}), new object[4] { font2.Name, font2.Size, font2.Style, font2.Unit });
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (!(value is string))
		{
			return base.ConvertFrom(context, culture, value);
		}
		string text = (string)value;
		text = text.Trim();
		if (text.Length == 0)
		{
			return null;
		}
		if (culture == null)
		{
			culture = CultureInfo.CurrentCulture;
		}
		string[] array = text.Split(new char[1] { culture.TextInfo.ListSeparator[0] });
		if (array.Length < 1)
		{
			throw new ArgumentException("Failed to parse font format");
		}
		text = array[0];
		float emSize = 8f;
		string text2 = "px";
		GraphicsUnit unit = GraphicsUnit.Pixel;
		if (array.Length > 1)
		{
			for (int i = 0; i < array[1].Length; i++)
			{
				if (char.IsLetter(array[1][i]))
				{
					emSize = (float)TypeDescriptor.GetConverter(typeof(float)).ConvertFromString(context, culture, array[1].Substring(0, i));
					text2 = array[1].Substring(i);
					break;
				}
			}
			switch (text2)
			{
			case "display":
				unit = GraphicsUnit.Display;
				break;
			case "doc":
				unit = GraphicsUnit.Document;
				break;
			case "pt":
				unit = GraphicsUnit.Point;
				break;
			case "in":
				unit = GraphicsUnit.Inch;
				break;
			case "mm":
				unit = GraphicsUnit.Millimeter;
				break;
			case "px":
				unit = GraphicsUnit.Pixel;
				break;
			case "world":
				unit = GraphicsUnit.World;
				break;
			}
		}
		FontStyle fontStyle = FontStyle.Regular;
		if (array.Length > 2)
		{
			for (int j = 2; j < array.Length; j++)
			{
				string obj = array[j];
				if (obj.IndexOf("Regular") != -1)
				{
					fontStyle |= FontStyle.Regular;
				}
				if (obj.IndexOf("Bold") != -1)
				{
					fontStyle |= FontStyle.Bold;
				}
				if (obj.IndexOf("Italic") != -1)
				{
					fontStyle |= FontStyle.Italic;
				}
				if (obj.IndexOf("Strikeout") != -1)
				{
					fontStyle |= FontStyle.Strikeout;
				}
				if (obj.IndexOf("Underline") != -1)
				{
					fontStyle |= FontStyle.Underline;
				}
			}
		}
		return new Font(text, emSize, fontStyle, unit);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		byte gdiCharSet = 1;
		float emSize = 8f;
		string text = null;
		bool gdiVerticalFont = false;
		FontStyle fontStyle = FontStyle.Regular;
		FontFamily fontFamily = null;
		GraphicsUnit unit = GraphicsUnit.Point;
		object obj;
		if ((obj = propertyValues["GdiCharSet"]) != null)
		{
			gdiCharSet = (byte)obj;
		}
		if ((obj = propertyValues["Size"]) != null)
		{
			emSize = (float)obj;
		}
		if ((obj = propertyValues["Unit"]) != null)
		{
			unit = (GraphicsUnit)obj;
		}
		if ((obj = propertyValues["Name"]) != null)
		{
			text = (string)obj;
		}
		if ((obj = propertyValues["GdiVerticalFont"]) != null)
		{
			gdiVerticalFont = (bool)obj;
		}
		if ((obj = propertyValues["Bold"]) != null && (bool)obj)
		{
			fontStyle |= FontStyle.Bold;
		}
		if ((obj = propertyValues["Italic"]) != null && (bool)obj)
		{
			fontStyle |= FontStyle.Italic;
		}
		if ((obj = propertyValues["Strikeout"]) != null && (bool)obj)
		{
			fontStyle |= FontStyle.Strikeout;
		}
		if ((obj = propertyValues["Underline"]) != null && (bool)obj)
		{
			fontStyle |= FontStyle.Underline;
		}
		if (text == null)
		{
			fontFamily = new FontFamily("Tahoma");
		}
		else
		{
			text = text.ToLower();
			FontFamily[] families = new InstalledFontCollection().Families;
			foreach (FontFamily fontFamily2 in families)
			{
				if (text == fontFamily2.Name.ToLower())
				{
					fontFamily = fontFamily2;
					break;
				}
			}
			if (fontFamily == null)
			{
				families = new PrivateFontCollection().Families;
				foreach (FontFamily fontFamily3 in families)
				{
					if (text == fontFamily3.Name.ToLower())
					{
						fontFamily = fontFamily3;
						break;
					}
				}
			}
			if (fontFamily == null)
			{
				fontFamily = FontFamily.GenericSansSerif;
			}
		}
		return new Font(fontFamily, emSize, fontStyle, unit, gdiCharSet, gdiVerticalFont);
	}

	public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		if (value is Font)
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
