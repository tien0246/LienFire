using System.IO;
using System.Reflection;

namespace System.Drawing;

[AttributeUsage(AttributeTargets.Class)]
public class ToolboxBitmapAttribute : Attribute
{
	private Image smallImage;

	private Image bigImage;

	public static readonly ToolboxBitmapAttribute Default = new ToolboxBitmapAttribute();

	private ToolboxBitmapAttribute()
	{
	}

	public ToolboxBitmapAttribute(string imageFile)
	{
	}

	public ToolboxBitmapAttribute(Type t)
	{
		smallImage = GetImageFromResource(t, null, large: false);
	}

	public ToolboxBitmapAttribute(Type t, string name)
	{
		smallImage = GetImageFromResource(t, name, large: false);
	}

	public override bool Equals(object value)
	{
		if (!(value is ToolboxBitmapAttribute))
		{
			return false;
		}
		if (value == this)
		{
			return true;
		}
		return ((ToolboxBitmapAttribute)value).smallImage == smallImage;
	}

	public override int GetHashCode()
	{
		return smallImage.GetHashCode() ^ bigImage.GetHashCode();
	}

	public Image GetImage(object component)
	{
		return GetImage(component.GetType(), null, large: false);
	}

	public Image GetImage(object component, bool large)
	{
		return GetImage(component.GetType(), null, large);
	}

	public Image GetImage(Type type)
	{
		return GetImage(type, null, large: false);
	}

	public Image GetImage(Type type, bool large)
	{
		return GetImage(type, null, large);
	}

	public Image GetImage(Type type, string imgName, bool large)
	{
		if (smallImage == null)
		{
			smallImage = GetImageFromResource(type, imgName, large: false);
		}
		if (large)
		{
			if (bigImage == null)
			{
				bigImage = new Bitmap(smallImage, 32, 32);
			}
			return bigImage;
		}
		return smallImage;
	}

	public static Image GetImageFromResource(Type t, string imageName, bool large)
	{
		if (imageName == null)
		{
			imageName = t.Name + ".bmp";
		}
		try
		{
			Bitmap bitmap;
			using (Stream stream = t.GetTypeInfo().Assembly.GetManifestResourceStream(t.Namespace + "." + imageName))
			{
				if (stream == null)
				{
					return null;
				}
				bitmap = new Bitmap(stream, useIcm: false);
			}
			if (large)
			{
				return new Bitmap(bitmap, 32, 32);
			}
			return bitmap;
		}
		catch
		{
			return null;
		}
	}
}
