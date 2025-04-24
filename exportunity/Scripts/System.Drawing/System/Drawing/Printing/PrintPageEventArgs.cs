namespace System.Drawing.Printing;

public class PrintPageEventArgs : EventArgs
{
	private bool cancel;

	private Graphics graphics;

	private bool hasmorePages;

	private Rectangle marginBounds;

	private Rectangle pageBounds;

	private PageSettings pageSettings;

	private GraphicsPrinter graphics_context;

	public bool Cancel
	{
		get
		{
			return cancel;
		}
		set
		{
			cancel = value;
		}
	}

	public Graphics Graphics => graphics;

	public bool HasMorePages
	{
		get
		{
			return hasmorePages;
		}
		set
		{
			hasmorePages = value;
		}
	}

	public Rectangle MarginBounds => marginBounds;

	public Rectangle PageBounds => pageBounds;

	public PageSettings PageSettings => pageSettings;

	internal GraphicsPrinter GraphicsContext
	{
		get
		{
			return graphics_context;
		}
		set
		{
			graphics_context = value;
		}
	}

	public PrintPageEventArgs(Graphics graphics, Rectangle marginBounds, Rectangle pageBounds, PageSettings pageSettings)
	{
		this.graphics = graphics;
		this.marginBounds = marginBounds;
		this.pageBounds = pageBounds;
		this.pageSettings = pageSettings;
	}

	internal void SetGraphics(Graphics g)
	{
		graphics = g;
	}
}
