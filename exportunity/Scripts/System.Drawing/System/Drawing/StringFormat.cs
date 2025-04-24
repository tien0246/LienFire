using System.ComponentModel;
using System.Drawing.Text;

namespace System.Drawing;

public sealed class StringFormat : MarshalByRefObject, IDisposable, ICloneable
{
	private IntPtr nativeStrFmt = IntPtr.Zero;

	private int language;

	public StringAlignment Alignment
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatAlign(nativeStrFmt, out var align));
			return align;
		}
		set
		{
			if (value < StringAlignment.Near || value > StringAlignment.Far)
			{
				throw new InvalidEnumArgumentException("Alignment");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatAlign(nativeStrFmt, value));
		}
	}

	public StringAlignment LineAlignment
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatLineAlign(nativeStrFmt, out var align));
			return align;
		}
		set
		{
			if (value < StringAlignment.Near || value > StringAlignment.Far)
			{
				throw new InvalidEnumArgumentException("Alignment");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatLineAlign(nativeStrFmt, value));
		}
	}

	public StringFormatFlags FormatFlags
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatFlags(nativeStrFmt, out var flags));
			return flags;
		}
		set
		{
			GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatFlags(nativeStrFmt, value));
		}
	}

	public HotkeyPrefix HotkeyPrefix
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatHotkeyPrefix(nativeStrFmt, out var hotkeyPrefix));
			return hotkeyPrefix;
		}
		set
		{
			if (value < HotkeyPrefix.None || value > HotkeyPrefix.Hide)
			{
				throw new InvalidEnumArgumentException("HotkeyPrefix");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatHotkeyPrefix(nativeStrFmt, value));
		}
	}

	public StringTrimming Trimming
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatTrimming(nativeStrFmt, out var trimming));
			return trimming;
		}
		set
		{
			if (value < StringTrimming.None || value > StringTrimming.EllipsisPath)
			{
				throw new InvalidEnumArgumentException("Trimming");
			}
			GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatTrimming(nativeStrFmt, value));
		}
	}

	public static StringFormat GenericDefault
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipStringFormatGetGenericDefault(out var format));
			return new StringFormat(format);
		}
	}

	public int DigitSubstitutionLanguage => language;

	public static StringFormat GenericTypographic
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipStringFormatGetGenericTypographic(out var format));
			return new StringFormat(format);
		}
	}

	public StringDigitSubstitute DigitSubstitutionMethod
	{
		get
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatDigitSubstitution(nativeStrFmt, language, out var substitute));
			return substitute;
		}
	}

	internal IntPtr NativeObject
	{
		get
		{
			return nativeStrFmt;
		}
		set
		{
			nativeStrFmt = value;
		}
	}

	internal IntPtr nativeFormat => nativeStrFmt;

	public StringFormat()
		: this((StringFormatFlags)0, 0)
	{
	}

	public StringFormat(StringFormatFlags options, int language)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateStringFormat(options, language, out nativeStrFmt));
	}

	internal StringFormat(IntPtr native)
	{
		nativeStrFmt = native;
	}

	~StringFormat()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (nativeStrFmt != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeleteStringFormat(nativeStrFmt);
			nativeStrFmt = IntPtr.Zero;
			GDIPlus.CheckStatus(status);
		}
	}

	public StringFormat(StringFormat format)
	{
		if (format == null)
		{
			throw new ArgumentNullException("format");
		}
		GDIPlus.CheckStatus(GDIPlus.GdipCloneStringFormat(format.NativeObject, out nativeStrFmt));
	}

	public StringFormat(StringFormatFlags options)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateStringFormat(options, 0, out nativeStrFmt));
	}

	public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatMeasurableCharacterRanges(nativeStrFmt, ranges.Length, ranges));
	}

	internal int GetMeasurableCharacterRangeCount()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatMeasurableCharacterRangeCount(nativeStrFmt, out var cnt));
		return cnt;
	}

	public object Clone()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCloneStringFormat(nativeStrFmt, out var format));
		return new StringFormat(format);
	}

	public override string ToString()
	{
		return "[StringFormat, FormatFlags=" + FormatFlags.ToString() + "]";
	}

	public void SetTabStops(float firstTabOffset, float[] tabStops)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatTabStops(nativeStrFmt, firstTabOffset, tabStops.Length, tabStops));
	}

	public void SetDigitSubstitution(int language, StringDigitSubstitute substitute)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipSetStringFormatDigitSubstitution(nativeStrFmt, this.language, substitute));
	}

	public float[] GetTabStops(out float firstTabOffset)
	{
		int count = 0;
		firstTabOffset = 0f;
		GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatTabStopCount(nativeStrFmt, out count));
		float[] array = new float[count];
		if (count != 0)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipGetStringFormatTabStops(nativeStrFmt, count, out firstTabOffset, array));
		}
		return array;
	}
}
