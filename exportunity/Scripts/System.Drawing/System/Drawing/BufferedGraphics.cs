namespace System.Drawing;

public sealed class BufferedGraphics : IDisposable
{
	private Rectangle size;

	private Bitmap membmp;

	private Graphics target;

	private Graphics source;

	public Graphics Graphics
	{
		get
		{
			if (source == null)
			{
				source = Graphics.FromImage(membmp);
			}
			return source;
		}
	}

	private BufferedGraphics()
	{
	}

	internal BufferedGraphics(Graphics targetGraphics, Rectangle targetRectangle)
	{
		size = targetRectangle;
		target = targetGraphics;
		membmp = new Bitmap(size.Width, size.Height);
	}

	~BufferedGraphics()
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
		if (disposing)
		{
			if (membmp != null)
			{
				membmp.Dispose();
				membmp = null;
			}
			if (source != null)
			{
				source.Dispose();
				source = null;
			}
			target = null;
		}
	}

	public void Render()
	{
		Render(target);
	}

	public void Render(Graphics target)
	{
		target?.DrawImage(membmp, size);
	}

	[System.MonoTODO("The targetDC parameter has no equivalent in libgdiplus.")]
	public void Render(IntPtr targetDC)
	{
		throw new NotImplementedException();
	}
}
