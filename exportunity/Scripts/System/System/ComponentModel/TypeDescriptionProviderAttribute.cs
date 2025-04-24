namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class TypeDescriptionProviderAttribute : Attribute
{
	public string TypeName { get; }

	public TypeDescriptionProviderAttribute(string typeName)
	{
		if (typeName == null)
		{
			throw new ArgumentNullException("typeName");
		}
		TypeName = typeName;
	}

	public TypeDescriptionProviderAttribute(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		TypeName = type.AssemblyQualifiedName;
	}
}
