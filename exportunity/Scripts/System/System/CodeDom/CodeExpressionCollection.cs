using System.Collections;

namespace System.CodeDom;

[Serializable]
public class CodeExpressionCollection : CollectionBase
{
	public CodeExpression this[int index]
	{
		get
		{
			return (CodeExpression)base.List[index];
		}
		set
		{
			base.List[index] = value;
		}
	}

	public CodeExpressionCollection()
	{
	}

	public CodeExpressionCollection(CodeExpressionCollection value)
	{
		AddRange(value);
	}

	public CodeExpressionCollection(CodeExpression[] value)
	{
		AddRange(value);
	}

	public int Add(CodeExpression value)
	{
		return base.List.Add(value);
	}

	public void AddRange(CodeExpression[] value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		for (int i = 0; i < value.Length; i++)
		{
			Add(value[i]);
		}
	}

	public void AddRange(CodeExpressionCollection value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		int count = value.Count;
		for (int i = 0; i < count; i++)
		{
			Add(value[i]);
		}
	}

	public bool Contains(CodeExpression value)
	{
		return base.List.Contains(value);
	}

	public void CopyTo(CodeExpression[] array, int index)
	{
		base.List.CopyTo(array, index);
	}

	public int IndexOf(CodeExpression value)
	{
		return base.List.IndexOf(value);
	}

	public void Insert(int index, CodeExpression value)
	{
		base.List.Insert(index, value);
	}

	public void Remove(CodeExpression value)
	{
		base.List.Remove(value);
	}
}
