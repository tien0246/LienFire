using System.ComponentModel;

namespace System.Drawing.Design;

public class ToolboxComponentsCreatedEventArgs : EventArgs
{
	private readonly IComponent[] comps;

	public IComponent[] Components => (IComponent[])comps.Clone();

	public ToolboxComponentsCreatedEventArgs(IComponent[] components)
	{
		comps = components;
	}
}
