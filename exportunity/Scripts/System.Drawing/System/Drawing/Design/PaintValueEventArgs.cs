using System.ComponentModel;

namespace System.Drawing.Design;

public class PaintValueEventArgs : EventArgs
{
	private readonly ITypeDescriptorContext context;

	private readonly object valueToPaint;

	private readonly Graphics graphics;

	private readonly Rectangle bounds;

	public Rectangle Bounds => bounds;

	public ITypeDescriptorContext Context => context;

	public Graphics Graphics => graphics;

	public object Value => valueToPaint;

	public PaintValueEventArgs(ITypeDescriptorContext context, object value, Graphics graphics, Rectangle bounds)
	{
		this.context = context;
		valueToPaint = value;
		this.graphics = graphics;
		if (graphics == null)
		{
			throw new ArgumentNullException("graphics");
		}
		this.bounds = bounds;
	}
}
