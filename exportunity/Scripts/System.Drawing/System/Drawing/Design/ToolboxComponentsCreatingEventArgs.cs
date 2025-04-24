using System.ComponentModel.Design;

namespace System.Drawing.Design;

public class ToolboxComponentsCreatingEventArgs : EventArgs
{
	private readonly IDesignerHost host;

	public IDesignerHost DesignerHost => host;

	public ToolboxComponentsCreatingEventArgs(IDesignerHost host)
	{
		this.host = host;
	}
}
