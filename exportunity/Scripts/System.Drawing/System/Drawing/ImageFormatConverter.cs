using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;

namespace System.Drawing;

public class ImageFormatConverter : TypeConverter
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
		if (text[0] == '[')
		{
			if (text.Equals(ImageFormat.Bmp.ToString()))
			{
				return ImageFormat.Bmp;
			}
			if (text.Equals(ImageFormat.Emf.ToString()))
			{
				return ImageFormat.Emf;
			}
			if (text.Equals(ImageFormat.Exif.ToString()))
			{
				return ImageFormat.Exif;
			}
			if (text.Equals(ImageFormat.Gif.ToString()))
			{
				return ImageFormat.Gif;
			}
			if (text.Equals(ImageFormat.Icon.ToString()))
			{
				return ImageFormat.Icon;
			}
			if (text.Equals(ImageFormat.Jpeg.ToString()))
			{
				return ImageFormat.Jpeg;
			}
			if (text.Equals(ImageFormat.MemoryBmp.ToString()))
			{
				return ImageFormat.MemoryBmp;
			}
			if (text.Equals(ImageFormat.Png.ToString()))
			{
				return ImageFormat.Png;
			}
			if (text.Equals(ImageFormat.Tiff.ToString()))
			{
				return ImageFormat.Tiff;
			}
			if (text.Equals(ImageFormat.Wmf.ToString()))
			{
				return ImageFormat.Wmf;
			}
		}
		else
		{
			if (string.Compare(text, "Bmp", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return ImageFormat.Bmp;
			}
			if (string.Compare(text, "Emf", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return ImageFormat.Emf;
			}
			if (string.Compare(text, "Exif", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return ImageFormat.Exif;
			}
			if (string.Compare(text, "Gif", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return ImageFormat.Gif;
			}
			if (string.Compare(text, "Icon", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return ImageFormat.Icon;
			}
			if (string.Compare(text, "Jpeg", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return ImageFormat.Jpeg;
			}
			if (string.Compare(text, "MemoryBmp", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return ImageFormat.MemoryBmp;
			}
			if (string.Compare(text, "Png", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return ImageFormat.Png;
			}
			if (string.Compare(text, "Tiff", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return ImageFormat.Tiff;
			}
			if (string.Compare(text, "Wmf", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return ImageFormat.Wmf;
			}
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is ImageFormat)
		{
			ImageFormat imageFormat = (ImageFormat)value;
			string text = null;
			if (imageFormat.Guid.Equals(ImageFormat.Bmp.Guid))
			{
				text = "Bmp";
			}
			else if (imageFormat.Guid.Equals(ImageFormat.Emf.Guid))
			{
				text = "Emf";
			}
			else if (imageFormat.Guid.Equals(ImageFormat.Exif.Guid))
			{
				text = "Exif";
			}
			else if (imageFormat.Guid.Equals(ImageFormat.Gif.Guid))
			{
				text = "Gif";
			}
			else if (imageFormat.Guid.Equals(ImageFormat.Icon.Guid))
			{
				text = "Icon";
			}
			else if (imageFormat.Guid.Equals(ImageFormat.Jpeg.Guid))
			{
				text = "Jpeg";
			}
			else if (imageFormat.Guid.Equals(ImageFormat.MemoryBmp.Guid))
			{
				text = "MemoryBmp";
			}
			else if (imageFormat.Guid.Equals(ImageFormat.Png.Guid))
			{
				text = "Png";
			}
			else if (imageFormat.Guid.Equals(ImageFormat.Tiff.Guid))
			{
				text = "Tiff";
			}
			else if (imageFormat.Guid.Equals(ImageFormat.Wmf.Guid))
			{
				text = "Wmf";
			}
			if (destinationType == typeof(string))
			{
				if (text == null)
				{
					return imageFormat.ToString();
				}
				return text;
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				if (text != null)
				{
					return new InstanceDescriptor(typeof(ImageFormat).GetTypeInfo().GetProperty(text), null);
				}
				return new InstanceDescriptor(typeof(ImageFormat).GetTypeInfo().GetConstructor(new Type[1] { typeof(Guid) }), new object[1] { imageFormat.Guid });
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		return new StandardValuesCollection(new ImageFormat[10]
		{
			ImageFormat.MemoryBmp,
			ImageFormat.Bmp,
			ImageFormat.Emf,
			ImageFormat.Wmf,
			ImageFormat.Gif,
			ImageFormat.Jpeg,
			ImageFormat.Png,
			ImageFormat.Tiff,
			ImageFormat.Exif,
			ImageFormat.Icon
		});
	}

	public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
