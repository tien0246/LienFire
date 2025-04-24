using System.Collections;

namespace System.Drawing.Design;

public sealed class CategoryNameCollection : ReadOnlyCollectionBase
{
	public string this[int index] => (string)base.InnerList[index];

	public CategoryNameCollection(CategoryNameCollection value)
	{
		base.InnerList.AddRange(value);
	}

	public CategoryNameCollection(string[] value)
	{
		base.InnerList.AddRange(value);
	}

	public bool Contains(string value)
	{
		return base.InnerList.Contains(value);
	}

	public void CopyTo(string[] array, int index)
	{
		base.InnerList.CopyTo(array, index);
	}

	public int IndexOf(string value)
	{
		return base.InnerList.IndexOf(value);
	}
}
