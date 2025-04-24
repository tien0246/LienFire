using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Imaging;

namespace System.Drawing.Printing;

[Serializable]
public class PrinterSettings : ICloneable
{
	public class PaperSourceCollection : ICollection, IEnumerable
	{
		private ArrayList _PaperSources = new ArrayList();

		public int Count => _PaperSources.Count;

		int ICollection.Count => _PaperSources.Count;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		public virtual PaperSource this[int index] => _PaperSources[index] as PaperSource;

		public PaperSourceCollection(PaperSource[] array)
		{
			foreach (PaperSource value in array)
			{
				_PaperSources.Add(value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Add(PaperSource paperSource)
		{
			return _PaperSources.Add(paperSource);
		}

		public void CopyTo(PaperSource[] paperSources, int index)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _PaperSources.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _PaperSources.GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			_PaperSources.CopyTo(array, index);
		}

		internal void Clear()
		{
			_PaperSources.Clear();
		}
	}

	public class PaperSizeCollection : ICollection, IEnumerable
	{
		private ArrayList _PaperSizes = new ArrayList();

		public int Count => _PaperSizes.Count;

		int ICollection.Count => _PaperSizes.Count;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		public virtual PaperSize this[int index] => _PaperSizes[index] as PaperSize;

		public PaperSizeCollection(PaperSize[] array)
		{
			foreach (PaperSize value in array)
			{
				_PaperSizes.Add(value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Add(PaperSize paperSize)
		{
			return _PaperSizes.Add(paperSize);
		}

		public void CopyTo(PaperSize[] paperSizes, int index)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _PaperSizes.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _PaperSizes.GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			_PaperSizes.CopyTo(array, index);
		}

		internal void Clear()
		{
			_PaperSizes.Clear();
		}
	}

	public class PrinterResolutionCollection : ICollection, IEnumerable
	{
		private ArrayList _PrinterResolutions = new ArrayList();

		public int Count => _PrinterResolutions.Count;

		int ICollection.Count => _PrinterResolutions.Count;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		public virtual PrinterResolution this[int index] => _PrinterResolutions[index] as PrinterResolution;

		public PrinterResolutionCollection(PrinterResolution[] array)
		{
			foreach (PrinterResolution value in array)
			{
				_PrinterResolutions.Add(value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Add(PrinterResolution printerResolution)
		{
			return _PrinterResolutions.Add(printerResolution);
		}

		public void CopyTo(PrinterResolution[] printerResolutions, int index)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _PrinterResolutions.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _PrinterResolutions.GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			_PrinterResolutions.CopyTo(array, index);
		}

		internal void Clear()
		{
			_PrinterResolutions.Clear();
		}
	}

	public class StringCollection : ICollection, IEnumerable
	{
		private ArrayList _Strings = new ArrayList();

		public int Count => _Strings.Count;

		int ICollection.Count => _Strings.Count;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		public virtual string this[int index] => _Strings[index] as string;

		public StringCollection(string[] array)
		{
			foreach (string value in array)
			{
				_Strings.Add(value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Add(string value)
		{
			return _Strings.Add(value);
		}

		public void CopyTo(string[] strings, int index)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Strings.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _Strings.GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			_Strings.CopyTo(array, index);
		}
	}

	private string printer_name;

	private string print_filename;

	private short copies;

	private int maximum_page;

	private int minimum_page;

	private int from_page;

	private int to_page;

	private bool collate;

	private PrintRange print_range;

	internal int maximum_copies;

	internal bool can_duplex;

	internal bool supports_color;

	internal int landscape_angle;

	private bool print_tofile;

	internal PrinterResolutionCollection printer_resolutions;

	internal PaperSizeCollection paper_sizes;

	internal PaperSourceCollection paper_sources;

	private PageSettings default_pagesettings;

	private Duplex duplex;

	internal bool is_plotter;

	private PrintingServices printing_services;

	internal NameValueCollection printer_capabilities;

	public bool CanDuplex => can_duplex;

	public bool Collate
	{
		get
		{
			return collate;
		}
		set
		{
			collate = value;
		}
	}

	public short Copies
	{
		get
		{
			return copies;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("The value of the Copies property is less than zero.");
			}
			copies = value;
		}
	}

	public PageSettings DefaultPageSettings
	{
		get
		{
			if (default_pagesettings == null)
			{
				default_pagesettings = new PageSettings(this, SupportsColor, landscape: false, new PaperSize("A4", 827, 1169), new PaperSource(PaperSourceKind.FormSource, "Tray"), new PrinterResolution(PrinterResolutionKind.Medium, 200, 200));
			}
			return default_pagesettings;
		}
	}

	public Duplex Duplex
	{
		get
		{
			return duplex;
		}
		set
		{
			duplex = value;
		}
	}

	public int FromPage
	{
		get
		{
			return from_page;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("The value of the FromPage property is less than zero");
			}
			from_page = value;
		}
	}

	public static StringCollection InstalledPrinters => SysPrn.GlobalService.InstalledPrinters;

	public bool IsDefaultPrinter => printer_name == printing_services.DefaultPrinter;

	public bool IsPlotter => is_plotter;

	public bool IsValid => printing_services.IsPrinterValid(printer_name);

	public int LandscapeAngle => landscape_angle;

	public int MaximumCopies => maximum_copies;

	public int MaximumPage
	{
		get
		{
			return maximum_page;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("The value of the MaximumPage property is less than zero");
			}
			maximum_page = value;
		}
	}

	public int MinimumPage
	{
		get
		{
			return minimum_page;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("The value of the MaximumPage property is less than zero");
			}
			minimum_page = value;
		}
	}

	public PaperSizeCollection PaperSizes
	{
		get
		{
			if (!IsValid)
			{
				throw new InvalidPrinterException(this);
			}
			return paper_sizes;
		}
	}

	public PaperSourceCollection PaperSources
	{
		get
		{
			if (!IsValid)
			{
				throw new InvalidPrinterException(this);
			}
			return paper_sources;
		}
	}

	public string PrintFileName
	{
		get
		{
			return print_filename;
		}
		set
		{
			print_filename = value;
		}
	}

	public string PrinterName
	{
		get
		{
			return printer_name;
		}
		set
		{
			if (!(printer_name == value))
			{
				printer_name = value;
				printing_services.LoadPrinterSettings(printer_name, this);
			}
		}
	}

	public PrinterResolutionCollection PrinterResolutions
	{
		get
		{
			if (!IsValid)
			{
				throw new InvalidPrinterException(this);
			}
			if (printer_resolutions == null)
			{
				printer_resolutions = new PrinterResolutionCollection(new PrinterResolution[0]);
				printing_services.LoadPrinterResolutions(printer_name, this);
			}
			return printer_resolutions;
		}
	}

	public PrintRange PrintRange
	{
		get
		{
			return print_range;
		}
		set
		{
			if (value != PrintRange.AllPages && value != PrintRange.Selection && value != PrintRange.SomePages)
			{
				throw new InvalidEnumArgumentException("The value of the PrintRange property is not one of the PrintRange values");
			}
			print_range = value;
		}
	}

	public bool PrintToFile
	{
		get
		{
			return print_tofile;
		}
		set
		{
			print_tofile = value;
		}
	}

	public bool SupportsColor => supports_color;

	public int ToPage
	{
		get
		{
			return to_page;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("The value of the ToPage property is less than zero");
			}
			to_page = value;
		}
	}

	internal NameValueCollection PrinterCapabilities
	{
		get
		{
			if (printer_capabilities == null)
			{
				printer_capabilities = new NameValueCollection();
			}
			return printer_capabilities;
		}
	}

	public PrinterSettings()
		: this(SysPrn.CreatePrintingService())
	{
	}

	internal PrinterSettings(PrintingServices printing_services)
	{
		this.printing_services = printing_services;
		printer_name = printing_services.DefaultPrinter;
		ResetToDefaults();
		printing_services.LoadPrinterSettings(printer_name, this);
	}

	private void ResetToDefaults()
	{
		printer_resolutions = null;
		paper_sizes = null;
		paper_sources = null;
		default_pagesettings = null;
		maximum_page = 9999;
		copies = 1;
		collate = true;
	}

	public object Clone()
	{
		return new PrinterSettings(printing_services);
	}

	[System.MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
	public Graphics CreateMeasurementGraphics()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
	public Graphics CreateMeasurementGraphics(bool honorOriginAtMargins)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
	public Graphics CreateMeasurementGraphics(PageSettings pageSettings)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("PrinterSettings.CreateMeasurementGraphics")]
	public Graphics CreateMeasurementGraphics(PageSettings pageSettings, bool honorOriginAtMargins)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("PrinterSettings.GetHdevmode")]
	public IntPtr GetHdevmode()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("PrinterSettings.GetHdevmode")]
	public IntPtr GetHdevmode(PageSettings pageSettings)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("PrinterSettings.GetHdevname")]
	public IntPtr GetHdevnames()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("IsDirectPrintingSupported")]
	public bool IsDirectPrintingSupported(Image image)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("IsDirectPrintingSupported")]
	public bool IsDirectPrintingSupported(ImageFormat imageFormat)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("PrinterSettings.SetHdevmode")]
	public void SetHdevmode(IntPtr hdevmode)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("PrinterSettings.SetHdevnames")]
	public void SetHdevnames(IntPtr hdevnames)
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		return "Printer [PrinterSettings " + printer_name + " Copies=" + copies + " Collate=" + collate + " Duplex=" + can_duplex + " FromPage=" + from_page + " LandscapeAngle=" + landscape_angle + " MaximumCopies=" + maximum_copies + " OutputPort= ToPage=" + to_page + "]";
	}
}
