using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace System.Drawing;

public sealed class Pen : MarshalByRefObject, ICloneable, IDisposable
{
	internal IntPtr nativeObject;

	internal bool isModifiable = true;

	private Color color;

	private CustomLineCap startCap;

	private CustomLineCap endCap;

	[System.MonoLimitation("Libgdiplus doesn't use this property for rendering")]
	public PenAlignment Alignment
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenMode(nativeObject, out var alignment));
			return alignment;
		}
		set
		{
			if (value < PenAlignment.Center || value > PenAlignment.Right)
			{
				throw new InvalidEnumArgumentException("Alignment", (int)value, typeof(PenAlignment));
			}
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenMode(nativeObject, value));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public Brush Brush
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenBrushFill(nativeObject, out var brush));
			return new SolidBrush(brush);
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("Brush");
			}
			if (!isModifiable)
			{
				throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetPenBrushFill(nativeObject, value.NativeBrush));
			color = Color.Empty;
		}
	}

	public Color Color
	{
		get
		{
			if (color.Equals(Color.Empty))
			{
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenColor(nativeObject, out var argb));
				color = Color.FromArgb(argb);
			}
			return color;
		}
		set
		{
			if (!isModifiable)
			{
				throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetPenColor(nativeObject, value.ToArgb()));
			color = value;
		}
	}

	public float[] CompoundArray
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenCompoundCount(nativeObject, out var count));
			float[] array = new float[count];
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenCompoundArray(nativeObject, array, count));
			return array;
		}
		set
		{
			if (isModifiable)
			{
				if (value.Length < 2)
				{
					throw new ArgumentException("Invalid parameter.");
				}
				foreach (float num in value)
				{
					if (num < 0f || num > 1f)
					{
						throw new ArgumentException("Invalid parameter.");
					}
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenCompoundArray(nativeObject, value, value.Length));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public CustomLineCap CustomEndCap
	{
		get
		{
			return endCap;
		}
		set
		{
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenCustomEndCap(nativeObject, value.nativeCap));
				endCap = value;
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public CustomLineCap CustomStartCap
	{
		get
		{
			return startCap;
		}
		set
		{
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenCustomStartCap(nativeObject, value.nativeCap));
				startCap = value;
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public DashCap DashCap
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenDashCap197819(nativeObject, out var dashCap));
			return dashCap;
		}
		set
		{
			if (value < DashCap.Flat || value > DashCap.Triangle)
			{
				throw new InvalidEnumArgumentException("DashCap", (int)value, typeof(DashCap));
			}
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenDashCap197819(nativeObject, value));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public float DashOffset
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenDashOffset(nativeObject, out var offset));
			return offset;
		}
		set
		{
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenDashOffset(nativeObject, value));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public float[] DashPattern
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenDashCount(nativeObject, out var count));
			float[] array;
			if (count <= 0)
			{
				array = ((DashStyle != DashStyle.Custom) ? new float[0] : new float[1] { 1f });
			}
			else
			{
				array = new float[count];
				GDIPlus.CheckStatus(GDIPlus.GdipGetPenDashArray(nativeObject, array, count));
			}
			return array;
		}
		set
		{
			if (isModifiable)
			{
				if (value.Length == 0)
				{
					throw new ArgumentException("Invalid parameter.");
				}
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i] <= 0f)
					{
						throw new ArgumentException("Invalid parameter.");
					}
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenDashArray(nativeObject, value, value.Length));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public DashStyle DashStyle
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenDashStyle(nativeObject, out var dashStyle));
			return dashStyle;
		}
		set
		{
			if (value < DashStyle.Solid || value > DashStyle.Custom)
			{
				throw new InvalidEnumArgumentException("DashStyle", (int)value, typeof(DashStyle));
			}
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenDashStyle(nativeObject, value));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public LineCap StartCap
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenStartCap(nativeObject, out var result));
			return result;
		}
		set
		{
			if (value < LineCap.Flat || value > LineCap.Custom)
			{
				throw new InvalidEnumArgumentException("StartCap", (int)value, typeof(LineCap));
			}
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenStartCap(nativeObject, value));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public LineCap EndCap
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenEndCap(nativeObject, out var result));
			return result;
		}
		set
		{
			if (value < LineCap.Flat || value > LineCap.Custom)
			{
				throw new InvalidEnumArgumentException("EndCap", (int)value, typeof(LineCap));
			}
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenEndCap(nativeObject, value));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public LineJoin LineJoin
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenLineJoin(nativeObject, out var lineJoin));
			return lineJoin;
		}
		set
		{
			if (value < LineJoin.Miter || value > LineJoin.MiterClipped)
			{
				throw new InvalidEnumArgumentException("LineJoin", (int)value, typeof(LineJoin));
			}
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenLineJoin(nativeObject, value));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public float MiterLimit
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenMiterLimit(nativeObject, out var miterLimit));
			return miterLimit;
		}
		set
		{
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenMiterLimit(nativeObject, value));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public PenType PenType
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenFillType(nativeObject, out var type));
			return type;
		}
	}

	public Matrix Transform
	{
		get
		{
			Matrix matrix = new Matrix();
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenTransform(nativeObject, matrix.nativeMatrix));
			return matrix;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("Transform");
			}
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenTransform(nativeObject, value.nativeMatrix));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public float Width
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetPenWidth(nativeObject, out var width));
			return width;
		}
		set
		{
			if (isModifiable)
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetPenWidth(nativeObject, value));
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	internal IntPtr NativePen => nativeObject;

	internal Pen(IntPtr p)
	{
		nativeObject = p;
	}

	public Pen(Brush brush)
		: this(brush, 1f)
	{
	}

	public Pen(Color color)
		: this(color, 1f)
	{
	}

	public Pen(Brush brush, float width)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCreatePen2(brush.NativeBrush, width, GraphicsUnit.World, out nativeObject));
		color = Color.Empty;
	}

	public Pen(Color color, float width)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreatePen1(color.ToArgb(), width, GraphicsUnit.World, out nativeObject));
		this.color = color;
	}

	public object Clone()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipClonePen(nativeObject, out var clonepen));
		return new Pen(clonepen)
		{
			startCap = startCap,
			endCap = endCap
		};
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing && !isModifiable)
		{
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
		if (nativeObject != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeletePen(nativeObject);
			nativeObject = IntPtr.Zero;
			GDIPlus.CheckStatus(status);
		}
	}

	~Pen()
	{
		Dispose(disposing: false);
	}

	public void MultiplyTransform(Matrix matrix)
	{
		MultiplyTransform(matrix, MatrixOrder.Prepend);
	}

	public void MultiplyTransform(Matrix matrix, MatrixOrder order)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipMultiplyPenTransform(nativeObject, matrix.nativeMatrix, order));
	}

	public void ResetTransform()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipResetPenTransform(nativeObject));
	}

	public void RotateTransform(float angle)
	{
		RotateTransform(angle, MatrixOrder.Prepend);
	}

	public void RotateTransform(float angle, MatrixOrder order)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipRotatePenTransform(nativeObject, angle, order));
	}

	public void ScaleTransform(float sx, float sy)
	{
		ScaleTransform(sx, sy, MatrixOrder.Prepend);
	}

	public void ScaleTransform(float sx, float sy, MatrixOrder order)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipScalePenTransform(nativeObject, sx, sy, order));
	}

	public void SetLineCap(LineCap startCap, LineCap endCap, DashCap dashCap)
	{
		if (isModifiable)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetPenLineCap197819(nativeObject, startCap, endCap, dashCap));
			return;
		}
		throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
	}

	public void TranslateTransform(float dx, float dy)
	{
		TranslateTransform(dx, dy, MatrixOrder.Prepend);
	}

	public void TranslateTransform(float dx, float dy, MatrixOrder order)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipTranslatePenTransform(nativeObject, dx, dy, order));
	}
}
