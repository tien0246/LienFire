using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace System.Drawing;

public sealed class TextureBrush : Brush
{
	public Matrix Transform
	{
		get
		{
			Matrix matrix = new Matrix();
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetTextureTransform(new HandleRef(this, base.NativeBrush), new HandleRef(matrix, matrix.nativeMatrix)));
			return matrix;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetTextureTransform(new HandleRef(this, base.NativeBrush), new HandleRef(value, value.nativeMatrix)));
		}
	}

	public WrapMode WrapMode
	{
		get
		{
			int wrapMode = 0;
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetTextureWrapMode(new HandleRef(this, base.NativeBrush), out wrapMode));
			return (WrapMode)wrapMode;
		}
		set
		{
			if (value < WrapMode.Tile || value > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(WrapMode));
			}
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipSetTextureWrapMode(new HandleRef(this, base.NativeBrush), (int)value));
		}
	}

	public Image Image
	{
		get
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipGetTextureImage(new HandleRef(this, base.NativeBrush), out var image));
			return Image.CreateImageObject(image);
		}
	}

	public TextureBrush(Image bitmap)
		: this(bitmap, WrapMode.Tile)
	{
	}

	public TextureBrush(Image image, WrapMode wrapMode)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
		{
			throw new InvalidEnumArgumentException("wrapMode", (int)wrapMode, typeof(WrapMode));
		}
		IntPtr texture = IntPtr.Zero;
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateTexture(new HandleRef(image, image.nativeImage), (int)wrapMode, out texture));
		SetNativeBrushInternal(texture);
	}

	public TextureBrush(Image image, WrapMode wrapMode, RectangleF dstRect)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
		{
			throw new InvalidEnumArgumentException("wrapMode", (int)wrapMode, typeof(WrapMode));
		}
		IntPtr texture = IntPtr.Zero;
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateTexture2(new HandleRef(image, image.nativeImage), (int)wrapMode, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out texture));
		SetNativeBrushInternal(texture);
	}

	public TextureBrush(Image image, WrapMode wrapMode, Rectangle dstRect)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
		{
			throw new InvalidEnumArgumentException("wrapMode", (int)wrapMode, typeof(WrapMode));
		}
		IntPtr texture = IntPtr.Zero;
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateTexture2I(new HandleRef(image, image.nativeImage), (int)wrapMode, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out texture));
		SetNativeBrushInternal(texture);
	}

	public TextureBrush(Image image, RectangleF dstRect)
		: this(image, dstRect, null)
	{
	}

	public TextureBrush(Image image, RectangleF dstRect, ImageAttributes imageAttr)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		IntPtr texture = IntPtr.Zero;
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateTextureIA(new HandleRef(image, image.nativeImage), new HandleRef(imageAttr, imageAttr?.nativeImageAttributes ?? IntPtr.Zero), dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out texture));
		SetNativeBrushInternal(texture);
	}

	public TextureBrush(Image image, Rectangle dstRect)
		: this(image, dstRect, null)
	{
	}

	public TextureBrush(Image image, Rectangle dstRect, ImageAttributes imageAttr)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		IntPtr texture = IntPtr.Zero;
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCreateTextureIAI(new HandleRef(image, image.nativeImage), new HandleRef(imageAttr, imageAttr?.nativeImageAttributes ?? IntPtr.Zero), dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out texture));
		SetNativeBrushInternal(texture);
	}

	internal TextureBrush(IntPtr nativeBrush)
	{
		SetNativeBrushInternal(nativeBrush);
	}

	public override object Clone()
	{
		IntPtr clonedBrush = IntPtr.Zero;
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipCloneBrush(new HandleRef(this, base.NativeBrush), out clonedBrush));
		return new TextureBrush(clonedBrush);
	}

	public void ResetTransform()
	{
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipResetTextureTransform(new HandleRef(this, base.NativeBrush)));
	}

	public void MultiplyTransform(Matrix matrix)
	{
		MultiplyTransform(matrix, MatrixOrder.Prepend);
	}

	public void MultiplyTransform(Matrix matrix, MatrixOrder order)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		if (!(matrix.nativeMatrix == IntPtr.Zero))
		{
			SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipMultiplyTextureTransform(new HandleRef(this, base.NativeBrush), new HandleRef(matrix, matrix.nativeMatrix), order));
		}
	}

	public void TranslateTransform(float dx, float dy)
	{
		TranslateTransform(dx, dy, MatrixOrder.Prepend);
	}

	public void TranslateTransform(float dx, float dy, MatrixOrder order)
	{
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipTranslateTextureTransform(new HandleRef(this, base.NativeBrush), dx, dy, order));
	}

	public void ScaleTransform(float sx, float sy)
	{
		ScaleTransform(sx, sy, MatrixOrder.Prepend);
	}

	public void ScaleTransform(float sx, float sy, MatrixOrder order)
	{
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipScaleTextureTransform(new HandleRef(this, base.NativeBrush), sx, sy, order));
	}

	public void RotateTransform(float angle)
	{
		RotateTransform(angle, MatrixOrder.Prepend);
	}

	public void RotateTransform(float angle, MatrixOrder order)
	{
		SafeNativeMethods.Gdip.CheckStatus(GDIPlus.GdipRotateTextureTransform(new HandleRef(this, base.NativeBrush), angle, order));
	}
}
