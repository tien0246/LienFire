namespace System.Drawing.Design;

public class PropertyValueUIItem
{
	private Image itemImage;

	private PropertyValueUIItemInvokeHandler handler;

	private string tooltip;

	public virtual Image Image => itemImage;

	public virtual PropertyValueUIItemInvokeHandler InvokeHandler => handler;

	public virtual string ToolTip => tooltip;

	public PropertyValueUIItem(Image uiItemImage, PropertyValueUIItemInvokeHandler handler, string tooltip)
	{
		itemImage = uiItemImage;
		this.handler = handler;
		if (itemImage == null)
		{
			throw new ArgumentNullException("uiItemImage");
		}
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		this.tooltip = tooltip;
	}

	public virtual void Reset()
	{
	}
}
