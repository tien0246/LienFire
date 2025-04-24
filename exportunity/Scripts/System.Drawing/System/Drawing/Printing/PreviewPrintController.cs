using System.Collections;

namespace System.Drawing.Printing;

public class PreviewPrintController : PrintController
{
	private bool useantialias;

	private ArrayList pageInfoList;

	public override bool IsPreview => true;

	public virtual bool UseAntiAlias
	{
		get
		{
			return useantialias;
		}
		set
		{
			useantialias = value;
		}
	}

	public PreviewPrintController()
	{
		pageInfoList = new ArrayList();
	}

	[System.MonoTODO]
	public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
	{
	}

	[System.MonoTODO]
	public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
	{
		if (!document.PrinterSettings.IsValid)
		{
			throw new InvalidPrinterException(document.PrinterSettings);
		}
		foreach (PreviewPageInfo pageInfo in pageInfoList)
		{
			pageInfo.Image.Dispose();
		}
		pageInfoList.Clear();
	}

	[System.MonoTODO]
	public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
	{
	}

	[System.MonoTODO]
	public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
	{
		Image image = new Bitmap(e.PageSettings.PaperSize.Width, e.PageSettings.PaperSize.Height);
		PreviewPageInfo previewPageInfo = new PreviewPageInfo(image, new Size(e.PageSettings.PaperSize.Width, e.PageSettings.PaperSize.Height));
		pageInfoList.Add(previewPageInfo);
		Graphics graphics = Graphics.FromImage(previewPageInfo.Image);
		graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(0, 0), new Size(image.Width, image.Height)));
		return graphics;
	}

	public PreviewPageInfo[] GetPreviewPageInfo()
	{
		PreviewPageInfo[] array = new PreviewPageInfo[pageInfoList.Count];
		pageInfoList.CopyTo(array);
		return array;
	}
}
