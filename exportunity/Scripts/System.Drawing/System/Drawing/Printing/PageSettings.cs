namespace System.Drawing.Printing;

[Serializable]
public class PageSettings : ICloneable
{
	internal bool color;

	internal bool landscape;

	internal PaperSize paperSize;

	internal PaperSource paperSource;

	internal PrinterResolution printerResolution;

	private Margins margins = new Margins();

	private float hardMarginX;

	private float hardMarginY;

	private RectangleF printableArea;

	private PrinterSettings printerSettings;

	public Rectangle Bounds
	{
		get
		{
			int width = paperSize.Width;
			int height = paperSize.Height;
			width -= margins.Left + margins.Right;
			height -= margins.Top + margins.Bottom;
			if (landscape)
			{
				int num = width;
				width = height;
				height = num;
			}
			return new Rectangle(margins.Left, margins.Top, width, height);
		}
	}

	public bool Color
	{
		get
		{
			if (!printerSettings.IsValid)
			{
				throw new InvalidPrinterException(printerSettings);
			}
			return color;
		}
		set
		{
			color = value;
		}
	}

	public bool Landscape
	{
		get
		{
			if (!printerSettings.IsValid)
			{
				throw new InvalidPrinterException(printerSettings);
			}
			return landscape;
		}
		set
		{
			landscape = value;
		}
	}

	public Margins Margins
	{
		get
		{
			if (!printerSettings.IsValid)
			{
				throw new InvalidPrinterException(printerSettings);
			}
			return margins;
		}
		set
		{
			margins = value;
		}
	}

	public PaperSize PaperSize
	{
		get
		{
			if (!printerSettings.IsValid)
			{
				throw new InvalidPrinterException(printerSettings);
			}
			return paperSize;
		}
		set
		{
			if (value != null)
			{
				paperSize = value;
			}
		}
	}

	public PaperSource PaperSource
	{
		get
		{
			if (!printerSettings.IsValid)
			{
				throw new InvalidPrinterException(printerSettings);
			}
			return paperSource;
		}
		set
		{
			if (value != null)
			{
				paperSource = value;
			}
		}
	}

	public PrinterResolution PrinterResolution
	{
		get
		{
			if (!printerSettings.IsValid)
			{
				throw new InvalidPrinterException(printerSettings);
			}
			return printerResolution;
		}
		set
		{
			if (value != null)
			{
				printerResolution = value;
			}
		}
	}

	public PrinterSettings PrinterSettings
	{
		get
		{
			return printerSettings;
		}
		set
		{
			printerSettings = value;
		}
	}

	public float HardMarginX => hardMarginX;

	public float HardMarginY => hardMarginY;

	public RectangleF PrintableArea => printableArea;

	public PageSettings()
		: this(new PrinterSettings())
	{
	}

	public PageSettings(PrinterSettings printerSettings)
	{
		PrinterSettings = printerSettings;
		color = printerSettings.DefaultPageSettings.color;
		landscape = printerSettings.DefaultPageSettings.landscape;
		paperSize = printerSettings.DefaultPageSettings.paperSize;
		paperSource = printerSettings.DefaultPageSettings.paperSource;
		printerResolution = printerSettings.DefaultPageSettings.printerResolution;
	}

	internal PageSettings(PrinterSettings printerSettings, bool color, bool landscape, PaperSize paperSize, PaperSource paperSource, PrinterResolution printerResolution)
	{
		PrinterSettings = printerSettings;
		this.color = color;
		this.landscape = landscape;
		this.paperSize = paperSize;
		this.paperSource = paperSource;
		this.printerResolution = printerResolution;
	}

	public object Clone()
	{
		PrinterResolution printerResolution = new PrinterResolution(this.printerResolution.Kind, this.printerResolution.X, this.printerResolution.Y);
		PaperSource paperSource = new PaperSource(this.paperSource.Kind, this.paperSource.SourceName);
		PaperSize paperSize = new PaperSize(this.paperSize.PaperName, this.paperSize.Width, this.paperSize.Height);
		paperSize.RawKind = (int)this.paperSize.Kind;
		return new PageSettings(printerSettings, color, landscape, paperSize, paperSource, printerResolution)
		{
			Margins = (Margins)margins.Clone()
		};
	}

	[System.MonoTODO("PageSettings.CopyToHdevmode")]
	public void CopyToHdevmode(IntPtr hdevmode)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("PageSettings.SetHdevmode")]
	public void SetHdevmode(IntPtr hdevmode)
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		return string.Format(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("[PageSettings: Color={0}" + ", Landscape={1}", ", Margins={2}"), ", PaperSize={3}"), ", PaperSource={4}"), ", PrinterResolution={5}"), "]"), color, landscape, margins, paperSize, paperSource, printerResolution);
	}
}
