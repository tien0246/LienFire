using System.Collections;

namespace System.Drawing.Design;

public sealed class ToolboxItemCollection : ReadOnlyCollectionBase
{
	public ToolboxItem this[int index] => (ToolboxItem)base.InnerList[index];

	public ToolboxItemCollection(ToolboxItemCollection value)
	{
		base.InnerList.AddRange(value);
	}

	public ToolboxItemCollection(ToolboxItem[] value)
	{
		base.InnerList.AddRange(value);
	}

	public bool Contains(ToolboxItem value)
	{
		return base.InnerList.Contains(value);
	}

	public void CopyTo(ToolboxItem[] array, int index)
	{
		base.InnerList.CopyTo(array, index);
	}

	public int IndexOf(ToolboxItem value)
	{
		return base.InnerList.IndexOf(value);
	}
}
