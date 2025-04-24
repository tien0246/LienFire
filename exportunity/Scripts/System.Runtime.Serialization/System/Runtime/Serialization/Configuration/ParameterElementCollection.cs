using System.Configuration;

namespace System.Runtime.Serialization.Configuration;

[ConfigurationCollection(typeof(ParameterElement), AddItemName = "parameter", CollectionType = ConfigurationElementCollectionType.BasicMap)]
public sealed class ParameterElementCollection : ConfigurationElementCollection
{
	public ParameterElement this[int index]
	{
		get
		{
			return (ParameterElement)BaseGet(index);
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

	public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

	protected override string ElementName => "parameter";

	public ParameterElementCollection()
	{
		base.AddElementName = "parameter";
	}

	public void Add(ParameterElement element)
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
		return new ParameterElement();
	}

	protected override object GetElementKey(ConfigurationElement element)
	{
		if (element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
		}
		return ((ParameterElement)element).identity;
	}

	public int IndexOf(ParameterElement element)
	{
		if (element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
		}
		return BaseIndexOf(element);
	}

	public void Remove(ParameterElement element)
	{
		if (!IsReadOnly() && element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
		}
		BaseRemove(GetElementKey(element));
	}

	public void RemoveAt(int index)
	{
		BaseRemoveAt(index);
	}
}
