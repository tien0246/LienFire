using System.Configuration;

namespace System.Runtime.Serialization.Configuration;

[ConfigurationCollection(typeof(TypeElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
public sealed class TypeElementCollection : ConfigurationElementCollection
{
	private const string KnownTypeConfig = "knownType";

	public TypeElement this[int index]
	{
		get
		{
			return (TypeElement)BaseGet(index);
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

	protected override string ElementName => "knownType";

	public void Add(TypeElement element)
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

	protected override ConfigurationElement CreateNewElement()
	{
		return new TypeElement();
	}

	protected override object GetElementKey(ConfigurationElement element)
	{
		if (element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
		}
		return ((TypeElement)element).Key;
	}

	public int IndexOf(TypeElement element)
	{
		if (element == null)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
		}
		return BaseIndexOf(element);
	}

	public void Remove(TypeElement element)
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
