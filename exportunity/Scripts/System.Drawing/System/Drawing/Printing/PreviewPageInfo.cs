namespace System.Drawing.Printing;

public sealed class PreviewPageInfo
{
	private Image _image;

	private Size _physicalSize = Size.Empty;

	public Image Image => _image;

	public Size PhysicalSize => _physicalSize;

	public PreviewPageInfo(Image image, Size physicalSize)
	{
		_image = image;
		_physicalSize = physicalSize;
	}
}
