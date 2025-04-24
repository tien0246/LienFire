using System.Configuration;

namespace System.Runtime.Serialization.Configuration;

[ConfigurationCollection(typeof(DeclaredTypeElement))]
public sealed class DeclaredTypeElementCollection : ConfigurationElementCollection
{
	public DeclaredTypeElement this[int index]
	{
		get
		{
			return (DeclaredTypeElement)BaseGet(index);
		}
		set
		{
			if (!IsReadOnly())
			{
				if (value == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
				}
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
			}
			BaseAdd(index, value);
		}
	}

	public new DeclaredTypeElement this[string typeName]
	{
		get
		{
			if (string.IsNullOrEmpty(typeName))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
			}
			return (DeclaredTypeElement)BaseGet(typeName);
		}
		set
		{
			if (!IsReadOnly())
			{
				if (string.IsNullOrEmpty(typeName))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
				}
				if (value == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
				}
				if (BaseGet(typeName) == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new IndexOutOfRangeException(SR.GetString("For type '{0}', configuration index is out of range.", typeName)));
				}
				BaseRemove(typeName);
			}
			Add(value);
		}
	}

	public void Add(DeclaredTypeElement element)
	{
		if (!IsReadOnly() && element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
		}
		BaseAdd(element);
	}

	public void Clear()
	{
		BaseClear();
	}

	public bool Contains(string typeName)
	{
		if (string.IsNullOrEmpty(typeName))
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
		}
		return BaseGet(typeName) != null;
	}

	protected override ConfigurationElement CreateNewElement()
	{
		return new DeclaredTypeElement();
	}

	protected override object GetElementKey(ConfigurationElement element)
	{
		if (element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
		}
		return ((DeclaredTypeElement)element).Type;
	}

	public int IndexOf(DeclaredTypeElement element)
	{
		if (element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
		}
		return BaseIndexOf(element);
	}

	public void Remove(DeclaredTypeElement element)
	{
		if (!IsReadOnly() && element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
		}
		BaseRemove(GetElementKey(element));
	}

	public void Remove(string typeName)
	{
		if (!IsReadOnly() && string.IsNullOrEmpty(typeName))
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
		}
		BaseRemove(typeName);
	}

	public void RemoveAt(int index)
	{
		BaseRemoveAt(index);
	}
}
